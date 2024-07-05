using System.Text.Json.Serialization;

namespace BookStore.Model
{
    public class CommentResponseForBookDetails
    {
        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }
    }
}
