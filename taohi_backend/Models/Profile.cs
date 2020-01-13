using System;

namespace taohi_backend.Models
{
    public class Profile
    {
        public Guid ProfileId { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string ProfileBio { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}