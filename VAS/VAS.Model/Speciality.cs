using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Speciality
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<SpecialityDoctor> SpecialityDoctors { get; set; }
    }
}
