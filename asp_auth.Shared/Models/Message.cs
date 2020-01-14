using System;

namespace taohi_backend.Models
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string MessageContent { get; set; }
        public Guid SenderId { get; set; }
        public User Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
