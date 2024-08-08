using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartDto> GetCartAsync(Guid userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return new ShoppingCartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                ShoppingCartItems = cart.ShoppingCartItems.Select(ci => new ShoppingCartItemDto
                {
                    Id = ci.Id,
                    //BookId = ci.BookId,
                    BookTitle = ci.Book.Title,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList()
            };
        }

        public async Task AddToCartAsync(Guid userId, Guid bookId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _context.ShoppingCartItems
                .SingleOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.BookId == bookId);

            if (cartItem == null)
            {
                var book = await _context.Books.FindAsync(bookId);
                cartItem = new ShoppingCartItem
                {
                    Id = Guid.NewGuid(),
                    ShoppingCartId = cart.Id,
                    BookId = bookId,
                    Quantity = quantity,
                    Price = book.Price
                };
                _context.ShoppingCartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> ReduceQuantityAsync(Guid userId, Guid bookId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _context.ShoppingCartItems
                .SingleOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.BookId == bookId);

            if (cartItem == null) return 0;

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                _context.ShoppingCartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return cartItem.Quantity;
        }

        public async Task<int> IncreaseQuantityAsync(Guid userId, Guid bookId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _context.ShoppingCartItems
                .SingleOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.BookId == bookId);

            if (cartItem == null) return 0;

            cartItem.Quantity++;
            await _context.SaveChangesAsync();

            return cartItem.Quantity;
        }

        public async Task RemoveFromCartAsync(Guid userId, Guid bookId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _context.ShoppingCartItems
                .SingleOrDefaultAsync(ci => ci.ShoppingCartId == cart.Id && ci.BookId == bookId);

            if (cartItem != null)
            {
                _context.ShoppingCartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItems = _context.ShoppingCartItems.Where(ci => ci.ShoppingCartId == cart.Id);

            _context.ShoppingCartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return await _context.ShoppingCartItems
                .Where(ci => ci.ShoppingCartId == cart.Id)
                .SumAsync(ci => ci.Price * ci.Quantity);
        }

        private async Task<ShoppingCart> GetOrCreateCartAsync(Guid userId)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.ShoppingCartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}
