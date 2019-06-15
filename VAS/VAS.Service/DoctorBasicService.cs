using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VAS.Data.Infrastructure;
using VAS.Data.Repositories;
using VAS.Model;

namespace VAS.Service
{
    public interface IDoctorBasicService
    {
        IQueryable<DoctorBasic> GetDoctorsBasic();
        IQueryable<DoctorBasic> GetDoctorsBasic(Expression<Func<DoctorBasic, bool>> where);
        DoctorBasic GetDoctorBasic(String id);
        void CreateDoctorBasic(DoctorBasic doctor, string userName);
        void UpdateDoctorBasic(DoctorBasic doctor, string userName);
        void DeleteDoctorBasic(DoctorBasic doctor);
        void Save();
    }
    public class DoctorBasicService : IDoctorBasicService
    {
        private readonly IDoctorBasicRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DoctorBasicService(IDoctorBasicRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateDoctorBasic(DoctorBasic doctor, string userName)
        {
            doctor.DateCreated = DateTime.Now;
            doctor.CreatedByUserName = userName;
            _repository.Add(doctor);

        }

        public void DeleteDoctorBasic(DoctorBasic doctor)
        {
            _repository.Delete(doctor);
        }

        public DoctorBasic GetDoctorBasic(String id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<DoctorBasic> GetDoctorsBasic()
        {
            return _repository.GetAll();
        }

        public IQueryable<DoctorBasic> GetDoctorsBasic(Expression<Func<DoctorBasic, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateDoctorBasic(DoctorBasic doctor, string userName)
        {
            doctor.DateUpdated = DateTime.Now;
            doctor.UpdatedByUserName = userName;
            _repository.Update(doctor);
        }
    }
}
