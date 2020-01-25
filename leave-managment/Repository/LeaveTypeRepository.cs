using leave_managment.Contracts;
using leave_managment.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LeaveTypeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Create(LeaveType entity)
        {
            _dbContext.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveType entity)
        {
            _dbContext.LeaveTypes.Remove(entity);
            return Save();
        }

        public ICollection<LeaveType> FindAll()
        {
            return _dbContext.LeaveTypes.ToList();
        }

        public LeaveType FindById(int id)
        {
            return _dbContext.LeaveTypes.Find(id);
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsExists(int id)
        {
           return _dbContext.LeaveTypes.Any(q => q.Id == id);
        }

        public bool Save()
        {
           return _dbContext.SaveChanges() > 0;
        }

        public bool Update(LeaveType entity)
        {
            _dbContext.Update(entity);
            return Save();
        }
    }
}
