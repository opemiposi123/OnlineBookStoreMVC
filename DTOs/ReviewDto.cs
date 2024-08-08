namespace OnlineBookStoreMVC.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } = 0;
    }
}
