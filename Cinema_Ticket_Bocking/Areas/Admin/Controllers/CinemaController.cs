using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<Cinema> _contextRepository;// = new Repositories<Cinema>();

        public CinemaController(IRepository<Cinema> cinemarepository)
        {
            _contextRepository = cinemarepository;
        }

        public async Task<IActionResult> Index()
        {
            //var cinema = _context.Cinemas.AsQueryable();
            var cinema = await _contextRepository.GetAsync();
            return View(cinema.AsQueryable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var cinema = _context.Cinemas.AsQueryable();

            var cimema = await _contextRepository.GetAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinemas ,IFormFile imgfile)
        {
            //var cinema = _context.Cinemas.AsQueryable();

            var cinema = await _contextRepository.GetAsync();

            if (!ModelState.IsValid)
            {
                return View(cinemas);
            }
            if(imgfile!=null&& imgfile.Length>0)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + imgfile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CinemaImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    imgfile.CopyTo(stream);
                }
                cinemas.Img = fileName;
            }
            //_context.Cinemas.Add(cinemas);
            //_context.SaveChanges();

            await _contextRepository.AddAsync(cinemas);
            await _contextRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var cinema = _context.Cinemas.FirstOrDefault(a => a.Id == id);

            var cinema = await _contextRepository.GetoneAsync(a => a.Id == id);

            if (cinema == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Cinema cinema, IFormFile imgfile)
        {
            //var cinemas = _context.Cinemas.AsNoTracking().FirstOrDefault(a => a.Id == cinema.Id);

            var cinemas = await _contextRepository.GetoneAsync(a => a.Id == cinema.Id, tracked: false);

            if (!ModelState.IsValid)
            {
                return View(cinema);
            }
            if (imgfile != null && imgfile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + imgfile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CinemaImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    imgfile.CopyTo(stream);
                }
                cinema.Img = fileName;
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CinemaImages", cinemas.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            //_context.Cinemas.Update(cinema);
            //_context.SaveChanges();

            _contextRepository.Update(cinema);
            await _contextRepository.CommittAsync();


            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var cinema = _context.Cinemas.FirstOrDefault(a => a.Id == id);

            var cinema = await _contextRepository.GetoneAsync(a => a.Id == id);
            if (cinema == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CinemaImages", cinema.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.Cinemas.Remove(cinema);
            //_context.SaveChanges();

             _contextRepository.Delete(cinema);
            await _contextRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
