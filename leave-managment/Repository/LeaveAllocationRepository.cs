using leave_managment.Contracts;
using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LeaveAllocationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Create(LeaveAllocation entity)
        {
            _dbContext.LeaveAllocations.Add(entity);
            return Save();

        }

        public bool Delete(LeaveAllocation entity)
        {
            _dbContext.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll()
        {
            return _dbContext.LeaveAllocations.ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _dbContext.LeaveAllocations.Find(id);
        }
        
        public bool IsExists(int id)
        {
            return _dbContext.LeaveAllocations.Any(q => q.Id == id);
        }

        public bool Save()
        {
            return _dbContext.SaveChanges() > 0;
        }

        public bool Update(LeaveAllocation entity)
        {
            _dbContext.Update(entity);
            return Save();
        }
    }
}
