using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Model
{
    public class SpecialityDoctor
    {
        public string DoctorBasicId { get; set; }
        public virtual DoctorBasic DoctorBasic { get; set; }
        public Guid SpecialityId { get; set; }
        public virtual Speciality Speciality { get; set; }
    }
}
