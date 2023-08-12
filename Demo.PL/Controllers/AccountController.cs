using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager  ,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #region Register
        // /Account/Register
        //[HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var User = new ApplicationUser()
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    IsAgree = model.IsAgree,
                    Fname = model.Fname,
                    Lname = model.Lname,
                };
                var Result = await _userManager.CreateAsync(User, model.Password);
                if (Result.Succeeded)
                    return RedirectToAction(nameof(Login));

                foreach (var Error in Result.Errors)
                    ModelState.AddModelError(string.Empty, Error.Description);
            }
            return View(model);
        }
        #endregion

        #region Login

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if(User is not null)
                {

                  var Flag = await _userManager.CheckPasswordAsync(User, model.Password);
                    if (Flag)
                    {
                        var Result = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false);
                        if (Result.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Incorrect Password");
                }
                ModelState.AddModelError(string.Empty, "Email is not Exsits");
            }
            return View(model);

        }
        #endregion

        #region SignOut

        public async Task<IActionResult> SingOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Forget Password
        public IActionResult ForgetPassword()
        {
            return View();
        }

        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(User);
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email , Token = Token} , Request.Scheme);
                    var Email = new Email()
                    {
                        Subject = "Reset Password",
                        To = User.Email,
                        Body = ResetPasswordLink
                    };

                    EmailSettings.SendEmail(Email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                ModelState.AddModelError(string.Empty, "Email is Not Exsits");
            }
            return View(model);
        }

        public IActionResult CheckYourInbox()
        {
            return View();
        }
        #endregion

        #region Reset Password
        public IActionResult ResetPassword(string email , string token)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword (ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                string token = TempData["Token"] as string;
                var user = await _userManager.FindByEmailAsync(email);
                var Result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (Result.Succeeded)
                    return RedirectToAction(nameof(Login));
                foreach (var error in Result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

            }
            return View(model);
          
        }
        #endregion
    }

}

