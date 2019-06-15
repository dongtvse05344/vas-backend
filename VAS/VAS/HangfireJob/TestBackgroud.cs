using System;
using VAS.Service;
using VAS.Utils;

namespace VAS.HangfireJob
{
    public class TestBackgroud
    {
        private readonly ITicketService _ticketService;

        public TestBackgroud(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public void ChangeStatusOnTicket()
        {
            var currentTickets = _ticketService.GetTickets(t =>
            t.Block.Date == DateTime.Now.Date
            && TimeSpan.Compare(t.Block.StartTime.Add(new TimeSpan(0,30,0)), DateTime.Now.TimeOfDay) < 0
            && t.Status == (int)CustomerStatus.Waiting);
            foreach (var item in currentTickets)
            {
                item.Status = (int)CustomerStatus.Absence;
                _ticketService.UpdateTicket(item, "Huy Background");
            }
            _ticketService.Save();
        }
    }
}
