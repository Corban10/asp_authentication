using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace taohi_backend.Models
{
    public class Relationship
    {
        public Guid LeftUserId { get; set; }
        public User LeftUser { get; set; }
        public Guid RightUserId { get; set; }
        public User RightUser { get; set; }
        [Column("IsFollowing", TypeName = "bit")]
        public bool IsFollowing { get; set; }
        [Column("IsBlocking", TypeName = "bit")]
        public bool IsBlocking { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}