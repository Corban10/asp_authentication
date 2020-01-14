using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using asp_auth.Models;

namespace asp_auth.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetMessages();
        Task<Message> GetMessage(Guid id, ClaimsPrincipal claim);
        Task<Message> PostNew(Message content, ClaimsPrincipal claim);
        Task<Message> PutById(Message content, ClaimsPrincipal claim);
        Task<Message> DeleteById(Guid id, ClaimsPrincipal claim);
        UserType UpdateTimeStamp(UserType user);
    }
}
