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
        private readonly PaymentService _paymentService;

        public OrderController(IOrderService orderService, IShoppingCartService shoppingCartService, PaymentService paymentService)
        {
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _paymentService = paymentService;
        }
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderSummary = await _orderService.GetOrderSummaryAsync(userId);

            return View("OrderSummary", orderSummary);
        }

        [HttpGet]
        public async Task<IActionResult> OrderSummaries()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var success = await _orderService.DeleteOrderAsync(id);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Order deleted successfully.";
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
                throw new InvalidOperationException($"The AuthorizationUrl is missing or invalid. Error: {paymentResponse?.Message}");
            }

            await _shoppingCartService.ClearCartAsync(userId);

            return Redirect(paymentResponse.Data.AuthorizationUrl);
        }


        //[HttpPost]
        //public async Task<IActionResult> PlaceOrder(OrderSummaryDto orderSummary)
        //{
        //    var order = await _orderService.PlaceOrderAsync(orderSummary);

        //    var email = User.FindFirstValue(ClaimTypes.Email);

        //    var callbackUrl = Url.Action("VerifyPayment", "Order", new { reference = order.Id.ToString() }, Request.Scheme);

        //    var paymentResponse = await _paymentService.InitializePaymentAsync(
        //        orderSummary.OrderTotal,
        //        email,
        //        callbackUrl,
        //        order.Id.ToString()
        //    );

        //    if (paymentResponse == null || paymentResponse.Data == null || string.IsNullOrEmpty(paymentResponse.Data.AuthorizationUrl))
        //    {
        //        throw new InvalidOperationException($"The AuthorizationUrl is missing or invalid. Error: {paymentResponse?.Message}");
        //    }
        //    TempData["OrderSummary"] = JsonConvert.SerializeObject(orderSummary);

        //    return Redirect(paymentResponse.Data.AuthorizationUrl);
        //}

        //[HttpGet]
        //public async Task<IActionResult> VerifyPayment(string reference)
        //{
        //    // Verify the payment with Paystack
        //    var verifyResponse = await _paymentService.VerifyPaymentAsync(reference);

        //    //if (!verifyResponse.Status || verifyResponse.Data.Status != "success")
        //    //{
        //    //    ModelState.AddModelError("", "Payment verification failed. Please try again.");
        //    //    return RedirectToAction("OrderSummary");
        //    //}

        //    // Retrieve the orderSummary from TempData
        //    var orderSummaryJson = TempData["OrderSummary"] as string;
        //    var orderSummary = JsonConvert.DeserializeObject<OrderSummaryDto>(orderSummaryJson);

        //    //if (orderSummary == null)
        //    //{
        //    //    ModelState.AddModelError("", "Order summary could not be retrieved. Please try again.");
        //    //    return RedirectToAction("OrderSummary");
        //    //}

        //    // Clear the shopping cart
        //    //await _shoppingCartService.ClearCartAsync(User.Identity.Name);

        //    // Redirect to the OrderConfirmation view after placing the order
        //    return RedirectToAction("OrderConfirmation", new { orderId = orderSummary.OrderId });
        //}

    }
}
