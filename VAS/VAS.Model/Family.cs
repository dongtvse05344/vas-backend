using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Model
{
    public class Family
    {
        public string MyUserId { get; set; }
        public virtual MyUser MyUser { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public string Relationship { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdatedByUserName { get; set; }
    }
}
