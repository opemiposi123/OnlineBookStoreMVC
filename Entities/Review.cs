namespace OnlineBookStoreMVC.Entities
{
    public class Review : BaseEntity
    {
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
