using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface IUsersService
    {
        public Task<string> IssueToken(User user);
        public List<Claim> IssueClaims(User user);
        public UserViewModel ReturnUserViewModel(User user);
    }
    public interface IUserService : IUsersService
    {
    }
    public interface IAdminService : IUsersService
    {
    }
}
