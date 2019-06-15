using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class SignalRConnection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string  ConnectionId { get; set; }
        public string Username { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
