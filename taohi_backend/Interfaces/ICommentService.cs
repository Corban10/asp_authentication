using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetCommentsByContent(Guid contentID, ClaimsPrincipal claim);
        IEnumerable<Comment> GetCommentsByUser(Guid userID, ClaimsPrincipal claim);
        Task<Comment> GetComment(Guid commentID, ClaimsPrincipal claim);
        Task<Comment> PostNew(Comment comment, ClaimsPrincipal claim);
        Task<Comment> PutById(Comment comment, ClaimsPrincipal claim);
        Task<Comment> DeleteCommentById(Guid commentID, ClaimsPrincipal claim);
        Comment UpdateTimeStamp(Comment user);
    }
}
