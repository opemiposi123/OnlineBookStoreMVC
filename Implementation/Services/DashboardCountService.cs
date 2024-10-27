using OnlineBookStoreMVC.DTOs;
using OnlineBookStoreMVC.Implementation.Interface;
using System.Net.WebSockets;

namespace OnlineBookStoreMVC.Implementation.Services
{
    public class DashboardCountService : IDashboardCountService
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;
        private readonly IUserService _userService; 
        private readonly IOrderService _orderService;  
        private readonly IDeliveryService _deliveryService;   
        public DashboardCountService(IOrderService orderService, IDeliveryService deliveryService, IUserService userService, 
            IBookService bookService, ICategoryService categoryService)
        {
            _orderService = orderService;
            _deliveryService = deliveryService;
            _userService = userService;
            _bookService = bookService;
            _categoryService = categoryService; 
        }

        public async Task<DashboardCountDto> DashBoardCount()
        {
            var userCount = _userService.GetAllUser();
            var deliveryCount = _deliveryService.GetAllDeliveries();
            var orderCount = _orderService.GetAllOrders();
            var categoryCount = _categoryService.GetAllCategories();
            var bookCount = _bookService.GetAllBooks();

            var data = new DashboardCountDto();

            data.UserCount = userCount.Count();
            data.DeliveryCount = deliveryCount.Count();
            data.OrderCount = orderCount.Count();
            data.CategoryCount = categoryCount.Count();
            data.BookCount = bookCount.Count();
            
            return data;
        }

    }
}
