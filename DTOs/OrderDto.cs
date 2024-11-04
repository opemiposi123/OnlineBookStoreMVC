using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public Guid? DeliveryId { get; set; }
        public string DeliveryEmail { get; set; }
        public string DeliveryName { get; set; } 
        public string DeliveryPhoneNumber { get; set; } 
        public OrderStatus OrderStatus { get; set; }
    }

}
