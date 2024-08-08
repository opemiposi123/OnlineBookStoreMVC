using Microsoft.AspNetCore.Identity;

namespace OnlineBookStoreMVC.Entities
{
    public class User : IdentityUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    } 
}
