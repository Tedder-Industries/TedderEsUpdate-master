using Elasticsearch.Net;

namespace TedderEsUpdate.Shared
{
    public class EsIndex
    {
        public static bool IndexCheck(string index)
        {
            EsClient client = new EsClient();
            bool success;
            try
            {
                success = client.Client.Indices.Exists<StringResponse>(index).Success;
            }
            catch { success = false; }

            return success;
        }

        public static bool CreateIndex(string index)
        {
            EsClient client = new EsClient();
            client.Client.Indices.Create<StringResponse>(index, PostData.Serializable(new { }));
            bool success;
            try
            {
                success = client.Client.Indices.Create<StringResponse>(index, PostData.Serializable(new { })).Success;
            }
            catch { success = false; }

            return success;
        }
    }
}