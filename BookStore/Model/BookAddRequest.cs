namespace BookStore.Model
{
    public class BookAddRequest
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public int PublishingYear { get; set; }

        public int Price { get; set; }
    }
}
