using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace taohi_backend.Models
{
    public class Image
    {
        public Guid ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string ImageDescription { get; set; }
        public bool Uploaded { get; set; }
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
