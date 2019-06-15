using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface INurseRepository : IRepository<Nurse>
    {

    }
    public class NurseRepository : RepositoryBase<Nurse>, INurseRepository
    {
        public NurseRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
