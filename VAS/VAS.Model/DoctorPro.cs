using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VAS.Model
{
    public class DoctorPro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Degree { get; set; }
        public string Language { get; set; }
        public string Certification { get; set; }
        public string Experience { get; set; }
        public virtual DoctorBasic DoctorBasic { get; set; }
    }
}
