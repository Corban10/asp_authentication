using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Basics.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public SignInManager<IdentityUser> _signInManager { get; set; }
        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Secret()
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
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return RedirectToAction("Index");

            var signedIn = await _signInManager.PasswordSignInAsync(user, password, true, false);
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
        public async Task<IActionResult> Register(string username, string password)
        {
            var newUser = new IdentityUser { UserName = username };

            var creationResponse = await _userManager.CreateAsync(newUser, password);
            if (!creationResponse.Succeeded)
                return View();

            var addedUser = await _userManager.FindByNameAsync(username);
            if (addedUser == null)
                return View();

            var signedIn = await _signInManager.PasswordSignInAsync(addedUser, password, true, false);
            if (!signedIn.Succeeded)
                return View();

            var addClaimsResponse = await _userManager.AddClaimsAsync(addedUser, new[]
            {
                new Claim(ClaimTypes.Name, addedUser.Id),
                new Claim(ClaimTypes.Role, "Admin"),
            });
            if (!addClaimsResponse.Succeeded)
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