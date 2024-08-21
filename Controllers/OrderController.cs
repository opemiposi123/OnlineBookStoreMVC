using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using System.Security.Claims;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, IShoppingCartService shoppingCartService)
        {
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get the order summary for the current user
            var orderSummary = await _orderService.GetOrderSummaryAsync(userId);

            return View("OrderSummary", orderSummary);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummaries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get all order summaries for the current user
            var orderSummaries = await _orderService.GetAllOrderSummariesAsync(userId);

            return View(orderSummaries);
        }

        // GET: Order/CheckoutComplete/{userId}
        public async Task<IActionResult> CheckoutComplete(string userId)
        {
            var order = await _orderService.CheckoutCompleteAsync(userId);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Order/CreateOrder
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequestModel orderRequest)
        {
            if (ModelState.IsValid)
            {
                var order = await _orderService.CreateOrderAsync(orderRequest);
                return RedirectToAction(nameof(CheckoutComplete), new { userId = order.UserId });
            }
            return View(orderRequest);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ListOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UserOrders(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                return NotFound();
            }
            return View(orders);
        }

        //[Authorize(Roles = "Admin,SuperAdmin")]
        //public async Task<IActionResult> OrderDetails(Guid id)
        //{
        //    var orders = await _orderService.GetOrderDetials(id);
        //    if (orders == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(orders);
        //}

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderSummaryDto orderSummary)
        {
            if (!ModelState.IsValid)
            {
                // Return to the summary view if the model is invalid
                return View("OrderSummary", orderSummary);
            }

            var order = await _orderService.PlaceOrderAsync(orderSummary);

            if (order == null)
            {
                // Handle the error (e.g., show an error message)
                ModelState.AddModelError("", "There was a problem placing your order. Please try again.");
                return View("OrderSummary", orderSummary);
            }

            // Redirect to the OrderConfirmation view after placing the order
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        //public async Task<IActionResult> OrderConfirmation(Guid orderId)
        //{
        //    var order = await _orderService.GetOrderDetials(orderId);

        //    if (order == null)
        //    {
        //        // Handle the case where the order is not found
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View(order);
        //}

        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            var order = await _orderService.GetOrderDetailsAsync(orderId);

            if (order == null)
            {
                // Optionally, show a custom error view or message
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }



    }
}
