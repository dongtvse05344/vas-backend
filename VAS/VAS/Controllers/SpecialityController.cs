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
    public class SpecialityController : ControllerBase
    {
        #region properties
        private readonly ISpecialityService _specialityService;
        private readonly IRoomService _roomService;
        private readonly IDoctorBasicService _doctorService;
        private readonly ISchedulingService _schedulingService;
        private readonly ISpecialityDoctorService _specialityDoctorService;
        private readonly UserManager<MyUser> _userManager;

        public SpecialityController(ISpecialityService specialityService, IRoomService roomService, IDoctorBasicService doctorService, ISchedulingService schedulingService, ISpecialityDoctorService specialityDoctorService, UserManager<MyUser> userManager)
        {
            _specialityService = specialityService;
            _roomService = roomService;
            _doctorService = doctorService;
            _schedulingService = schedulingService;
            _specialityDoctorService = specialityDoctorService;
            _userManager = userManager;
        }
        #endregion

        //load all speciality/ load by name
        [HttpGet]
        public ActionResult LoadSpecialitys(string name)
        {
            name = name == null ? "" : name;
            var Specialitys = _specialityService.GetSpecialitys(r => r.Name.Contains(name));
            var result = Specialitys.ToPageList<SpecialityViewModel, Speciality>(1, Specialitys.Count());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult LoadSpecialityById(Guid id)
        {
            var speciality = _specialityService.GetSpeciality(id);
            if (speciality == null) return NotFound();
            return Ok(speciality.Adapt<SpecialityViewModel>());
        }

        [HttpGet("{id}/Rooms")]
        public ActionResult LoadRoomsBySpecialityId(Guid id)
        {
            var rooms = _roomService.GetRooms(r => r.SpecialityId == id && r.IsAvailable).Adapt<List<RoomBySpecialViewModel>>();
            return Ok(rooms);
        }

        [HttpGet("{id}/Doctors")]
        public ActionResult GetDoctorsBySpeciality(Guid id)
        {
            try
            {
                var doctors = _specialityDoctorService.GetSpecialityDoctors(_ => _.SpecialityId == id && _.DoctorBasic.MyUser.IsActive == true);
                List<DoctorBasicViewModel> result = new List<DoctorBasicViewModel>();
                foreach (var item in doctors)
                {
                    result.Add(item.DoctorBasic.Adapt<DoctorBasicViewModel>());
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// Từ Khoa + Ngày lấy ra list SchedulingId => để load ra các block có giờ để KH đặt lịch
        [HttpGet("{id}/Scheduling")]
        public ActionResult Time([FromQuery]Guid id, [FromQuery] DateTime date)
        {
            try
            {
                var schedulingIds = _schedulingService.GetSchedulings(s => s.Date.Date == date.Date
                && s.SpecialityId.Contains(id.ToString())).ToList();
                var group = schedulingIds.GroupBy(_ => _.Doctor.Id);
                var result = new List<BlockOfDoctorViewModel>();
                foreach (var data in group)
                {
                    var item = new BlockOfDoctorViewModel()
                    {
                        FullName = _doctorService.GetDoctorBasic(data.Key).FullName
                    };
                    foreach (var scheduling in data)
                    {
                        item.Blocks.AddRange(scheduling.Blocks.Select(_ => _.Adapt<BlockBookingViewModel>()));
                        foreach (var block in item.Blocks)
                        {
                            if (TimeSpan.Compare(TimeSpan.Parse(block.Time), DateTime.Now.TimeOfDay) < 0)
                            {
                                block.IsFull = true;
                            }
                        }
                    }
                    result.Add(item);
                }
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateSpecialityAsync([FromBody]SpecialityCreateModel speciality)
        {
            try
            {
                _specialityService.CreateSpeciality(speciality.Adapt<Speciality>(), (await _userManager.GetUserAsync(User)).UserName);
                _specialityService.Save();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return StatusCode(201);
        }

        [HttpPut]
        public async Task<ActionResult> UpdareSpecialityAsync([FromBody]SpecialityUpdateModel specialityVM)
        {
            try
            {
                var isExistSpeciality = _specialityService.GetSpeciality(specialityVM.Id);
                if (isExistSpeciality == null) return NotFound();
                _specialityService.UpdateSpeciality(specialityVM.Adapt(isExistSpeciality), (await _userManager.GetUserAsync(User)).UserName);
                _specialityService.Save();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return Ok();
        }

        [HttpDelete("id")]
        public ActionResult DeleteSpeciality(Guid id)
        {
            try
            {
                var speciality = _specialityService.GetSpeciality(id);
                if (speciality == null) return NotFound();
                _specialityService.DeleteSpeciality(speciality);
                _specialityService.Save();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }
            return Ok();
        }
    }
}