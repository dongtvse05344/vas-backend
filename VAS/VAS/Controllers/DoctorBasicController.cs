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
    [Route("api/Doctor")]
    [ApiController]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        #region properties
        private readonly IDoctorBasicService _doctorBasicService;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ISpecialityDoctorService _specialityDoctorService;
        private readonly ISchedulingService _schedulingService;

        public DoctorController(IDoctorBasicService doctorBasicService, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager, ISpecialityDoctorService specialityDoctorService, ISchedulingService schedulingService)
        {
            _doctorBasicService = doctorBasicService;
            _userManager = userManager;
            _roleManager = roleManager;
            _specialityDoctorService = specialityDoctorService;
            _schedulingService = schedulingService;
        }

        #endregion

        /// Lấy các chuyên khoa của 1 bác sĩ bằng bác sĩ Id
        private List<string> GetListSpecialityName(string doctorId)
        {
            var specialityDoctors = _specialityDoctorService.GetSpecialityDoctors(s => s.DoctorBasicId.Equals(doctorId));
            var result = new List<string>();
            foreach (var item in specialityDoctors)
            {
                result.Add(item.Speciality.Name);
            }
            return result;
        }

        /// get all doctor basic
        [HttpGet]
        public ActionResult GetDoctorsBasic(Guid? specialityId, string name, string hiscode, int index = 1, int pageSize = 5)
        {
            name = name == null ? "" : name;
            hiscode = hiscode == null ? "" : hiscode;
            var Doctors = _doctorBasicService.GetDoctorsBasic(d =>
                d.FullName.Contains(name)
                && d.HisCode.Contains(hiscode));
            if (specialityId != null)
            {
                Doctors = Doctors.Where(_ => _.SpecialityDoctors.Count(__=>__.SpecialityId == specialityId) >0);
            }
            var result = Doctors.ToPageList<DoctorBasicViewModel, DoctorBasic>(index, pageSize);
            foreach (var item in result.List)
            {
                item.SpecialityName = GetListSpecialityName(item.Id);
            }
            return Ok(result);
        }

        /// Check xem hiscode tồn tại hay chưa 
        [HttpGet("code/{hiscode}")]
        public ActionResult CheckHisCode(string hiscode)
        {
            var doctor = _doctorBasicService.GetDoctorsBasic(_ => _.HisCode == hiscode).FirstOrDefault();
            if (doctor   == null) return NotFound();
            return Ok();
        }

        /// Get doctor basic and doctor pro 's info by doctor basicID
        [HttpGet("{id}")]
        public ActionResult GetDoctorInfoById(string id)
        {
            var doctor = _doctorBasicService.GetDoctorBasic(id);
            if (doctor == null) return NotFound();
            ////xu ly view model
            var result = doctor.Adapt<DoctorDetailVM>();
            result = doctor.DoctorPro.Adapt(result);
            result.SpecialityName = GetListSpecialityName(result.Id);
            return Ok(result);
        }

        /// <summary>
        /// Mobile
        /// </summary>
        [HttpGet("Work")]
        public ActionResult GetDoctorsDuringSixDays(int numberOfDay)
        {
            numberOfDay = numberOfDay == 0 ? 7 : numberOfDay;
            var today = DateTime.Now.Date;
            var sixDaysLater = DateTime.Now.Date.AddDays((double)(numberOfDay - 1));
            var schedulings = _schedulingService.GetSchedulings(_ => _.Date.Date >= today && _.Date.Date <= sixDaysLater);
            var doctors = schedulings.GroupBy(_ => _.DoctorId);
            List<DoctorBasicViewModel> result = new List<DoctorBasicViewModel>();
            foreach (var doctor in doctors)
            {
                DoctorBasicViewModel item = _doctorBasicService.GetDoctorBasic(doctor.Key).Adapt<DoctorBasicViewModel>();
                item.SpecialityName = GetListSpecialityName(item.Id);
                result.Add(item);
            }
            return Ok(result);
        }

        /// <summary>
        /// Mobile
        /// </summary>
        [HttpGet("{id}/Blocks")]
        public ActionResult GetBlocksByDoctorId(string id, int day)
        {
            day = day == 0 ? 7 : day;
            var schedulings = _schedulingService.GetSchedulings(s => s.DoctorId == id
            && s.IsAvailable
            && s.Date.Date >= DateTime.Now.Date
            && s.Date.Date <= DateTime.Now.AddDays((double)(day - 1)).Date)
            .OrderBy(_ => _.Date).ToList();
            // grooup by date
            var groupDate = schedulings.GroupBy(_ => _.Date);

            List<SchedulingVM> result = new List<SchedulingVM>();

            foreach (var dataDate in groupDate)
            {
                SchedulingVM item = new SchedulingVM();
                item.Date = dataDate.Key;
                foreach (var scheduling in dataDate)
                {
                    item.Blocks.AddRange(scheduling.Blocks.Select(_ => _.Adapt<BlockBookingViewModel>()));
                }
                result.Add(item);
            }
            return Ok(result);
        }

        /// Get All Scheduling only of Doctor
        [HttpGet("{id}/FutureScheduling")]
        public ActionResult GetSchedulingsByDoctorId(string id, int day)
        {
            day = day == 0 ? 1 : day;
            var schedulings = _schedulingService.GetSchedulings(_ => _.DoctorId == id
                                  && _.IsAvailable
                                  && _.Date >= DateTime.Now.Date
                                  && _.Date <= DateTime.Now.AddDays(day - 1).Date).OrderBy(_ => _.Date).ToList();
            var groupDate = schedulings.GroupBy(_ => _.Date);
            var result = new List<SchedulingsByDoctorIdVM>();
            foreach (var data in groupDate)
            {
                var schedulingVM = new SchedulingsByDoctorIdVM();
                schedulingVM.Date = data.Key;

                foreach (var item in data.OrderBy(_ => _.StartTime))
                {
                    schedulingVM.Schedulings.Add(item.Adapt<SchedulingOfDoctor>());
                }
                result.Add(schedulingVM);
            }
            return Ok(result);
        }

        /// create doctor's account + role + doctor info(in doctorBasic & doctor pro)
        [HttpPost]
        public async Task<ActionResult> CreateDoctorAsync([FromBody] DoctorBasicCM doctorCM)
        {
            MyUser user = null;
            try
            {
                //asp.net user_ create account
                user = doctorCM.Adapt<MyUser>();
                user.IsActive = false;
                user.DateCreated = DateTime.Now;
                user.UserName = doctorCM.HisCode;
                var currentUser = await _userManager.CreateAsync(user, DefaultPassword.PasswordDoctor);
                if (currentUser.Succeeded)
                {
                    if (!(await _userManager.AddToRoleAsync(user, nameof(UserRoles.Doctor))).Succeeded)
                    {
                        _userManager.DeleteAsync(user);
                        return BadRequest(user + "\n Add role fail!");
                    }

                    //create doctor basic
                    DoctorBasic doctorBasic = doctorCM.Adapt<DoctorBasic>();
                    doctorBasic.Id = user.Id;
                    //create info doctor pro
                    DoctorPro doctorPro = doctorCM.Adapt<DoctorPro>();
                    doctorPro.Id = user.Id;
                    //create Speciality
                    var specialities = new List<SpecialityDoctor>();
                    foreach (var item in doctorCM.SpecialityIds.ToList())
                    {
                        specialities.Add(new SpecialityDoctor()
                        {
                            SpecialityId = item,
                            DoctorBasicId = user.Id
                        });
                    }
                    doctorBasic.SpecialityDoctors = specialities;

                    //Auto create doctorPro when insert doctorBasic
                    doctorBasic.DoctorPro = doctorPro;

                    _doctorBasicService.CreateDoctorBasic(doctorBasic, (await _userManager.GetUserAsync(User)).UserName);
                }
                else
                {
                    if (user != null) _userManager.DeleteAsync(user);
                    return BadRequest(currentUser.Errors);
                }
                _doctorBasicService.Save();
                return StatusCode(201);

            }
            catch (Exception e)
            {
                if (user != null) _userManager.DeleteAsync(user);
                return BadRequest(e.Message);
            }
        }

        /// create list doctor's account + role + doctor info(in doctorBasic & doctor pro)
        [HttpPost("List")]
        public async Task<ActionResult> CreateDoctorsAsync([FromBody] List<DoctorBasicCM> doctorCMs)
        {
            string _ = (await _userManager.GetUserAsync(User)).UserName;
            var createdDoctor = new List<MyUser>();
            try
            {
                //asp.net user_ create list doctors
                foreach (var doctorCM in doctorCMs)
                {
                    MyUser user = doctorCM.Adapt<MyUser>();
                    user.UserName = doctorCM.HisCode; //Username sẽ là HisCode
                    user.IsActive = false;
                    user.DateCreated = DateTime.Now;

                    //Create doctor
                    var currentUser = await _userManager.CreateAsync(user, DefaultPassword.PasswordDoctor);
                    //Create success
                    if (currentUser.Succeeded)
                    {
                        if (!(await _userManager.AddToRoleAsync(user, nameof(UserRoles.Doctor))).Succeeded)
                        {
                            return BadRequest(user + "\n Add role fail!");
                        }

                        //create doctor basic
                        DoctorBasic doctorBasic = doctorCM.Adapt<DoctorBasic>();
                        doctorBasic.Id = user.Id;

                        //create info doctor pro
                        doctorBasic.DoctorPro = doctorCM.Adapt<DoctorPro>();
                        doctorBasic.DoctorPro.Id = user.Id;
                        //create Speciality
                        var specialities = new List<SpecialityDoctor>();
                        foreach (var item in doctorCM.SpecialityIds.ToList())
                        {
                            specialities.Add(new SpecialityDoctor()
                            {
                                SpecialityId = item,
                                DoctorBasicId = user.Id
                            });
                        }
                        doctorBasic.SpecialityDoctors = specialities;

                        _doctorBasicService.CreateDoctorBasic(doctorBasic, _);
                        createdDoctor.Add(user);
                    }
                    else //Create fail
                    {
                        foreach (var item in createdDoctor)
                        {
                            _userManager.DeleteAsync(item);
                        }
                        return BadRequest(currentUser.Errors);
                    }
                }
                _doctorBasicService.Save();
                return Ok();

            }
            catch (Exception e)
            {
                foreach (var item in createdDoctor)
                {
                    _userManager.DeleteAsync(item);
                }
                return BadRequest(e.Message);
            }
        }

        /// update doctor's info: account + doctorBasic + doctorPro
        [HttpPut]
        public async Task<ActionResult> UpdateDoctorAsync(DoctorBasicUpdateModel doctorUM)
        {
            try
            {
                //update account(fullname, email, phone)

                MyUser user = await _userManager.FindByIdAsync(doctorUM.Id);
                if (user == null)
                {
                    return NotFound();

                }
                user = doctorUM.Adapt(user);
                user.DateUpdated = DateTime.Now;
                user.Doctor = doctorUM.Adapt(user.Doctor);
                user.Doctor.DateUpdated = DateTime.Now;
                user.Doctor.UpdatedByUserName = (await _userManager.GetUserAsync(User)).UserName;
                user.Doctor.DoctorPro = doctorUM.Adapt(user.Doctor.DoctorPro);
                var success = await _userManager.UpdateAsync(user);
                if (success.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(success.Errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}