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
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;
        public CommentService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Comment>> GetCommentsByContent(Guid contentID, ClaimsPrincipal claim)
        {
            var comments = await _context.Comments.ToListAsync();
            if (claim.IsInRole(UserType.Admin.ToString()))
                return comments;

            return comments.Select(c =>
            {
                var user = _context.Users.Find(c.CommenterId);
                if (user == null ||
                    user.IsActive == false ||
                    c.ContentId != contentID)
                    return null;
                return c;
            })
            .Where(c => c != null);
        }
        public IEnumerable<Comment> GetCommentsByUser(Guid id, ClaimsPrincipal claim)
        {
            var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());
            if (id.ToString() != userID && !isAdmin)
                return null;

            return _context.Comments.Where(c => c.CommenterId == id);
        }
        public async Task<Comment> GetComment(Guid commentID, ClaimsPrincipal claim)
        {
            var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var isAdmin = claim.IsInRole(UserType.Admin.ToString());

            var comment = await _context.Comments.FindAsync(commentID);
            var user = await _context.Users.FindAsync(comment.CommenterId);
            if (comment == null ||
                user == null ||
                !user.IsActive ||
                (!isAdmin && comment.CommenterId.ToString() != userID))
                return null;
            return comment;
        }
        public async Task<Comment> PostNew(Comment comment, ClaimsPrincipal claim)
        {
            try
            {
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _context.Users.FindAsync(userID);
                if (user == null)
                    return null;

                if (comment.CommenterId == Guid.Empty)
                    comment.CommenterId = user.Id;

                // var video = await _context.Users.FindAsync(comment.ContentID);

                comment = UpdateTimeStamp(comment);

                _context.Comments.Add(comment);
                _context.SaveChanges();
                return comment;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<Comment> PutById(Comment newComment, ClaimsPrincipal claim)
        {
            try
            {
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var isAdmin = claim.IsInRole(UserType.Admin.ToString());

                Comment oldComment = await _context.Comments.FirstAsync(a => a.CommentId == newComment.CommentId);
                if (oldComment == null || (oldComment.CommenterId.ToString() != userID && !isAdmin)) return null;

                oldComment.CommentContent = ReplacePutProperty(newComment.CommentContent, oldComment.CommentContent);
                oldComment = UpdateTimeStamp(oldComment);

                _context.SaveChanges();
                return oldComment;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CommentExists(newComment.CommentId)) return null;
                else throw;
            }
        }
        public async Task<Comment> DeleteCommentById(Guid commentID, ClaimsPrincipal claim)
        {
            try
            {
                var userID = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
                var isAdmin = claim.IsInRole(UserType.Admin.ToString());

                var comment = await _context.Comments.FindAsync(commentID);
                if ((comment.CommenterId.ToString() != userID && !isAdmin) || comment == null)
                    return null;

                _context.Comments.Remove(comment);
                _context.SaveChanges();

                return comment;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public Comment UpdateTimeStamp(Comment comment)
        {
            var currentTime = DateTime.Now;
            if (comment.CreatedDateTime == null)
                comment.CreatedDateTime = currentTime;
            else
                comment.UpdatedDateTime = currentTime;
            return comment;
        }

        // not inherited by interface
        private async Task<bool> CommentExists(Guid commentID) => await _context.Comments.AnyAsync(e => e.CommentId == commentID);
        public T ReplacePutProperty<T>(T newProp, T oldProp) => newProp == null ? oldProp : newProp;
    }
}
