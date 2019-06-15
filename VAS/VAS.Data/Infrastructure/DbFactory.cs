using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Data.Infrastructure
{
    public class DbFactory : Disposable, IDbFactory
    {
        VASDbContext dbContext;

        public VASDbContext Init()
        {
            return dbContext ?? (dbContext = new VASDbContext());
        }

        protected override void DisposeCore()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }
    }
}
