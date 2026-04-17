using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using SocialMediaApplication.DAL.Models;
using SocialMediaApplication.PL.Helpers;
using SocialMediaApplication.PL.Services.EmailSender;
using SocialMediaApplication.PL.ViewModels.Account;
using System.Threading.Tasks;

namespace SocialMediaApplication.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
        }
        #region Sign Up
        [HttpGet]
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();  
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user is null)
                {
                    var fileName = DocumentSettings.UploadFile(model.profilePicture, "images/Users");
                    user = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsAgree = model.IsAgree,
                        profilePictureName =fileName,
                    };  
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if(result.Succeeded)
                        return RedirectToAction(nameof(SignIn));
                    foreach(var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    DocumentSettings.DeleteFile(fileName, "images/Users");

                }
                else
                    ModelState.
                        AddModelError(
                        string.Empty, "this username is already in use for another account!");

            }
            return View(model);
        }
        #endregion

        #region Sign In

        [HttpGet]
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user is not null)
                {
                    var flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if(flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

                        if (result.IsLockedOut)
                            ModelState.AddModelError(string.Empty, "Your Account is locked!!");

                        if (result.Succeeded)
                            return RedirectToAction(nameof(HomeController.Index), "Home");

                        if (result.IsNotAllowed)
                            ModelState.AddModelError(string.Empty, "Your Account is not confirmed yet!!");

                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login");
            }
            return View(model);
        }

        #endregion

        #region Sign Out

        [Authorize]
        [HttpGet]
        public async new Task<IActionResult> SignOut()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(SignIn));
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
        #endregion

        #region Forget Password

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordEmail(ForgetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = resetPasswordToken });
                    await _emailSender.SendAsync(
                        from: _configuration["EmailSettings:SenderEmail"],
                         recipients: model.Email,
                         subject: "Reset Your Password",
                         body: resetPasswordUrl
                        );
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                ModelState.AddModelError(string.Empty, "There is no Account with this Email!");
            }
            return View("ForgetPassword", model);   
        }
        #endregion

        #region Check Your Inbox
        public IActionResult CheckYourInbox()
        {
            return View();
        }
        #endregion

        #region Reset Password

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(SignIn));

            TempData["Email"] = email;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(SignIn));
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;
                var user = await _userManager.FindByEmailAsync(email);
                var currentUserId = _userManager.GetUserId(User);
                if (user?.Id != currentUserId)
                {
                    ModelState.AddModelError(string.Empty, "Access Denied, You are not allowed!");
                    return View(model);
                }
                if (user is not null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(SignIn));
                    else
                    {
                        foreach (var item in result.Errors)
                            ModelState.AddModelError(string.Empty, item.Description);
                        return View(model);
                    }
                }
                ModelState.AddModelError(string.Empty, "Url is not valid");
            }
            return View(model);
        }
        #endregion
    }
}
