using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Basics.Controllers
{
    [Authorize(Policy = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        private readonly IConfiguration _config;
        public UserController(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        [HttpGet("GetId")]
        public IActionResult GetSecureNumbers()
        {
            var name = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();

            return Ok(name);
        }
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate()
        {
            var username = "SomeUserName";
            var password = "Lenny123$";

            // validate user
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return BadRequest();

            var signedIn = await _userManager.CheckPasswordAsync(user, password);
            if (!signedIn)
                return BadRequest();

            var token = IssueToken(user);
            if (token == null)
                return BadRequest();

            return Ok(token); // token
        }

        public string IssueToken(IdentityUser user)
        {
            try
            {
                var securityKey = Encoding.UTF8.GetBytes(_config["JwtAuthentication:Secret"]);
                var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                var claims = IssueClaims(user);
                var token = new JwtSecurityToken(
                    issuer: _config["JwtAuthentication:Issuer"],
                    audience: _config["JwtAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: signingCredentials);

                var serealizedToken = new JwtSecurityTokenHandler().WriteToken(token);
                return serealizedToken;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<Claim> IssueClaims(IdentityUser user)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "User")
            };
        }
    }
}