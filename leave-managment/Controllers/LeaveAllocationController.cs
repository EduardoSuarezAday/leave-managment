using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_managment.Contracts;
using leave_managment.Data;
using leave_managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace leave_managment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(ILeaveTypeRepository leaveTypeRepo,
                                   ILeaveAllocationRepository leaveAllocationRepo,
                                   IMapper mapper,
                                   UserManager<Employee> userManager)
        {
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }
        // GET: LeaveAllocation
        public ActionResult Index()
        {
            var leaveTypes = _leaveTypeRepo.FindAll().ToList();
            var mappedleaveTypes= _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);
            var model = new CreateLeaveAllocationVM
            {
                NumberUpdated = 0,
                leaveTypesVM = mappedleaveTypes
            };
            return View(model);
        }

        public ActionResult SetLeave(int id) 
        {
            var leaveType = _leaveTypeRepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var empl in employees)
            {
                if (_leaveAllocationRepo.CheckAllocation(id, empl.Id))
                    continue;

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = empl.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                _leaveAllocationRepo.Create(leaveAllocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees() 
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);

        }

        // GET: LeaveAllocation/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
            var period = DateTime.Now.Year;
            var allocation = _mapper.Map<List<LeaveAllocationVM>>(_leaveAllocationRepo.GetLeaveAllocationByEmployee(id));
            var model = new ViewAllocationVM 
            { 
                Employee = employee,
                LeaveAllocations = allocation
            };
            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var model = _mapper.Map<EditLeaveAllocationVM>(_leaveAllocationRepo.FindById(id));
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var record = _leaveAllocationRepo.FindById(model.Id);
                record.NumberOfDays = model.NumberOfDays;
                var isSuccess = _leaveAllocationRepo.Update(record);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "The Leave allocation was not edited...");
                    return View(model);
                }

                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}