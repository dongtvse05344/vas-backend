using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface IBlockRepository : IRepository<Block>
    {

    }
    public class BlockRepository : RepositoryBase<Block>, IBlockRepository
    {
        public BlockRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
