using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace asp_auth.Models
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserType UserType { get; set; }
        [Column("IsActive", TypeName = "bit")]
        public bool IsActive { get; set; } = true;
        // in days
        public int SuspensionTime { get; set; }
        public string Token { get; set; }
        [NotMapped]
        public virtual ICollection<Message> Messages { get; set; }
    }
}
