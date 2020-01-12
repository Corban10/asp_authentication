using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using taohi_backend.Data;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Services
{
    public class VideoService : IVideoService
    {
        private readonly AppDbContext _context;
        private UserManager<User> _userManager;
        private readonly IAuthorizationService _authService;
        public VideoService(
            AppDbContext context,
            UserManager<User> userManager,
            IAuthorizationService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<IEnumerable<Video>> GetAll(ClaimsPrincipal claim) // GetAllRti vs GetAllThi
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var requestingUser = await _userManager.FindByIdAsync(userId);
            var isAdmin = await _userManager.IsInRoleAsync(requestingUser, "Admin");
            var videos = await _context.Videos.ToListAsync();
            if (requestingUser == null || !videos.Any())
                return null;

            var isTaohi = _authService.AuthorizeAsync(claim, "Taohi").Result.Succeeded;

            return videos
                .Where(vid =>
                {
                    var contentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                    return (vid.ContentType == contentType || vid.ContentType == ContentType.Provided);
                })
                .Select(vid =>
                {
                    var user = _userManager.FindByIdAsync(vid.UserId.ToString()).Result;
                    if (user == null || (!isAdmin && vid.IsPrivate == true))
                        return null;
                    if (user.Id.ToString() == userId)
                    {
                        vid.User = user;
                        vid.User.PasswordHash = null;
                        vid.User.Videos = null;
                        return vid;
                    }
                    vid.User = new User();
                    vid.User.DisplayName = user.DisplayName;
                    vid.UserId = Guid.Empty;
                    return vid;
                })
                .Where(vid => vid != null);
        }
        public Task<Video> GetById(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }

        public Task<Video> DeleteById(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }

        public async Task<Video> PostNew(Video video, ClaimsPrincipal claim)
        {
            try
            {
                var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                var isTaohi = _authService.AuthorizeAsync(claim, "Taohi").Result.Succeeded;

                if (video.ContentType != ContentType.Provided)
                    video.ContentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                if (video.UserId == Guid.Empty)
                    video.UserId = user.Id;

                video = UpdateTimeStamp(video);

                _context.Videos.Add(video);
                _context.SaveChanges();

                if (video.User != null)
                    video.User.PasswordHash = null;

                return video;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<Video> PutById(Video content, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }

        public Task<Video> ToggleIsPrivate(Guid id, ClaimsPrincipal claim)
        {
            throw new NotImplementedException();
        }

        public Video UpdateTimeStamp(Video video)
        {
            var currentTime = DateTime.Now;
            if (video.CreatedDateTime == null)
                video.CreatedDateTime = currentTime;
            else
                video.UpdatedDateTime = currentTime;
            return video;
        }
    }
}
