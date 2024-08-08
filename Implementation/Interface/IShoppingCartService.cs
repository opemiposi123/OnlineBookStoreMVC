using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartAsync(Guid userId);
        Task AddToCartAsync(Guid userId, Guid bookId, int quantity);
        Task<int> ReduceQuantityAsync(Guid userId, Guid bookId);
        Task<int> IncreaseQuantityAsync(Guid userId, Guid bookId);
        Task RemoveFromCartAsync(Guid userId, Guid bookId);
        Task ClearCartAsync(Guid userId);
        Task<decimal> GetCartTotalAsync(Guid userId);
    }
}
