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

        public ShoppingCartController(IShoppingCartService shoppingCartService, IAddressService addressService)
        {
            _shoppingCartService = shoppingCartService;
            _addressService = addressService;
        }

        // GET: ShoppingCart/Index
        [HttpGet]
        public async Task<IActionResult> Index(string userId)
        {
            var userCId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = userCId;

            // Get the cart and cart item count
            var cart = await _shoppingCartService.GetCartAsync(userId);
            var itemCount = await _shoppingCartService.GetCartItemCountAsync(userId);

            // Set the cart item count in ViewBag
            ViewBag.CartItemCount = itemCount;

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
                // Assuming that _addressService handles the saving of the address
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
                await _addressService.AddAddressAsync(model, userId);

                // Redirect to the Order Summary page after saving the address
                return RedirectToAction("OrderSummary", "Order");
            }

            return View(model); // If the model is invalid, return to the input address view
        }
    }
}
