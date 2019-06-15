using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ISpecialityDoctorRepository : IRepository<SpecialityDoctor>
    {

    }
    public class SpecialityDoctorRepository : RepositoryBase<SpecialityDoctor>, ISpecialityDoctorRepository
    {
        public SpecialityDoctorRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
