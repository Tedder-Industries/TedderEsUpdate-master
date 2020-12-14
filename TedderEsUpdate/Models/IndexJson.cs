using System.Text.Json.Serialization;

namespace TedderEsUpdate.Models
{
    public class IndexJson
    {
        [JsonPropertyName("index")]
        public IndexModel Index { get; set; }

        public IndexJson()
        {
            this.Index = new IndexModel
            {
                Index = "product_search",
                Type = "doc"
            };
        }
    }
}