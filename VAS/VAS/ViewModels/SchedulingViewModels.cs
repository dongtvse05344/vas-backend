using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class SchedulingCreateModel
    {
        //scheduling
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorId { get; set; }
        public string NurseId { get; set; }
        public Guid RoomId { get; set; }
        

        //block
        public int TotalTicket { get; set; }
    }
    public class SchedulingUpdateModel
    {
        public Guid Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string NurseId { get; set; }
        public Guid RoomId { get; set; }
        public int TotalTicket { get; set; }
    }

    public class ActiveVM
    {
        public string[] Schedulings { get; set; }
    }
    public class SchedulingViewModel
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public Guid RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string SpecialityId { get; set; }
        public string SpecialityName { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalTicket { get; set; }
    }

    public class SchedulingVM2
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string NurseId { get; set; }
        public string NurseName { get; set; }
        public Guid RoomId { get; set; }
        public string RoomName { get; set; }
        public string SpecialityId { get; set; }
        public string SpecialityName { get; set; }
        public bool IsAvailable { get; set; }
        public int TotalTicket { get; set; }
    }

	public class SchedulingsByDoctorIdVM
	{
		public SchedulingsByDoctorIdVM()
		{
			Schedulings = new List<SchedulingOfDoctor>();
		}
		public DateTime Date { get; set; }
		public List<SchedulingOfDoctor> Schedulings { get; set; }
	}

	public class SchedulingOfDoctor
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public Guid RoomId { get; set; }
		public string RoomName { get; set; }

	}
}
