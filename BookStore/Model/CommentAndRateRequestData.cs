namespace BookStore.Model
{
    public class CommentAndRateRequestData
    {
        public Guid BookId { get; set; }
        
        public Guid UserId { get; set; }

        public string Comment { get; set; }

        public int Rate { get; set; }
    }
}
