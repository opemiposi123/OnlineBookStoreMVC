namespace OnlineBookStoreMVC.Entities
{
    public class Author : BaseEntity
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public List<Book> Books { get; set; }
    }
}
