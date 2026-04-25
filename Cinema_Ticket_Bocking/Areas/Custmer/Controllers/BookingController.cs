using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    public class BookingController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var bookings = _context.Bookin.ToList();
            return View(bookings);
        }
        public IActionResult Create(int movieId)
        {
            ViewBag.Movies = _context.Movies.ToList();

            var model = new Book
            {
                MovieId = movieId
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Create(Book booking)
        {
         
            booking.UserId = 1; 

            booking.BookingDate = DateTime.Now;

            var movie = _context.Movies.FirstOrDefault(x => x.Id == booking.MovieId);
            if (movie != null)
            {
                booking.TotalPrice = booking.NumberOfTickets * movie.Price;
            }

            _context.Bookin.Add(booking);
            _context.SaveChanges();

            TempData["Success"] = "Booking Created Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
    
}
