namespace BookStore.Model
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Message { get; set; }
    }
}
