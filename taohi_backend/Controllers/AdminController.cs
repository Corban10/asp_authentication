using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        public RoleManager<IdentityRole> _roleManager { get; set; }
        public UserManager<IdentityUser> _userManager { get; set; }
        public SignInManager<IdentityUser> _signInManager { get; set; }
        public AdminController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateRole()
        {
            ViewBag.roles = _roleManager.Roles;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(string role)
        {
            var newRole = new IdentityRole { Name = role };
            await _roleManager.CreateAsync(newRole);

            ViewBag.roles = _roleManager.Roles;
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return RedirectToAction("Index");

            var signedIn = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!signedIn.Succeeded)
                return View();

            #region without identity
            //var validated = await _userManager.CheckPasswordAsync(user, password);
            //if (!validated)
            //    return View();

            //var adminClaims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.Id.ToString()),
            //    new Claim(ClaimTypes.Email, user.UserName),
            //    new Claim(ClaimTypes.Role, "Admin"),
            //};
            //var adminIdentity = new ClaimsIdentity(adminClaims, "Admin");
            //var userPrincipal = new ClaimsPrincipal(new[] { adminIdentity });

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);
            #endregion

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            var newUser = new IdentityUser { UserName = email, Email = email };

            var creationResponse = await _userManager.CreateAsync(newUser, password);
            if (!creationResponse.Succeeded)
                return View();

            var addClaimsResponse = await _userManager.AddClaimsAsync(newUser, new[]
            {
                new Claim(ClaimTypes.Name, newUser.Id),
                new Claim(ClaimTypes.Role, "Admin"),
            });
            if (!addClaimsResponse.Succeeded)
                return View();

            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (!result.Succeeded)
                return View();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Logout()
        {
            // HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
