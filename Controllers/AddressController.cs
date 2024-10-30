using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;
using System.Security.Claims;

namespace OnlineBookStoreMVC.Controllers
{
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressController(IAddressService addressService, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _addressService = addressService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = await _addressService.GetAllAddressesByUserIdAsync(userId);
            return View(addresses);
        }
        [HttpGet]
        public async Task<IActionResult> AddAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUserByIdAsync(userId);

            var model = new AddressRequestModel
            {
                FullName = user.FullName 
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressRequestModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _addressService.AddAddressAsync(model, userId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAddress(Guid addressId, AddressRequestModel model)
        {
            await _addressService.UpdateAddressAsync(addressId, model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(Guid selectedAddress)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _addressService.SetDefaultAddressAsync(userId, selectedAddress);

            return RedirectToAction("OrderSummary", "Order", new { selectedAddressId = selectedAddress });
        }

    }
}

