using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class NurseViewModel
    {
		public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string HisCode { get; set; }
		public bool IsActive { get; set; }
	}

    public class NurseDetailVM
    {
		public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string HisCode { get; set; }
		public bool IsActive { get; set; }
	}
    public class NurseCreateModel
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string HisCode { get; set; }
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
    public class NurseListCreateModel
    {
        //create account for nurse
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string HisCode { get; set; }
        public DateTime DateCreated { get; set; }
        //create nurseinffo
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
    public class NurseUpdateModel
    {
        //asp.net user
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        //update nurse info
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }

    }
}
