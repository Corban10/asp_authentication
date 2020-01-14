using System;

namespace taohi_backend.Models
{
    public class Comment
    {
        public Guid CommentId { get; set; }
        public string CommentContent { get; set; }
        public Guid ContentId { get; set; }
        public Guid CommenterId { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}