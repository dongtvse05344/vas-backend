using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAS.ViewModels
{
    public class TicketBookingVM
    {
        public Guid TicketId { get; set; }
        public string Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
    }
    public class BlockVM
    {
        public BlockVM()
        {
            Customers = new List<TicketBookingVM>();
        }
        public Guid Id { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool IsFull { get; set; }
        public List<TicketBookingVM> Customers { get; set; }
    }
    public class DoctorBookingVM
    {
        public DoctorBookingVM()
        {
            Blocks = new List<BlockVM>();
        }
        public string Id { get; set; }
        public string FullName { get; set; }
        public List<BlockVM> Blocks { get; set; }
    }
    public class ManageBlockVM
    {
        public ManageBlockVM()
        {
            Doctors = new List<DoctorBookingVM>();
        }
        public string SpecialityId { get; set; }
        public string SpecialityName { get; set; }
        public List<DoctorBookingVM> Doctors { get; set; }
    }

    public class BlockBookingViewModel
    {
        public Guid Id { get; set; }
        public string Time { get; set; }
        public bool IsFull { get; set; }
    }
    public class BlockOfDoctorViewModel
    {
        public BlockOfDoctorViewModel()
        {
            Blocks = new List<BlockBookingViewModel>();
        }
        public string FullName { get; set; }
        public List<BlockBookingViewModel> Blocks { get; set; }
    }
    public class SchedulingVM
    {
        public SchedulingVM ()
        {
            Blocks = new List<BlockBookingViewModel>();
        }
        public DateTime Date { get; set; }
        public List<BlockBookingViewModel> Blocks { get; set; }
    }
}
