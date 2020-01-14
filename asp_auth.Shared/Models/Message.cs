using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_auth.Models
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string MessageContent { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        [NotMapped]
        public UserViewModel SenderView { get; set; }
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
