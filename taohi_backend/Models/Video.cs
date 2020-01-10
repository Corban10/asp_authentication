using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace taohi_backend.Models
{
    public class Video
    {

        [Key]
        public Guid VideoId { get; set; }
        [Required]
        public string VideoUrl { get; set; }
        [Required]
        public string VideoTitle { get; set; }
        [Required]
        public string VideoDescription { get; set; }
        [Column("IsPrivate", TypeName = "bit")]
        public bool IsPrivate { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
