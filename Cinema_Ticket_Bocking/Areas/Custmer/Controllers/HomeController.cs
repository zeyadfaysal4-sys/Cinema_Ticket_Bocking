using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.FilterMovieVm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    public class HomeController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
           return View();
        }
        public IActionResult Custmers(FilterMovieVM filter)
        {
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;

            var movies = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Actors)
                .Include(m => m.Cinema)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                movies = movies.Where(m => m.Name.Contains(filter.Name));

            if (filter.Price > 0)
                movies = movies.Where(m => m.Price == filter.Price);

            if (filter.CategoryId != 0)
                movies = movies.Where(m => m.CategoryId == filter.CategoryId);

            if (filter.CinemaId != 0)
                movies = movies.Where(m => m.CinemaId == filter.CinemaId);

            if (filter.ActorId != 0)
                movies = movies.Where(m => m.Actors.Any(a => a.Id == filter.ActorId));

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.Actors = _context.Actors.ToList();

            int pageSize = 8;
            int totalItems = movies.Count();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = filter.Page;

            var result = movies
                .Skip((filter.Page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(result);

        }
        
    }
}
