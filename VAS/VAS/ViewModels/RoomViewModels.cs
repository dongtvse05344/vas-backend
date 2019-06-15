using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class RoomViewModel
    {
		public Guid Id { get; set; }
		public string Name { get; set; }
        public string SpecialityName { get; set; }
        public string Number { get; set; }
        public string HisCode { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class RoomUpdateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
    }
    public class RoomCreateModel
    {
        public string HisCode { get; set; }
        public Guid SpecialityId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsAvailable { get; set; }
    }

	public class RoomBySpecialViewModel
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
