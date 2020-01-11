using System;
using System.ComponentModel.DataAnnotations;

namespace taohi_backend.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; }
        [Required]
        public string CommentContent { get; set; }
        [Required]
        public Guid ContentId { get; set; }
        [Required]
        public Guid CommenterId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
