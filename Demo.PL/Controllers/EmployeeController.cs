using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]

    public class EmployeeController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork, 
            IMapper mapper )
        {
 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            IEnumerable<Employee> employees;
            if (string.IsNullOrEmpty(SearchValue))
                 employees = await _unitOfWork.EmployeeRepository.GetAll();
            else
                 employees = _unitOfWork.EmployeeRepository.SearchEmployeesByName(SearchValue);
              
            var MappedEmployee = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
            return View(MappedEmployee);

        }
        public IActionResult Create()
        {
           ViewBag.Departments = _unitOfWork.DepartmentRepository.GetAll().Result;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVm)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                // Manual Mapping 

                ///var employee = new Employee()
                ///{
                ///    Name = employeeVm.Name,
                ///    Address = employeeVm.Address,
                ///    Email = employeeVm.Email,
                ///    Age = employeeVm.Age,
                ///    DepartmentId = employeeVm.DepartmentId,
                ///    HireDate = employeeVm.HireDate,
                ///    PhoneNumber = employeeVm.PhoneNumber,
                ///};

                //Employee employee = (Employee) employeeVm;
               employeeVm.ImageName =  DocumentSettings.UploadFile(employeeVm.Image, "Images");
               var MappedEmployee =  _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                await _unitOfWork.EmployeeRepository.Add(MappedEmployee); 
                await _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(employeeVm);

        }

        public async Task<IActionResult> Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var employee = await _unitOfWork.EmployeeRepository.Get(id.Value);
            if (employee is null)
                return NotFound();
            var MappedEmployee = _mapper.Map<Employee, EmployeeViewModel>(employee);

            return View(ViewName, MappedEmployee);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            return  await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVm)
        {
            if (id != employeeVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                    _unitOfWork.EmployeeRepository.Update(MappedEmployee);
                   await _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(employeeVm);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeVm)
        {
            if (id != employeeVm.Id)
                return BadRequest();
            try
            {
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVm);
                _unitOfWork.EmployeeRepository.Delete(MappedEmployee);
              int Result = await _unitOfWork.Complete();
                if (Result > 0)
                    DocumentSettings.DeleteFile(employeeVm.ImageName, "Images");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVm);
            }
        }
    }
}
