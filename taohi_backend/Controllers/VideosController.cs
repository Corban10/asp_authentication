using taohi_backend.Interfaces;
using taohi_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "User")]
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
    }
}
