using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface IUserService
    {
        public Task<string> IssueToken(User user);
        public List<Claim> IssueClaims(User user);
        public UserViewModel ReturnUserViewModel(User user);
    }
}
