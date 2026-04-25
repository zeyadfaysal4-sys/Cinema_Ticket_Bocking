using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var Category = _context.Categories.AsQueryable();
            return View(Category.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var Category = _context.Categories.AsQueryable();
            return View( new Category());
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            var Category = _context.Categories.AsQueryable();
            if(!ModelState.IsValid)
            {
                return View(category);
            }
            _context.Categories.Add(category);
            _context.SaveChanges();
            TempData["Success-Notification"] = "category Created Successfully ";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var Category = _context.Categories.FirstOrDefault(c=>c.Id==id);
            if(Category==null)
            {
                 return RedirectToAction("Not Found" , "Home");
            }

            return View(Category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            var Category = _context.Categories.Update(category);
            _context.SaveChanges();
            TempData["Success-Notification"] = "category Updated Successfully ";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var Category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (Category == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            _context.Categories.Remove(Category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
