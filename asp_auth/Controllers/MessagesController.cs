using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using asp_auth.Interfaces;

namespace asp_auth.Controllers
{
    [Authorize(Roles = "User, Admin")]
    [Authorize(Policy = "IsActive")]
    public class MessagesController : Controller
    {
        private readonly IMessageService _service;
        public MessagesController(IMessageService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var messages = await _service.GetMessages();

            ViewBag.Messages = messages;

            return View();
        }
        [Authorize(Policy = "Bearer")]
        [HttpGet("GetMessages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _service.GetMessages();

            return Ok(messages);
        }
    }
}
