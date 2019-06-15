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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SchedulingController : ControllerBase
    {
        #region properties
        private readonly UserManager<MyUser> _userManager;
        private readonly ISchedulingService _schedulingService;
        private readonly IBlockService _blockService;
        private readonly ITicketService _ticketService;
        private readonly IRoomService _roomService;
        private readonly IDoctorBasicService _doctorService;
        private readonly IMailService _emailService;

		public SchedulingController(UserManager<MyUser> userManager, ISchedulingService schedulingService, IBlockService blockService, ITicketService ticketService, IRoomService roomService, IDoctorBasicService doctorService, IMailService emailService)
		{
			_userManager = userManager;
			_schedulingService = schedulingService;
			_blockService = blockService;
			_ticketService = ticketService;
			_roomService = roomService;
			_doctorService = doctorService;
			_emailService = emailService;
		}


		#endregion

		/// Xem lịch trong 1 ngày cụ thể. từ starttime đến end time
		/// Nếu không truyền ngày, mặc định hôm nay. nêu không truyền giờ, mặc định từ 0h00 đến 23:59
		[HttpGet]
        public ActionResult GetSchedulings(DateTime? date, TimeSpan? startTime, TimeSpan? endTime, string doctorName, string roomNumber)
        {
            doctorName = doctorName == null ? "" : doctorName;
            roomNumber = roomNumber == null ? "" : roomNumber;
            date = date == null ? DateTime.Now.Date : date.Value.Date;
            startTime = startTime == null ? new TimeSpan(0, 0, 0) : startTime;
            endTime = endTime == null ? new TimeSpan(23, 59, 59) : endTime;
            var schedulings = _schedulingService.GetSchedulings(_ =>
            _.Date.Equals(date.Value.Date)
            && _.StartTime >= startTime
            && _.EndTime <= endTime
            && _.Doctor.FullName.Contains(doctorName)
            && _.Room.Number.Contains(roomNumber));
            var result = schedulings.ToPageList<SchedulingViewModel, Scheduling>(1, schedulings.Count());
            return Ok(result);
        }

        /// Lấy ra lịch dựa theo Id
        [HttpGet("{id}")]
        public ActionResult GetSchedulingById(Guid id)
        {
            var scheduling = _schedulingService.GetScheduling(id).Adapt<SchedulingVM2>();
            if (scheduling == null) return NotFound();
            return Ok(scheduling);
        }

        /// Trả về 1 list Chuyên Khoa modelView theo ngày 
        [HttpGet("Date")]
        public ActionResult GetSchedulingByDay(DateTime date)
        {
            try
            {
                var schedulings = _schedulingService.GetSchedulings(_ => _.Date.Date.Equals(date.Date) && _.IsAvailable);
				
				var specialityGroup = schedulings.GroupBy(_ => _.SpecialityId);
				List<SpecialityViewModel> specialitiesVM = new List<SpecialityViewModel>();
				foreach (var data in specialityGroup)
				{
					var specialityVM = new SpecialityViewModel()
					{
						Id = Guid.Parse(data.Key),
						Name = data.ToList()[0].SpecialityName
					};
					specialitiesVM.Add(specialityVM);
				}
                return Ok(specialitiesVM);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// Tạo scheduling và block. 
        [HttpPost]
        public async Task<ActionResult> CreateSchedulingAsync([FromBody]SchedulingCreateModel schedulingCM)
        {
            try
            {
                //create scheduling + block với isAvailable mặc định là FALSE cho cả 2 bảng(để k tạo ticket- k cho khách hàng đặt lịch)
                schedulingCM.Date = schedulingCM.Date.Date;
                var scheduling = schedulingCM.Adapt<Scheduling>();
                //dung roomservice lấy SpeciaId ra rồi gắn vô scheduling
                var speciality = _roomService.GetRoom(schedulingCM.RoomId).Speciality;
                scheduling.SpecialityId = speciality.Id.ToString();
                scheduling.SpecialityName = speciality.Name;
				var totalBlock = ((schedulingCM.EndTime - schedulingCM.StartTime) * 2).TotalHours;

				if (totalBlock <= 0 || ((scheduling.StartTime.Minutes != 30 && scheduling.StartTime.Minutes != 0) || (scheduling.EndTime.Minutes != 0 && scheduling.EndTime.Minutes != 30)))
				{
					return BadRequest("Vui lòng nhập đúng giờ!");
				}
				_schedulingService.CreateScheduling(scheduling, Convert.ToInt32(totalBlock), (await _userManager.GetUserAsync(User)).UserName);
				_schedulingService.Save();
			}
            catch (Exception)
            {
                return BadRequest("Insert fail vì trùng phòng hoặc trùng bác sĩ tại một thời điểm.");
            }
            return StatusCode(201);
        }

        /// Update 1 lịch dựa theo Id - Nếu đã Active thì không cho update
        [HttpPut]
        public async Task<ActionResult> UpdateSchedulingAsync([FromBody]SchedulingUpdateModel schedulingUM)
        {
            try
            {
                //lấy id của scheduling cần update
                var scheduling = _schedulingService.GetScheduling(schedulingUM.Id);
                if (scheduling == null)
                {
                    return NotFound();
                }

                if(_roomService.GetRoom(schedulingUM.RoomId).SpecialityId != Guid.Parse(scheduling.SpecialityId))
                {
                    return BadRequest("Room này không đúng chuyên khoa hiện hành!");
                }

                if (scheduling.IsAvailable)
                {
                    return BadRequest("Can not update this scheduling!");
                }

                var totalBlock = ((schedulingUM.EndTime - schedulingUM.StartTime) * 2).TotalHours;
                if (totalBlock <= 0 || ((schedulingUM.StartTime.Minutes != 30 && schedulingUM.StartTime.Minutes != 0) || (schedulingUM.EndTime.Minutes != 0 && schedulingUM.EndTime.Minutes != 30)))
                {
                    return BadRequest("Vui lòng nhập đúng giờ!");
                }


                //xóa blocks dựa vào id của scheduling
                _blockService.DeleteBlock(b => b.SchedulingId == scheduling.Id);
                //update data trong bảng scheduling
                _schedulingService.UpdateScheduling(schedulingUM.Adapt(scheduling), (await _userManager.GetUserAsync(User)).UserName);

                //tạo lại số lượng blocks dựa vào starttime và endtime cần update
                _schedulingService.CreateBlocks(scheduling,Convert.ToInt32(totalBlock));
                _schedulingService.Save();
            }
            catch (Exception)
            {
                return BadRequest("update fail vì trùng phòng hoặc trùng bác sĩ tại một thời điểm.");
            }
            return Ok();
        }

        /// thay đổi trạng thái của lịch dựa vào scheduling id, tạo ra các ticket cho scheduling
        /// isAvailable=fasle : không cho đăng kí khám bệnh
        /// isAvailable trong scheduling thay đổi thì isAvailable trong block cũng thay đổi
        [HttpPut("SetAvailable")]
        public async Task<ActionResult> ActiveTicketAsync([FromBody] List<string> Schedulings)
        {
            try
            {
                foreach (var schedulingId in Schedulings)
                {
                    var scheduling = _schedulingService.GetScheduling(Guid.Parse(schedulingId));
                    if (!scheduling.IsAvailable)
                    {
                        scheduling.IsAvailable = true;
                        var blocks = _blockService.GetBlocks(_ => _.SchedulingId == Guid.Parse(schedulingId));
                        foreach (var block in blocks)
                        {
                            block.IsAvailable = true;
                            //create tickets for each block (số lượng ticket dựa vào số lượng totalticket trong block)
                            int count = 0;//index của ticket
                            for (int j = 0; j < block.TotalTicket; j++)
                            {
                                Ticket ticket = new Ticket();
                                ticket.BlockId = block.Id;
                                ticket.DateCreated = DateTime.Now;
                                ticket.Index = (++count).ToString();
                                _ticketService.CreateTicket(ticket, (await _userManager.GetUserAsync(User)).UserName);
                            }
                        }

                    }
                }
                _schedulingService.Save();

				//lấy những bác sĩ được xếp lịch để gửi mail
				List<string> doctorIds = new List<string>();
				foreach (var schedulingId in Schedulings)
				{
					var doctorId = _schedulingService.GetScheduling(Guid.Parse(schedulingId)).DoctorId;
					doctorIds.Add(doctorId);
				}

				foreach (var data in doctorIds.GroupBy(_=>_))
				{
					var doctor = _doctorService.GetDoctorBasic(data.Key);
					EmailModel email = new EmailModel();
					email.ToMail = doctor.Email;
					email.Subject = "This is subject";
					email.Message = "Vô coi lịch trực của mày kìa";
					_emailService.SendEmail(email);
				}
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

		/// xóa lịch trực bác sĩ chỉ khi isVailable = false;
		[HttpDelete]
		public ActionResult DeleteScheduling(List<Guid> schedulingId)
		{
			try
			{
				foreach (var id in schedulingId)
				{
					var scheduling = _schedulingService.GetScheduling(id);
					if (scheduling == null) return NotFound();
					if (!scheduling.IsAvailable)
					{
						_schedulingService.DeleteScheduling(scheduling);

					}

				}
				_schedulingService.Save();
			}
			catch (Exception e)
			{
				return BadRequest(e.Message);
			}
			return Ok();
		}
	}
}