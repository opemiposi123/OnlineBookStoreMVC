using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IOrderService
    {
        Task<OrderDto> CheckoutAsync(OrderRequestModel orderRequest);
        Task<OrderDto> CheckoutCompleteAsync(string userId);
        Task<OrderDto> CreateOrderAsync(OrderRequestModel orderRequest);
    }
}
