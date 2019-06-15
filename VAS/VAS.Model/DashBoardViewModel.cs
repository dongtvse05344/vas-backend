using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Model
{
    public class DashboardAccountVM
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class DashboardTicketVM
    {
        public DateTime Date { get; set; }
        public int NumberBooked { get; set; }
        public int NumberChecked { get; set; }
    }
}
