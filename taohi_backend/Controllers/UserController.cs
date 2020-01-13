using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Interfaces;
using taohi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Jwt")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserManager<User> _userManager;
        public SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        public IAuthorizationService _authService;
        public UserController(
            IUserService userService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthorizationService authService)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }
        [Authorize(Roles = "User, Admin")]
        [HttpGet("GetId")]
        public IActionResult GetId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            return Ok(userId);
        }
        [AllowAnonymous]
        [HttpPost("Authenticate")]
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
            var isActive = await _authService.AuthorizeAsync(claimsPrincipal, "IsActive");
            if (!isActive.Succeeded)
                return BadRequest();

            user.Token = await _userService.IssueToken(user);
            var userViewModel = _userService.ReturnUserViewModel(user);

            return Ok(userViewModel);
        }
    }
}
