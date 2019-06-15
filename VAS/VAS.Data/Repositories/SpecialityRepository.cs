using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ISpecialityRepository : IRepository<Speciality>
    {

    }
    public class SpecialityRepository : RepositoryBase<Speciality>, ISpecialityRepository
    {
        public SpecialityRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
