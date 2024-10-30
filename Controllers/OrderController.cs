using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace OnlineBookStoreMVC.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly IDeliveryService _deliveryService;
        private readonly PaymentService _paymentService;
        private readonly INotyfService _notyf;

        public OrderController(IAddressService addressService,IOrderService orderService, IShoppingCartService shoppingCartService, PaymentService paymentService, IDeliveryService deliveryService, INotyfService notyf)
        {
            _addressService = addressService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _paymentService = paymentService;
            _deliveryService = deliveryService;
            _notyf = notyf;
        }

        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummary(Guid selectedAddressId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var selectedAddress = await _addressService.GetAddressByIdAsync(selectedAddressId);
            if (selectedAddress == null || selectedAddress.UserId != userId)
            {
                return BadRequest("Invalid address selected.");
            }
            var orderSummary = await _orderService.GetOrderSummaryAsync(userId, selectedAddressId);

            if (orderSummary == null)
            {
                return NotFound("Order summary not found.");
            }
            return View("OrderSummary", orderSummary);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummaries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderSummaries = await _orderService.GetAllOrderSummariesAsync(userId);
            return View(orderSummaries);
        }

        public async Task<IActionResult> CheckoutComplete(string userId)
        {
            var order = await _orderService.CheckoutCompleteAsync(userId);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ListOrders(int page = 1, int pageSize = 10)
        {
            var paginatedOrders = await _orderService.GetPaginatedOrdersAsync(page, pageSize); 
            return View(paginatedOrders);
        }


        [HttpGet]
        public async Task<IActionResult> UserOrders(string userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success)
            {
                return NotFound();
            }

            _notyf.Success("Order deleted successfully.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderConfirmation(Guid orderId)
        {
            var order = await _orderService.GetOrderDetailsAsync(orderId);
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderSummaryDto orderSummary)
        {
            var order = await _orderService.PlaceOrderAsync(orderSummary);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);

            var callbackUrl = Url.Action("OrderConfirmation", "Order", new { orderId = order.Id }, Request.Scheme);

            var paymentResponse = await _paymentService.InitializePaymentAsync(
                orderSummary.OrderTotal,
                email,
                callbackUrl,
                order.Id.ToString()
            );

            if (paymentResponse == null || paymentResponse.Data == null || string.IsNullOrEmpty(paymentResponse.Data.AuthorizationUrl))
            {
                _notyf.Error("Failed to initialize payment. Please try again.");
                throw new InvalidOperationException($"The AuthorizationUrl is missing or invalid. Error: {paymentResponse?.Message}");
            }

            await _shoppingCartService.ClearCartAsync(userId);
            _notyf.Success("Order placed successfully. Redirecting to payment...");
            return Redirect(paymentResponse.Data.AuthorizationUrl);
        }

        [HttpGet]
        public async Task<IActionResult> AllPendingOrderSummaries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderSummaries = await _orderService.GetAllPendingOrdersAsync(userId);
            return View(orderSummaries);
        }

        [HttpGet]
        public async Task<IActionResult> AssignDeliveryToOrder(Guid id)
        {
            return View(new OrderDto { Id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AssignDeliveryToOrder([FromRoute] Guid id, Guid deliveryId)
        {
            var result = await _orderService.AssignDeliveryToOrderAsync(id, deliveryId);

            _notyf.Success(result != null ? "Delivery assigned successfully." : "Failed to assign delivery. Please try again.");

            return RedirectToAction("AllPendingOrderSummaries");
        }
    }
}
