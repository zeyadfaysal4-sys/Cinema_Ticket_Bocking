using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.FilterMovieVm;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<Movie> _contextrepository;// = new Repositories<Movie>();
        IRepository<Category> _contextrepositoryCategory;// = new Repositories<Category>();
        IRepository<Cinema> _contextrepositoryCinema;// = new Repositories<Cinema>();
        IRepository<Actors> _contextrepositoryActors;// = new Repositories<Actors>();

        public MovieController(IRepository<Movie> movietrepository, IRepository<Category> contextrepositoryCategory, IRepository<Cinema> contextrepositoryCinema, IRepository<Actors> contextrepositoryActors)
        {
            _contextrepository = movietrepository;
            _contextrepositoryCategory = contextrepositoryCategory;
            _contextrepositoryCinema = contextrepositoryCinema;
            _contextrepositoryActors = contextrepositoryActors;
        }

        public async Task<IActionResult> Index(FilterMovieVM filter)
        {
            //var movies = _context.Movies.AsQueryable()
            //movies = movies.Include(m => m.Category).Include(m => m.Actors).Include(m => m.Cinema);

            var movies = await _contextrepository.GetAsync(icludes: [m => m.Category, m => m.Actors, m => m.Cinema]);
            if (filter.Name != null)
            {
                movies = movies.Where(m => m.Name.Contains(filter.Name));
                ViewBag.Name = filter.Name;
            }
            if (filter.Price > 0)
            {
                movies = movies.Where(m => m.Price == filter.Price);
                ViewBag.Price = filter.Price;
            }

            if (filter.CategoryId != 0)
            {
                movies = movies.Where(m => m.CategoryId == filter.CategoryId);
                ViewBag.CategoryId = filter.CategoryId;
            }
            if (filter.CinemaId != 0)
            {
                movies = movies.Where(m => m.CinemaId == filter.CinemaId);
                ViewBag.CinemaId = filter.CinemaId;
            }
            if (filter.ActorId != 0)
            {
                movies = movies.Where(m => m.Actors.Any(a => a.Id == filter.ActorId));
                ViewBag.ActorId = filter.ActorId;
            }
            //ViewBag.Categories = _context.Categories.ToList();
            //ViewBag.Cinemas = _context.Cinemas.ToList();
            //ViewBag.Actors = _context.Actors.ToList();

            ViewBag.Categories = await _contextrepository.GetAsync();
            ViewBag.Cinemas = await _contextrepository.GetAsync();
            ViewBag.Actors = await _contextrepository.GetAsync();
            ViewBag.TotalPages = (int)Math.Ceiling(movies.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            movies = movies.Skip((filter.Page - 1) * 8).Take(8);

            return View(movies.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var Categories = _context.Categories.ToList();
            //var Cinemas = _context.Cinemas.ToList();
            //var Actors = _context.Actors.ToList();

            var Categories = await _contextrepositoryCategory.GetAsync();
            var Cinemas = await _contextrepositoryCinema.GetAsync();
            var Actors = await _contextrepositoryActors.GetAsync();
            return View(new MovieVM
            {   Categories = Categories,
                Cinemas = Cinemas,
                Actors = Actors
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile imgfile, List<IFormFile> SupImagesfile)
        {

            if (imgfile != null && imgfile.Length > 0)
            {
                var fileName = Guid.NewGuid() + "-" + imgfile.FileName;

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\MovieSupImages\\Img", fileName);

                using (var stream = System.IO.File.Create(path))
                {
                    imgfile.CopyTo(stream);
                }

                movie.MainImg = fileName;
            }


            if (SupImagesfile != null && SupImagesfile.Count > 0)
            {
                if (movie.SupImages == null)
                {
                    movie.SupImages = new List<string>();
                }

                foreach (var image in SupImagesfile)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + "-" + image.FileName;

                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot\\Images\\MovieSupImages",
                            fileName
                        );

                        using (var stream = System.IO.File.Create(path))
                        {
                            image.CopyTo(stream);
                        }

                        movie.SupImages.Add(fileName);
                    }
                }
            }

            //_context.Movies.Add(movie);
            //_context.SaveChanges();
             
            await _contextrepository.AddAsync(movie);
            await _contextrepository.CommittAsync();

            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var movie = _context.Movies.FirstOrDefault(a => a.Id == id);

            var movie = await _contextrepository.GetoneAsync(a => a.Id == id);
            if (movie == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            //var Categories = _context.Categories.ToList();
            //var Cinemas = _context.Cinemas.ToList();
            //var Actors = _context.Actors.ToList();

            var Categories = await _contextrepositoryCategory.GetAsync();
            var Cinemas = await _contextrepositoryCinema.GetAsync();
            var Actors = await _contextrepositoryActors.GetAsync();

            return View(new MovieVM
            {
                Movie = movie,
                Categories = Categories,
                Cinemas = Cinemas,
                Actors = Actors,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Movie movie, IFormFile imgfile, List<IFormFile> SupImagesfile)
        {
            //var movieup = _context.Movies.AsNoTracking().FirstOrDefault(b => b.Id == movie.Id);

            var movieup = await _contextrepository.GetoneAsync(a => a.Id == movie.Id, tracked:false);

            if (movieup == null)
                return NotFound();

            if (imgfile != null && imgfile.Length > 0)
            {
                var fileName = Guid.NewGuid() + "-" + imgfile.FileName;

                var path = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot\\Images\\MovieSupImages\\Img", fileName);

                using (var stream = System.IO.File.Create(path))
                {
                    imgfile.CopyTo(stream);
                }

                movie.MainImg = fileName;

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot\\Images\\MovieSupImages\\Img", movieup.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            else
            {
                movie.MainImg = movieup.MainImg;
            }

            if (SupImagesfile != null && SupImagesfile.Count > 0)
            {
                if (movieup.SupImages != null)
                {
                    foreach (var oldImg in movieup.SupImages)
                    {
                        var oldPath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/Images/MovieSupImages",
                            oldImg
                        );

                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }
                }

                movie.SupImages = new List<string>();

                foreach (var image in SupImagesfile)
                {
                    if (image != null && image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + "-" + image.FileName;

                        var path = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/Images/MovieSupImages",
                            fileName
                        );

                        using (var stream = System.IO.File.Create(path))
                        {
                            image.CopyTo(stream);
                        }

                        movie.SupImages.Add(fileName);
                    }
                   
                }
            }
            else
            {

                movie.SupImages = movieup.SupImages;
            }

            //_context.Movies.Update(movie);
            //_context.SaveChanges();

            _contextrepository.Update(movie);
            await _contextrepository.CommittAsync();


            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var movie= _context.Movies.FirstOrDefault(a => a.Id == id);
            var movie = await _contextrepository.GetoneAsync(a => a.Id == id);
            if (movie == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\MovieSupImages\\Img", movie.MainImg);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            if(movie.SupImages.Count > 0)
            {
                foreach (var oldImg in movie.SupImages)
                {
                    var oldPathSup = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\MovieSupImages", oldImg);
                    if (System.IO.File.Exists(oldPathSup))
                    {
                        System.IO.File.Delete(oldPathSup);
                    }
                }
            }
            //_context.Movies.Remove(movie);
            //_context.SaveChanges();

            _contextrepository.Delete(movie);
            await _contextrepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
