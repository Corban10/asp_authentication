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
            var requestingUserId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var videos = await _context.Videos.ToListAsync();
            if (requestingUser == null || !videos.Any())
                return null;

            var isTaohi = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded;
            return videos
                .Where(vid =>
                {
                    var contentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                    return isAdmin || (vid.ContentType == contentType || vid.ContentType == ContentType.Provided);
                })
                .Select(vid =>
                {
                    var user = _userManager.FindByIdAsync(vid.UserId.ToString()).Result;
                    if (user == null || (!isAdmin && vid.IsPrivate == true))
                        return null;

                    vid.UserView = ReturnContentUserViewModel(user);
                    vid.User = null;
                    return vid;
                })
                .Where(vid => vid != null);
        }
        public async Task<Video> GetById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var contentType = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded ?
                ContentType.Taohi :
                ContentType.Rangatahi;
            var video = await _context.Videos.FindAsync(id);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (!isAdmin && video.ContentType != contentType)
                return null;

            video.UserView = ReturnContentUserViewModel(user);
            video.User = null;

            return video;
        }

        public async Task<Video> DeleteById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var video = await _context.Videos.FindAsync(id);
            if (user.Id != video.UserId && !isAdmin)
                return null;

            _context.Videos.Remove(video);
            _context.SaveChanges();

            video.UserView = ReturnContentUserViewModel(user);
            video.User = null;
            return video;
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

        public async Task<Video> PutById(Video model, ClaimsPrincipal claim)
        {
            try
            {
                var video = await _context.Videos.FirstAsync(a => a.VideoId == model.VideoId);
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var isAdmin = claim.IsInRole(UserType.Admin.ToString());
                if (video == null || (video.UserId.ToString() != userID && !isAdmin))
                    return null;

                video.UserId = ReplacePutProperty(new Guid(userID), video.UserId);
                video.VideoUrl = ReplacePutProperty(model.VideoUrl, video.VideoUrl);
                video.VideoTitle = ReplacePutProperty(model.VideoTitle, video.VideoTitle);
                video.VideoDescription = ReplacePutProperty(model.VideoDescription, video.VideoDescription);
                video = UpdateTimeStamp(video);

                _context.SaveChanges();
                return model;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await VideoExists(model.VideoId)) return null;
                else throw;
            }
        }

        public async Task<Video> ToggleIsPrivate(Guid id, ClaimsPrincipal claim)
        {
            var video = await _context.Videos.FirstAsync(a => a.VideoId == id);
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (userId != video.UserId.ToString() && !isAdmin)
                return null;

            video.IsPrivate = !video.IsPrivate;
            _context.SaveChanges();

            var user = await _userManager.FindByIdAsync(userId);
            video.UserView = ReturnContentUserViewModel(user);
            video.User = null;
            return video;
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
        private UserViewModel ReturnContentUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                IsActive = user.IsActive,
            };
        }
        // not inherited by interface
        private async Task<bool> VideoExists(Guid videoID) => await _context.Videos.AnyAsync(e => e.VideoId == videoID);
        private T ReplacePutProperty<T>(T newProp, T oldProp) => newProp == null ? oldProp : newProp;
    }
}
