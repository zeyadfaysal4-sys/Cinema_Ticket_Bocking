using Cinema_Ticket_Bocking.Data;
using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    public class BookingController : Controller
    {
        //ApplicationDbContext _context;//= new ApplicationDbContext();

        IRepository<Actors> _actorsRepository;
        IRepository<Book> _bookRepository;
        IRepository<Movie> _movieRepository;
        public BookingController(IRepository<Actors> actorsRepository, IRepository<Book> bookRepository, IRepository<Movie> movieRepository)
        {
            _actorsRepository = actorsRepository;
            _bookRepository = bookRepository;
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var bookings = _context.Bookin.ToList();
            var bookings = await _bookRepository.GetAsync();

            return View(bookings);
        }
        public async Task<IActionResult> Create(int movieId)
        {
            //ViewBag.Movies = _context.Movies.ToList();
            ViewBag.Movies = await _movieRepository.GetAsync();
            var model = new Book
            {
                MovieId = movieId
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Book booking)
        {
         
            booking.UserId = 1; 

            booking.BookingDate = DateTime.Now;

            //var movie = _context.Movies.FirstOrDefault(x => x.Id == booking.MovieId);


            var movie = await _movieRepository.GetoneAsync(x => x.Id == booking.MovieId);
            if (movie != null)
            {
                booking.TotalPrice = booking.NumberOfTickets * movie.Price;
            }

            //_context.Bookin.Add(booking);
            //_context.SaveChanges();

            await _bookRepository.AddAsync(booking);
            await _bookRepository.CommittAsync();
            TempData["Success"] = "Booking Created Successfully";

            return RedirectToAction(nameof(Index));
        }
    }
    
}
