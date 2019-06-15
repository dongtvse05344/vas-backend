using System;
using System.Collections.Generic;
using System.Text;

namespace VAS.Data.Infrastructure
{
    public interface IUnitOfWork
    {
        void Commit();
    }
}
