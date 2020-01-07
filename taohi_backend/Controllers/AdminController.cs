using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        public RoleManager<UserRole> _roleManager { get; set; }
        public UserManager<User> _userManager { get; set; }
        public SignInManager<User> _signInManager { get; set; }
        public IAdminService _adminService { get; set; }
        public AdminController(
            RoleManager<UserRole> roleManager,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAdminService adminService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            ViewBag.users = _userManager.Users;
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult EditUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EditUser(string role, string userId)
        {
            return View();
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

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var newUser = new User { UserName = model.Email, Email = model.Email };

            var creationResponse = await _userManager.CreateAsync(newUser, model.Password);
            if (!creationResponse.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            var addClaimsResponse = await _userManager.AddClaimsAsync(newUser, _adminService.IssueClaims(newUser));
            if (!addClaimsResponse.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
                return View();
            }

            return RedirectToAction("Index");
        }
        public IActionResult CreateRole()
        {
            ViewBag.roles = _roleManager.Roles;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            ViewBag.roles = _roleManager.Roles;
            if (!ModelState.IsValid)
                return View();

            var newRole = new UserRole { Name = model.RoleName };
            var result = await _roleManager.CreateAsync(newRole);
            if (!result.Succeeded)
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
