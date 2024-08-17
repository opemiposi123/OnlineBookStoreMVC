using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Interface
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartAsync(string userId);
        Task AddToCartAsync(string userId, Guid bookId, int quantity);
        Task<int> ReduceQuantityAsync(string userId, Guid bookId);
        Task<int> IncreaseQuantityAsync(string userId, Guid bookId);
        Task RemoveFromCartAsync(string userId, Guid bookId);
        Task ClearCartAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<int> GetTotalItemsQuantityCountAsync(string userId);
    }
}
