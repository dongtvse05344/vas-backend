using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VAS.Model;

namespace VAS.Utils
{
    public static class DefaultPassword
    {
        public static string PasswordDoctor = "zaq@123";
        public static string PasswordNurse = "zaq@123";
        public static string PasswordCustomer = "zaq@123";
    }
    public enum UserRoles
    {
        [Display(Name = "Quản lý tài khoản")]
        Admin = 0,
        [Display(Name = "Quản lý nhân sự")]
        HRM = 1,
        [Display(Name = "Quản lý phòng")]
        RoomManager = 2,
        [Display(Name = "Quản lý xếp lịch")]
        ScheduleManager = 3,
        [Display(Name = "Chăm sóc khách hàng")]
        CustomerCare = 4,
        [Display(Name = "Bác Sĩ")]
        Doctor = 5,
        [Display(Name = "Điều dưỡng")]
        Nurse = 6,
        [Display(Name = "Bệnh nhân")]
        Customer = 7,
    }

	public enum CustomerStatus
	{
		[Display(Name = "Khách hàng đã check-in")]
		Arrived = 1,
		[Display(Name = "Khách hàng vắng mặt")]
		Absence = -1,
		[Display(Name = "Khách hàng đang chờ")]
		Waiting = 0
	}


	public static class RolesExtenstions
    {
        public static async Task InitAsync(RoleManager<IdentityRole> roleManager, UserManager<MyUser> userManager)
        {
            foreach (string roleName in Enum.GetNames(typeof(UserRoles)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var admin = new MyUser
            {
                UserName = "_",
                Email = "dong.tran@hisoft.vn"
            };
            await userManager.CreateAsync(admin, "zaq@123");

            var role = nameof(UserRoles.Admin);
            await userManager.AddToRoleAsync(admin, role);
        }
    }
}
