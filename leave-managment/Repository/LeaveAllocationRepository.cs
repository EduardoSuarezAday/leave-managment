using leave_managment.Contracts;
using leave_managment.Data;
using Microsoft.EntityFrameworkCore;
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

        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period)
                .Any();
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
            return _dbContext.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .ToList();
        }

        public LeaveAllocation FindById(int id)
        {
            return _dbContext.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Employee)
                .FirstOrDefault(q=> q.Id == id);
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == id && q.Period == period)
                .ToList();
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
