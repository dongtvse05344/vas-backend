using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface IDoctorBasicRepository: IRepository<DoctorBasic>
    {

    }
    public class DoctorBasicRepository : RepositoryBase<DoctorBasic>, IDoctorBasicRepository
    {
        public DoctorBasicRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
