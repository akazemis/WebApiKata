using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DataAccess.ExternalApi
{
    public class TrolleyRepository : ITrolleyRepository
    {
        const string JsonMimeType = "application/json";
        private readonly HttpClient _httpClient;
        private readonly IExternalApiPathProvider _externalApiPathProvider;
        private readonly ISerializer _serializer;
        private readonly IConfigProvider _configProvider;

        public TrolleyRepository(IHttpClientFactory httpClientFactory,
                                 IExternalApiPathProvider externalApiPathProvider,
                                 IConfigProvider configProvider,
                                 ISerializer serializer)
        {
            _httpClient = httpClientFactory.CreateClient();
            _externalApiPathProvider = externalApiPathProvider;
            _configProvider = configProvider;
            _serializer = serializer;
        }

        public async Task<double> CalculateTrolley(TrolleyInfo trolleyInfo)
        {
            if (TrolleyInfoOrItsComponentsIsNull(trolleyInfo))
            {
                return 0;
            }
            string fullApiUrl = GetFullApiUrl(ExternalApiPathName.CalculateTrolley);
            var requestContent = new StringContent(_serializer.Serialize(trolleyInfo));
            SetRequestContentHeaders(requestContent);

            var response = await _httpClient.PostAsync(fullApiUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw GetExternalApiCallException(ExternalApiPathName.CalculateTrolley, response.StatusCode);
            }
            var responseText = await response.Content.ReadAsStringAsync();

            var result = Convert.ToDouble(responseText);
            return result;
        }

        private bool TrolleyInfoOrItsComponentsIsNull(TrolleyInfo trolleyInfo)
        {
            return  trolleyInfo == null ||
                    trolleyInfo.Products == null ||
                    trolleyInfo.Quantities == null ||
                    trolleyInfo.Specials == null;
        }

        private static void SetRequestContentHeaders(StringContent requestContent)
        {
            requestContent.Headers.ContentType = new MediaTypeHeaderValue(JsonMimeType);
        }

        private ExternalApiCallException GetExternalApiCallException(ExternalApiPathName externalApiPathName, HttpStatusCode statusCode)
        {
            return new ExternalApiCallException($"Error in calling '{externalApiPathName}' API. StatusCode = {statusCode}");
        }

        private string GetFullApiUrl(ExternalApiPathName externalApiPathName)
        {
            var apiUrl = _externalApiPathProvider.GetApiPath(externalApiPathName);
            var token = _configProvider.GetConfigValue(ConfigKeys.Token);
            var fullApiUrl = $"{apiUrl}?token={token}";
            return fullApiUrl;
        }
    }
}
