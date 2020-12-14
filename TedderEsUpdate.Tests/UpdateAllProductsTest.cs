using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TedderEsUpdate.Models;
using TedderEsUpdate.Shared;
using Xunit;

namespace TedderEsUpdate.Tests
{
    public class UpdateAllProductsTest
    {
        internal async Task<List<ProductModel>> GetAllWebProducts()
        {
            using AghMageContext db = new AghMageContext();
            db.Database.OpenConnection();

            using DbCommand command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "call get_current_products();";

            using var result = await command.ExecuteReaderAsync();
            List<ProductModel> DbProducts = result.Cast<IDataRecord>()
            .Select(dr => new ProductModel
            {
                Type = dr.GetString(0),
                Sku = dr.GetString(1),
                Image = dr.GetString(2),
                Url = dr.GetString(3),
                Price = dr.GetDecimal(4),
                Name = dr.GetString(5),
                Description = dr.GetString(6)
            }).AsParallel()
            .ToList();

            Assert.NotNull(DbProducts);
            Assert.IsType<List<ProductModel>>(DbProducts);
            Assert.NotEmpty(DbProducts);

            return DbProducts;
        }

        internal async Task<JsonElement> GetAllEsProducts()
        {
            EsClient client = new EsClient();

            StringResponse searchResponse = await client.Client.SearchAsync<StringResponse>("product_search", PostData.Serializable(new
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

            return JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");
        }

        internal ConcurrentBag<ProductModel> CreateEsProductObjects(JsonElement jsonElement)
        {
            ConcurrentBag<ProductModel> products = new ConcurrentBag<ProductModel>();
            List<JsonElement> catchList = new List<JsonElement>();

            void body(JsonElement source)
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
                else
                {
                    catchList.Add(source);
                }
            }
            bool isComplete = Parallel.ForEach(jsonElement.EnumerateArray(), body).IsCompleted;

            return products;
        }

        internal List<ProductModel> GetAllNewProducts
            (List<ProductModel> dbProductList, ConcurrentBag<ProductModel> esProductList)
        {
            List<ProductModel> updateAllProducts = new List<ProductModel>();

            HashSet<string> diffIdentifiers = new HashSet<string>(esProductList.Select(s => s.Url));
            updateAllProducts = dbProductList.Where(m => !diffIdentifiers.Contains(m.Url)).ToList();

            return updateAllProducts;
        }

        [Fact]
        public void CallFindAllNewProducts()
        {
            Task<List<ProductModel>> task01 = Task.Run(() => GetAllWebProducts());
            Debug.WriteLine($"Database Product Count: {task01.Result.Count}");

            Task<JsonElement> task02 = Task.Run(() => GetAllEsProducts());
            task02.Wait();

            Debug.WriteLine($"Enumerated Json Object Count: {task02.Result.EnumerateArray().Count()}");

            Assert.IsType<JsonElement>(task02.Result);
            Assert.NotEmpty(task02.Result.EnumerateArray());

            Task<ConcurrentBag<ProductModel>> task03 = Task.Run(() => CreateEsProductObjects(task02.Result));
            task03.Wait();

            Debug.WriteLine($"ElasticSearch Product Count: {task03.Result.Count}");

            Assert.IsType<ConcurrentBag<ProductModel>>(task03.Result);
            Assert.NotEmpty(task03.Result);

            task01.Wait();
            Assert.IsType<List<ProductModel>>(task01.Result);
            Assert.NotEmpty(task01.Result);

            Task<List<ProductModel>> task04 = Task.Run(() =>
                GetAllNewProducts(task01.Result, task03.Result));
            task04.Wait();
            Debug.WriteLine($"Change Count: {task04.Result.Count}");
            Assert.IsType<List<ProductModel>>(task04.Result);
        }
    }
}