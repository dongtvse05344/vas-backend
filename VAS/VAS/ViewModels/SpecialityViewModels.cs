using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class SpecialityViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class SpecialityCreateModel
    {
        public string Name { get; set; }
    }

    public class SpecialityUpdateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
