using leave_managment.Contracts;
using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveHistoryRepository : ILeaveHistoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LeaveHistoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Create(LeaveHistory entity)
        {
            _dbContext.LeaveHistories.Add(entity);
            return Save();

        }

        public bool Delete(LeaveHistory entity)
        {
            _dbContext.LeaveHistories.Remove(entity);
            return Save();
        }

        public ICollection<LeaveHistory> FindAll()
        {
            return _dbContext.LeaveHistories.ToList();
        }

        public LeaveHistory FindById(int id)
        {
            return _dbContext.LeaveHistories.Find(id);
        }

        public bool IsExists(int id)
        {
            return _dbContext.LeaveHistories.Any(q => q.Id == id);
        }

        public bool Save()
        {
            return _dbContext.SaveChanges() > 0;
        }

        public bool Update(LeaveHistory entity)
        {
            _dbContext.Update(entity);
            return Save();
        }
    }
}
