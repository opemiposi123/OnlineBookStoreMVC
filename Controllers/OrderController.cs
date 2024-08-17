using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: Order/Checkout
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderRequestModel orderRequest)
        {
            if (ModelState.IsValid)
            {
                var order = await _orderService.CheckoutAsync(orderRequest);
                return RedirectToAction(nameof(CheckoutComplete), new { userId = order.UserId });
            }
            return View(orderRequest);
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
        [ValidateAntiForgeryToken]
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


        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var orders = await _orderService.GetOrderDetials(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

    }
}
