using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Moderator")]
    [ApiController]
    [Route("api/Admin")]
    public class ModeratorController : ControllerBase
    {
        public UserManager<User> _userManager { get; set; }
        private readonly IAdminService _userService;
        public ModeratorController(IAdminService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }
        [HttpGet("GetId")]
        public IActionResult GetId()
        {
            var name = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();

            return Ok(name);
        }
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthViewModel loginUser)
        {
            // find user
            var user = await _userManager.FindByNameAsync(loginUser.username);
            if (user == null)
                return BadRequest();

            // check is admin

            // sign in if passed
            var signedIn = await _userManager.CheckPasswordAsync(user, loginUser.password);
            if (!signedIn)
                return BadRequest();

            // issue token
            var token = await _userService.IssueToken(user);
            if (token == null)
                return BadRequest();

            return Ok(new
            {
                token
            });
        }
    }
}
