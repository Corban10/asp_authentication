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

        // Authentication
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
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // for blacklisting tokens
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            };
        }
        public List<Claim> IssueIdentityClaims(User user)
        {
            return new List<Claim>
            {
                new Claim("IsActive", user.IsActive.ToString()), // for blacklisting tokens
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()),
            };
        }
        public async Task UpdateClaims(User user)
        {
            await ReplaceClaim(user, "IsActive", user.IsActive.ToString());
            await ReplaceClaim(user, ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).First());
            await ReplaceClaim(user, ClaimTypes.DateOfBirth, user.DateOfBirth.ToString());
        }
        public async Task UpdateRole(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await _userManager.AddToRoleAsync(user, user.UserType.ToString());
        }
        public async Task DeleteUserClaims(User user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            await _userManager.RemoveClaimsAsync(user, claims.ToArray());
        }
        public async Task RemoveUserRoles(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
        }
        private async Task ReplaceClaim(User user, string claimName, string claimValue)
        {
            var oldClaim = new ClaimsIdentity(await _userManager.GetClaimsAsync(user)).FindFirst(claimName);
            var newClaim = new Claim(claimName, claimValue);
            if (oldClaim == null)
            {
                await _userManager.AddClaimAsync(user, newClaim);
            }
            else
            {
                await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
            }
        }
        public UserViewModel ReturnUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
                UserType = user.UserType,
                UserName = user.UserName,
                DateOfBirth = user.DateOfBirth.ToShortDateString(),
                Token = user.Token
            };
        }

        // Crud
        public Task<UserType> GetById(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
        }
        public Task<UserType> PutById(UserViewModel user, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<UserType> PostNew(UserViewModel user)
        {
            throw new NotImplementedException();
        }
        public Task<UserType> DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<User> ToggleIsActive(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }

        // Relationships
        public Task<Relationship> GetRelationship(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Relationship> CreateRelationship(Guid rightUserID, Guid leftUserID)
        {
            throw new NotImplementedException();
        }
        public Task<Relationship> BlockUser(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Relationship> UnblockUser(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Relationship> FollowUser(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Relationship> UnfollowUser(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
    }
}