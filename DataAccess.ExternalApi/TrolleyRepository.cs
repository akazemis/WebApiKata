using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DataAccess.ExternalApi
{
    public class TrolleyRepository : ITrolleyRepository
    {
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
            string fullApiUrl = GetFullApiUrl(ExternalApiPathName.CalculateTrolley);
            var requestContent = new StringContent(_serializer.Serialize(trolleyInfo));
            var response = await _httpClient.PostAsync(fullApiUrl, requestContent);

            if (!response.IsSuccessStatusCode)
            {
                throw GetExternalApiCallException(ExternalApiPathName.CalculateTrolley, response.StatusCode);
            }
            var responseText = await response.Content.ReadAsStringAsync();

            var result = Convert.ToDouble(responseText);
            return result;
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
