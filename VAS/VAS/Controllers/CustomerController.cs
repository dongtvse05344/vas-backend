using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VAS.Model;
using VAS.Service;
using VAS.Utils;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/Customer")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        #region properties
        private readonly UserManager<MyUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IFamilyService _familyService;
        private readonly ITicketService _ticketService;
        private readonly IDoctorBasicService _doctorService;
        private readonly IBlockService _blockService;

        public CustomerController(UserManager<MyUser> userManager, ICustomerService customerService, IFamilyService familyService, ITicketService ticketService, IDoctorBasicService doctorService, IBlockService blockService)
        {
            _userManager = userManager;
            _customerService = customerService;
            _familyService = familyService;
            _ticketService = ticketService;
            _doctorService = doctorService;
            _blockService = blockService;
        }
        #endregion

        
        [HttpGet]
        public ActionResult GetAllCustomer(string fullName, string phoneNumber, int index = 1, int pageSize = 5)
        {
            fullName = fullName == null ? "" : fullName;
            phoneNumber = phoneNumber == null ? "" : phoneNumber;
            var data = _customerService.GetCustomers(_ => _.FullName.Contains(fullName) && phoneNumber.Contains(phoneNumber));
            return Ok(data.ToPageList<CustomerVM, Customer>(index, pageSize));

        }


        /// <summary>
        /// Mobile
        /// </summary>
        /// Lấy hồ sơ của 1 tài khoảng ( SĐT )
        [HttpGet("Family")]
        public async Task<ActionResult> CustomerAsync(string phoneNumber, string fullName)
        {
            fullName = fullName == null ? "" : fullName;
            phoneNumber = phoneNumber == null ? "" : phoneNumber;

            var myUser = await _userManager.GetUserAsync(User);
            var customers = new List<CustomerVM>();
            foreach (var item in myUser.Families)
            {
                var customer = item.Customer;
                if (customer.PhoneNumber.Contains(phoneNumber) && customer.FullName.Contains(fullName))
                {
                    var _customer = customer.Adapt<CustomerVM>();
                    var currentTicket = _ticketService.GetTickets(t => t.CustomerId == item.CustomerId.ToString()).Where(t =>
                    t.Block.Date.Date >= DateTime.Now.Date
                    && t.Status == 0)
                    .FirstOrDefault();
                    if (currentTicket != null) _customer.IsBooking = true;
                    else _customer.IsBooking = false;
                    customers.Add(_customer);
                }

            }
            return Ok(customers);
        }

        /// <summary>
        /// Mobile
        /// </summary>
        /// Lấy thông tin đầy đủ của 1 hồ sơ
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult GetCustomerDetail(Guid id)
        {
            try
            {
                var customer = _customerService.GetCustomer(id);
                if(customer != null)
                {
                    return Ok(customer.Adapt<CustomerDetailVM>());
                }
                else
                {
                    return NotFound();
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
        /// Lay lich su da~ dat cua 1 benh nhan
		[HttpGet("{id}/HistoryBook")]
		public async Task<ActionResult> GetCustomerHistoryBookingAsync(Guid id)
		{
			var customer = _customerService.GetCustomer(id);
			if (customer == null) return NotFound();

			List<TicketVM> listTicketVM = new List<TicketVM>();
			var item = new CustomerTicketHistoryVM();
			item.CustomerId = customer.Id;
			item.CustomerName = customer.FullName;

			var tickets = _ticketService
						.GetTickets(_ => _.CustomerId == customer.Id.ToString())
						.OrderByDescending(_ => _.BookingDate).ToList();
			if (tickets.Count != 0)
			{
				foreach (var ticket in tickets)
				{
					var ticketVM = ticket.Adapt<TicketVM>();
					ticketVM.StartTime = ticket.Block.StartTime;
					ticketVM.DoctorName = _doctorService.GetDoctorBasic(ticket.Block.DoctorId).FullName;
					ticketVM.Date = ticket.Block.Date;
					listTicketVM.Add(ticketVM);
				}
				item.Tickets = listTicketVM;

			}
			return Ok(item);
		}

        /// <summary>
        /// Mobile
        /// </summary>
        /// Check xem customer đã đặt lịch hay chưa , lấy cả ngày hôm nay 
        /// Lay nhung dua co status = 0
        [HttpGet("{Id}/FutureTicket")]
        public ActionResult CheckBocked(Guid id)
        {
            try
            {
                var ticket = _ticketService.GetTickets(t => t.CustomerId == id.ToString()
                                            && t.Block.Date.Date >= DateTime.Now.Date
                                            && t.Status == 0).FirstOrDefault();
                var ticketVM = ticket.Adapt<TicketVM>();
                if (ticketVM != null)
                {
                    ticketVM.StartTime = ticket.Block.StartTime;
                    ticketVM.DoctorName = _doctorService.GetDoctorBasic(ticket.Block.DoctorId).FullName;
                    ticketVM.Date = ticket.Block.Date;
                    return Ok(ticketVM);
                }
                else
                {
                    return Ok(false);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mobile
        /// </summary>
        /// Tạo 1 hồ sơ mới
        [HttpPost]
        public async Task<ActionResult> CreateCustomerAsync(CustomerCM model)
        {
            try
            {
                model.Relationship = model.Relationship == null ? "Mine" : model.Relationship;
                var myUser = await _userManager.GetUserAsync(User);
                var customer = model.Adapt<Customer>();
                _customerService.CreateCustomer(customer, myUser.UserName);

                var id = customer.Id;
                var family = new Family()
                {
                    CustomerId = id,
                    MyUserId = myUser.Id,
                    Relationship = model.Relationship
                };
                _familyService.CreateFamily(family,myUser.UserName);
                _familyService.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Mobile
        /// </summary>
        [HttpPost("His")]
        public ActionResult CreateCustomerByHisCode(CustomerCMHis model)
        {
            return Ok();
        }

        /// <summary>
        /// Mobile
        /// </summary>
        /// Update hồ sơ của bệnh nhân
        [HttpPut]
        public async Task<ActionResult> UpdateCustomerAsync(CustomerUM model)
        {
            try
            {
				var user = (await _userManager.GetUserAsync(User)).UserName;

				var customer = _customerService.GetCustomer(model.Id);
                customer = model.Adapt(customer);

				if (model.Relationship != null)
				{
					var family = model.Adapt(customer.Families.FirstOrDefault());
					family.Relationship = model.Relationship;
					_familyService.UpdateFamily(family, user);
				}
                _customerService.UpdateCustomer(customer,user);
                _customerService.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        /// <summary>
        /// Mobile
        /// </summary>
        [HttpDelete("{Id}/Ticket")]
        public async Task<ActionResult> CancelBookingAsync(Guid cusId)
        {
            try
            {
                var user = (await _userManager.GetUserAsync(User)).UserName;
                var ticket = _ticketService.GetTickets(_ => _.CustomerId == cusId.ToString() && _.Block.Date.Date >= DateTime.Now.Date).FirstOrDefault();

                if (ticket == null)
                {
                    return BadRequest("Hồ sơ này không có lịch để hủy.");
                }
                if (ticket.Block.StartTime <= DateTime.Now.TimeOfDay)
                {
                    return BadRequest("Đã quá trễ để hủy lịch hôm nay.");
                }

                ticket.BookingDate = null;
                ticket.Note = null;
                ticket.CustomerId = null;

                //Thay đổi isFull ở bảng block
                if (ticket.Block.IsFull)
                {
                    ticket.Block.IsFull = false;
                    _blockService.UpdateBlock(ticket.Block, user);
                }
                _ticketService.UpdateTicket(ticket, user);
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