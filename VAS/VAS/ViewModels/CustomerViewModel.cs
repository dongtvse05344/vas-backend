using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class CustomerVM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsBooking { get; set; }
    }
    public class CustomerDetailVM
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CMND { get; set; }
        public string Nation { get; set; }
        public string Job { get; set; }
        public string WorkPlace { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
    }
    public class CustomerCM
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CMND { get; set; }
        public string Nation { get; set; }
        public string Job { get; set; }
        public string WorkPlace { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
    }

    public class CustomerCMHis
    {
        public string HisCode { get; set; }
    }
    public class CustomerUM
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CMND { get; set; }
        public string Nation { get; set; }
        public string Job { get; set; }
        public string WorkPlace { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Relationship { get; set; }
    }
}
