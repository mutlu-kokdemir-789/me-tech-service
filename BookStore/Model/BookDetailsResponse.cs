namespace BookStore.Model
{
    public class BookDetailsResponse
    {
        public Book Book { get; set; }

        public List<CommentResponseForBookDetails> Comments { get; set; }
    }
}
