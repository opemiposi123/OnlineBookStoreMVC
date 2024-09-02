using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using System.Security.Claims;

namespace OnlineBookStoreMVC.Controllers
{
    //[Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAddressService _addressService;
        private readonly INotyfService _notyf;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IAddressService addressService, INotyfService notyfService)
        {
            _shoppingCartService = shoppingCartService;
            _addressService = addressService;
            _notyf = notyfService;
        }

        // GET: ShoppingCart/Index
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            var userCId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userCId;

            var cart = await _shoppingCartService.GetCartAsync(userId);
            var itemCount = await _shoppingCartService.GetCartItemCountAsync(userId);
            ViewBag.CartItemCount = itemCount;

            return View(cart);
        }

        // POST: ShoppingCart/Add
        [HttpPost]
        public async Task<IActionResult> AddToCart(string userId, Guid bookId, int quantity = 1)
        {
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }
            await _shoppingCartService.AddToCartAsync(userId, bookId, quantity);
            _notyf.Success("Book has been successfully added to the cart.");
            return RedirectToAction("Index", "Store", new { userId });
        }

        // POST: ShoppingCart/Reduce
        [HttpPost]
        public async Task<IActionResult> ReduceCartItemQuantity(string userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.ReduceQuantityAsync(userId, bookId);
            _notyf.Success("Cart item quantity has been successfully reduced.");
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Increase
        [HttpPost]
        public async Task<IActionResult> IncreaseCartItemQuantity(string userId, Guid bookId)
        {
            var remainingQuantity = await _shoppingCartService.IncreaseQuantityAsync(userId, bookId);
            _notyf.Success("Cart item quantity has been successfully increased.");
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Remove
        [HttpPost]
        public async Task<IActionResult> RemoveCartItem(string userId, Guid bookId)
        {
            await _shoppingCartService.RemoveFromCartAsync(userId, bookId);
            _notyf.Success("Book has been successfully removed from the cart.");
            return RedirectToAction("Index", new { userId });
        }

        // POST: ShoppingCart/Clear
        [HttpPost]
        public async Task<IActionResult> ClearCart(string userId)
        {
            await _shoppingCartService.ClearCartAsync(userId);
            _notyf.Success("Cart has been successfully cleared.");
            return RedirectToAction("Index", new { userId });
        }

        // GET: ShoppingCart/Total
        [HttpGet]
        public async Task<IActionResult> Total(string userId)
        {
            var total = await _shoppingCartService.GetCartTotalAsync(userId);
            return Json(new { Total = total });
        }

        [HttpGet]
        public IActionResult InputAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> InputAddress(AddressRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _addressService.AddAddressAsync(model, userId);
                _notyf.Success("Address has been successfully added.");
                return RedirectToAction("OrderSummary", "Order");
            }
            return View(model);
        }
    }
}
