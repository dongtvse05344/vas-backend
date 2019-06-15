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
    public interface IFamilyService
    {
        IQueryable<Family> GetFamilys();
        IQueryable<Family> GetFamilys(Expression<Func<Family, bool>> where);
        Family GetFamily(Guid id);
        void CreateFamily(Family Family, string userName);
        void UpdateFamily(Family Family, string userName);
        void DeleteFamily(Family Family);
        void DeleteFamily(Expression<Func<Family, bool>> where);
        void Save();
    }
    public class FamilyService : IFamilyService
    {
        private readonly IFamilyRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public FamilyService(IFamilyRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateFamily(Family Family, string userName)
        {
            Family.DateCreated = DateTime.Now;
            Family.CreatedByUserName = userName;
            _repository.Add(Family);
        }

        public void DeleteFamily(Family Family)
        {
            _repository.Delete(Family);
        }

        public void DeleteFamily(Expression<Func<Family, bool>> where)
        {
            _repository.Delete(where);
        }

        public Family GetFamily(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Family> GetFamilys()
        {
            return _repository.GetAll();
        }

        public IQueryable<Family> GetFamilys(Expression<Func<Family, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateFamily(Family Family, string userName)
        {
            Family.DateUpdated = DateTime.Now;
            Family.UpdatedByUserName = userName;
            _repository.Update(Family);
        }
    }
}
