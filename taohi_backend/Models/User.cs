using Microsoft.AspNetCore.Identity;
using System;

namespace taohi_backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public UserType UserType { get; set; }
    }
    public enum UserType
    {
        undefined,
        Taohi,
        Rangatahi,
        Heitiki
    }
}
