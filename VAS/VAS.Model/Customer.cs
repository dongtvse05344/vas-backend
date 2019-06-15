using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Gender { get; set; }
        public string CMND { get; set; }
        public string Nation { get; set; }  //Quốc Gia
        public string Email { get; set; }  
        public string Address { get; set; }
        public string Job { get; set; }
        public string WorkPlace { get; set; }
        public string HisCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
        public virtual ICollection<Family> Families { get; set; }
    }
}
