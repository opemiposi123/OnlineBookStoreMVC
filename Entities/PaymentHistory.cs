using OnlineBookStoreMVC.Enums;

namespace OnlineBookStoreMVC.Entities;

public class PaymentHistory : BaseEntity
{
    public Guid? UserId { get; set; }
    public User User { get; set; }
    public string ReferenceNumber { get; set; }
    public Guid ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public bool IsPaid { get; set; } = false;
}