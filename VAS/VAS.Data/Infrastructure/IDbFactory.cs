using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        VASDbContext Init();
    }
}
