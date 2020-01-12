using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taohi_backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserType UserType { get; set; }
        [Required]
        [Column("IsActive", TypeName = "bit")]
        public bool IsActive { get; set; } = true;
        public DateTime SuspensionTime { get; set; }
        public string Token { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
    }
}
