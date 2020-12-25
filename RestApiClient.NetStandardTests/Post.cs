using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace TheProcessE.RestApiClient.Tests
{
    public class Post
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Post post &&
                   post == this;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, Id, Title, Body);
        }

        public static bool operator ==(Post left, Post right)
        {
            return left.UserId == right.UserId
                && left.Id == right.Id
                && left.Title == right.Title
                && left.Body == right.Body;
        }

        public static bool operator !=(Post left, Post right)
        {
            return !(left == right);
        }
    }
}