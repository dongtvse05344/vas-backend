using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class DoctorBasic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public bool Gender { get; set; }
        public string  FullName { get; set; }
        public string  Email { get; set; }
        public string  PhoneNumber { get; set; }
        public string HisCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
        public virtual MyUser MyUser { get; set; }
        public virtual ICollection<Scheduling> Schedulings { get; set; }
        public virtual DoctorPro DoctorPro { get; set; }
        public virtual ICollection<SpecialityDoctor> SpecialityDoctors { get; set; }
    }
}
