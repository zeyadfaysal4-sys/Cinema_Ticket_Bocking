using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IndexDashboard : Controller
    {
        ApplicationDbContext _context =new ApplicationDbContext();
        public IActionResult Index()
        {
            var model = new DashboardVM
            {
                MoviesCount = _context.Movies.Count(),
                UsersCount = _context.Users.Count(),
                BookingsCount = _context.Bookin.Count(),
                Revenue = _context.Bookin.Sum(x => x.TotalPrice)
            };

            return View(model);
            
        }
    }
}
