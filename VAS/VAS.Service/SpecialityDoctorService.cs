using Microsoft.AspNetCore.Identity;
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
    public interface ISpecialityDoctorService
    {
        IQueryable<SpecialityDoctor> GetSpecialityDoctors();
        IQueryable<SpecialityDoctor> GetSpecialityDoctors(Expression<Func<SpecialityDoctor, bool>> where);
        SpecialityDoctor GetSpecialityDoctor(Guid id);
        void CreateSpecialityDoctor(SpecialityDoctor SpecialityDoctor);
        void UpdateSpecialityDoctor(SpecialityDoctor SpecialityDoctor);
        void DeleteSpecialityDoctor(SpecialityDoctor SpecialityDoctor);
        void DeleteSpecialityDoctor(Expression<Func<SpecialityDoctor, bool>> where);
        void Save();
    }
    public class SpecialityDoctorService : ISpecialityDoctorService
    {
        private readonly ISpecialityDoctorRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SpecialityDoctorService(ISpecialityDoctorRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateSpecialityDoctor(SpecialityDoctor SpecialityDoctor)
        {
            _repository.Add(SpecialityDoctor);
        }

        public void DeleteSpecialityDoctor(SpecialityDoctor SpecialityDoctor)
        {
            _repository.Delete(SpecialityDoctor);
        }

        public void DeleteSpecialityDoctor(Expression<Func<SpecialityDoctor, bool>> where)
        {
            _repository.Delete(where);
        }

        public SpecialityDoctor GetSpecialityDoctor(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<SpecialityDoctor> GetSpecialityDoctors()
        {
            return _repository.GetAll();
        }

        public IQueryable<SpecialityDoctor> GetSpecialityDoctors(Expression<Func<SpecialityDoctor, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateSpecialityDoctor(SpecialityDoctor SpecialityDoctor)
        {
            _repository.Update(SpecialityDoctor);
        }
    }
}
