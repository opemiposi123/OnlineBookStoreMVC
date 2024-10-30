using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IOrderService
    {
        Task<List<OrderSummaryDto>> GetAllOrderSummariesAsync(string userId);
        Task<OrderDto> CheckoutCompleteAsync(string userId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId);
        Task<OrderDto> GetOrderDetailsAsync(Guid id);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<PaginatedDto<OrderDto>> GetPaginatedOrdersAsync(int page, int pageSize);
        Task<OrderDto> PlaceOrderAsync(OrderSummaryDto orderSummary);
        Task<IEnumerable<OrderDto>> GetAllPendingOrdersAsync(string userId);
        List<Order> GetAllOrders();
        Task<OrderDto> AssignDeliveryToOrderAsync(Guid orderId, Guid deliveryId);
        Task<OrderSummaryDto> GetOrderSummaryAsync(string userId, Guid? selectedAddressId);
    }
}
