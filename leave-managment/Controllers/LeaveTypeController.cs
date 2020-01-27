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
using Microsoft.AspNetCore.Mvc;

namespace leave_managment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveTypeController : Controller
    {
        private readonly ILeaveTypeRepository _repository;
        private readonly IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper; 
        }

        // GET: LeaveType
        public ActionResult Index()
        {
            var leaveTypes = _repository.FindAll().ToList();
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes);
            return View(model);
        }

        // GET: LeaveType/Details/5
        public ActionResult Details(int id)
        {
            if (!_repository.IsExists(id))
            {
                return NotFound();
            }
            var leaveType = _repository.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            
            return View(model);
        }

        // GET: LeaveType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                
                model.DateCreated = DateTime.Now;
                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = _repository.Create(leaveType);

                if (!isSuccess)
                {
                    ModelState.AddModelError("","Somethig went wrong When you try to create a Leave Type...");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveType/Edit/5
        public ActionResult Edit(int id)
        {
            if (!_repository.IsExists(id))
            {
                ModelState.AddModelError("","Somethig went wrong. ID incorrect...");
                return NotFound();
            }
            var leaveType = _repository.FindById(id);

            var model = _mapper.Map<LeaveTypeVM>(leaveType);

            return View(model);
        }

        // POST: LeaveType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = _repository.Update(leaveType);

                if (!isSuccess)
                {
                    ModelState.AddModelError("","The Leave Type was not edited...");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "The Leave Type was not deleted...");
                return View(model);
            }
        }

        // GET: LeaveType/Delete/5
        public ActionResult Delete(int id)
        {
            var leaveType = _repository.FindById(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            var isSuccess = _repository.Delete(leaveType);

            if (!isSuccess)
            {
                ModelState.AddModelError("", "The Leave Type was not deleted...");
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveType/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LeaveTypeVM model)
        {
            try
            {
                var leaveType = _repository.FindById(id);
                var isSuccess = _repository.Delete(leaveType);

                if (!isSuccess)
                {
                    ModelState.AddModelError("", "The Leave Type was not deleted...");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}