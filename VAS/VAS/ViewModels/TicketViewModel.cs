using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
	
    public class CustomerTicketHistoryVM
    {
        public CustomerTicketHistoryVM()
        {
            Tickets = new List<TicketVM>();
        }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<TicketVM> Tickets { get; set; }
    }

    public class TicketVM
    {
        public Guid Id { get; set; }
        public string DoctorName { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Note { get; set; }
    }
	public class PastNowFutureTicketVM
	{
		public PastNowFutureTicketVM()
		{
			Tickets = new List<AllTicketTodayVM>();
		}

		public string TicketType { get; set; }
		public List<AllTicketTodayVM> Tickets { get; set; }
	}
	public class AllTicketTodayVM
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCMDN { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public TimeSpan StartTime { get; set; }
        public int Status { get; set; }
		public string TicketType { get; set; }
	}

    public class BookingCM
	{
		public Guid BlockId { get; set; }
		public Guid CustomerId { get; set; }
		public string Note { get; set; }
	}

	
}
