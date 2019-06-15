using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using VAS.Data.Infrastructure;
using VAS.Data.Repositories;
using VAS.Model;

namespace VAS.Service
{
    public interface ISignalRConnectionService
    {
        void CreateConnection(SignalRConnection connection);
        void DeleteConnection(SignalRConnection connection);
        void DeleteConnection(Expression<Func<SignalRConnection, bool>> where);

        void Save();

    }
    public class SignalRConnectionService : ISignalRConnectionService
    {
        private readonly ISignalRConnectionRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public SignalRConnectionService(ISignalRConnectionRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateConnection(SignalRConnection connection)
        {
            _repository.Add(connection);
        }

        public void DeleteConnection(SignalRConnection connection)
        {
            _repository.Delete(connection);
        }

        public void DeleteConnection(Expression<Func<SignalRConnection, bool>> where)
        {
            _repository.Delete(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
    }
}
