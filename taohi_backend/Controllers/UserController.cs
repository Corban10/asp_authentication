using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Interfaces;
using taohi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserManager<User> _userManager { get; set; }
        private readonly IUserService _userService;
        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [Authorize(Policy = "Admin")]
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
        public async Task<IActionResult> Authenticate([FromBody] AuthViewModel loginUser)
        {
            var user = await _userManager.FindByNameAsync(loginUser.username);
            if (user == null)
                return BadRequest();

            var signedIn = await _userManager.CheckPasswordAsync(user, loginUser.password);
            if (!signedIn)
                return BadRequest();

            var token = await _userService.IssueToken(user);
            if (token == null)
                return BadRequest();

            user.Token = token;
            var userViewModel = _userService.ReturnUserViewModel(user);

            return Ok(userViewModel);
        }
    }
}
