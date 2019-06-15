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
    [Route("api/Account")]
    [ApiController]
    [Authorize]
    //[Authorize(Roles = nameof(UserRoles.Admin))]
    public class AccountController : ControllerBase
    {
        #region properties
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDoctorBasicService _doctorBasicService;
        private readonly INurseService _nurseService;

        public AccountController(UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager, IDoctorBasicService doctorBasicService, INurseService nurseService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _doctorBasicService = doctorBasicService;
            _nurseService = nurseService;
        }
        #endregion

        // Lấy role để tạo tài khoản
        [HttpGet("RoleCreate")]
        public ActionResult GetRolesToCreate()
        {
            List<string> roleNames = new List<string>()
            { nameof(UserRoles.Admin), nameof(UserRoles.Doctor), nameof(UserRoles.Nurse) , nameof(UserRoles.Customer) };
            var data = _roleManager.Roles.Where(_ => !roleNames.Contains(_.Name)).Adapt<List<RoleVM>>();
                                            //.Select(_ => _.Adapt<RoleVM>())
                                            //.ToList();

            return Ok(data);
        }

        // Lấy role để tìm kiếm nhân viên
        [HttpGet("RoleStaff")]
        public ActionResult GetAllRoles()
        {
            List<string> roleNames = new List<string>()
            { nameof(UserRoles.Customer) };
            var data = _roleManager.Roles.Where(_ => !roleNames.Contains(_.Name)).Adapt<List<RoleVM>>();
            //.Select(_ => _.Adapt<RoleVM>())
            //.ToList();

            return Ok(data);
        }

        [HttpGet("Id")]
        public async Task<ActionResult> GetIdOfCurrentUserAsync()
        {
            return Ok(new { Id= (await _userManager.GetUserAsync(User)).Id });
        }

        /// Get all Account in app - or get with fullname without Customer
        [HttpGet]
        public async Task<ActionResult<AccountViewModel>> GetListAccountAsync(string username, string fullName, string role, int index = 1, int pageSize = 5)
        {
            username = username == null ? "" : username;
            fullName = fullName == null ? "" : fullName;
            role = role == null ? "" : role;

            try
            {
                var users = role.Length == 0 ? _userManager.Users.Where(_ => !_.IsCustomer)
                    : (await _userManager.GetUsersInRoleAsync(role)).AsQueryable();
                users = users.Where(u=> u.FullName.Contains(fullName) && u.UserName.Contains(username)).OrderByDescending(_ => _.DateCreated);

                #region Phan Trang
                var result = users.ToPageList<AccountViewModel, MyUser>(index, pageSize);

                foreach (var item in result.List)
                {
                    item.Roles =await _userManager.GetRolesAsync(item.Adapt<MyUser>());
                }
                #endregion

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// Get account with id
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountViewModel>> GetAccount(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {

                    AccountViewModel accountVM = user.Adapt<AccountViewModel>();
                    accountVM.Roles = await _userManager.GetRolesAsync(user);

                    //Check This user is Customer or not ?
                    //if (accountVM.Roles.Contains(nameof(UserRoles.Customer)))
                    //{
                    //    return Ok();
                    //}
                    return Ok(accountVM);
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

        /// Create a new account with many roles Without Doctor , Admin , Nurse , Customer
        [HttpPost]
        public async Task<ActionResult> CreateAccount(AccountCreateModel accountCM)
        {
            //Check role
            List<string> roles = accountCM.Roles.ToList();
            foreach (var i in roles)
            {
                //Not allow to add Admin , Doctor , Nurse
                if (
                    //i.ToLower().Equals(nameof(UserRoles.Admin).ToLower()) ||
                    i.ToLower().Equals(nameof(UserRoles.Doctor).ToLower()) ||
                    i.ToLower().Equals(nameof(UserRoles.Nurse).ToLower())  ||
                    i.ToLower().Equals(nameof(UserRoles.Customer).ToLower())) 
                    return BadRequest("Role can not contains Admin , Doctor , Nurse, Customer");
            }
            try
            {
                MyUser user = accountCM.Adapt<MyUser>();
                user.IsActive = true;
                user.IsCustomer = false;
                user.DateCreated = DateTime.Now;
                var currentUser = await _userManager.CreateAsync(user, accountCM.Password);
                if (currentUser.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, accountCM.Roles);
                    return StatusCode(201);
                }
                else
                {
                    return BadRequest(currentUser.Errors);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// Customer can register for themselft with role customer
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody]CustomerRegisterVM registerVM)
        {
            try
            {
                var user = new MyUser()
                {
                    UserName = registerVM.Username,
                    Email = registerVM.Email,
                    FullName = registerVM.Fullname,
                    IsActive = true,
                    IsCustomer = true,
                    DateCreated = DateTime.Now
                };
                var resultUser = await _userManager.CreateAsync(user, registerVM.Password);
                var resultRole = await _userManager.AddToRoleAsync(user, nameof(UserRoles.Customer));
                if (resultUser.Succeeded && resultRole.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(resultUser.Errors + " \n" + resultRole.Errors);
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// Update data cho 1 account [FE phải truyền 1 dãy role vào ... Vì api chỉ nhận những role đang được truyền k lưu role cũ]
        /// Xem lại. 
        [HttpPut]
        public async Task<ActionResult> UpdateAccount(AccountUpdateModel accountUM)
        {
            try
            {
                if (accountUM.Roles.Contains(nameof(UserRoles.Admin)))
                {
                    return BadRequest("Can not update to role Admin!");
                }
                MyUser user = await _userManager.FindByNameAsync(accountUM.UserName);
                if (user != null)
                {
                    user = accountUM.Adapt(user); //Chuyen data tu View -> Model
                    var roles = await _userManager.GetRolesAsync(user);
                    if(roles.Contains(nameof(UserRoles.Doctor)))
                    {
                        accountUM.Roles.Add(nameof(UserRoles.Doctor));
                    }
                    if (roles.Contains(nameof(UserRoles.Nurse)))
                    {
                        accountUM.Roles.Add(nameof(UserRoles.Nurse));
                    }

                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRolesAsync(user, accountUM.Roles);
                    user.DateUpdated = DateTime.Now;
                    var currentAccount = await _userManager.UpdateAsync(user);
                    if (currentAccount.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(currentAccount.Errors);
                    }
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

        /// Update password without updating info by Admin
        [HttpPut("ResetPassword")]
        public async Task<ActionResult> ResetPassword(PasswordResetModel accountUM)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                MyUser user = await _userManager.FindByIdAsync(accountUM.Id);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), accountUM.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
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

        /// Update password without updating info by User
        [HttpPut("UpdatePassword")]
        [Authorize]
        public async Task<ActionResult> UpdatePassword(PasswordUpdateModel accountUM)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }
                MyUser user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, accountUM.CurrentPassword, accountUM.NewPassword);
                    if (result.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
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


        /// Set IsDelete = true for a account with AccountId
        [HttpPut("{id}/ChangeActive")]
        public async Task<ActionResult> ChangeActive(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    user.IsActive = !user.IsActive;
                    var check = await _userManager.UpdateAsync(user);
                    if (check.Succeeded)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(check.Errors);
                    }
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
    }
}
