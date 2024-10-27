using Microsoft.AspNetCore.Identity;
using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.Entities
{
    public class User : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string FullName { get; set; }
        public string Gender { get; set; }
        public Role Role { get; set; }
        public List<Order> Orders { get; set; }
    }
}
