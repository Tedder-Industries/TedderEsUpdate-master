using System.Collections.Concurrent;
using System.Text.Json;

namespace TedderEsUpdate.Models
{
    public class PostResponse
    {
        public int DatabaseItemCount { get; set; }
        public int EsItemCount { get; set; }
        public int NewItemCount { get; set; }
        public JsonSerializerOptions SerializerOptions { get; set; }
        public ConcurrentBag<ProductModel> ProductModels { get; set; }

        public PostResponse()
        {
            this.SerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }
    }
}