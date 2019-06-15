using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Model;

namespace VAS.Data.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        void CreateRooms(List<Room> rooms);
    }
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        private readonly DbSet<Room> dbSet;
        public RoomRepository(IDbFactory dbFactory) : base(dbFactory)
        {
            dbSet = base.DbContext.Set<Room>();
        }
        public void CreateRooms(List<Room> rooms)
        {
            dbSet.AddRange(rooms);
        }
    }
}
