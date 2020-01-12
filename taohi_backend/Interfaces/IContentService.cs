using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Models;

namespace taohi_backend.Interfaces
{
    public interface IContentService<ContentType>
    {
        Task<IEnumerable<ContentType>> GetAll(ClaimsPrincipal claim);
        Task<ContentType> GetById(Guid id, ClaimsPrincipal claim);
        Task<ContentType> PutById(ContentType content, ClaimsPrincipal claim);
        Task<ContentType> DeleteById(Guid id, ClaimsPrincipal claim);
        Task<ContentType> PostNew(ContentType content, ClaimsPrincipal claim);
        Task<ContentType> ToggleIsPrivate(Guid id, ClaimsPrincipal claim);
        ContentType UpdateTimeStamp(ContentType user);
    }
    public interface IVideoService : IContentService<Video>
    {
    }
}
