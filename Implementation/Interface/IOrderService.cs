using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(Guid id);
        //Task<OrderDto> CreateOrderAsync(OrderRequestModel orderRequest);
        //Task<OrderDto> UpdateOrderAsync(Guid id, OrderRequestModel orderRequest);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
