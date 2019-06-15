using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VAS.Model;
using VAS.Utils;
using VAS.ViewModels;

namespace VAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region properties
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        [Authorize]
        [HttpGet]
        public ActionResult GetRoles()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var roles = _userManager.GetRolesAsync(user).Result;
            return Ok(roles);
        }

        [HttpPost("token")]
        public async Task<ActionResult> GetToken([FromBody]LoginVM model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("Invalid Username");
            }
            if (!user.IsActive)
            {
                return BadRequest("User is blocked");
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!result)
            {
                return BadRequest("Invalid Password");
            }
            return new OkObjectResult(GenerateToken(user).Result);
        }

        private async Task<Token> GenerateToken(MyUser user)
        {
            //security key
            string securityKey = "qazedcVFRtgbNHYujmKIolp";
            //symmectric security key
            var symmectricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            //signing credentials
            var signingCredentials = new SigningCredentials(symmectricSecurityKey, SecurityAlgorithms.HmacSha256);

            //add Claims
            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            //create token
            var token = new JwtSecurityToken(
                    issuer: "dongtv",
                    audience: user.FullName,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: signingCredentials,
                    claims: claims
                );

            //return token
            return new Token
            {
                roles = _userManager.GetRolesAsync(user).Result.ToArray(),
                access_token = new JwtSecurityTokenHandler().WriteToken(token),
                expires_in = (int)TimeSpan.FromDays(1).TotalSeconds
            };
        }

        /// <summary>
        /// mobile
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> LoginOrRegisterByPhoneNumberAsync([FromBody] customerLoginVM model)
        {
            try
            {
                var customer = await _userManager.FindByNameAsync(model.PhoneNumber);
                if (customer != null) // Tai khoan da ton tai -> Dăng nhập
                {
                    return Ok
                            (GenerateToken(customer).Result);
                }
                else // Tai Khoan chua ton tai -> Đăng kí
                {
                    customer = new MyUser()
                    {
                        UserName = model.PhoneNumber,
                        IsActive = true,
                        IsCustomer = true,
                        DateCreated = DateTime.Now,
                        PhoneNumber = model.PhoneNumber
                    };
                    var resultUser = await _userManager.CreateAsync(customer, DefaultPassword.PasswordCustomer);
                    var resultRole = await _userManager.AddToRoleAsync(customer, nameof(UserRoles.Customer));
                    if (resultUser.Succeeded && resultRole.Succeeded)
                    {
                        return Ok
                            (GenerateToken(customer).Result);
                    }
                    else
                    {
                        if (!resultRole.Succeeded)
                        {
                            _userManager.DeleteAsync(customer);
                        }
                        return BadRequest(resultUser.Errors + " \n" + resultRole.Errors);
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// mobile
        /// </summary>
        /// <returns>The otp.</returns>
        /// <param name="model">Model.</param>
        [HttpPost("OTP")]
        public ActionResult CheckOTP([FromBody] otpVM model)
        {
            if (model.OTP == "0000") return Ok();
            return BadRequest();
        }

    }
}