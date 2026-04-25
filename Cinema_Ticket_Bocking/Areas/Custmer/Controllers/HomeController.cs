using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.FilterMovieVm;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    public class HomeController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<Movie> _movieRepository;
        IRepository<Category> _categoryRepository;
        IRepository<Actors> _actorsRepository;
        IRepository<Cinema> _cinemaRepository;
        //IRepository<Cinema> _cinemaRepository;
        public HomeController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Actors> actorsRepository, IRepository<Cinema> cinemaRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _actorsRepository = actorsRepository;
            _cinemaRepository = cinemaRepository;
        }

        public IActionResult Index()
        {
           return View();
        }
        public async Task<IActionResult> Custmers(FilterMovieVM filter)
        {
            filter.Page = filter.Page <= 0 ? 1 : filter.Page;

            var movies = await _movieRepository.GetAsync(icludes: [m => m.Category, m => m.Actors, m => m.Cinema]);
            ////.Include(m => m.Category)
            ////.Include(m => m.Actors)
            ////.Include(m => m.Cinema)
            //.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Name))
                //movies = movies.(m => m.Name.Contains(filter.Name));
                movies = await _movieRepository.GetAsync(filter: m => m.Name.Contains(filter.Name));

            if (filter.Price > 0)
                //movies = movies.Where(m => m.Price == filter.Price);
                movies = await _movieRepository.GetAsync(filter: m => m.Price == filter.Price);

            if (filter.CategoryId != 0)
                //movies = movies.Where(m => m.CategoryId == filter.CategoryId);
                movies = await _movieRepository.GetAsync(filter: m => m.CategoryId == filter.CategoryId);

            if (filter.CinemaId != 0)
                //movies = movies.Where(m => m.CinemaId == filter.CinemaId);
                movies = await _movieRepository.GetAsync(filter: m => m.CinemaId == filter.CinemaId);

            if (filter.ActorId != 0)
                //movies = movies.Where(m => m.Actors.Any(a => a.Id == filter.ActorId));
                movies = await _movieRepository.GetAsync(filter: a => a.Id == filter.ActorId);

            //ViewBag.Categories = _context.Categories.ToList();
            //ViewBag.Cinemas = _context.Cinemas.ToList();
            //ViewBag.Actors = _context.Actors.ToList();

            ViewBag.Categories = await _categoryRepository.GetAsync();
            ViewBag.Cinemas = await _cinemaRepository.GetAsync();
            ViewBag.Actors = await _actorsRepository.GetAsync();

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
