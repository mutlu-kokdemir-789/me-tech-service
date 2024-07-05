﻿using System.Text.Json.Serialization;

namespace BookStore.Model
{
    public class Comment
    {
        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid Id { get; set; }

        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid UserId { get; set; }

        [JsonConverter(typeof(GuidJsonConverter))]
        public Guid BookId { get; set; }

        public string Message { get; set; }
    }
}
