using System.Text.Json.Serialization;

namespace TedderEsUpdate.Models
{
    public class IndexModel
    {
        [JsonPropertyName("_index")]
        public string Index { get; set; }

        [JsonPropertyName("_type")]
        public string Type { get; set; }
    }
}