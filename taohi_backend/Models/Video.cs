using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace taohi_backend.Models
{
    public class Video
    {
        public Guid VideoId { get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }
        [Column("IsPrivate", TypeName = "bit")]
        public bool IsPrivate { get; set; }
        public ContentType ContentType { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
        [NotMapped]
        public UserViewModel UserView { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
