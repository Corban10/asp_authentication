using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using taohi_backend.Interfaces;
using taohi_backend.Models;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Bearer")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _service;
        public CommentsController(ICommentService service)
        {
            _service = service;
        }

        // GET: api/Comments/ByContent/5
        [HttpGet("ByContent/{id}")]
        public async Task<IActionResult> GetCommentsByContent([FromRoute] Guid id)
        {
            var comments = await _service.GetCommentsByContent(id, User);

            if (comments == null)
                return NotFound();

            return Ok(comments);
        }

        // GET: api/Comments/ByContent/5
        [HttpGet("ByUser/{id}")]
        public IActionResult GetCommentsByUser([FromRoute] Guid id)
        {
            var comments = _service.GetCommentsByUser(id, User);

            if (comments == null)
                return NotFound();

            return Ok(comments);
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment([FromRoute] Guid id)
        {
            var comments = await _service.GetComment(id, User);

            if (comments == null)
                return NotFound();

            return Ok(comments);
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var postedComment = await _service.PostNew(comment, User);
            if (postedComment == null)
                return BadRequest();

            return CreatedAtAction("PostComment", new { id = comment.CommentId }, comment);
        }

        // PUT: api/Videos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment([FromRoute] Guid id, [FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            comment.CommentId = id;
            var result = await _service.PutById(comment, User);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var deletedComment = await _service.DeleteCommentById(id, User);
            if (deletedComment == null)
                return BadRequest();

            return Ok(new { id = deletedComment.CommentId });
        }
    }
}
