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
        Task<bool> DeleteOrderAsync(Guid id);
        Task<OrderSummaryDto> GetOrderSummaryAsync(string userId);
        Task<OrderDto> PlaceOrderAsync(OrderSummaryDto orderSummary);
        Task<IEnumerable<OrderDto>> GetAllPendingOrdersAsync(string userId);
        //Task<List<OrderSummaryDto>> GetAllOrderPendingSummariesAsync(string userId);
        Task<OrderDto> AssignDeliveryToOrderAsync(Guid orderId, Guid deliveryId);
    }
}
