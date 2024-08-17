using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<OrderDto> CheckoutAsync(OrderRequestModel orderRequest)
        {
            try
            {
                var order = new Order
                {
                    UserId = orderRequest.UserId,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = orderRequest.OrderItems.Select(oi => new OrderItem
                    {
                        BookId = oi.BookId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList(),
                    TotalAmount = orderRequest.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Load the Book data for the order items
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Book = await _context.Books.FindAsync(orderItem.BookId);
                    if (orderItem.Book == null)
                    {
                        throw new Exception($"Book with ID {orderItem.BookId} not found.");
                    }
                }

                return new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        BookId = oi.BookId,
                        BookTitle = oi.Book?.Title ?? "Unknown Book",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList(),
                    TotalAmount = order.TotalAmount
                };
            }
            catch (Exception ex)
            {
                // Log exception (ex) for debugging
                throw;
            }
        }

        public async Task<OrderDto> CheckoutCompleteAsync(string userId)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                              .ThenInclude(oi => oi.Book)
                                              .Where(o => o.UserId == userId)
                                              .OrderByDescending(o => o.OrderDate)
                                              .FirstOrDefaultAsync();

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                TotalAmount = order.TotalAmount
            };
        }

        public async Task<OrderDto> CreateOrderAsync(OrderRequestModel orderRequest)
        {
            var order = new Order
            {
                UserId = orderRequest.UserId,
                OrderDate = DateTime.UtcNow,
                OrderItems = orderRequest.OrderItems.Select(oi => new OrderItem
                {
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                TotalAmount = orderRequest.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                TotalAmount = order.TotalAmount
            };
        }
       
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Include(o => o.User)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName ?? "Unknown User",
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                TotalAmount = order.TotalAmount
            });
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .ToListAsync();

            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName ?? "Unknown User",
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book?.Title ?? "Unknown Book",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList(),
                TotalAmount = order.TotalAmount
            });
        }

        public async Task<OrderDto> GetOrderDetials(Guid id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Select(order => new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = order.User.UserName ?? "Unknown User",
                    OrderDate = order.OrderDate,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        BookId = oi.BookId,
                        BookTitle = oi.Book.Title ?? "Unknown Book",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList(),
                    TotalAmount = order.TotalAmount
                }).FirstOrDefaultAsync();
            return order;
        }

    }

}
