using System.ComponentModel.DataAnnotations;

namespace VAS.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
    public class customerLoginVM
    {
        public string PhoneNumber { get; set; }
    }
    public class Token
    {
        public string[] roles { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }

    public class otpVM
    {
        public string OTP { get; set; }
    }

    public class CustomerRegisterVM
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string Email { get; set; }

        public string Fullname { get; set; }
    }

    public class RoleVM
    {
        public string Name { get; set; }
    }
}
