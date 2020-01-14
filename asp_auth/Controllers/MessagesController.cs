using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using taohi_backend.Interfaces;

namespace taohi_backend.Controllers
{
    [Authorize(Policy = "Bearer")]
    [Authorize(Policy = "IsActive")]
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
