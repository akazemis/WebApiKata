using System.Net.Http;

namespace DataAccess.ExternalApi
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}
