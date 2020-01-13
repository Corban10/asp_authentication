using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        public RoleManager<UserRole> _roleManager;
        public UserManager<User> _userManager;
        public SignInManager<User> _signInManager;
        public IUserService _userService;
        public IAuthorizationService _authService;
        public AdminController(
            RoleManager<UserRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUserService userService,
            IAuthorizationService authService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _authService = authService;
        }

        public async Task<IActionResult> Index([FromQuery] string role)
        {
            IEnumerable<User> users =
                !string.IsNullOrWhiteSpace(role) ?
                await _userManager.GetUsersInRoleAsync(role) :
                users = await _userManager.Users.ToListAsync();

            var model = users.Select(user => _userService.ReturnUserViewModel(user));
            ViewBag.roles = _roleManager.Roles.Select(r => r.Name);
            return View(model);
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

            var model = _userService.ReturnUserViewModel(user);
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Email = user.Email;
            model.Roles = roles;
            model.Claims = claims;

            ViewBag.roles = _roleManager.Roles.Select(r => r.Name);
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
            if (user.UserType != model.UserType)
            {
                user.UserType = model.UserType;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = $"Error updating user with Id: {modelId}";
                return View("Error");
            }

            await _userService.UpdateRole(user);
            await _userService.UpdateClaims(user);

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // refresh cookie if logged in user is modified
            if (user.Id == new Guid(loggedInUserId))
            {
                var loggedinUser = await _userManager.FindByIdAsync(loggedInUserId);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(loggedinUser, true, "");
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
            if (user == null || user.Email == "corbanhirawani@gmail.com")
                return View("Error");

            await _userManager.DeleteAsync(user);
            await _userService.DeleteUserClaims(user);
            await _userService.RemoveUserRoles(user);

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

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            var isAdmin = await _authService.AuthorizeAsync(claimsPrincipal, "Admin");
            var isActive = await _authService.AuthorizeAsync(claimsPrincipal, "IsActive");
            if (!isAdmin.Succeeded || !isActive.Succeeded)
                return RedirectToAction("Logout");

            return RedirectToAction("Index");
        }
        public IActionResult NewUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewUser(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var newUser = new User
            {
                UserName = model.Email,
                Email = model.Email,
                UserType = model.UserType
            };

            var creationResponse = await _userManager.CreateAsync(newUser, model.Password);
            if (!creationResponse.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Error registering user.");
                return View();
            }
            var claims = _userService.IssueIdentityClaims(newUser);
            var addClaimsResponse = await _userManager.AddClaimsAsync(newUser, claims);
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
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
