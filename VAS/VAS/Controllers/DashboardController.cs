using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VAS.Model;
using VAS.Service;
using VAS.Utils;

namespace VAS.Controllers
{
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITicketService _ticketService;

        public DashboardController(IMailService mailService, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager, ITicketService ticketService)
        {
            _mailService = mailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _ticketService = ticketService;
        }

        [HttpGet("Account")]
        public ActionResult GetAccount()
        {
            var result = new List<DashboardAccountVM>();
            var total = 0;
            foreach (var item in Enum.GetValues( typeof(UserRoles)))
            {
                if (item.ToString() == nameof(UserRoles.Customer)) continue;
                var quantity = _userManager.GetUsersInRoleAsync(item.ToString()).Result.Count;
                total += quantity;
                result.Add(new DashboardAccountVM
                {
                    Name = item.ToString(),
                    Value = quantity,
                });
            };
            return Ok(result);
        }

        [HttpGet("Ticket")]
        public ActionResult Ticket(int numberDay =7)
        {

            var result = new List<DashboardTicketVM>();
            for(var i = 1; i <= numberDay;i++)
            {
                var date =  DateTime.Now.AddDays(-i);
                int booked =  _ticketService.GetTickets(_ => _.CustomerId != null && _.Block.Date.Date == date.Date).Count();
                int _checked = _ticketService.GetTickets(_ => _.CustomerId != null && _.Block.Date.Date == date.Date && _.Status == 1).Count();
                result.Add(new DashboardTicketVM
                {
                    Date = date,
                    NumberBooked = booked,
                    NumberChecked = _checked
                });
            }
            return Ok(result);
        }
    }
}