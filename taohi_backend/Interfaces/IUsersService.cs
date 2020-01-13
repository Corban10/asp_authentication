using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
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

        // Relationships
        Task<Relationship> GetRelationship(Guid id, ClaimsPrincipal claim);
        Task<Relationship> CreateRelationship(Guid rightUserID, Guid leftUserID);
        Task<Relationship> BlockUser(Guid id, ClaimsPrincipal claim);
        Task<Relationship> UnblockUser(Guid id, ClaimsPrincipal claim);
        Task<Relationship> FollowUser(Guid id, ClaimsPrincipal claim);
        Task<Relationship> UnfollowUser(Guid id, ClaimsPrincipal claim);
    }
}
