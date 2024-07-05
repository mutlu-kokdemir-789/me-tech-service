using System.Text.Json.Serialization;

namespace BookStore.Model
{
    public class Book
    {
        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int? PublishingYear { get; set; }

        public int? Price { get; set; }

        public double? Rate { get; set; }
    }
}
