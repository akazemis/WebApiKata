using DataAccess.Interfaces;
using DataAccess.Models;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DataAccess.ExternalApi.Tests
{
    public class ProductRepositoryTest
    {
        [Theory]
        [MemberData(nameof(GetSortByNameAscendingInputAndExpectedOutputList))]
        public async void GetSortedProducts_WhenSortAscending_SortsByProductNameAscending(List<Product> externalApiResult, List<Product> expectedSortedResult)
        {
            // Arrange
            var sutFactory = new SutProductRepositoryFactory();
            sutFactory.SetupGetProductsApiResult(externalApiResult);

            var sutProductRepository = sutFactory.Create();

            // Act
            var result = await sutProductRepository.GetSortedProducts(SortType.Ascending);

            // Assert
            result.Should().BeEquivalentTo(expectedSortedResult, options => options.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(GetSortByNameDescendingInputAndExpectedOutputList))]
        public async void GetSortedProducts_WhenSortDescending_SortsByProductNameDescending(List<Product> externalApiResult, List<Product> expectedSortedResult)
        {
            // Arrange
            var sutFactory = new SutProductRepositoryFactory();
            sutFactory.SetupGetProductsApiResult(externalApiResult);

            var sutProductRepository = sutFactory.Create();

            // Act
            var result = await sutProductRepository.GetSortedProducts(SortType.Descending);

            // Assert
            result.Should().BeEquivalentTo(expectedSortedResult, options => options.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(GetSortByPriceAscendingInputAndExpectedOutputList))]
        public async void GetSortedProducts_WhenSortByLow_SortsByPriceAscending(List<Product> externalApiResult, List<Product> expectedSortedResult)
        {
            // Arrange
            var sutFactory = new SutProductRepositoryFactory();
            sutFactory.SetupGetProductsApiResult(externalApiResult);

            var sutProductRepository = sutFactory.Create();

            // Act
            var result = await sutProductRepository.GetSortedProducts(SortType.Low);

            // Assert
            result.Should().BeEquivalentTo(expectedSortedResult, options => options.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(GetSortByPriceDescendingInputAndExpectedOutputList))]
        public async void GetSortedProducts_WhenSortByHigh_SortsByPriceDescending(List<Product> externalApiResult, List<Product> expectedSortedResult)
        {
            // Arrange
            var sutFactory = new SutProductRepositoryFactory();
            sutFactory.SetupGetProductsApiResult(externalApiResult);

            var sutProductRepository = sutFactory.Create();

            // Act
            var result = await sutProductRepository.GetSortedProducts(SortType.High);

            // Assert
            result.Should().BeEquivalentTo(expectedSortedResult, options => options.WithStrictOrdering());
        }

        [Theory]
        [MemberData(nameof(GetSortByPopularityDescendingInputAndShopperHistoryAndExpectedOutputList))]
        public async void GetSortedProducts_WhenSortByRecommended_SortsByPopularityDescending(List<Product> externalApiResult, List<ShopperHistory> shopperHistoryList, List<Product> expectedSortedResult)
        {
            // Arrange
            var sutFactory = new SutProductRepositoryFactory();
            sutFactory.SetupGetProductsApiResult(externalApiResult);
            sutFactory.SetupGetGetShopperHistoryApiResult(shopperHistoryList);

            var sutProductRepository = sutFactory.Create();

            // Act
            var result = await sutProductRepository.GetSortedProducts(SortType.Recommended);

            // Assert
            result.Should().BeEquivalentTo(expectedSortedResult, options => options.WithStrictOrdering());
        }

        #region GetSortedProducts Inputs and Expected Outputs Generators

        public static IEnumerable<object[]> GetSortByNameAscendingInputAndExpectedOutputList()
        {
            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("a-3", 10, 100),
                    GetProduct("a-1", 20, 120)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("a-1", 20, 120),
                    GetProduct("a-3", 10, 100)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("def", 10, 100),
                    GetProduct("abc", 20, 120),
                    GetProduct("aaa", 30, 130)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("aaa", 30, 130),
                    GetProduct("abc", 20, 120),
                    GetProduct("def", 10, 100)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>(),
                // Expected Output
                new List<Product>()
            };

            yield return new object[]
            {
                // Input
                null,
                // Expected Output
                null
            };
        }

        public static IEnumerable<object[]> GetSortByNameDescendingInputAndExpectedOutputList()
        {
            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("a-1", 20, 120),
                    GetProduct("a-3", 10, 100)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("a-3", 10, 100),
                    GetProduct("a-1", 20, 120),
                }

            };

            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("aaa", 30, 130),
                    GetProduct("def", 10, 100),
                    GetProduct("abc", 20, 120)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("def", 10, 100),
                    GetProduct("abc", 20, 120),
                    GetProduct("aaa", 30, 130)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>(),
                // Expected Output
                 new List<Product>()
            };

            yield return new object[]
            {
                // Input
                null,
                // Expected Output
                null
            };
        }

        public static IEnumerable<object[]> GetSortByPriceAscendingInputAndExpectedOutputList()
        {
            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 2, 5),
                    GetProduct("product2", 1, 10)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("product2", 1, 10),
                    GetProduct("product1", 2, 5)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 2.0f, 1),
                    GetProduct("product2", 1.0f, 3),
                    GetProduct("product3", 1.2f, 2)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("product2", 1.0f, 3),
                    GetProduct("product3", 1.2f, 2),
                    GetProduct("product1", 2.0f, 1)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>(),
                
                // Expected Output
                 new List<Product>()
            };

            yield return new object[]
            {
               // Input
               null,
               // Expected Output
               null
            };
        }

        public static IEnumerable<object[]> GetSortByPriceDescendingInputAndExpectedOutputList()
        {
            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 1, 4),
                    GetProduct("product2", 2, 3)
                },
                // Expected Output 
                new List<Product>()
                {
                    GetProduct("product2", 2, 3),
                    GetProduct("product1", 1, 4)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 2.0f, 1),
                    GetProduct("product2", 1.0f, 3),
                    GetProduct("product3", 1.2f, 2)
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("product1", 2.0f, 1),
                    GetProduct("product3", 1.2f, 2),
                    GetProduct("product2", 1.0f, 3)
                }
            };

            yield return new object[]
            {
                 // Input
                 new List<Product>(),
                 // Expected Output
                 new List<Product>()
            };

            yield return new object[]
            {
                // Input
                null,
                // Expected Output
                null
            };
        }

        public static IEnumerable<object[]> GetSortByPopularityDescendingInputAndShopperHistoryAndExpectedOutputList()
        {
            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 1, 1),
                    GetProduct("product2", 1, 1),
                    GetProduct("product3", 1, 1)
                },
                // ShopperHistoryList
                new List<ShopperHistory>()
                {
                    new ShopperHistory()
                    {
                        CustomerId = 1,
                        Products = new List<Product>()
                        {
                            GetProduct("product2", 1, 1)
                        }
                    },
                    new ShopperHistory()
                    {
                        CustomerId = 2,
                        Products = new List<Product>()
                        {
                            GetProduct("product2", 1, 1),
                            GetProduct("product1", 1, 1)
                        }
                    }
                },
                // Expected Output 
                new List<Product>()
                {
                    GetProduct("product2", 1, 1),
                    GetProduct("product1", 1, 1),
                    GetProduct("product3", 1, 1)
                }
            };

            yield return new object[]
            {
                // Input
                new List<Product>()
                {
                    GetProduct("product1", 1, 1),
                    GetProduct("product2", 1, 1),
                    GetProduct("product3", 1, 1),
                    GetProduct("product4", 1, 1)
                },
                // ShopperHistoryList
                new List<ShopperHistory>()
                {
                    new ShopperHistory()
                    {
                        CustomerId = 1,
                        Products = new List<Product>()
                        {
                            GetProduct("product4", 1, 1),
                            GetProduct("product3", 1, 1)
                        }
                    },
                    new ShopperHistory()
                    {
                        CustomerId = 2,
                        Products = new List<Product>()
                        {
                            GetProduct("product4", 1, 1),
                            GetProduct("product3", 1, 1)
                        }
                    },
                    new ShopperHistory()
                    {
                        CustomerId = 3,
                        Products = new List<Product>()
                        {
                            GetProduct("product4", 1, 1),
                            GetProduct("product1", 1, 1)
                        }
                    },
                    new ShopperHistory()
                    {
                        CustomerId = 4,
                        Products = new List<Product>()
                    }
                },
                // Expected Output
                new List<Product>()
                {
                    GetProduct("product4", 1, 1),
                    GetProduct("product3", 1, 1),
                    GetProduct("product1", 1, 1),
                    GetProduct("product2", 1, 1),
                }
            };

            yield return new object[]
            {
                 // Input
                 new List<Product>(),
                 // ShopperHistoryList
                 new List<ShopperHistory>(),
                 // Expected Output
                 new List<Product>()
            };

            yield return new object[]
            {
                // Input
                null,
                // ShopperHistoryList
                null,
                // Expected Output
                null
            };
        }

        #endregion

        private static Product GetProduct(string name, float price, float quantity)
        {
            return new Product()
            {
                Name = name,
                Price = price,
                Quantity = quantity
            };
        }

        class SutProductRepositoryFactory
        {
            Mock<IHttpClientFactory> MockHttpClientFactory { get; set; } = new Mock<IHttpClientFactory>();
            MockHttpMessageHandler MockMessageHandler { get; set; } = new MockHttpMessageHandler();
            Mock<IExternalApiPathProvider> MockExternalApiPathProvider { get; set; } = new Mock<IExternalApiPathProvider>();
            Mock<IConfigProvider> MockConfigProvider { get; set; } = new Mock<IConfigProvider>();
            Mock<ISerializer> MockSerializer { get; set; } = new Mock<ISerializer>();

            public ProductRepository Create()
            {
                var httpClient = new HttpClient(MockMessageHandler);
                MockHttpClientFactory
                    .Setup(o => o.CreateClient(It.IsAny<string>()))
                    .Returns(httpClient);

                return new ProductRepository(MockHttpClientFactory.Object,
                                             MockExternalApiPathProvider.Object,
                                             MockConfigProvider.Object,
                                             MockSerializer.Object);
            }

            public void SetupGetProductsApiResult(List<Product> products)
            {
                var httpContent = JsonConvert.SerializeObject(products);
                var apiUrl = "http://apihost/api/resource/products";
                var token = "TestToken";

                MockSerializer
                    .Setup(o => o.Deserialize<List<Product>>(It.IsAny<string>()))
                    .Returns<string>(s => JsonConvert.DeserializeObject<List<Product>>(s));

                MockExternalApiPathProvider
                   .Setup(o => o.GetApiPath(ExternalApiPathName.GetProducts))
                   .Returns(apiUrl);

                MockConfigProvider
                    .Setup(o => o.GetConfigValue(ConfigKeys.Token))
                    .Returns(token);

                var apiRequestUrl = $"{apiUrl}?token={token}";

                MockMessageHandler
                    .When(HttpMethod.Get, apiRequestUrl)
                    .Respond("application/json", httpContent);
            }

            public void SetupGetGetShopperHistoryApiResult(List<ShopperHistory> shopperHistoryList)
            {
                var httpContent = JsonConvert.SerializeObject(shopperHistoryList);
                var apiUrl = "http://apihost/api/resource/shopperhistory";
                var token = "TestToken";

                MockSerializer
                    .Setup(o => o.Deserialize<List<ShopperHistory>>(It.IsAny<string>()))
                    .Returns<string>(s => JsonConvert.DeserializeObject<List<ShopperHistory>>(s));

                MockExternalApiPathProvider
                   .Setup(o => o.GetApiPath(ExternalApiPathName.GetShopperHistory))
                   .Returns(apiUrl);

                MockConfigProvider
                    .Setup(o => o.GetConfigValue(ConfigKeys.Token))
                    .Returns(token);

                var apiRequestUrl = $"{apiUrl}?token={token}";

                MockMessageHandler
                    .When(HttpMethod.Get, apiRequestUrl)
                    .Respond("application/json", httpContent);
            }
        }
    }
}
