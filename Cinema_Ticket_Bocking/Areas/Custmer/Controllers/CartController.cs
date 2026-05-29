using Cinema_Ticket_Bocking.Models;
using Cinema_Ticket_Bocking.Repository;
using Cinema_Ticket_Bocking.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Cinema_Ticket_Bocking.Areas.Custmer.Controllers
{
    [Area("Custmer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Movie> _MovieRepository;
        private readonly IRepository<Promotion> _PromotionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepository<Cart> cartRepository, UserManager<ApplicationUser> userManager, IRepository<Movie> movieRepository, IRepository<Promotion> promotionRepository)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _MovieRepository = movieRepository;
            _PromotionRepository = promotionRepository;
        }

        public async Task<IActionResult> Index(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            if (code != null)
            {
                var promotion = await _PromotionRepository.GetoneAsync(p => p.Code == code &&
                    p.IsValid == true &&
                    DateTime.Now < p.ValidTo &&
                    p.MaxUsage > 0
                    );
                if (promotion != null)
                {
                    var cart = await _cartRepository.GetoneAsync(c => c.ApplicationUserId == user.Id
                    && c.MovieId == promotion.MovieId
                    );
                    if (cart != null)
                    {
                        cart.Price = cart.Price - (cart.Price * promotion.Discount / 100m);
                        await _cartRepository.CommittAsync();
                        promotion.MaxUsage--;
                        await _PromotionRepository.CommittAsync();
                        TempData["Success-Notification"] = "Promotion applied successfully";
                    }
                    else
                    {
                        TempData["Error-Notification"] = "There is no Product item for this promotion";
                    }

                }
                else
                {
                    TempData["Error-Notification"] = "Invalied/Expired promotion";
                }
            }
            
            var carts = await _cartRepository.GetAsync(m => m.ApplicationUserId == user.Id, icludes: [c => c.Movie]);
            var TotalPrice = carts.Sum(c => c.Price * c.Count);

            ViewBag.TotalPrice = TotalPrice;
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
        public async Task<IActionResult> Pay(int movieId)
        {

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            var user = await _userManager.GetUserAsync(User);
            if (user is null) { return NotFound(); }
            var carts = await _cartRepository.GetAsync(c => c.ApplicationUserId == user.Id, icludes: [p => p.Movie]);
            if (carts is null) { return NotFound(); }

            foreach (var cart in carts)
            {
                var sessionLineItemOptions = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = cart.Movie.Name,
                            Description = cart.Movie.Description,
                        },
                        UnitAmount = (long)cart.Price * 100,
                    },
                    Quantity = cart.Count,
                };
                options.LineItems.Add(sessionLineItemOptions);
            }
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }
    }
}
