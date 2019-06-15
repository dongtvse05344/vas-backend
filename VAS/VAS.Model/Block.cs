using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Block
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string DoctorId { get; set; }
        public Guid RoomId { get; set; }
		public DateTime Date { get; set; }
		public TimeSpan StartTime { get; set; }
        public int TotalTicket { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsFull { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }

        public Guid SchedulingId { get; set; }
        public virtual Scheduling Scheduling { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
