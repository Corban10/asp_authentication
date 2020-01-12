using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        public UserManager<User> _userManager { get; set; }
        public UserService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }
        public async Task<string> IssueToken(User user)
        {
            try
            {
                var securityKey = Encoding.UTF8.GetBytes(_config["JwtAuthentication:Secret"]);
                var symmetricSecurityKey = new SymmetricSecurityKey(securityKey);
                var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
                var claims = IssueTokenClaims(user);
                var userClaims = await _userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims);
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
        public List<Claim> IssueTokenClaims(User user)
        {
            var tokenClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // for blacklisting tokens
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            };
            return tokenClaims;
        }
        public async Task UpdateClaims(User user)
        {
            await ReplaceClaim(user, "IsActive", user.IsActive.ToString());
            await ReplaceClaim(user, ClaimTypes.Role, user.UserType.ToString());
            await ReplaceClaim(user, ClaimTypes.DateOfBirth, user.DateOfBirth.ToString());
        }
        private async Task ReplaceClaim(User user, string claimName, string claimValue)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var oldClaim = claims.Where(claim => claim.Type == claimName);
            if (!oldClaim.Any())
            {
                var newClaim = new Claim(claimName, claimValue);
                await _userManager.AddClaimAsync(user, newClaim);
            }
            else
            {
                var newClaim = new Claim(claimName, claimValue);
                await _userManager.ReplaceClaimAsync(user, oldClaim.First(), newClaim);
            }
        }
        public UserViewModel ReturnUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                DateOfBirth = user.DateOfBirth.ToShortDateString(),
                IsActive = user.IsActive,
                Token = user.Token,
                UserType = user.UserType
            };
        }
    }
}