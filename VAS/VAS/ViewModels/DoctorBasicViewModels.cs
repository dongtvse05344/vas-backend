using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{

    //viewmodel for searching Doctor
    public class DoctorDetailVM
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
		public bool Gender { get; set; }
        public IList<string> SpecialityName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string HisCode { get; set; }
        //pro
        public string Degree { get; set; }
        public string Language { get; set; }
        public string Certification { get; set; }
        public string Experience { get; set; }
		public bool IsActive { get; set; }
	}
	public class DoctorBasicViewModel
	{
		public string Id { get; set; }
		public string FullName { get; set; }
		public bool Gender { get; set; }
        public string HisCode { get; set; }
        public IList<string> SpecialityName { get; set; }
        public DateTime? BirthDate { get; set; }
		public bool IsActive { get; set; }
	}

	public class DoctorBasicCM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HisCode { get; set; }
        public bool Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public IList<Guid> SpecialityIds { get; set; }


        //Pro
        public string Degree { get; set; }
        public string Language { get; set; }
        public string Certification { get; set; }
        public string Experience { get; set; }
    }
    //viewmodel for updating Doctor
    public class DoctorBasicUpdateModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        //doctor pro
        public string Degree { get; set; }
        public string Language { get; set; }
        public string Certification { get; set; }
        public string Experience { get; set; }
    }

}
