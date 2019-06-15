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
    public interface ISpecialityService
    {
        IQueryable<Speciality> GetSpecialitys();
        IQueryable<Speciality> GetSpecialitys(Expression<Func<Speciality, bool>> where);
        Speciality GetSpeciality(Guid id);
        void CreateSpeciality(Speciality speciality, string userName);
        void UpdateSpeciality(Speciality speciality, string userName);
        void DeleteSpeciality(Speciality Speciality);
        void Save();
    }
    public class SpecialityService : ISpecialityService
    {
        private readonly ISpecialityRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialityService(ISpecialityRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateSpeciality(Speciality speciality, string userName)
        {
            speciality.DateCreated = DateTime.Now;
            speciality.CreatedByUserName = userName;
            _repository.Add(speciality);
        }

        public void DeleteSpeciality(Speciality speciality)
        {
            _repository.Delete(speciality);
        }

        public Speciality GetSpeciality(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Speciality> GetSpecialitys()
        {
            return _repository.GetAll();
        }

        public IQueryable<Speciality> GetSpecialitys(Expression<Func<Speciality, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateSpeciality(Speciality speciality, string userName)
        {
            speciality.DateUpdated = DateTime.Now;
            speciality.UpdatedByUserName = userName;
            _repository.Update(speciality);
        }
    }
}
