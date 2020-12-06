using System.Net.Http;

namespace WebApiKata.Services
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}
