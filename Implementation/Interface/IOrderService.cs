using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IOrderService
    {
        Task<List<OrderSummaryDto>> GetAllOrderSummariesAsync(string userId);
        Task<OrderDto> CheckoutAsync(OrderRequestModel orderRequest);
        Task<OrderDto> CheckoutCompleteAsync(string userId);
        Task<OrderDto> CreateOrderAsync(OrderRequestModel orderRequest);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<OrderDto> GetOrderDetailsAsync(Guid id);
        Task<OrderSummaryDto> GetOrderSummaryAsync(string userId);
        Task<OrderDto> PlaceOrderAsync(OrderSummaryDto orderSummary);
    }
}
