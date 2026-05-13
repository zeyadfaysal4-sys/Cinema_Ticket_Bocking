using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Movie> _MovieRepository;

        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Movie> movieRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _MovieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var carts = await _cartRepository.GetAsync(m => m.ApplicationUserId == user.Id, icludes: [c => c.Movie]);
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int count, int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var movie = await _MovieRepository.GetoneAsync(m => m.Id == movieId);
            if (movie is null)
            {
                return NotFound();
            }
            var cartindb = await _cartRepository.GetoneAsync(m => m.MovieId == movieId && m.ApplicationUserId == user.Id);
            if (cartindb is not null)
            {
                cartindb.Count += count;
                await _cartRepository.CommittAsync();
                return RedirectToAction(nameof(Index));
            }
            var cart = new Cart();
            cart.ApplicationUserId = user.Id;
            cart.MovieId = movieId;
            cart.Count = count;
            cart.Price = movie.Price;
            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementCount(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var cartindb = await _cartRepository.GetoneAsync(m => m.MovieId == movieId && m.ApplicationUserId == user.Id);
            if (cartindb is null)
            {
                return NotFound();
            }
            cartindb.Count++;
            var movie = await _MovieRepository.GetoneAsync(m => m.Id == movieId);

            await _cartRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementCount(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var cartindb = await _cartRepository.GetoneAsync(m => m.MovieId == movieId && m.ApplicationUserId == user.Id);
            if (cartindb is null)
            {
                return NotFound();
            }
            var movie = await _MovieRepository.GetoneAsync(m => m.Id == movieId);
            if (cartindb.Count > 1)
            {
                cartindb.Count--;
            }

            await _cartRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var cartindb = await _cartRepository.GetoneAsync(m => m.MovieId == movieId && m.ApplicationUserId == user.Id);
            if (cartindb is null)
            {
                return NotFound();
            }     

            _cartRepository.Delete(cartindb);
            await _cartRepository.CommittAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
