using Microsoft.AspNetCore.Identity;

namespace OnlineBookStoreMVC.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string FullName { get; set; }
        public string Gender { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        // public ShoppingCart ShoppingCart { get; set;  }
    }
}
