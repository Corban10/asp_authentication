using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using asp_auth.Models;

namespace asp_auth.Interfaces
{
    public interface IUserService
    {
        // Authentication
        public Task<string> IssueToken(User user);
        public List<Claim> IssueTokenClaims(User user);
        public List<Claim> IssueIdentityClaims(User user);
        public UserViewModel ReturnUserViewModel(User user);
        public Task UpdateClaims(User user);
        public Task UpdateRole(User user);
        public Task DeleteUserClaims(User user);
        public Task RemoveUserRoles(User user);

        // Crud
        Task<UserType> GetById(Guid id, ClaimsPrincipal claim);
        Task<IEnumerable<User>> GetAll();
        Task<UserType> PutById(UserViewModel user, ClaimsPrincipal claim);
        Task<UserType> PostNew(UserViewModel user);
        Task<UserType> DeleteById(Guid id);
        Task<User> ToggleIsActive(Guid id, ClaimsPrincipal claim);
    }
}
