using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace taohi_backend.Models
{
    public class User : IdentityUser<Guid>
    {
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public UserType UserType { get; set; }
        public ContentType ContentType { get; set; }
        [Column("IsActive", TypeName = "bit")]
        public bool IsActive { get; set; } = true;
        // in days
        public int SuspensionTime { get; set; }
        public string Token { get; set; }
        public virtual Profile UserProfile { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<Text> Texts { get; set; }
        [NotMapped]
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Relationship> Relationships { get; set; }
    }
}
