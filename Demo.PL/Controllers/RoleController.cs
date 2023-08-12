using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using AutoMapper;

namespace Demo.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager , IMapper mapper)
        {
            this._roleManager = roleManager;
            this._mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            if (string.IsNullOrEmpty(SearchValue))
            {
                var Roles = _roleManager.Roles.Select(
                    R => new RoleViewModel()
                    {
                        Id = R.Id,
                        RoleName = R.Name,
                       
                    }).ToList();

                return View(Roles);
            }
            else
            {
                var Role = await _roleManager.FindByIdAsync(SearchValue);
                var MappedRole = new RoleViewModel()
                {
                    Id = Role.Id,
                    RoleName= Role.Name,
                };

                return View(new List<RoleViewModel> { MappedRole });
            }
        }

        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role is null)
                return NotFound();
            var MappedRole = _mapper.Map< IdentityRole , RoleViewModel>(Role);

            return View(ViewName, MappedRole);
        }

        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleViewModel RoleVm)
        {
            if (id != RoleVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await _roleManager.FindByIdAsync(id);
                    Role.Id = RoleVm.Id;
                    Role.Name = RoleVm.RoleName;
                    await _roleManager.UpdateAsync(Role);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(RoleVm);
        }

        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete( string Id)
        {
            try
            {
                var Role = await _roleManager.FindByIdAsync(Id);
                await _roleManager.DeleteAsync(Role);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Create ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mappedRole = _mapper.Map<RoleViewModel , IdentityRole>(model);
                await _roleManager.CreateAsync(mappedRole);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
