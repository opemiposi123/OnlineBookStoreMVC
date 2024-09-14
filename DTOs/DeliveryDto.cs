using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.DTOs
{
    public class DeliveryDto
    {
        public Guid Id { get; set; } 
        public DeliveryStatus DeliveryStatus { get; set; }
        public TransportationType TransportationType { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
