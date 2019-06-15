using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ISchedulingRepository : IRepository<Scheduling>
    {

    }
    public class SchedulingRepository : RepositoryBase<Scheduling>, ISchedulingRepository
    {
        public SchedulingRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
