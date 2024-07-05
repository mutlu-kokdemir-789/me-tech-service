namespace BookStore.Model
{
    public class BookListResponse
    {
        public int? NumberOfBooks { get; set; }

        public List<Book> Books { get; set; }
    }
}
