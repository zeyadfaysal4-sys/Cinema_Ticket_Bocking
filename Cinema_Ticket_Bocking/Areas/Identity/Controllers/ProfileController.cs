using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_Ticket_Bocking.Areas.Identity.Controllers
{
    [Area("Identity")]


    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            //var applicationUserVM = new ApplicationUserVM();

            //applicationUserVM.Name = user.Name;
            //applicationUserVM.Email = user.Email;
            //applicationUserVM.Address = user.Address;
            //applicationUserVM.PhoneNumber = user.PhoneNumber;

            var applicationUserVM = user.Adapt<ApplicationUserVM>();


            return View(applicationUserVM);
        }
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user is null) 
            {
                return NotFound();
            }

            user.Name = applicationUserVM.Name;
            user.Address = applicationUserVM.Address;
            user.PhoneNumber = applicationUserVM.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded)
            {
                string error = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error-Notification"] = error;

                return View(nameof(Index), applicationUserVM);
            }
            else
            {
                TempData["Success-Notification"] = "User updated Successfully ";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, applicationUserVM.CurrentPassword, applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                string error = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error-Notification"] = error;

                return View(nameof(Index), applicationUserVM);
            }
            else
            {
                TempData["Success-Notification"] = "Passwor Change Successfully ";
            }
            return RedirectToAction(nameof(Index));

        }
    }
}
