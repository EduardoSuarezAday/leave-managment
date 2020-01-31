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
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveHistoryController(ILeaveHistoryRepository leaveHistoryRepo,
                                      IMapper mapper,
                                      ILeaveTypeRepository leaveTypeRepo,
                                      UserManager<Employee> userManager)
        {
            _leaveHistoryRepo = leaveHistoryRepo;
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
            return View();
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