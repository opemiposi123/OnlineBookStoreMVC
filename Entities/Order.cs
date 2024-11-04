using OnlineBookStoreMVC.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStoreMVC.Entities
{
   public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
        public Guid AddressId { get; set; }
        public Address Address { get; set; }
        public Guid? DeliveryId { get; set; }
        public Delivery Delivery { get; set; }
        public OrderStatus OrderStatus { get; set; }
    } 
}
