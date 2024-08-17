using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}
