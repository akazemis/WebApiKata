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
    public class TrolleyRepositoryTest
    {
        [Fact]
        public async void CalculateTrolley_WhenTrolleyInfoIsValid_ReturnsTotalReceivedFromTheExternalApi()
        {
            // Arrange
            var trolleyInfo = new TrolleyInfo();
            double expectedResult = 10.2;
            var sutFactory = new SutTrolleyRepositoryFactory();
            sutFactory.SetupCalculatorTrolleyApiResult(trolleyInfo, expectedResult);

            var sutTrolleyCalculator = sutFactory.Create();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(trolleyInfo);

            // Assert
            result.Should().Be(expectedResult);
        }

        [Fact]
        public async void CalculateTrolley_WhenTrolleyInfoIsNull_ReturnsZero()
        {
            // Arrange
            var sutFactory = new SutTrolleyRepositoryFactory();
            var sutTrolleyCalculator = sutFactory.Create();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(null);

            // Assert
            result.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(GetTrolleyInfoWithNullProperty))]
        public async void CalculateTrolley_WhenTrolleyInfoPropertyIsNull_ReturnsZero(TrolleyInfo trolleyInfo)
        {
            // Arrange
            var sutFactory = new SutTrolleyRepositoryFactory();
            var sutTrolleyCalculator = sutFactory.Create();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(trolleyInfo);

            // Assert
            result.Should().Be(0);
        }

        public static IEnumerable<object[]> GetTrolleyInfoWithNullProperty()
        {
            yield return new object[]
            {
                new TrolleyInfo(){ Products = null, Quantities = new List<TrolleyInfo.TrolleyProduct>(), Specials = new List<TrolleyInfo.TrolleySpecial>() }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = new List<TrolleyInfo.TrolleyProduct>(), Quantities = null, Specials = new List<TrolleyInfo.TrolleySpecial>() }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = new List<TrolleyInfo.TrolleyProduct>(), Quantities = new List<TrolleyInfo.TrolleyProduct>(), Specials = null }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = null, Quantities = null, Specials = null }
            };
        }

        // TODO: add more tests for the edge cases

        class SutTrolleyRepositoryFactory
        {
            Mock<IHttpClientFactory> MockHttpClientFactory { get; set; } = new Mock<IHttpClientFactory>();
            MockHttpMessageHandler MockMessageHandler { get; set; } = new MockHttpMessageHandler();
            Mock<IExternalApiPathProvider> MockExternalApiPathProvider { get; set; } = new Mock<IExternalApiPathProvider>();
            Mock<IConfigProvider> MockConfigProvider { get; set; } = new Mock<IConfigProvider>();
            Mock<ISerializer> MockSerializer { get; set; } = new Mock<ISerializer>();

            public TrolleyRepository Create()
            {
                var httpClient = new HttpClient(MockMessageHandler);
                MockHttpClientFactory
                    .Setup(o => o.CreateClient(It.IsAny<string>()))
                    .Returns(httpClient);

                return new TrolleyRepository(MockHttpClientFactory.Object,
                                             MockExternalApiPathProvider.Object,
                                             MockConfigProvider.Object,
                                             MockSerializer.Object);
            }

            public void SetupCalculatorTrolleyApiResult(TrolleyInfo trolleyInfo, double calculatedTotal)
            {
                var responseHttpContent = JsonConvert.SerializeObject(calculatedTotal);
                var apiUrl = "http://apihost/api/resource/trolleycalulator";
                var token = "TestToken";

                MockSerializer
                    .Setup(o => o.Serialize(trolleyInfo))
                    .Returns<TrolleyInfo>(t => JsonConvert.SerializeObject(t));

                MockExternalApiPathProvider
                   .Setup(o => o.GetApiPath(ExternalApiPathName.CalculateTrolley))
                   .Returns(apiUrl);

                MockConfigProvider
                    .Setup(o => o.GetConfigValue(ConfigKeys.Token))
                    .Returns(token);

                var apiRequestUrl = $"{apiUrl}?token={token}";

                MockMessageHandler
                    .When(HttpMethod.Post, apiRequestUrl)
                    .WithContent(JsonConvert.SerializeObject(trolleyInfo))
                    .WithHeaders("Content-Type", "application/json")
                    .Respond("application/json", responseHttpContent);
            }
        }
    }
}
