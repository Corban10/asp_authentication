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
    public class TextService : ITextService
    {
        private readonly AppDbContext _context;
        private UserManager<User> _userManager;
        private readonly IAuthorizationService _authService;
        public TextService(
            AppDbContext context,
            UserManager<User> userManager,
            IAuthorizationService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        public async Task<IEnumerable<Text>> GetAll(ClaimsPrincipal claim) // GetAllRti vs GetAllThi
        {
            var requestingUserId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var requestingUser = await _userManager.FindByIdAsync(requestingUserId);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var text = await _context.Texts.ToListAsync();
            if (requestingUser == null || !text.Any())
                return null;

            var isTaohi = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded;
            return text
                .Where(txt =>
                {
                    var contentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                    return isAdmin || (txt.ContentType == contentType || txt.ContentType == ContentType.Provided);
                })
                .Select(txt =>
                {
                    var user = _userManager.FindByIdAsync(txt.UserId.ToString()).Result;
                    if (user == null || (!isAdmin && txt.IsPrivate == true))
                        return null;

                    txt.UserView = ReturnContentUserViewModel(user);
                    txt.User = null;
                    return txt;
                })
                .Where(txt => txt != null);
        }
        public async Task<Text> GetById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var contentType = (await _authService.AuthorizeAsync(claim, "Taohi")).Succeeded ?
                ContentType.Taohi :
                ContentType.Rangatahi;
            var text = await _context.Texts.FindAsync(id);
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (!isAdmin && text.ContentType != contentType)
                return null;

            text.UserView = ReturnContentUserViewModel(user);
            text.User = null;

            return text;
        }

        public async Task<Text> DeleteById(Guid id, ClaimsPrincipal claim)
        {
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            var text = await _context.Texts.FindAsync(id);
            if (user.Id != text.UserId && !isAdmin)
                return null;

            _context.Texts.Remove(text);
            _context.SaveChanges();

            text.UserView = ReturnContentUserViewModel(user);
            text.User = null;
            return text;
        }

        public async Task<Text> PostNew(Text text, ClaimsPrincipal claim)
        {
            try
            {
                var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return null;

                var isTaohi = _authService.AuthorizeAsync(claim, "Taohi").Result.Succeeded;

                if (text.ContentType != ContentType.Provided)
                    text.ContentType = isTaohi ? ContentType.Taohi : ContentType.Rangatahi;
                if (text.UserId == Guid.Empty)
                    text.UserId = user.Id;

                text = UpdateTimeStamp(text);

                _context.Texts.Add(text);
                _context.SaveChanges();

                if (text.User != null)
                    text.User.PasswordHash = null;

                return text;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Text> PutById(Text model, ClaimsPrincipal claim)
        {
            try
            {
                var text = await _context.Texts.FirstAsync(a => a.TextId == model.TextId);
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var isAdmin = claim.IsInRole(UserType.Admin.ToString());
                if (text == null || (text.UserId.ToString() != userID && !isAdmin))
                    return null;

                text.UserId = ReplacePutProperty(new Guid(userID), text.UserId);
                text.TextContent = ReplacePutProperty(model.TextContent, text.TextContent);
                text = UpdateTimeStamp(text);

                _context.SaveChanges();
                return model;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TextExists(model.TextId)) return null;
                else throw;
            }
        }

        public async Task<Text> ToggleIsPrivate(Guid id, ClaimsPrincipal claim)
        {
            var text = await _context.Texts.FirstAsync(a => a.TextId == id);
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (userId != text.UserId.ToString() && !isAdmin)
                return null;

            text.IsPrivate = !text.IsPrivate;
            _context.SaveChanges();

            var user = await _userManager.FindByIdAsync(userId);
            text.UserView = ReturnContentUserViewModel(user);
            text.User = null;
            return text;
        }

        public Text UpdateTimeStamp(Text text)
        {
            var currentTime = DateTime.Now;
            if (text.CreatedDateTime == null)
                text.CreatedDateTime = currentTime;
            else
                text.UpdatedDateTime = currentTime;
            return text;
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
        private async Task<bool> TextExists(Guid textId) => await _context.Texts.AnyAsync(e => e.TextId == textId);
        private T ReplacePutProperty<T>(T newProp, T oldProp) => newProp == null ? oldProp : newProp;
    }
}
