using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using taohi_backend.Interfaces;

namespace taohi_backend.Controllers
{
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
