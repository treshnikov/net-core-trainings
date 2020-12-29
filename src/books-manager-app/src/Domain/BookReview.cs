namespace Domain
{
    public class BookReview
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
    }
}