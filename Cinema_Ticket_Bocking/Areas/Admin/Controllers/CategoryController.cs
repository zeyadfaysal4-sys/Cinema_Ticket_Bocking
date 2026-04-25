using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket_Bocking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();

        IRepository<Category> _contextRepository;//= new Repository<Category>();

        public CategoryController(IRepository<Category> categoryrepository)
        {
            _contextRepository = categoryrepository;
        }

        public async Task<IActionResult> Index()
        {
            //var Category = _context.Categories.AsQueryable();
            var Category = await _contextRepository.GetAsync();

            return View(Category.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var Category = _context.Categories.AsQueryable();
            var Category = await _contextRepository.GetAsync();

            return View( new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            //var Category = _context.Categories.AsQueryable();

            var Category = await _contextRepository.GetAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            //_context.Categories.Add(category);
            //_context.SaveChanges();

            await _contextRepository.AddAsync(category);
            await _contextRepository.CommittAsync();
            TempData["Success-Notification"] = "category Created Successfully ";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var Category = _context.Categories.FirstOrDefault(c=>c.Id==id);
            var Category = await _contextRepository.GetoneAsync(c => c.Id == id);
            if (Category==null)
            {
                 return RedirectToAction("Not Found" , "Home");
            }

            return View(Category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            ////var Category = _context.Categories.Update(category);
            //_context.SaveChanges();

             _contextRepository.Update(category);
             await _contextRepository.CommittAsync();

            TempData["Success-Notification"] = "category Updated Successfully ";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //var Category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var Category = await _contextRepository.GetoneAsync(c => c.Id == id);


            if (Category == null)
            {
                return RedirectToAction("Not Found", "Home");
            }
            //_context.Categories.Remove(Category);
            //_context.SaveChanges();

            _contextRepository.Delete(Category);
            await _contextRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
