using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Entities;
using OnlineBookStoreMVC.Enums;
using OnlineBookStoreMVC.Helper;
using OnlineBookStoreMVC.Implementation.Interface;

namespace OnlineBookStoreMVC.Implementation.Services
{
    [Authorize]
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<OrderService> _logger;

        public OrderService(ApplicationDbContext context, IShoppingCartService shoppingCartService, IEmailService emailService, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, ILogger<OrderService> logger)
        {
            _context = context;
            _shoppingCartService = shoppingCartService;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }
        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        //public async Task<OrderDto> CheckoutAsync(OrderRequestModel orderRequest)
        //{
        //    try
        //    {
        //        var order = new Order
        //        {
        //            UserId = orderRequest.UserId,
        //            OrderDate = DateTime.UtcNow,
        //            OrderItems = orderRequest.OrderItems.Select(oi => new OrderItem
        //            {
        //                BookId = oi.BookId,
        //                Quantity = oi.Quantity,
        //                UnitPrice = oi.UnitPrice
        //            }).ToList(),
        //            TotalAmount = orderRequest.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
        //        };

        //        _context.Orders.Add(order);
        //        await _context.SaveChangesAsync();

        //        foreach (var orderItem in order.OrderItems)
        //        {
        //            orderItem.Book = await _context.Books.FindAsync(orderItem.BookId);
        //            if (orderItem.Book == null)
        //            {
        //                throw new Exception($"Book with ID {orderItem.BookId} not found.");
        //            }
        //        }

        //        return new OrderDto
        //        {
        //            Id = order.Id,
        //            UserId = order.UserId,
        //            OrderDate = order.OrderDate,
        //            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
        //            {
        //                Id = oi.Id,
        //                BookId = oi.BookId,
        //                BookTitle = oi.Book?.Title ?? "Unknown Book",
        //                Quantity = oi.Quantity,
        //                UnitPrice = oi.UnitPrice
        //            }).ToList(),
        //            TotalAmount = order.TotalAmount
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

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

        public async Task<IEnumerable<OrderDto>> GetAllPendingOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Pending)
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

        //public async Task<OrderSummaryDto> GetOrderSummaryAsync(string userId)
        // {
        //     var cart = await _shoppingCartService.GetCartAsync(userId);
        //     var address = await _context.Addresses
        //         .Where(a => a.UserId == userId)
        //         .OrderByDescending(a => a.CreatedAt)
        //         .FirstOrDefaultAsync();

        //     return new OrderSummaryDto
        //     {
        //         ShoppingCart = cart ?? new ShoppingCartDto(),
        //         UserId = userId,
        //         Address = address != null ? new AddressDto
        //         {
        //             FullName = address.FullName,
        //             PhoneNumber = address.PhoneNumber,
        //             AddittionalPhoneNumber = address.AddittionalPhoneNumber,
        //             City = address.City,
        //             Region = address.Region,
        //             DeliveryAddress = address.DeliveryAddress
        //         } : new AddressDto()
        //     };
        // }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(string userId, Guid? selectedAddressId)
        {
            var cart = await _shoppingCartService.GetCartAsync(userId);

            Address address;

            // If an address is selected, use that. Otherwise, get the most recent address.
            if (selectedAddressId.HasValue)
            {
                address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == selectedAddressId.Value && a.UserId == userId);
            }
            else
            {
                address = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();
            }

            return new OrderSummaryDto
            {
                ShoppingCart = cart ?? new ShoppingCartDto(),
                UserId = userId,
                Address = address != null ? new AddressDto
                {
                    FullName = address.FullName,
                    PhoneNumber = address.PhoneNumber,
                    AddittionalPhoneNumber = address.AddittionalPhoneNumber,
                    City = address.City,
                    Region = address.Region,
                    DeliveryAddress = address.DeliveryAddress
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
                    PhoneNumber = o.Address.PhoneNumber,
                    AddittionalPhoneNumber = o.Address.AddittionalPhoneNumber,
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
                // Validate stock availability for each book
                foreach (var item in orderSummary.ShoppingCart.ShoppingCartItems)
                {
                    var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
                    if (book == null)
                    {
                        throw new InvalidOperationException($"The book '{item.BookTitle}' does not exist.");
                    }

                    if (item.Quantity > book.TotalQuantity)
                    {
                        throw new InvalidOperationException($"Not enough stock for '{item.BookTitle}'. Only {book.TotalQuantity} available.");
                    }
                }

                // Fetch the existing address from the database based on the user ID and address details
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.UserId == orderSummary.UserId &&
                                              a.FullName == orderSummary.Address.FullName &&
                                              a.DeliveryAddress == orderSummary.Address.DeliveryAddress &&
                                              a.City == orderSummary.Address.City &&
                                              a.Region == orderSummary.Address.Region &&
                                              a.PhoneNumber == orderSummary.Address.PhoneNumber);

                if (existingAddress == null)
                {
                    throw new InvalidOperationException("Selected address does not exist.");
                }

                // Create the new order and link it to the existing address by Id
                var order = new Order
                {
                    UserId = orderSummary.UserId,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = orderSummary.OrderTotal,
                    OrderStatus = OrderStatus.Pending,
                    OrderItems = orderSummary.ShoppingCart.ShoppingCartItems?.Select(item => new OrderItem
                    {
                        BookId = item.BookId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price
                    }).ToList() ?? new List<OrderItem>(),
                    AddressId = existingAddress.Id // Link to existing address
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Reduce stock quantity for each book
                foreach (var orderItem in order.OrderItems)
                {
                    var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == orderItem.BookId);
                    if (book != null)
                    {
                        book.TotalQuantity -= orderItem.Quantity;
                        _context.Books.Update(book);
                    }
                }

                await _context.SaveChangesAsync();

                // Send confirmation email
                var code = CodeGenerator.GenerateRandomCode(6);
                var deliveryDate = DateTime.UtcNow.AddDays(3);
                var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
                var emailSent = await _emailService.SendOrderConfirmationEmailAsync(currentUser.Email, orderSummary.Address.FullName, code, deliveryDate);

                // Return the order details
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
            var order = await _context.Orders
                .Include(o => o.Delivery)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
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
