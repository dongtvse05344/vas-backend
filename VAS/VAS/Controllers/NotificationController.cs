using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using VAS.Hubs;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<CenterHub> _hubContext;

        public NotificationController(IHubContext<CenterHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public ActionResult SendNotification([FromBody] NotificationCM model)
        {
            _hubContext.Clients.All.SendAsync("Notify", JsonConvert.SerializeObject(model));
            return Ok();
        }
    }
}