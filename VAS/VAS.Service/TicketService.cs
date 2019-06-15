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
    public interface ITicketService
    {
        IQueryable<Ticket> GetTickets();
        IQueryable<Ticket> GetTickets(Expression<Func<Ticket, bool>> where);
        Ticket GetTicket(Guid id);
        void CreateTicket(Ticket Ticket, string userName);
        void UpdateTicket(Ticket Ticket, string userName);
        void DeleteTicket(Ticket Ticket);
        void Save();
    }
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(ITicketRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateTicket(Ticket Ticket, string userName)
        {
            Ticket.DateCreated = DateTime.Now;
            Ticket.CreatedByUserName = userName;
            _repository.Add(Ticket);
        }

        public void DeleteTicket(Ticket Ticket)
        {
            _repository.Delete(Ticket);
        }

        public Ticket GetTicket(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Ticket> GetTickets()
        {
            return _repository.GetAll();
        }

        public IQueryable<Ticket> GetTickets(Expression<Func<Ticket, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateTicket(Ticket Ticket, string userName)
        {
            Ticket.DateUpdated = DateTime.Now;
            Ticket.UpdatedByUserName = userName;
            _repository.Update(Ticket);
        }
    }
}
