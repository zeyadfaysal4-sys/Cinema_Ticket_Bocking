using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.FilterMovieVm;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    public class HomeController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Actors> _actorsRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
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
        public async Task<IActionResult> ProductDetailes(int id)
        {
            var movie = await _movieRepository.GetoneAsync(m => m.Id == id, icludes: [m => m.Category]);
            if (movie is null)
            {
                return NotFound();
            }

            var relatedMovies = await _movieRepository.GetAsync(m => m.CategoryId == movie.CategoryId && m.Id != movie.Id, icludes: [m => m.Category]);
            relatedMovies = relatedMovies.Skip(0).Take(4);
            if (relatedMovies is null)
            {
                return NotFound();
            }
            return View(new MovieWithRelatedVM()
            {
                Movie = movie,
                RelatedMovies = relatedMovies
            });
        }
    }
}
