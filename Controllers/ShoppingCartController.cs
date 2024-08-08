using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;

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
        public async Task<IActionResult> Index(Guid userId)
        {
            var cart = await _shoppingCartService.GetCartAsync(userId);
            return View(cart);
        }

        // POST: ShoppingCart/Add
        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid userId, Guid bookId, int quantity = 1)
        {
            await _shoppingCartService.AddToCartAsync(userId, bookId, quantity);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Reduce
        [HttpPost]
        public async Task<IActionResult> ReduceCartItemQuantity(Guid userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.ReduceQuantityAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        } 

        // POST: ShoppingCart/Increase
        [HttpPost] 
        public async Task<IActionResult> IncreaseCartItemQuantity(Guid userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.IncreaseQuantityAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Remove
        [HttpPost] 
        public async Task<IActionResult> RemoveAllCartItems(Guid userId, Guid bookId)
        {
            await _shoppingCartService.RemoveFromCartAsync(userId, bookId);
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Clear
        [HttpPost]
        public async Task<IActionResult> ClearCart(Guid userId)
        {
            await _shoppingCartService.ClearCartAsync(userId);
            return RedirectToAction("Index", new { userId });
        }

        // GET: ShoppingCart/Total
        [HttpGet] 
        public async Task<IActionResult> Total(Guid userId)
        {
            var total = await _shoppingCartService.GetCartTotalAsync(userId);
            return Json(new { Total = total });
        }
    }
}
