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
    public interface IBlockService
    {
        IQueryable<Block> GetBlocks();
        IQueryable<Block> GetBlocks(Expression<Func<Block, bool>> where);
        Block GetBlock(Guid id);
        void CreateBlock(Block Block, string userName);
        void UpdateBlock(Block Block, string userName);
        void DeleteBlock(Block Block);
        void DeleteBlock(Expression<Func<Block, bool>> where);
        void Save();
    }
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public BlockService(IBlockRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public void CreateBlock(Block Block, string userName)
        {
            Block.DateCreated = DateTime.Now;
            Block.CreatedByUserName = userName;
            _repository.Add(Block);
        }

        public void DeleteBlock(Block Block)
        {
            _repository.Delete(Block);
        }

        public void DeleteBlock(Expression<Func<Block, bool>> where)
        {
            _repository.Delete(where);
        }

        public Block GetBlock(Guid id)
        {
            return _repository.GetById(id);
        }

        public IQueryable<Block> GetBlocks()
        {
            return _repository.GetAll();
        }

        public IQueryable<Block> GetBlocks(Expression<Func<Block, bool>> where)
        {
            return _repository.GetMany(where);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void UpdateBlock(Block Block, string userName)
        {
            Block.DateUpdated = DateTime.Now;
            Block.UpdatedByUserName = userName;
            _repository.Update(Block);
        }
    }
}
