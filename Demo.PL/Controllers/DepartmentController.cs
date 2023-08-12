using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        // /Department/Index
        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAll();
            return View(departments);
        }

        //[HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department)
        {

            if(ModelState.IsValid) // Server Side Validation
            {
                // 3. TempData -> Dicionary Object 
                // To Transfer data from action to action
               await _unitOfWork.DepartmentRepository.Add(department);
                int Result = await _unitOfWork.Complete();
                if (Result > 0)
                    TempData["Message"] = "Department Added Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(department);

        }

        // /Department/Details/1
        // /Department/Details
        public async Task<IActionResult> Details(int? id , string ViewName= "Details")
        {
            if (id is null)
                return BadRequest();
            var department = await _unitOfWork.DepartmentRepository.Get(id.Value);
            if (department is null)
                return NotFound();
            return View(ViewName,department);
        }

        // /Department/Edit/1
        // /Department/Edit

        public async Task<IActionResult> Edit (int? id)
        {
            //if(id is null)
            //    retu`rn BadRequest();
            //var department = _departmentRepositpory.Get(id.Value);
            //if(department is null)
            //    return NotFound();
            //return View(department);
            return await Details(id , "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int id , Department department)
        {
            if(id != department.Id)
                return BadRequest();
            if(ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    await _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty , ex.Message);     
                }
               
            }
            return View(department);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute]int id,Department department)
        {
            if(id!= department.Id)
                return BadRequest();
            try
            {
                _unitOfWork.DepartmentRepository.Delete(department);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError (string.Empty , ex.Message);
                return View(department);
            }
        }
    }
}
