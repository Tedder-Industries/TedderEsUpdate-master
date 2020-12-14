using Elasticsearch.Net;
using TedderEsUpdate.Shared;
using Xunit;

namespace TedderEsUpdate.Tests
{
    public class IndexCheckTest
    {
        [Fact]
        public void IndexCheck()
        {
            string index = "product_search";
            EsClient client = new EsClient();
            bool success;
            success = client.Client.Indices.Exists<StringResponse>(index).Success;

            Assert.True(success.Equals(true));
        }
    }
}