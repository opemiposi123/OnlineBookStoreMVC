using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Implementation.Services;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IDeliveryService _deliveryService; 
        private readonly INotyfService _notyf;

        public DeliveryController(IDeliveryService deliveryService, INotyfService notyf)
        {
            _deliveryService = deliveryService;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var deliveries = await _deliveryService.GetAllDeliverisAsync();
            return View(deliveries); 
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }
            return View(delivery);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DeliveryRequestModel deliveryRequest)
        {
            if (ModelState.IsValid)
            {
                var delivery = await _deliveryService.CreateDeliveryAsync(deliveryRequest);
                _notyf.Success("Delivery Created Succesfully");
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryRequest);
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var delivery = await _deliveryService.GetDeliveryByIdAsync(id);
            if (delivery == null)
            {
                return NotFound();
            }

            var deliveryRequest = new DeliveryRequestModel
            {
                FirstName = delivery.FirstName,
                LastName = delivery.LastName,
                DeliveryStatus = delivery.DeliveryStatus,
                TransportationType = delivery.TransportationType,
                Email = delivery.Email,
                Gender = delivery.Gender,
                PhoneNumber = delivery.PhoneNumber
            };
            return View(deliveryRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, DeliveryRequestModel deliveryRequest)
        {
            if (ModelState.IsValid)
            {
                var updatedDelivery = await _deliveryService.UpdateDeliveryAsync(id, deliveryRequest);
                _notyf.Success("Delivery Updated Succesfully");
                if (updatedDelivery == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(deliveryRequest);
        }

        [HttpGet("Delete")]
        public async Task<IActionResult> DeleteDelvery(Guid id) 
        {
            var success = await _deliveryService.DeleteDeliveryAsync(id);
            _notyf.Success("Delivery Deleted Succesfully");
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
