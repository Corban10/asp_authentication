using taohi_backend.Interfaces;
using taohi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Bearer")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;
        public UserManager<User> _userManager;
        public VideosController(IVideoService videoService, UserManager<User> userManager)
        {
            _videoService = videoService;
            _userManager = userManager;
        }
        // GET: api/Videos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var videos = await _videoService.GetAll(User);
            return Ok(videos);
        }
        // GET: api/Videos
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var videos = await _videoService.GetById(id, User);
            return Ok(videos);
        }
        // POST: api/Videos
        [HttpPost]
        public async Task<IActionResult> PostVideo([FromBody] Video video)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var postedVideo = await _videoService.PostNew(video, User);
            if (postedVideo == null)
                return BadRequest();

            return Ok(video);
        }

        // PUT: api/Videos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo([FromRoute] Guid id, [FromBody] Video video)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            video.VideoId = id;
            var result = await _videoService.PutById(video, User);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            var deletedVideo = await _videoService.DeleteById(id, User);
            if (deletedVideo == null)
                return BadRequest();

            return Ok(new { id = deletedVideo.VideoId });
        }

        // POST: api/Videos/ToggleIsPrivate/5
        [HttpPost("ToggleIsPrivate/{id}")]
        public async Task<IActionResult> ToggleIsPrivate(Guid id)
        {
            var video = await _videoService.ToggleIsPrivate(id, User);
            if (video == null)
                return BadRequest();

            return Ok(video);
        }
    }
}
