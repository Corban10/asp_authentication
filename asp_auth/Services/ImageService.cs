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
    public class ImageService : IImageService
    {
        private readonly AppDbContext _context;
        private UserManager<User> _userManager;
        private readonly IAuthorizationService _authService;
        public ImageService(
            AppDbContext context,
            UserManager<User> userManager,
            IAuthorizationService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<IEnumerable<Image>> GetAll(ClaimsPrincipal claim) // GetAllRti vs GetAllThi
        {
            var requestingUserId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var images = await _context.Images.ToListAsync();
            if (requestingUser == null || !images.Any())
                return null;

            var isTaohi = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded;
            return images
                .Where(img =>
                {
                    var contentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                    return isAdmin || (img.ContentType == contentType || img.ContentType == ContentType.Provided);
                })
                .Select(img =>
                {
                    var user = _userManager.FindByIdAsync(img.UserId.ToString()).Result;
                    if (user == null || (!isAdmin && img.IsPrivate == true))
                        return null;

                    img.UserView = ReturnContentUserViewModel(user);
                    img.User = null;
                    return img;
                })
                .Where(img => img != null);
        }
        public async Task<Image> GetById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var contentType = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded ?
                ContentType.Taohi :
                ContentType.Rangatahi;
            var image = await _context.Images.FindAsync(id);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (!isAdmin && image.ContentType != contentType)
                return null;

            image.UserView = ReturnContentUserViewModel(user);
            image.User = null;

            return image;
        }

        public async Task<Image> DeleteById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var image = await _context.Images.FindAsync(id);
            if (user.Id != image.UserId && !isAdmin)
                return null;

            _context.Images.Remove(image);
            _context.SaveChanges();

            image.UserView = ReturnContentUserViewModel(user);
            image.User = null;
            return image;
        }

        public async Task<Image> PostNew(Image image, ClaimsPrincipal claim)
        {
            try
            {
                var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                var isTaohi = _authService.AuthorizeAsync(claim, "Taohi").Result.Succeeded;

                if (image.ContentType != ContentType.Provided)
                    image.ContentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                if (image.UserId == Guid.Empty)
                    image.UserId = user.Id;

                image = UpdateTimeStamp(image);

                _context.Images.Add(image);
                _context.SaveChanges();

                if (image.User != null)
                    image.User.PasswordHash = null;

                return image;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Image> PutById(Image model, ClaimsPrincipal claim)
        {
            try
            {
                var image = await _context.Images.FirstAsync(a => a.ImageId == model.ImageId);
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var isAdmin = claim.IsInRole(UserType.Admin.ToString());
                if (image == null || (image.UserId.ToString() != userID && !isAdmin))
                    return null;

                image.UserId = ReplacePutProperty(new Guid(userID), image.UserId);
                image.ImageUrl = ReplacePutProperty(model.ImageUrl, image.ImageUrl);
                image.ImageDescription = ReplacePutProperty(model.ImageDescription, image.ImageDescription);
                image = UpdateTimeStamp(image);

                _context.SaveChanges();
                return model;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ImageExists(model.ImageId)) return null;
                else throw;
            }
        }

        public async Task<Image> ToggleIsPrivate(Guid id, ClaimsPrincipal claim)
        {
            var image = await _context.Images.FirstAsync(a => a.ImageId == id);
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (userId != image.UserId.ToString() && !isAdmin)
                return null;

            image.IsPrivate = !image.IsPrivate;
            _context.SaveChanges();

            var user = await _userManager.FindByIdAsync(userId);
            image.UserView = ReturnContentUserViewModel(user);
            image.User = null;
            return image;
        }

        public Image UpdateTimeStamp(Image image)
        {
            var currentTime = DateTime.Now;
            if (image.CreatedDateTime == null)
                image.CreatedDateTime = currentTime;
            else
                image.UpdatedDateTime = currentTime;
            return image;
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
        private async Task<bool> ImageExists(Guid imageId) => await _context.Images.AnyAsync(e => e.ImageId == imageId);
        private T ReplacePutProperty<T>(T newProp, T oldProp) => newProp == null ? oldProp : newProp;
    }
}
