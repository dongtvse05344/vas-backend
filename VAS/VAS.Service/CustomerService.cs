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
    public interface ICustomerService
    {
        IQueryable<Customer> GetCustomers();
        IQueryable<Customer> GetCustomers(Expression<Func<Customer, bool>> where);
        Customer GetCustomer(Guid id);
        void CreateCustomer(Customer Customer, string userName);
        void UpdateCustomer(Customer Customer, string userName);
        void DeleteCustomer(Customer Customer);
        void Save();
    }
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(ICustomerRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateCustomer(Customer Customer, string userName)
        {
            Customer.DateCreated = DateTime.Now;
            Customer.CreatedByUserName = userName;
            _repository.Add(Customer);
        }

        public void DeleteCustomer(Customer Customer)
        {
            _repository.Delete(Customer);
        }

        public Customer GetCustomer(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Customer> GetCustomers()
        {
            return _repository.GetAll();
        }

        public IQueryable<Customer> GetCustomers(Expression<Func<Customer, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateCustomer(Customer Customer, string userName)
        {
            Customer.DateUpdated = DateTime.Now;
            Customer.UpdatedByUserName = userName;
            _repository.Update(Customer);
        }
    }
}
