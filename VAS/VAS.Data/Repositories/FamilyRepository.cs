using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface IFamilyRepository : IRepository<Family>
    {

    }
    public class FamilyRepository : RepositoryBase<Family>, IFamilyRepository
    {
        public FamilyRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
