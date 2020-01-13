using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
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
