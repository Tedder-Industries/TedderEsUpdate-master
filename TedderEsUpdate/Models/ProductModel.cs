using System;

namespace TedderEsUpdate.Models
{
    public class ProductModel
    {
        public string Type { get; set; }
        public string Sku { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}