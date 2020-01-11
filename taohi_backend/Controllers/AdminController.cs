﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Admin")]
    [Authorize(Policy = "IsActive")]
    public class AdminController : Controller
    {
        public RoleManager<UserRole> _roleManager { get; set; }
        public UserManager<User> _userManager { get; set; }
        public SignInManager<User> _signInManager { get; set; }
        public IAdminService _adminService { get; set; }
        public IAuthorizationService _authService { get; set; }
        public AdminController(
            RoleManager<UserRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAdminService adminService,
            IAuthorizationService authService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _adminService = adminService;
            _authService = authService;
        }

        public async Task<IActionResult> Index([FromQuery] string role = "")
        {
            if (!string.IsNullOrWhiteSpace(role))
            {
                return View(await _userManager.GetUsersInRoleAsync(role));
            }

            return View(await _userManager.Users.ToListAsync());
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Couldn't find user with Id: {id}";
                return View("Error");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var claims = await _userManager.GetClaimsAsync(user);
            // var claimValues = claims.Select(claim => claim.Value);
            var model = _adminService.ReturnUserViewModel(user);
            model.Email = user.Email;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Roles = roles;
            model.Claims = claims;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            var modelId = model.Id.ToString();

            var user = await _userManager.FindByIdAsync(modelId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Couldn't find user with Id: {modelId}";
                return View("Error");
            }

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            user.DisplayName = model.DisplayName;
            user.DateOfBirth = Convert.ToDateTime(model.DateOfBirth);
            user.IsActive = model.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = $"Error updating user with Id: {modelId}";
                return View("Error");
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Couldn't find user with Id: {id}";
                return View("Error");
            }
            var model = new UserViewModel { Id = user.Id, UserName = user.UserName, Email = user.Email };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(UserViewModel model)
        {
            var modelId = model.Id.ToString();
            var user = await _userManager.FindByIdAsync(modelId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Couldn't find user with Id: {modelId}";
                return View("Error");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = $"Error deleting user with Id: {modelId}";
                return View("Error");
            }
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
                return View();

            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            if (!(await _authService.AuthorizeAsync(claimsPrincipal, "Admin")).Succeeded)
                return RedirectToAction("Logout");

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] AuthViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByNameAsync(model.username);
            if (user == null)
                return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.password, false);
            if (!result.Succeeded)
                return BadRequest();

            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            if (!(await _authService.AuthorizeAsync(claimsPrincipal, "Admin")).Succeeded)
                return BadRequest();

            user.Token = await _adminService.IssueToken(user);
            var userViewModel = _adminService.ReturnUserViewModel(user);

            return Ok(userViewModel);
        }
        [AllowAnonymous]
        public IActionResult Decode(string part)
        {
            var bytes = Convert.FromBase64String(part);
            var decoded = Encoding.UTF8.GetString(bytes);
            return Ok(decoded);
        }
        public IActionResult CreateNewUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateNewUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var newUser = new User { UserName = model.Email, Email = model.Email };

            var creationResponse = await _userManager.CreateAsync(newUser, model.Password);
            if (!creationResponse.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error registering user.");
                return View();
            }

            var addClaimsResponse = await _userManager.AddClaimsAsync(newUser, _adminService.IssueClaims(newUser));
            if (!addClaimsResponse.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error registering user.");
                return View();
            }

            var result = await _userManager.AddToRoleAsync(newUser, "User");
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error registering user.");
                return View();
            }

            return RedirectToAction("Index");
        }
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newRole = new UserRole { Name = model.RoleName };
                var result = await _roleManager.CreateAsync(newRole);
                if (!result.Succeeded)
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
            }
            return RedirectToAction("ListRoles");
        }
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} was not found";
                return View("Error");
            }

            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            var userIds = new List<string>();
            foreach (var user in users)
                userIds.Add(user.Email);

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                Users = userIds
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel role)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error updating role.");
                return View(role);
            }

            var updateRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (updateRole == null)
            {
                ModelState.AddModelError(string.Empty, "Error updating role.");
                return View(role);
            }

            updateRole.Name = role.RoleName;

            var result = await _roleManager.UpdateAsync(updateRole);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error updating role.");
                return View(role);
            }

            return RedirectToAction("ListRoles");
        }
        public async Task<IActionResult> EditUserRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {id} was not found";
                return View("Error");
            }
            ViewBag.RoleId = role.Id;

            var usersInRole = new List<UserRoleViewModel>();
            var allUsers = await _userManager.Users.ToListAsync();
            foreach (var userItem in allUsers)
            {
                var user = new UserRoleViewModel
                {
                    UserId = userItem.Id.ToString(),
                    UserName = userItem.UserName,
                    IsSelected = await _userManager.IsInRoleAsync(userItem, role.Name)
                };
                usersInRole.Add(user);
            }
            var model = new EditUserRoleViewModel { Id = role.Id, Users = usersInRole };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserRole(EditUserRoleViewModel model)
        {
            if (!ModelState.IsValid)
                ModelState.AddModelError(string.Empty, "Error updating role.");

            var roleId = model.Id.ToString();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {roleId} was not found";
                return View("Error");
            }

            var roleChanges = model.Users;
            var allUsers = await _userManager.Users.ToListAsync();
            for (int i = 0; i < allUsers.Count; i++)
            {
                if (roleChanges[i].IsSelected != await _userManager.IsInRoleAsync(allUsers[i], role.Name))
                {
                    if (roleChanges[i].IsSelected)
                        await _userManager.AddToRoleAsync(allUsers[i], role.Name);
                    else
                        await _userManager.RemoveFromRoleAsync(allUsers[i], role.Name);
                }
            }
            return Redirect($"~/Admin/EditRole/{role.Id.ToString()}");
        }
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"No Role with Id: {id} was found";
                return View("Error");
            }
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(DeleteRoleViewModel role)
        {
            var oldRole = await _roleManager.FindByIdAsync(role.Id.ToString());
            if (oldRole == null)
            {
                ViewBag.ErrorMessage = $"Couldn't find role with Id: {oldRole.Id}";
                return View("Error");
            }
            var result = await _roleManager.DeleteAsync(oldRole);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = $"Error deleting role with Id: {oldRole.Id}";
                return View("Error");
            }
            return RedirectToAction("ListRoles");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
