using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using VAS.Hubs;
using VAS.Model;
using VAS.Service;
using VAS.Utils;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/Ticket")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        #region properties
        private readonly UserManager<MyUser> _userManager;
        private readonly ITicketService _ticketService;
        private readonly IBlockService _blockService;
        private readonly IFamilyService _familyService;
        private readonly ICustomerService _customerService;
        private readonly IDoctorBasicService _doctorService;
        private readonly IHubContext<CenterHub> _hubContext;

        public TicketController(UserManager<MyUser> userManager, ITicketService ticketService, IBlockService blockService, IFamilyService familyService, ICustomerService customerService, IDoctorBasicService doctorService, IHubContext<CenterHub> hubContext)
        {
            _userManager = userManager;
            _ticketService = ticketService;
            _blockService = blockService;
            _familyService = familyService;
            _customerService = customerService;
            _doctorService = doctorService;
            _hubContext = hubContext;
        }

        #endregion



        /// lấy tất cả tickets trong hôm nay để nhân viên bệnh viện kiểm tra lúc check in
        [HttpGet("Today")]
        public ActionResult GetAllTicketsToday(DateTime? time)
        {
            var now = time == null ? DateTime.Now : time.Value;
            try
            {
                var tickets = _ticketService.GetTickets(_ => _.Block.Date.Date == now.Date && _.CustomerId != null)
                    .OrderByDescending(_ => _.Block.StartTime);
                var ticketsVM = new List<AllTicketTodayVM>();
                var result = new List<PastNowFutureTicketVM>();
                foreach (var ticket in tickets)
                {
                    var ticketVM = ticket.Adapt<AllTicketTodayVM>();
                    //block now
                    if (ticket.Block.StartTime >= now.TimeOfDay.Subtract(new TimeSpan(0, 30, 0)) 
                        && ticket.Block.StartTime <= now.TimeOfDay)
                    {
                        ticketVM.TicketType = "Now";
                    }
                    if (ticket.Block.StartTime < now.TimeOfDay.Subtract(new TimeSpan(0, 30, 0)))
                    {
                        ticketVM.TicketType = "Past";
                    }
                    if (ticket.Block.StartTime > now.TimeOfDay)
                    {
                        ticketVM.TicketType = "Future";
                    }
                    var cus = _customerService.GetCustomer(Guid.Parse(ticket.CustomerId));
                    ticketVM.CustomerName = cus.FullName;
                    ticketVM.CustomerCMDN = cus.CMND;
                    ticketVM.CustomerPhoneNumber = cus.PhoneNumber;
                    ticketVM.DoctorId = ticket.Block.DoctorId;
                    ticketVM.DoctorName = _doctorService.GetDoctorBasic(ticket.Block.DoctorId).FullName;
                    ticketVM.StartTime = ticket.Block.StartTime;
                    ticketsVM.Add(ticketVM);
                }

                var groupTicketType = ticketsVM.GroupBy(_ => _.TicketType);
                foreach (var data in groupTicketType)
                {
                    PastNowFutureTicketVM passNowFuture = new PastNowFutureTicketVM();
                    passNowFuture.TicketType = data.Key;
                    passNowFuture.Tickets.AddRange(data.Select(_ => _.Adapt<AllTicketTodayVM>()));
                    result.Add(passNowFuture);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Mobike
        /// </summary>
        /// Check xem khách hàng đã đặt hay chưa
        private Boolean CanBook(Guid customerId)

        {
            var ticket = _ticketService.GetTickets(t => t.CustomerId == customerId.ToString()
                                            && t.Block.Date.Date >= DateTime.Now.Date
                                            && t.Status == 0).FirstOrDefault();
            return ticket == null;
        }

        /// <summary>
        /// Mobile
        /// </summary>
        [HttpPut("ChangeBook")]
        public async Task<ActionResult> ChangeBookingAsync([FromBody]BookingCM model)
        {
            if (CanBook(model.CustomerId)) return Ok(false);
            try
            {
                var customer = _customerService.GetCustomer(model.CustomerId);
                if (customer != null)
                {
                    string currentUserName = (await _userManager.GetUserAsync(User)).UserName;

                    //BlockId da duoc check isFull va time lon hon time hien tai
                    var tickets = _ticketService.GetTickets(t => t.BlockId == model.BlockId).ToList();

                    List<Ticket> newTickets = new List<Ticket>();
                    //Check xem Khach hang này đã đặt trong block nay chưa
                    foreach (var item in tickets)
                    {
                        if (item.CustomerId == null)
                            newTickets.Add(item);
                        else if (item.CustomerId.Equals(model.CustomerId.ToString()))
                            return Ok(false);
                    }

                    if (newTickets.Count != 0)
                    {
                        //Xoa customer tu Ticket cu va check xem isFull=true thi set isFull = false
                        var currentTicket = _ticketService.GetTickets(t => t.CustomerId == model.CustomerId.ToString()
                        && t.Block.Date.Date >= DateTime.Now.Date
                        && t.Status == 0).FirstOrDefault();
                        currentTicket.CustomerId = null;
                        currentTicket.Note = null;
                        if (currentTicket.Block.IsFull)
                        {
                            currentTicket.Block.IsFull = false;
                        }
                        _ticketService.UpdateTicket(currentTicket, currentUserName);

                        //Gan customer vao ticket va check xem neu con 1 ticket thi set block isFull = true
                        newTickets[0].CustomerId = model.CustomerId.ToString();
                        newTickets[0].BookingDate = DateTime.Now;
                        newTickets[0].Note = model.Note;
                        _ticketService.UpdateTicket(newTickets[0], currentUserName);
                        if (newTickets.Count == 1) // Chỉ còn 1 ticket thì update isFull
                        {
                            var block = _blockService.GetBlock(model.BlockId);
                            block.IsFull = true;
                            _blockService.UpdateBlock(block, currentUserName);
                        }
                        _blockService.Save();
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return BadRequest("Customer does not existed!");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Mobile
        /// </summary>
        ///  Tìm ra Ticket đầu tiên rồi gắn cusId vào ****Nếu còn 1 Ticket thì update isFull cho block đó****
        [HttpPost("Book")]
        public async Task<ActionResult> BookingAsync([FromBody]BookingCM model)
        {
            if (!CanBook(model.CustomerId)) return Ok(false);

            try
            {
                var customer = _customerService.GetCustomer(model.CustomerId);
                if (customer != null)
                {
                    string currentUserName = (await _userManager.GetUserAsync(User)).UserName;
                    var tickets = _ticketService.GetTickets(t => t.BlockId == model.BlockId).ToList();
                    List<Ticket> newTickets = new List<Ticket>();
                    //Check xem Khach hang này đã đặt trong block nay chưa
                    foreach (var item in tickets)
                    {
                        if (item.CustomerId == null)
                            newTickets.Add(item);
                        else if (item.CustomerId.Equals(model.CustomerId.ToString()))
                            return Ok(false);
                    }

                    if (newTickets.Count != 0)
                    {
                        newTickets[0].CustomerId = model.CustomerId.ToString();
                        newTickets[0].BookingDate = DateTime.Now;
                        newTickets[0].Note = model.Note;
                        _ticketService.UpdateTicket(newTickets[0], currentUserName);
                        if (newTickets.Count == 1) // Chỉ còn 1 ticket thì update isFull
                        {
                            var block = _blockService.GetBlock(model.BlockId);
                            block.IsFull = true;
                            _blockService.UpdateBlock(block, currentUserName);
                        }
                        _blockService.Save();
                        _hubContext.Clients.All.SendAsync("Notify", JsonConvert.SerializeObject(new { Mess = "Co nguoi moi dat lich" }));
                        return Ok(true);
                    }
                    else
                    {
                        return Ok(false);
                    }
                }
                else
                {
                    return BadRequest("Customer does not existed!");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// khi khách hàng đến check-in, điều dưỡng sẽ check là KH đã có mặt
        [HttpPut("Checkin")]
        public async Task<ActionResult> UpdateArrivedCustomerStatusAsync(List<string> cusIds)
        {
            try
            {
                foreach (var cusId in cusIds)
                {
                    var user = (await _userManager.GetUserAsync(User)).UserName;
                    var ticket = _ticketService.GetTickets(_ => _.CustomerId == cusId
                    && _.Block.Date.Date >= DateTime.Now.Date
                    && _.Status == 0).FirstOrDefault();
                    if (ticket == null)
                    {
                        return BadRequest("Khách hàng này chưa đặt lịch.");
                    }
                    ticket.Status = (int)CustomerStatus.Arrived;
                    _ticketService.UpdateTicket(ticket, user);
                }
                _ticketService.Save();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }


    }
}
