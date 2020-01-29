using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment.Models
{
    public class LeaveAllocationVM
    {
        public int Id { get; set; }

        public int NumberOfDays { get; set; }

        public int Period { get; set; }

        public DateTime DateCreated { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public LeaveTypeVM LeaveType { get; set; }

        public int LeaveTypeId { get; set; }
    }

    public class CreateLeaveAllocationVM 
    {
        public List<LeaveTypeVM> leaveTypesVM { get; set; }

        public int NumberUpdated { get; set; }
    }

    public class ViewAllocationVM
    {
        public EmployeeVM Employee { get; set; }

        public String EmployeeId { get; set; }
        
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }
    
    public class EditLeaveAllocationVM
    {
        public int Id { get; set; }

        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; }

        public LeaveTypeVM leaveTypesVM { get; set; }

        public int NumberOfDays { get; set; }
    }
}
