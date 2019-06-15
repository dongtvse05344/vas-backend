using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Scheduling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
        public bool IsAvailable { get; set; }
		public int TotalTicket { get; set; }
		public string DoctorId { get; set; }
		
        [ForeignKey("DoctorId")]
        public virtual DoctorBasic Doctor { get; set; }
        public string NurseId { get; set; }
        [ForeignKey("NurseId")]
        public virtual Nurse Nurse { get; set; }
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; }
        public string SpecialityId { get; set; }
        public string SpecialityName { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
    }
}
