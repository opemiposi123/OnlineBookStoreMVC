using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineBookStoreMVC.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } // Change to string to match IdentityUser
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }
    }

}
