using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using taohi_backend.Interfaces;

namespace taohi_backend.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        public UserService(IConfiguration config)
        {
            _config = config;
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