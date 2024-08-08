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
    }

}
