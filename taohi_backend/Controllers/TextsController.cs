using taohi_backend.Interfaces;
using taohi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Jwt")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
    public class TextsController : ControllerBase
    {
        private readonly ITextService _textService;
        public UserManager<User> _userManager;
        public TextsController(ITextService textService, UserManager<User> userManager)
        {
            _textService = textService;
            _userManager = userManager;
        }
        // GET: api/Texts
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var texts = await _textService.GetAll(User);
            return Ok(texts);
        }
        // GET: api/Texts
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var texts = await _textService.GetById(id, User);
            return Ok(texts);
        }
        // POST: api/Texts
        [HttpPost]
        public async Task<IActionResult> PostText([FromBody] Text text)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var postedText = await _textService.PostNew(text, User);
            if (postedText == null)
                return BadRequest();

            return Ok(text);
        }

        // PUT: api/Texts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutText([FromRoute] Guid id, [FromBody] Text text)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            text.TextId = id;
            var result = await _textService.PutById(text, User);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        // DELETE: api/Texts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            var deletedText = await _textService.DeleteById(id, User);
            if (deletedText == null)
                return BadRequest();

            return Ok(new { id = deletedText.TextId });
        }

        // POST: api/Texts/ToggleIsPrivate/5
        [HttpPost("ToggleIsPrivate/{id}")]
        public async Task<IActionResult> ToggleIsPrivate(Guid id)
        {
            var text = await _textService.ToggleIsPrivate(id, User);
            if (text == null)
                return BadRequest();

            return Ok(text);
        }
    }
}
