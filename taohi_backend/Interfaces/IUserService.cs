using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface IUsersService
    {
        public string IssueToken(User user);
        public List<Claim> IssueClaims(User user);
    }
    public interface IUserService : IUsersService
    {
    }
    public interface IAdminService : IUsersService
    {
    }
}
