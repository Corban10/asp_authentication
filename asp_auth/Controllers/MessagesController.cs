using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using asp_auth.Interfaces;

namespace asp_auth.Controllers
{
    [Authorize(Policy = "Bearer")]
    [Authorize(Roles = "User, Admin")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _service;
        public MessagesController(IMessageService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _service.GetMessages();

            return Ok(messages);
        }
    }
}
