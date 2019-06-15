using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Model
{
    public class MyUser: IdentityUser
    {
        public String FullName { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public virtual DoctorBasic Doctor { get; set; }
        public virtual Nurse Nurse { get; set; }

        public virtual ICollection<Family> Families { get; set; }
    }
}
