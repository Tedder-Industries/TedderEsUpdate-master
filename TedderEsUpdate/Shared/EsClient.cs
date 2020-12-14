using Elasticsearch.Net;
using System;

namespace TedderEsUpdate.Shared
{
    public class EsClient
    {
        public ConnectionConfiguration ConnectionConfiguration { get; protected set; }
        public ElasticLowLevelClient Client { get; protected set; }

        protected static readonly Uri Uri = new Uri("https://search-dev-aliengearholsters-mgmpy5ufy7k37ylumwlnssdlh4.us-west-2.es.amazonaws.com:443");

        public EsClient()
        {
            this.ConnectionConfiguration = new ConnectionConfiguration(Uri)
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromSeconds(30))
                .ThrowExceptions()
                .BasicAuthentication("dev-agh", "Ma@252537");

            this.Client = new ElasticLowLevelClient(this.ConnectionConfiguration);
        }
    }
}