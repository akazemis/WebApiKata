using WebApiKata.Interfaces;
using WebApiKata.Models;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;
using System;
using System.Threading.Tasks;

namespace WebApiKata.Services.Tests
{
    public class TrolleyCalculatorTest
    {
        [Fact]
        public async void CalculateTrolley_WhenTrolleyInfoIsValid_ReturnsTotalReceivedFromTheExternalApi()
        {
            // Arrange
            var trolleyInfo = new TrolleyInfo();
            decimal expectedResult = 10.2m;
            var sutFactory = new SutTrolleyCalculatorFactory();
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
            var sutFactory = new SutTrolleyCalculatorFactory();
            var sutTrolleyCalculator = sutFactory.Create();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(null);

            // Assert
            result.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(GetTrolleyInfoWithNullProperty))]
        public void CalculateTrolley_WhenTrolleyInfoPropertyIsNull_ReturnsZero(TrolleyInfo trolleyInfo)
        {
            // Arrange
            var sutFactory = new SutTrolleyCalculatorFactory();
            var sutTrolleyCalculator = sutFactory.Create();

            // Act
            Func<Task> action = async () => await sutTrolleyCalculator.CalculateTrolley(trolleyInfo);

            // Assert
            action.Should().ThrowExactlyAsync<ArgumentException>()
                  .WithMessage("Products, Quantities and Specials properties cannot be null.");
        }

        public static IEnumerable<object[]> GetTrolleyInfoWithNullProperty()
        {
            yield return new object[]
            {
                new TrolleyInfo(){ Products = null, Quantities = new List<TrolleyQuantity>(), Specials = new List<TrolleySpecial>() }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = new List<TrolleyProduct>(), Quantities = null, Specials = new List<TrolleySpecial>() }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = new List<TrolleyProduct>(), Quantities = new List<TrolleyQuantity>(), Specials = null }
            };
            yield return new object[]
            {
                new TrolleyInfo(){ Products = null, Quantities = null, Specials = null }
            };
        }

        // TODO: add more tests for the edge cases

        class SutTrolleyCalculatorFactory
        {
            Mock<IHttpClientFactory> MockHttpClientFactory { get; set; } = new Mock<IHttpClientFactory>();
            MockHttpMessageHandler MockMessageHandler { get; set; } = new MockHttpMessageHandler();
            Mock<IExternalApiPathProvider> MockExternalApiPathProvider { get; set; } = new Mock<IExternalApiPathProvider>();
            Mock<IConfigProvider> MockConfigProvider { get; set; } = new Mock<IConfigProvider>();
            Mock<ISerializer> MockSerializer { get; set; } = new Mock<ISerializer>();

            public TrolleyCalculator Create()
            {
                var httpClient = new HttpClient(MockMessageHandler);
                MockHttpClientFactory
                    .Setup(o => o.CreateClient(It.IsAny<string>()))
                    .Returns(httpClient);

                return new TrolleyCalculator(MockHttpClientFactory.Object,
                                             MockExternalApiPathProvider.Object,
                                             MockConfigProvider.Object,
                                             MockSerializer.Object);
            }

            public void SetupCalculatorTrolleyApiResult(TrolleyInfo trolleyInfo, decimal calculatedTotal)
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
                var jsonMimeType = "application/json";

                MockMessageHandler
                    .When(HttpMethod.Post, apiRequestUrl)
                    .WithContent(JsonConvert.SerializeObject(trolleyInfo))
                    .WithHeaders("Content-Type", jsonMimeType)
                    .Respond(jsonMimeType, responseHttpContent);
            }
        }
    }
}
