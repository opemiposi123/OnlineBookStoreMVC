using OnlineBookStoreMVC.DTOs;

namespace OnlineBookStoreMVC.Models.RequestModels
{
    public class OrderRequestModel
    {
        public string UserId { get; set; }
        public List<OrderItemRequestModel> OrderItems { get; set; }
    }
}
