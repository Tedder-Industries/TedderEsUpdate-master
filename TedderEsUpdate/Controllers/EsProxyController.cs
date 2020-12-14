using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TedderEsUpdate.Models;
using TedderEsUpdate.Shared;

namespace TedderEsUpdate.Controllers
{
    [Route("api/[controller]")]
    public class EsProxyController : ControllerBase
    {
        public static readonly EsClient client = new EsClient();

        // GET api/EsProxy
        [HttpGet]
        public JsonElement Get()
        {
            StringResponse searchResponse = client.Client.Search<StringResponse>("product_search", PostData.Serializable(new
            {
                query = new
                {
                    match = new
                    {
                        type = "product"
                    }
                },
                size = 50,
                _source = new object[]
                {
                   "name", "image", "sku", "price"
                }
            }));
            return JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");
        }

        // GET api/EsProxy/query
        [HttpGet("{query}")]
        public async Task<JsonElement> Get(string query)
        {
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
                        }
                    }
                },
                size = 50,
                _source = new object[]
                {
                   "name", "image", "sku", "price"
                }
            }));
            Debug.WriteLine(@"[""name"", ""image"" , ""sku"",""price""]");
            var tempT = JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");
            return JsonDocument.Parse(searchResponse.Body).RootElement.GetProperty("hits").GetProperty("hits");
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            bool indexExist = EsIndex.IndexCheck("product_search");
            if (!indexExist)
            {
                bool indexCreated = EsIndex.CreateIndex("product_search");
                indexExist = indexCreated == true ? indexExist = true
                    : throw new ArgumentException("Index could not be created");
            }
            //StringResponse deleteResponse1 = client.Client.Delete<StringResponse>("product_search", "85f3cnIB0jvdFYl1x5dB");

            Task<List<ProductModel>> databaseProducts = Task.Run(() => ProductDatabase.GetAllWebProducts());
            Task<ConcurrentBag<ProductModel>> esIndexProducts = Task.Run(() => EsProductIndex.GetEsSearchableProducts());
            List<ProductModel> updateProducts = new List<ProductModel>();

            if (esIndexProducts != null)
            {
                esIndexProducts.Wait();
                HashSet<string> diffIdentifiers = new HashSet<string>(esIndexProducts.Result.Select(s => s.Url));
                databaseProducts.Wait();
                updateProducts = databaseProducts.Result.Where(m => !diffIdentifiers.Contains(m.Url)).ToList();
            }
            else
            {
                updateProducts = databaseProducts.Result;
            }

            PostResponse postResponse = new PostResponse
            {
                DatabaseItemCount = databaseProducts.Result.Count,
                EsItemCount = esIndexProducts.Result.Count,
                NewItemCount = updateProducts.Count,
                ProductModels = new ConcurrentBag<ProductModel>(updateProducts)
            };
            Byte[] jsonString = JsonSerializer.SerializeToUtf8Bytes(postResponse, postResponse.SerializerOptions);
            await this.Response.Body.WriteAsync(jsonString);

            try
            {
                if (this.Response.StatusCode == 200)
                {
                    Debug.WriteLine("\r\nResponse Status Code is OK and StatusDescription is: {0}",
                                    this.Response.StatusCode);
                }
            }
            catch (WebException e)
            {
                Debug.WriteLine("\r\nWebException Raised. The following error occurred : {0}", e.Status);
            }
            catch (Exception e)
            {
                Debug.WriteLine("\nThe following Exception was raised : {0}", e.Message);
            }

            ConcurrentBag<object> bulkUpdateObjectList = new ConcurrentBag<object>();

            Parallel.ForEach(postResponse.ProductModels, record =>
            {
                bulkUpdateObjectList.Add(new { index = new { _index = "product_search", _type = "doc" } });
                bulkUpdateObjectList.Add(new
                {
                    type = record.Type,
                    sku = record.Sku,
                    image = record.Image,
                    url = record.Url,
                    price = record.Price,
                    name = record.Name,
                    description = record.Description
                });
            });

            Task<StringResponse> esResponse = Task.Run(() =>
                client.Client.BulkAsync<StringResponse>(PostData.MultiJson(bulkUpdateObjectList)));

            esResponse.Wait();

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }
    }
}