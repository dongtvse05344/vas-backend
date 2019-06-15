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
    public interface INurseService
    {
        IQueryable<Nurse> GetNurses();
        IQueryable<Nurse> GetNurses(Expression<Func<Nurse, bool>> where);
        Nurse GetNurse(string id);
        void CreateNurse(Nurse Nurse, string userName);
        void UpdateNurse(Nurse Nurse, string userName);
        void DeleteNurse(Nurse Nurse);
        void Save();
    }
    public class NurseService : INurseService
    {
        private readonly INurseRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NurseService(INurseRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateNurse(Nurse Nurse, string userName)
        {
            Nurse.DateCreated = DateTime.Now;
            Nurse.CreatedByUserName = userName;
            _repository.Add(Nurse);
        }

        public void DeleteNurse(Nurse Nurse)
        {
            _repository.Delete(Nurse);
        }

        public Nurse GetNurse(string id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Nurse> GetNurses()
        {
            return _repository.GetAll();
        }

        public IQueryable<Nurse> GetNurses(Expression<Func<Nurse, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateNurse(Nurse Nurse, string userName)
        {
            Nurse.DateUpdated = DateTime.Now;
            Nurse.UpdatedByUserName = userName;
            _repository.Update(Nurse);
        }
    }
}
