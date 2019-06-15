using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Data.Repositories;
using VAS.Model;

namespace VAS.Service
{
    
    public interface IRoomService
    {
        IQueryable<Room> GetRooms();
        IQueryable<Room> GetRooms(Expression<Func<Room, bool>> where);
        Room GetRoom(Guid id);
        void CreateRoom(Room Room, string userName);
        void CreateRooms(List<Room> Room, string userName);
        void UpdateRoom(Room Room, string userName);
        void DeleteRoom(Room Room);
        void Save();
    }
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IRoomRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateRoom(Room Room, string userName)
        {
            Room.DateCreated = DateTime.Now;
            Room.CreatedByUserName = userName;
            _repository.Add(Room);
        }

        public void CreateRooms(List<Room> rooms, string userName)
        {
            foreach (var item in rooms)
            {
				item.IsAvailable = true;
				item.DateCreated = DateTime.Now;
                item.CreatedByUserName = userName;
            }
            _repository.CreateRooms(rooms);
        }

        public void DeleteRoom(Room Room)
        {
            _repository.Delete(Room);
        }

        public Room GetRoom(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Room> GetRooms()
        {
            return _repository.GetAll();
        }

        public IQueryable<Room> GetRooms(Expression<Func<Room, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateRoom(Room Room, string userName)
        {
            Room.DateUpdated = DateTime.Now;
            Room.UpdatedByUserName = userName;
            _repository.Update(Room);
        }
    }
}
