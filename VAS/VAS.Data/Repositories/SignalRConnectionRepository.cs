using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ISignalRConnectionRepository : IRepository<SignalRConnection>
    {

    }
    public class SignalRConnectionRepository : RepositoryBase<SignalRConnection>, ISignalRConnectionRepository
    {
        public SignalRConnectionRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
