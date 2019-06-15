using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VAS.Model;
using VAS.Service;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/Block")]
    [ApiController]
    [Authorize]
    public class BlockController : ControllerBase
    {
        #region properties
        private readonly IBlockService _blockService;
        private readonly IDoctorBasicService _doctorService;
        private readonly ISchedulingService _schedulingService;
        private readonly ICustomerService _customerService;
        private readonly ITicketService _ticketService;

        public BlockController(IBlockService blockService, IDoctorBasicService doctorService, ISchedulingService schedulingService, ICustomerService customerService, ITicketService ticketService)
        {
            _blockService = blockService;
            _doctorService = doctorService;
            _schedulingService = schedulingService;
            _customerService = customerService;
            _ticketService = ticketService;
        }
        #endregion



        /// Trả về các block của ngày hôm đó kèm theo khách hàng đặt lịch - Nếu không có đặt sẽ là Null ở mỗi ticket
        /// Chia theo khoa -> Theo bác sĩ -> các block -> các ticket của block
        [HttpGet("OnDate")]
        public ActionResult AllBlock([FromQuery]DateTime date, string specialityId, string doctorId)
        {
            try
            {
                doctorId = doctorId == null ? "" : doctorId;
                specialityId = specialityId == null ? "" : specialityId;
                var blocks = _blockService.GetBlocks(b => b.Date == date && b.IsAvailable && b.DoctorId.Contains(doctorId) && b.Scheduling.SpecialityId.Contains(specialityId));
                var groupBySpeciality = blocks.GroupBy(s => s.Scheduling.SpecialityName);
                List<ManageBlockVM> result = new List<ManageBlockVM>();
                //Lấy block theo speciality
                foreach (var speciality in groupBySpeciality)
                {
                    var itemSpeciality = new ManageBlockVM()
                    {
                        SpecialityName = speciality.Key,
                        SpecialityId = speciality.ToList()[0].Scheduling.SpecialityId
                    };
                    var groupByDoctor = speciality.GroupBy(_ => _.DoctorId);
                    //Lấy block theo doctor
                    foreach (var doctor in groupByDoctor)
                    {
                        var itemDoctor = new DoctorBookingVM() { FullName = _doctorService.GetDoctorBasic(doctor.Key).FullName, Id = doctor.Key };
                        itemDoctor.Blocks.AddRange(doctor.OrderByDescending(s => s.Date).Select(s => s.Adapt<BlockVM>()));
                        //Chay tất cả các block của 1 bác sĩ
                        foreach (var item in itemDoctor.Blocks)
                        {
                            //lấy ra các Ticket của block
                            foreach (var ticket in _ticketService.GetTickets(_ => _.BlockId == item.Id))
                            {
                                if (ticket.CustomerId != null)
                                {
                                    var cus = _customerService.GetCustomer(Guid.Parse(ticket.CustomerId)).Adapt<TicketBookingVM>();
                                    cus.Note = ticket.Note;
                                    cus.TicketId = ticket.Id;
                                    item.Customers.Add(cus);
                                }
                                else
                                {
                                    item.Customers.Add(null);
                                }
                            }
                        }
                        itemDoctor.Blocks = itemDoctor.Blocks.OrderBy(_ => _.StartTime).ToList();
                        itemSpeciality.Doctors.Add(itemDoctor);
                    }
                    result.Add(itemSpeciality);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}