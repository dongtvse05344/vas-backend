using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
    }
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        private readonly DbSet<Customer> dbSet;
        public CustomerRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            dbSet = base.DbContext.Set<Customer>();
        }
    }
}
