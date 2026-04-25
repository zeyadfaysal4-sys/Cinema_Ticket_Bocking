using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<User> _userRepository;//= new Repositories<User>();

        public UsersController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var users = _context.Users.AsQueryable();

            var users = await _userRepository.GetAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            //var users = _context.Users.AsQueryable();

            var users = await _userRepository.GetAsync();
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            //_context.Users.Add(user);
            //_context.SaveChanges();

            await _userRepository.AddAsync(user);
            await _userRepository.CommittAsync();

             TempData["Success-Notification"] = "User Created Successfully ";
            return RedirectToAction(nameof(Index));
        }
    }
}
