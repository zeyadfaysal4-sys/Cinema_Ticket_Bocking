using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_Ticket_Bocking.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOtp> applicationUserOtpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOtpRepository = applicationUserOtpRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                Address = registerVM.Address,
                UserName = registerVM.UserName,
            };
            var result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            TempData["Success-Notification"] = "User Registered successfully.";
            await _emailSender.SendEmailAsync(registerVM.Email,
                "Cinema Confirm Email",
                 $"<h1>Click <a href={link}> here </a> To Confirm Your Email</h1>");
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                TempData["Error-Notification"] = "Invalid User.";
                return RedirectToAction(nameof(Login));
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error-Notification"] = "Cant Confirm Email";
                return RedirectToAction(nameof(Login));
            }
            TempData["Success-Notification"] = "confirmed Email successfully.";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult ResendEmailConfirmation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail)
            ?? await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View(resendEmailConfirmationVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email,
                "Cinema Confirm Email",
                 $"<h1>Click <a href={link}> here </a> To Confirm Your Email</h1>");
            TempData["Success-Notification"] = "Resend Email Confirmation successfully.";

            return RedirectToAction(nameof(Login));

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserNameOrEmail)
                ?? await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View(loginVM);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "to many attempts Please try again Later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "Please Confirm Your Email First");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid UserName or Password");
                }
                return View(loginVM);
            }
            TempData["Success-Notification"] = "Login successful.";

            return RedirectToAction("Custmers", "Home", new { area = "Custmer" });
        }
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail)
               ?? await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View(forgetPasswordVM);
            }

            var applicationUserOtps = await _applicationUserOtpRepository.GetAsync(a => a.ApplicationUserId == user.Id);
            var count = applicationUserOtps.Count(a => (DateTime.UtcNow - a.CreatedAt).TotalHours <= 24);
            if (count >= 5)
            {
                ModelState.AddModelError("", "to many Attempts Please try again Later");
                return View(forgetPasswordVM);
            }


            var otp = new Random().Next(1000, 9999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(user.Id, otp);
            await _applicationUserOtpRepository.AddAsync(applicationUserOtp);
            await _applicationUserOtpRepository.CommittAsync();
            await _emailSender.SendEmailAsync(user.Email,
               "Cinema Forget Password",
                $"<h1> Use This OTP <span style =\"color:red\">{otp}</span>To Reset Your Password</h1>");
            return RedirectToAction(nameof(ValidateOTP), new { userId = user.Id });
        }
        [HttpGet]
        public IActionResult ValidateOTP(string userId)
        {
            return View(new ValidateOTPVM { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
            {
                return View(validateOTPVM);
            }

            var user = await _userManager.FindByIdAsync(validateOTPVM.UserId);

            if (user is null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(validateOTPVM);
            }
            var otps = await _applicationUserOtpRepository.GetAsync(a =>
             a.ApplicationUserId == user.Id &&
             a.IsValied == true &&
             a.ValiedTo >= DateTime.UtcNow
             );

            var otp = otps.OrderByDescending(otp => otp.CreatedAt).FirstOrDefault();

            if (otp is null || otp.OTP != validateOTPVM.OTP)
            {
                ModelState.AddModelError("", "Invalid OTP / Expierd OTP");
                return View(validateOTPVM);
            }

            otp.IsValied = false;
            await _applicationUserOtpRepository.CommittAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["token"] = token;
            return RedirectToAction(nameof(NewPassword), new { userId = user.Id });
        }
        [HttpGet]
        public IActionResult NewPassword(string userId)
        {
            var token = TempData["token"] as string;
            if (token is null)
            {
                return RedirectToAction(nameof(Login));
            }
            return View(new NewPasswordVM { UserId = userId , Token = token});
        }
        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (newPasswordVM.Token is null)
            {
                return RedirectToAction(nameof(Login));
            }
            var user = await _userManager.FindByIdAsync(newPasswordVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid User");
                return View(newPasswordVM);
            }

            var result = await _userManager.ResetPasswordAsync(user, newPasswordVM.Token, newPasswordVM.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(newPasswordVM);
            }
            return RedirectToAction(nameof(Login));
        }
    }
}