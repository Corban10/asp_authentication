using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using asp_auth.Data;
using asp_auth.Interfaces;
using asp_auth.Models;

namespace asp_auth.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _context;
        public MessageService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Message>> GetMessages()
        {
            var all = await _context.Messages.OrderBy(msg => msg.CreatedDateTime).ToListAsync();
            return all == null ? null : all.Select(message =>
            {
                var user = _context.Users.Find(message.SenderId);
                if (user == null || user.IsActive == false)
                    return null;
                message.SenderView = ReturnUserViewModel(user);
                message.Sender = null;
                message.Receiver = null;
                return message;
            }).Where(message => message != null);
        }
        public Task<Message> DeleteById(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Message> GetMessage(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Message> PostNew(Message content, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public Task<Message> PutById(Message content, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }
        public UserType UpdateTimeStamp(UserType user)
        {
            throw new NotImplementedException();
        }
        private UserViewModel ReturnUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                UserType = user.UserType,
                IsActive = user.IsActive
            };
        }
    }
}
