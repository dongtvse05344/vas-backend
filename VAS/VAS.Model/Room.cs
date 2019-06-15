using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsAvailable { get; set; }
        public string HisCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }

        public virtual ICollection<Scheduling> Schedulings { get; set; }
        public Guid SpecialityId { get; set; }
        public virtual Speciality Speciality { get; set; }
    }
}
