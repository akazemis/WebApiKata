using WebApiKata.Interfaces;
using WebApiKata.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiKata.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IExternalApiPathProvider _externalApiPathProvider;
        private readonly ISerializer _serializer;
        private readonly IConfigProvider _configProvider;

        public ProductRepository(IHttpClientFactory httpClientFactory,
                                 IExternalApiPathProvider externalApiPathProvider,
                                 IConfigProvider configProvider,
                                 ISerializer serializer)
        {
            _httpClient = httpClientFactory.CreateClient();
            _externalApiPathProvider = externalApiPathProvider;
            _configProvider = configProvider;
            _serializer = serializer;
        }

        public async Task<List<Product>> GetSortedProducts(SortType sortType)
        {
            var products = await GetProducts();
            if (products != null)
            {
                var productComparisonFunction = await GetProductComparisonFunction(sortType);
                products.Sort(new Comparison<Product>(productComparisonFunction));
            }
            return products;
        }

        private async Task<Func<Product, Product, int>> GetProductComparisonFunction(SortType sortType)
        {
            var productPopularityDictionary = new Dictionary<string, int>();
            if (sortType == SortType.Recommended)
            {
                productPopularityDictionary = await GetProductPopularityDictionary();
            }
            Func<Product, Product, int> comparisonFunction = (Product product1, Product product2) =>
            {
                switch (sortType)
                {
                    case SortType.Low:
                        return product1.Price.CompareTo(product2.Price);
                    case SortType.High:
                        return product2.Price.CompareTo(product1.Price);
                    case SortType.Ascending:
                        return String.Compare(product1.Name, product2.Name);
                    case SortType.Descending:
                        return String.Compare(product2.Name, product1.Name);
                    case SortType.Recommended:
                        /// Sort by popularity in descending order
                        return ProductPopularityCompare(product2, product1, productPopularityDictionary);
                    default:
                        throw new NotSupportedException($"SortType not supported. SortType = [{sortType}]");
                }
            };
            return comparisonFunction;
        }

        private int ProductPopularityCompare(Product product1, Product product2, Dictionary<string, int> productPopularityDictionary)
        {
            int product1Popularity = GetProductPopularity(product1, productPopularityDictionary);
            var product2Popularity = GetProductPopularity(product2, productPopularityDictionary);
            return product1Popularity.CompareTo(product2Popularity);
        }

        private int GetProductPopularity(Product product, Dictionary<string, int> productPopularityDictionary)
        {
            return productPopularityDictionary.ContainsKey(product.Name)
                   ? productPopularityDictionary[product.Name]
                   : 0;
        }

        private async Task<List<Product>> GetProducts()
        {
            string fullApiUrl = GetFullApiUrl(ExternalApiPathName.GetProducts);
            var response = await _httpClient.GetAsync(fullApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw GetExternalApiCallException(ExternalApiPathName.GetProducts, response.StatusCode);
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            // It would have been better to use a separate model class for the object returned from the external API
            // The same model (ShopperHistory) is used exceptionally and only for simplicity though
            var result = _serializer.Deserialize<List<Product>>(responseJson);
            return result;
        }

        private async Task<Dictionary<string, int>> GetProductPopularityDictionary()
        {
            var shopperHistoryList = await GetShopperHistoryList();
            var productPopularityDictionary = new Dictionary<string, int>();
            if (shopperHistoryList == null)
            {
                return productPopularityDictionary;
            }
            shopperHistoryList
                .ForEach(item => item?.Products?.ForEach(product =>
                {
                    SetProductPopularity(product, productPopularityDictionary);
                }));
            return productPopularityDictionary;
        }

        /// <summary>
        /// Sets product popularity based on the number of times a product was in a customer purchase list
        /// </summary>
        /// <param name="product"></param>
        /// <param name="productPopularityDictionary"></param>
        private static void SetProductPopularity(Product product, Dictionary<string, int> productPopularityDictionary)
        {
            var productName = product?.Name;

            if (productName != null)
            {
                if (productPopularityDictionary.ContainsKey(productName))
                {
                    productPopularityDictionary[productName] += 1;
                }
                else
                {
                    productPopularityDictionary[productName] = 1;
                }
            }
        }

        private async Task<List<ShopperHistory>> GetShopperHistoryList()
        {
            string fullApiUrl = GetFullApiUrl(ExternalApiPathName.GetShopperHistory);
            var response = await _httpClient.GetAsync(fullApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw GetExternalApiCallException(ExternalApiPathName.GetShopperHistory, response.StatusCode);
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            // It would have been better to use a separate model class for the object returned from the external API
            // The same model (ShopperHistory) is used exceptionally and only for simplicity though
            var result = _serializer.Deserialize<List<ShopperHistory>>(responseJson);
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
