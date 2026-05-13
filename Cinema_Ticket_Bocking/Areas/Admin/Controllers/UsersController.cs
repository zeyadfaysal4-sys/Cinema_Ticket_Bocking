using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.Utiltes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE},{CD.ADMIN_ROLE}")]

    public class UsersController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<User> _userRepository;//= new Repositories<User>();
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IRepository<User> userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var users = _context.Users.AsQueryable();
            var user = _userManager.Users;
            return View(user);
        }

        public async Task<IActionResult> LockUnLock(string id)
        {
            //var users = _context.Users.AsQueryable();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            bool isSuperAdmin = await _userManager.IsInRoleAsync(user, CD.SUPER_ADMIN_ROLE);

            if (isSuperAdmin)
            {
                TempData["Error-Notification"] = "You Can't Lock/Unlock Super Admin !!!";
                return RedirectToAction(nameof(Index));
            }

            if (user.LockoutEnd is null || DateTime.UtcNow > user.LockoutEnd)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(2));
                TempData["Success-Notification"] = "User Locked Successfully ";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success-Notification"] = "User Unlocked Successfully ";

            }

            return RedirectToAction(nameof(Index));
        }
    }
}
