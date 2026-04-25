using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var users = _context.Users.AsQueryable();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            var users = _context.Users.AsQueryable();
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            _context.Users.Add(user);
            _context.SaveChanges();
             TempData["Success-Notification"] = "User Created Successfully ";
            return RedirectToAction(nameof(Index));
        }
    }
}
