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
    [Route("api/Nurse")]
    [ApiController]
    [Authorize]
    public class NurseController : ControllerBase
    {
        #region properties
        private readonly INurseService _nurseService;
        private readonly UserManager<MyUser> _userManager;

        public NurseController(INurseService nurseService, UserManager<MyUser> userManager)
        {
            _nurseService = nurseService;
            _userManager = userManager;
        }
        #endregion

        //seach
        [HttpGet]
        public ActionResult GetNurses(string name, string hiscode, int index = 1, int pageSize = 5)
        {
            name = name == null ? "" : name;
            hiscode = hiscode == null ? "" : hiscode;
            var nurses = _nurseService.GetNurses(d => 
            d.MyUser.FullName.Contains(name)
            && d.HisCode.Contains(hiscode)
			//&& d.MyUser.IsActive==true
            );
            var result = nurses.ToPageList<NurseViewModel, Nurse>(index, pageSize);
            

            return Ok(result);
        }
        [HttpGet("{id}")]
        public ActionResult GetNurseById(string id)
        {
            var nurse = _nurseService.GetNurse(id);
            if (nurse == null) return NotFound();
            //xu ly view model
            var result = nurse.Adapt<NurseDetailVM>();
            return Ok(result);
        }

        [HttpGet("code/{hiscode}")]
        public ActionResult CheckHisCode(string hiscode)
        {
            var nurse = _nurseService.GetNurses(_ => _.HisCode == hiscode).FirstOrDefault();
            if (nurse == null) return NotFound();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> CreateNurseAsync([FromBody] NurseCreateModel nurseCM)
        {
            MyUser user = null;
            try
            {
                //asp.net user_ create account
                user = nurseCM.Adapt<MyUser>();
                user.IsActive = false;
                user.DateCreated = DateTime.Now;
                user.UserName = nurseCM.HisCode;
                var currentUser = await _userManager.CreateAsync(user, DefaultPassword.PasswordNurse);
                if (currentUser.Succeeded)
                {
                    if (!(await _userManager.AddToRoleAsync(user, nameof(UserRoles.Nurse))).Succeeded)
                    {
                        _userManager.DeleteAsync(user);
                        return BadRequest(user + "\n Add role fail!");
                    }

                    //create nurse basic
                    Nurse nurse = nurseCM.Adapt<Nurse>();
                    nurse.Id = user.Id;

                    _nurseService.CreateNurse(nurse, (await _userManager.GetUserAsync(User)).UserName);
                }
                else
                {
                    if (user != null) _userManager.DeleteAsync(user);
                    return BadRequest(currentUser.Errors);
                }
                _nurseService.Save();
                return StatusCode(201);

            }
            catch (Exception e)
            {
                if (user != null) _userManager.DeleteAsync(user);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("CreateList")]
        public async Task<ActionResult> CreateNursesAsync([FromBody] List<NurseListCreateModel> nurseCMs)
        {
            string _ = (await _userManager.GetUserAsync(User)).UserName;
            var createdNurse = new List<MyUser>();
            try
            {
                //asp.net user_ create list nurses
                foreach (var nurseCM in nurseCMs)
                {
                    MyUser user = nurseCM.Adapt<MyUser>();
                    user.UserName = nurseCM.HisCode; //Username sẽ là HisCode
                    user.IsActive = false;
                    user.DateCreated = DateTime.Now;

                    //Create nurse
                    var currentUser = await _userManager.CreateAsync(user, DefaultPassword.PasswordNurse);
                    //Create success
                    if (currentUser.Succeeded)
                    {
                        if (!(await _userManager.AddToRoleAsync(user, nameof(UserRoles.Nurse))).Succeeded)
                        {
                            return BadRequest(user + "\n Add role fail!");
                        }

                        //create nurse basic
                        Nurse nurse = nurseCM.Adapt<Nurse>();
                        nurse.Id = user.Id;

                        _nurseService.CreateNurse(nurse, _);
                        createdNurse.Add(user);
                    }
                    else //Create fail
                    {
                        foreach (var item in createdNurse)
                        {
                            _userManager.DeleteAsync(item);
                        }
                        return BadRequest(currentUser.Errors);
                    }
                }
                _nurseService.Save();
                return Ok();

            }
            catch (Exception e)
            {
                foreach (var item in createdNurse)
                {
                    _userManager.DeleteAsync(item);
                }
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateNurse([FromBody] NurseUpdateModel nurseUM)
        {
            try
            {
                //update account(fullname, email, phone)

                MyUser user = await _userManager.FindByIdAsync(nurseUM.Id.ToString());
                if (user == null)
                {
                    return NotFound();

                }
                user = nurseUM.Adapt(user);
                user.DateUpdated = DateTime.Now;
                user.Nurse = nurseUM.Adapt(user.Nurse);
                user.Nurse.DateUpdated = DateTime.Now;
                user.Nurse.UpdatedByUserName = (await _userManager.GetUserAsync(User)).UserName;
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
