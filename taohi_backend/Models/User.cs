using Microsoft.AspNetCore.Identity;
using System;

namespace taohi_backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public ContentType ContentType { get; set; }
        public string Token { get; set; }
    }
    public enum ContentType
    {
        undefined,
        Taohi,
        Rangatahi,
        Heitiki
    }
}
