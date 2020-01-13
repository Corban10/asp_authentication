using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface IUserService
    {
        public Task<string> IssueToken(User user);
        public List<Claim> IssueTokenClaims(User user);
        public List<Claim> IssueIdentityClaims(User user);
        public UserViewModel ReturnUserViewModel(User user);
        public Task UpdateClaims(User user);
        public Task UpdateRole(User user);
        public Task DeleteUserClaims(User user);
        public Task RemoveUserRoles(User user);
    }
}
