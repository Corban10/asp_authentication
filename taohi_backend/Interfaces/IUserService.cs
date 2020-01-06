using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace taohi_backend.Interfaces
{
    public interface IUsersService
    {
        public string IssueToken(IdentityUser user);
        public List<Claim> IssueClaims(IdentityUser user);
    }
    public interface IUserService : IUsersService
    {
    }
    public interface IAdminService : IUsersService
    {
    }
}
