using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ApplicationDbContext _context;
        public DeliveryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryDto> CreateDeliveryAsync(DeliveryRequestModel deliveryRequest)
        {
            var delivery = new Delivery 
            {
                Id = Guid.NewGuid(),
                FirstName = deliveryRequest.FirstName,
                Email = deliveryRequest.Email,
                LastName = deliveryRequest.LastName,
                Gender = deliveryRequest.Gender,
                TransportationType = deliveryRequest.TransportationType,
                PhoneNumber = deliveryRequest.PhoneNumber,
            };
            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            return new DeliveryDto
            {
                Id = delivery.Id,
                FirstName = delivery.FirstName,
            };
        }

        public async Task<bool> DeleteDeliveryAsync(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return false;

            _context.Deliveries.Remove(delivery);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DeliveryDto>> GetAllDeliverisAsync() 
        {
            var deliveries = await _context.Deliveries.ToListAsync(); 
            return deliveries.Select(c => new DeliveryDto  
            {
                Id = c.Id, 
                FirstName = c.FirstName,
                LastName = c.LastName,
                TransportationType = c.TransportationType, 
                DeliveryStatus = c.DeliveryStatus, 
                PhoneNumber = c.PhoneNumber,
                Gender = c.Gender,
                Email = c.Email
            });
        }

        public async Task<DeliveryDto> GetDeliveryByIdAsync(Guid id)
        {
            var delivery = await _context.Deliveries.FindAsync(id); 
            if (delivery == null) return null;

            return new DeliveryDto
            {
                Id = delivery.Id,
                FirstName = delivery.FirstName,
                LastName = delivery.LastName,
                TransportationType = delivery.TransportationType,
                DeliveryStatus = delivery.DeliveryStatus,
                PhoneNumber = delivery.PhoneNumber,
                Gender = delivery.Gender,
                Email = delivery.Email
            };
        }

        public async Task<IEnumerable<SelectListItem>> GetDeliverySelectList()
        {
            var deliveries = await GetAllDeliverisAsync(); 
            var deliveryList = deliveries.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.FirstName
            });

            return new SelectList(deliveryList, "Value", "Text");
        }

        public async  Task<DeliveryDto> UpdateDeliveryAsync(Guid id, DeliveryRequestModel deliveryRequest)
        {
            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null) return null;

            delivery.FirstName = delivery.FirstName;
            delivery.LastName = delivery.LastName; 
            delivery.TransportationType = delivery.TransportationType;
            delivery.Email = delivery.Email;
            delivery.Gender = delivery.Gender;
            delivery.PhoneNumber = delivery.PhoneNumber;

            await _context.SaveChangesAsync();

            return new DeliveryDto 
            {
                Id = delivery.Id,
                FirstName = delivery.FirstName
            };
        }
    }
}
