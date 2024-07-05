using System.Text.Json.Serialization;

namespace BookStore.Model
{
    public class User
    {
        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
