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
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;
        public UserManager<User> _userManager;
        public ImagesController(IImageService imageService, UserManager<User> userManager)
        {
            _imageService = imageService;
            _userManager = userManager;
        }
        // GET: api/Images
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _imageService.GetAll(User);
            return Ok(images);
        }
        // GET: api/Images
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var images = await _imageService.GetById(id, User);
            return Ok(images);
        }
        // POST: api/Images
        [HttpPost]
        public async Task<IActionResult> PostImage([FromBody] Image image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var postedImage = await _imageService.PostNew(image, User);
            if (postedImage == null)
                return BadRequest();

            return Ok(image);
        }

        // PUT: api/Images/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImage([FromRoute] Guid id, [FromBody] Image image)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            image.ImageId = id;
            var result = await _imageService.PutById(image, User);
            if (result == null)
                return BadRequest();

            return Ok(result);
        }

        // DELETE: api/Images/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var deletedImage = await _imageService.DeleteById(id, User);
            if (deletedImage == null)
                return BadRequest();

            return Ok(new { id = deletedImage.ImageId });
        }

        // POST: api/Images/ToggleIsPrivate/5
        [HttpPost("ToggleIsPrivate/{id}")]
        public async Task<IActionResult> ToggleIsPrivate(Guid id)
        {
            var image = await _imageService.ToggleIsPrivate(id, User);
            if (image == null)
                return BadRequest();

            return Ok(image);
        }
    }
}
