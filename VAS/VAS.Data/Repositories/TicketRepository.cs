using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ITicketRepository : IRepository<Ticket>
    {

    }
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
