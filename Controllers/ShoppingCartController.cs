using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using System.Security.Claims;

namespace OnlineBookStoreMVC.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // GET: ShoppingCart/Index
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            var cart = await _shoppingCartService.GetCartAsync(userId);
            var userCId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userCId;
            return View(cart);
        }

        // POST: ShoppingCart/Add
        [HttpPost]
        public async Task<IActionResult> AddToCart(string userId, Guid bookId, int quantity = 1)
        {
            await _shoppingCartService.AddToCartAsync(userId, bookId, quantity);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Reduce
        [HttpPost]
        public async Task<IActionResult> ReduceCartItemQuantity(string userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.ReduceQuantityAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        } 

        // POST: ShoppingCart/Increase
        [HttpPost] 
        public async Task<IActionResult> IncreaseCartItemQuantity(string userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.IncreaseQuantityAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Remove
        [HttpPost] 
        public async Task<IActionResult> RemoveCartItem(string userId, Guid bookId)
        {
            await _shoppingCartService.RemoveFromCartAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Clear
        [HttpPost]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _shoppingCartService.ClearCartAsync(userId);
            return RedirectToAction("Index", new { userId });
        }

        // GET: ShoppingCart/Total
        [HttpGet] 
        public async Task<IActionResult> Total(string userId)
        {
            var total = await _shoppingCartService.GetCartTotalAsync(userId);
            return Json(new { Total = total });
        }
    }
}
