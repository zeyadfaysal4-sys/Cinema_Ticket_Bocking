using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorsController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        IRepository<Actors> _contextRepository;//= new Repositories<Actors>();

        public ActorsController(IRepository<Actors> atorsrepository)
        {
            _contextRepository = atorsrepository;
        }

        public async Task<IActionResult> Index()
        {
            //var Actors = _context.Actors.AsQueryable();
            var Actors = await _contextRepository.GetAsync();
            return View(Actors.AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var Actors = _context.Actors.AsQueryable();
            var Actors = await _contextRepository.GetAsync();
            return View(new Actors());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actors actor,IFormFile ImgFile)
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
            //_context.Actors.Add(actor); 
            //_context.SaveChanges();

            await _contextRepository.AddAsync(actor);
            await _contextRepository.CommittAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var actor = _context.Actors.FirstOrDefault(a => a.Id == id);

            var actor = await _contextRepository.GetoneAsync(a => a.Id == id);
            if (actor == null)
            {
                return RedirectToAction("Not Found", "Home");
            } 
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Actors actor, IFormFile ImgFile)
        {
            //var actors = _context.Actors.AsNoTracking().FirstOrDefault(a => a.Id == actor.Id);

            var actors = await _contextRepository.GetoneAsync(a => a.Id == actor.Id ,tracked : false);
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
            //_context.Actors.Update(actor);
            //_context.SaveChanges();

            _contextRepository.Update(actor);
            await _contextRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var actor = _context.Actors.FirstOrDefault(a => a.Id == id);

            var actor = await _contextRepository.GetoneAsync(a => a.Id == id);
            if (actor == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Images\\ActorsImages", actor.Img);
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.Actors.Remove(actor);
            //_context.SaveChanges();

            _contextRepository.Delete(actor);
            await _contextRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
