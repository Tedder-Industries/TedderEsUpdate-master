using Elasticsearch.Net;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TedderEsUpdate.Models;
using TedderEsUpdate.Shared;
using Xunit;

namespace TedderEsUpdate.Tests
{
    public class ElasticSearchTest
    {
        [Fact]
        public void GetAllEsProductsV_1()
        {
            EsClient client = new EsClient();
            StringResponse searchResponse = client.Client.Search<StringResponse>("product_search", PostData.Serializable(new
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

            string responseJson = searchResponse.Body;
            List<ProductModel> products = new List<ProductModel>();

            using JsonDocument document = JsonDocument.Parse(responseJson);

            JsonElement root = document.RootElement;
            JsonElement sourceElement = root.GetProperty("hits").GetProperty("hits");

            foreach (JsonElement source in sourceElement.EnumerateArray())
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
            }

            Assert.NotNull(products);
            Assert.IsType<List<ProductModel>>(products);
            Assert.NotEmpty(products);
            Debug.WriteLine($"V1 Count: {products.Count}");
        }

        [Fact]
        public async Task GetAllEsProductsV_2()
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

            string responseJson = searchResponse.Body;

            Assert.NotNull(responseJson);
            Assert.NotEmpty(responseJson);

            List<ProductModel> products = new List<ProductModel>();

            using JsonDocument document = JsonDocument.Parse(responseJson);

            Assert.NotNull(document);

            JsonElement root = document.RootElement;
            JsonElement sourceElement = root.GetProperty("hits").GetProperty("hits");

            foreach (JsonElement source in sourceElement.EnumerateArray())
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
            }

            Assert.NotNull(products);
            Assert.IsType<List<ProductModel>>(products);
            Assert.NotEmpty(products);
        }

        internal async Task<JsonElement> GetAllEsProductsV_3A()
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

        internal ConcurrentBag<ProductModel> GetAllEsProductsV_3B(JsonElement jsonElement)
        {
            ConcurrentBag<ProductModel> products = new ConcurrentBag<ProductModel>();

            Parallel.ForEach(jsonElement.EnumerateArray().AsParallel(), source =>
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

        [Fact]
        public void CallGetAllEsProductsV_3()
        {
            Task<JsonElement> task01 = Task.Run(() => GetAllEsProductsV_3A());
            task01.Wait();

            Assert.IsType<JsonElement>(task01.Result);
            Assert.NotEmpty(task01.Result.EnumerateArray());

            Task<ConcurrentBag<ProductModel>> task02 = Task.Run(() => GetAllEsProductsV_3B(task01.Result));
            task02.Wait();

            Assert.IsType<ConcurrentBag<ProductModel>>(task02.Result);
            Assert.NotEmpty(task02.Result);
        }

        [Fact]
        public async Task GetAllEsProductsV_4()
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

            var responseJson = JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");

            List<ProductModel> products = new List<ProductModel>();

            Parallel.ForEach(responseJson.EnumerateArray(), source =>
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

            Assert.NotNull(products);
            Assert.IsType<List<ProductModel>>(products);
            Assert.NotEmpty(products);

            Debug.WriteLine($"V4 Count: {products.Count}");
        }

        [Fact]
        internal void StringTester()
        {
            string fields = @"[""name"", ""sku""]";

            Debug.WriteLine(fields);
        }

        [Fact]
        public async Task Get()
        {
            string query = "shap";
            EsClient client = new EsClient();
            StringResponse searchResponse = await client.Client.SearchAsync<StringResponse>("product_search", PostData.Serializable(new
            {
                query = new
                {
                    @bool = new
                    {
                        should = new
                        {
                            @bool = new
                            {
                                must = new
                                {
                                    match_phrase_prefix = new
                                    {
                                        name = new
                                        {
                                            query = $"{query}",
                                            slop = 3,
                                            max_expansions = 10
                                        }
                                    }
                                },
                                should = new
                                {
                                    multi_match = new
                                    {
                                        query = $"{query}",
                                        fields = @"[""name"",""url"", ""image""]",
                                        fuzziness = "auto",
                                        @operator = "or"
                                    }
                                }
                            }
                        },
                        filter = new
                        {
                            term = new
                            {
                                type = "product"
                            }
                        }
                    }
                },
                size = 50,
                _source = @"[""name"", ""price"", ""sku""]",
            }));
            var jsonDoc = JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");

            var count = jsonDoc.EnumerateArray().Count();

            Debug.WriteLine($"Get Count: {count}");
        }
    }
}