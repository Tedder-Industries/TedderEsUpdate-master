using Elasticsearch.Net;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TedderEsUpdate.Models;

namespace TedderEsUpdate.Shared
{
    public class EsProductIndex
    {
        public static async Task<ConcurrentBag<ProductModel>> GetEsSearchableProducts()
        {
            EsClient client = new EsClient();
            StringResponse searchResponse = new StringResponse();

            try
            {
                searchResponse = await client.Client.SearchAsync<StringResponse>("product_search", PostData.Serializable(
                new
                {
                    from = 0,
                    size = 10000,

                    query = new
                    {
                        match = new
                        {
                            type = "product"
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.InnerException.ToString());
            }

            string responseJson = searchResponse.Body;

            using JsonDocument document = JsonDocument.Parse(responseJson);
            JsonElement root = document.RootElement;
            JsonElement sourceElement = root.GetProperty("hits").GetProperty("hits");

            ConcurrentBag<ProductModel> products = new ConcurrentBag<ProductModel>();

            Parallel.ForEach(sourceElement.EnumerateArray().AsParallel(), source =>
            {
                if (source.TryGetProperty("_source", out JsonElement rec))
                {
                    ProductModel product = new ProductModel
                    {
                        Type = rec.GetProperty("type").GetString(),
                        Sku = rec.GetProperty("sku").GetString(),
                        Image = rec.GetProperty("image").GetString(),
                        Url = rec.GetProperty("url").GetString(),
                        Price = rec.GetProperty("price").GetDecimal(),
                        Name = rec.GetProperty("name").GetString(),
                        Description = rec.GetProperty("description").GetString()
                    };

                    products.Add(product);
                }
            });
            return products;
        }
    }
}