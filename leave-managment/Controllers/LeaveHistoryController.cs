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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_managment.Controllers
{
    [Authorize]
    public class LeaveHistoryController : Controller
    {
        private readonly ILeaveHistoryRepository _leaveHistoryRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveHistoryController(ILeaveHistoryRepository leaveHistoryRepo,
                                      IMapper mapper,
                                      ILeaveAllocationRepository leaveAllocationRepo,
                                      ILeaveTypeRepository leaveTypeRepo,
                                      UserManager<Employee> userManager)
        {
            _leaveHistoryRepo = leaveHistoryRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: LeaveHistory
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            var leaveHistory = _leaveHistoryRepo.FindAll();
            
            var leaveHistoryVM = _mapper.Map<List<LeaveHistoryVM>>(leaveHistory);

            var model = new AdminLeaveHistoryViewVM 
            {
                TotalRequests = leaveHistoryVM.Count,
                
                AprovedRequests = leaveHistoryVM.Count(q => q.Approved == true),
                
                PendingRequests = leaveHistoryVM.Count(q => q.Approved == null),
                
                RejectedRequests = leaveHistoryVM.Count(q => q.Approved == false),

                LeaveHistories = leaveHistoryVM
            };

            return View(model);
        }

        // GET: LeaveHistory/Details/5
        public ActionResult Details(int id)
        {
            var leaveHistory = _leaveHistoryRepo.FindById(id);
            var model = _mapper.Map<LeaveHistoryVM>(leaveHistory);
            return View(model);
        }
        
        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaveHistory = _leaveHistoryRepo.FindById(id);
                var employeeId = leaveHistory.RequestingEmployeeId;
                var leaveTypeId = leaveHistory.LeaveTypeId;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(employeeId, leaveTypeId);
                int daysRequested = (int)(leaveHistory.EndDate - leaveHistory.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;
                
                leaveHistory.Approved = true;
                leaveHistory.ApprovedById = user.Id;

                 _leaveHistoryRepo.Update(leaveHistory);
                 _leaveAllocationRepo.Update(allocation);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult RejectRequest(int id) 
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaveHistory = _leaveHistoryRepo.FindById(id);
                leaveHistory.Approved = false;
                leaveHistory.ApprovedById = user.Id;

                var isSucces = _leaveHistoryRepo.Update(leaveHistory);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult MyLeave() 
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeAllocations = _leaveAllocationRepo.GetLeaveAllocationByEmployee(employee.Id);
            var employeeRequests = _leaveHistoryRepo.GetLeaveRequestsByEmployee(employee.Id);

            var employeeAllocationVM = _mapper.Map<List<LeaveAllocationVM>>(employeeAllocations);
            var employeeHistoryVM = _mapper.Map<List<LeaveHistoryVM>>(employeeRequests);

            var model = new EmployeeLeaveHistoryVM
            {
                LeaveAllocations = employeeAllocationVM,
                LeaveHistories = employeeHistoryVM
            };

            return View(model);

        }

        public ActionResult CancelRequest(int Id) 
        {  
                var leaveHistory = _leaveHistoryRepo.FindById(Id);
                _leaveHistoryRepo.Delete(leaveHistory);
                return RedirectToAction("MyLeave");
        }

        // GET: LeaveHistory/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();

            var leaveTypesItems = leaveTypes.Select(q => new SelectListItem {Text = q.Name, Value = q.Id.ToString()});
            
            var model = new CreateLeaveRequestVM { LeaveTypes = leaveTypesItems };
            
            return View(model);
        }

        // POST: LeaveHistory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var leaveTypes = _leaveTypeRepo.FindAll();

                var leaveTypesItems = leaveTypes.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString() });

                model.LeaveTypes = leaveTypesItems;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (DateTime.Compare(model.StartDate, model.EndDate) > 0)
                {
                    ModelState.AddModelError("","Start Date cannot be futher in the future than the end date");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;

                var allocation = _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);

                int daysRequested = (int) (model.EndDate.Date - model.StartDate.Date).TotalDays;

                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You dont have suficient days for this request");
                    return View(model);
                }

                var leaveHistorymodel = new LeaveHistoryVM 
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId
                };

                var leaveHistory = _mapper.Map<LeaveHistory>(leaveHistorymodel);
                
                var isSuccess = _leaveHistoryRepo.Create(leaveHistory);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong with submitting your record");
                    return View(model);
                }
                    
                return RedirectToAction(nameof(Index),"Home");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveHistory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveHistory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveHistory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveHistory/Delete/5
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