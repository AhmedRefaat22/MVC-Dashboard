using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager ,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        public  async Task<IActionResult> Index(string SearchValue)
        {
            if (string.IsNullOrEmpty(SearchValue))
            {
                var users = _userManager.Users.Select(
                    U => new UserViewModel()
                    {
                        Id = U.Id,
                        fname = U.Fname,
                        lname = U.Lname,
                        PhoneNumber = U.PhoneNumber,
                        Email = U.Email,
                        Roles = _userManager.GetRolesAsync(U).Result
                    }).ToList() ;

                return View(users);
            }
            else
            {
                var User = await _userManager.FindByEmailAsync(SearchValue);
                var MappedUser = new UserViewModel()
                {
                    Id = User.Id,
                    fname = User.Fname,
                    lname = User.Lname,
                    PhoneNumber = User.PhoneNumber,
                    Email = User.Email,
                    Roles = _userManager.GetRolesAsync(User).Result

                };

                return View(new List<UserViewModel> { MappedUser });
            }
        }

        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            var MappedUser = _mapper.Map<ApplicationUser, UserViewModel>(user);

            return View(ViewName, MappedUser);
        }

        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, UserViewModel UserVm)
        {
            if (id != UserVm.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var User = await _userManager.FindByIdAsync(id);
                    User.Fname = UserVm.fname;
                    User.Lname = UserVm.lname;
                    User.PhoneNumber = UserVm.PhoneNumber;
                   await _userManager.UpdateAsync(User);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

            }
            return View(UserVm);
        }

        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
       
            try
            {
                var user = await _userManager.FindByIdAsync(id);
               await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error" , "Home");
            }
        }
    }
}
