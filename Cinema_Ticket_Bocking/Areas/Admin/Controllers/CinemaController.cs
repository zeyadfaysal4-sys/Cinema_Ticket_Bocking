using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var cinema = _context.Cinemas.AsQueryable();
            return View(cinema.AsQueryable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var cinema = _context.Cinemas.AsQueryable();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Cinema cinemas ,IFormFile imgfile)
        {
            var cinema = _context.Cinemas.AsQueryable();
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
            _context.Cinemas.Add(cinemas);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(a => a.Id == id);
            if (cinema == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Update(Cinema cinema, IFormFile imgfile)
        {
            var cinemas = _context.Cinemas.AsNoTracking().FirstOrDefault(a => a.Id == cinema.Id);
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
            _context.Cinemas.Update(cinema);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(a => a.Id == id);
            if (cinema == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\CinemaImages", cinema.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
