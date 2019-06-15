using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
		
		public string Index { get; set; }
        public DateTime? BookingDate { get; set; }
        public string Note { get; set; }
        public string  CustomerId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
		public int Status { get; set; }
        public Guid BlockId { get; set; }
        public virtual Block Block { get; set; }
    }
}
