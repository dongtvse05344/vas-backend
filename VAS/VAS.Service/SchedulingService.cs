using Mapster;
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
    public interface ISchedulingService
    {
        IQueryable<Scheduling> GetSchedulings();
        IQueryable<Scheduling> GetSchedulings(Expression<Func<Scheduling, bool>> where);
        Scheduling GetScheduling(Guid id);
        void CreateScheduling(Scheduling Scheduling , int totalBlock, string userName);
		void CreateBlocks(Scheduling scheduling,int totalBlock);

		void UpdateScheduling(Scheduling Scheduling, string userName);
        void DeleteScheduling(Scheduling Scheduling);
        void Save();
    }
    public class SchedulingService : ISchedulingService
    {
        private readonly ISchedulingRepository _schedulingRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SchedulingService(ISchedulingRepository schedulingRepository, IBlockRepository blockRepository, ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
        {
            _schedulingRepository = schedulingRepository;
            _blockRepository = blockRepository;
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
        }

		public void CreateBlocks(Scheduling scheduling, int totalBlock)
		{
			//create blocks for each scheduling(số lượng block dựa vào thời gian bắt đầu kết thúc)
			var startTime = scheduling.StartTime;
			for (int i = 0; i < totalBlock; i++)
			{
				Block block = scheduling.Adapt<Block>();
				block.Id = Guid.Empty;
				block.StartTime = startTime;
				block.SchedulingId = scheduling.Id;
				_blockRepository.Add(block);

				startTime = startTime.Add(new TimeSpan(0, 30, 0));
			}
		}

		public void CreateScheduling(Scheduling scheduling, int totalBlock, string userName)
        {
			//tạo scheduling
            scheduling.DateCreated = DateTime.Now;
            scheduling.CreatedByUserName = userName;
            _schedulingRepository.Add(scheduling);

			//tạo block
			CreateBlocks(scheduling, totalBlock);

		}

        public void DeleteScheduling(Scheduling Scheduling)
        {
            _schedulingRepository.Delete(Scheduling);
        }

        public Scheduling GetScheduling(Guid id)
        {
            return _schedulingRepository.GetById(id);
        }

        public IQueryable<Scheduling> GetSchedulings()
        {
            return _schedulingRepository.GetAll();
        }

        public IQueryable<Scheduling> GetSchedulings(Expression<Func<Scheduling, bool>> where)
        {
            return _schedulingRepository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateScheduling(Scheduling Scheduling, string userName)
        {
            Scheduling.DateUpdated = DateTime.Now;
            Scheduling.UpdatedByUserName = userName;
            _schedulingRepository.Update(Scheduling);
        }
    }
}
