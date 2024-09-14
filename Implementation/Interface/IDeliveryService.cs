using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IDeliveryService
    {
        Task<IEnumerable<DeliveryDto>> GetAllDeliverisAsync(); 
        Task<DeliveryDto> GetDeliveryByIdAsync(Guid id); 
        Task<DeliveryDto> CreateDeliveryAsync(DeliveryRequestModel deliveryRequest);  
        Task<DeliveryDto> UpdateDeliveryAsync(Guid id, DeliveryRequestModel deliveryRequest);  
        Task<bool> DeleteDeliveryAsync(Guid id); 
        Task<IEnumerable<SelectListItem>> GetDeliverySelectList(); 
    }
}
