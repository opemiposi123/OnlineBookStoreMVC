using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Enums;
using OnlineBookStoreMVC.Implementation.Interface;
using OnlineBookStoreMVC.Models.RequestModels;

namespace OnlineBookStoreMVC.Implementation.Services
{
    [Authorize]
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IShoppingCartService _emailSService;

        public OrderService(ApplicationDbContext context, IShoppingCartService shoppingCartService)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
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

        public async Task<OrderDto> GetOrderDetailsAsync(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User.UserName ?? "Unknown User",
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

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems)
                                             .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<OrderSummaryDto> GetOrderSummaryAsync(string userId)
        {
            var cart = await _shoppingCartService.GetCartAsync(userId);
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);

            return new OrderSummaryDto
            {
                ShoppingCart = cart ?? new ShoppingCartDto(),
                UserId = userId,
                Address = address != null ? new AddressDto
                {
                    FullName = address.FullName,
                    Email = address.Email,
                    PhoneNumber = address.PhoneNumber,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    Country = address.Country
                } : new AddressDto()
            };
        }


        public async Task<List<OrderSummaryDto>> GetAllOrderSummariesAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .ToListAsync();

            var orderSummaries = orders.Select(o => new OrderSummaryDto
            {
                ShoppingCart = new ShoppingCartDto
                {
                    ShoppingCartItems = o.OrderItems.Select(oi => new ShoppingCartItemDto
                    {
                        BookId = oi.BookId,
                        BookTitle = oi.Book.Title,
                        Quantity = oi.Quantity,
                        Price = oi.UnitPrice
                    }).ToList(),
                },
                Address = new AddressDto
                {
                    FullName = o.Address.FullName,
                    Email = o.Address.Email,
                    PhoneNumber = o.Address.PhoneNumber,
                },
                UserId = userId,
            }).ToList();

            return orderSummaries;
        }

        public async Task<List<OrderSummaryDto>> GetAllOrderPendingSummariesAsync(string userId)
        {
            // Fetch all pending orders for the specified user
            var pendingOrders = await _context.Orders
                .Where(o => o.UserId == userId && o.OrderStatus == OrderStatus.Pendind)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .Include(o => o.Address)
                .ToListAsync();

            // Map each pending order to an OrderSummaryDto
            var orderSummaries = pendingOrders.Select(o => new OrderSummaryDto
            {
                ShoppingCart = new ShoppingCartDto
                {
                    ShoppingCartItems = o.OrderItems.Select(oi => new ShoppingCartItemDto
                    {
                        BookId = oi.BookId,
                        BookTitle = oi.Book.Title,
                        Quantity = oi.Quantity,
                        Price = oi.UnitPrice
                    }).ToList(),
                },
                Address = new AddressDto
                {
                    FullName = o.Address.FullName,
                    Email = o.Address.Email,
                    PhoneNumber = o.Address.PhoneNumber,
                    Street = o.Address.Street,
                    City = o.Address.City,
                    State = o.Address.State,
                    PostalCode = o.Address.PostalCode,
                    Country = o.Address.Country
                },
                UserId = userId,
            }).ToList();

            return orderSummaries;
        }

        public async Task<OrderDto> PlaceOrderAsync(OrderSummaryDto orderSummary)
        {
            if (orderSummary == null) throw new ArgumentNullException(nameof(orderSummary), "Order summary cannot be null.");
            if (orderSummary.ShoppingCart == null) throw new ArgumentNullException(nameof(orderSummary.ShoppingCart), "Shopping cart cannot be null.");
            if (orderSummary.Address == null) throw new ArgumentNullException(nameof(orderSummary.Address), "Address cannot be null.");

            try
            {
                var order = new Order
                {
                    UserId = orderSummary.UserId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = orderSummary.OrderTotal,
                    OrderStatus = OrderStatus.Pendind,
                    OrderItems = orderSummary.ShoppingCart.ShoppingCartItems?.Select(item => new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList() ?? new List<OrderItem>(),
                    Address = new Address
                    {
                        UserId = orderSummary.UserId,
                        FullName = orderSummary.Address.FullName,
                        Email = orderSummary.Address.Email,
                        PhoneNumber = orderSummary.Address.PhoneNumber,
                        Street = orderSummary.Address.Street,
                        City = orderSummary.Address.City,
                        State = orderSummary.Address.State,
                        PostalCode = orderSummary.Address.PostalCode,
                        Country = orderSummary.Address.Country
                    }
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return new OrderDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        BookId = oi.BookId,
                        BookTitle = oi.Book?.Title,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<OrderDto> AssignDeliveryToOrderAsync(Guid orderId, Guid deliveryId)
        {
            var order = await _context.Orders.Include(o => o.Delivery)
                                             .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            var delivery = await _context.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId);

            if (delivery == null)
            {
                throw new Exception("Delivery not found");
            }
            order.DeliveryId = delivery.Id;
            order.OrderStatus = OrderStatus.Shipping;
            order.Delivery = delivery;

            delivery.DeliveryStatus = DeliveryStatus.InTransit;

           
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.UserName ?? "Unknown User",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                DeliveryId = delivery.Id,
                DeliveryEmail = delivery.Email,
                DeliveryName = $"{delivery.FirstName} {delivery.LastName}",
                DeliveryPhoneNumber = delivery.PhoneNumber,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    BookId = oi.BookId,
                    BookTitle = oi.Book?.Title ?? "Unknown Book",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList()
            };
        }
    }
}
