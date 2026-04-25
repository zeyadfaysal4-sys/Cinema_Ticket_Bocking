using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var Actors = _context.Actors.AsQueryable();
            return View(Actors.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            var Actors = _context.Actors.AsQueryable();
            return View(new Actors());
        }
        [HttpPost]
        public IActionResult Create(Actors actor,IFormFile ImgFile)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if(ImgFile != null && ImgFile.Length>0)
            {
                var fileName =Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ActorsImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                actor.Img = fileName;
            }
            _context.Actors.Add(actor); 
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null)
            {
                return RedirectToAction("Not Found", "Home");
            } 
            return View(actor);
        }
        [HttpPost]
        public IActionResult Update(Actors actor, IFormFile ImgFile)
        {
            var actors = _context.Actors.AsNoTracking().FirstOrDefault(a => a.Id == actor.Id);
            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (ImgFile != null && ImgFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ActorsImages", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                actor.Img = fileName;
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ActorsImages", actors.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Actors.Update(actor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ActorsImages", actor.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Actors.Remove(actor);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
