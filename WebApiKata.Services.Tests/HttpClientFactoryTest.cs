using FluentAssertions;
using System.Net.Http;
using Xunit;

namespace WebApiKata.Services.Tests
{
    public class HttpClientFactoryTest
    {
        [Fact]
        public void CreateClient_ReturnsHttpClient()
        {
            // Arrange
            var sutHttpClientFactory = new HttpClientFactory();
            // Act
            var result = sutHttpClientFactory.CreateClient("");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<HttpClient>();
        }

        [Fact]
        public void CreateClient_IfCalledMultipleTimes_ReturnsDifferentHttpClientInstances()
        {
            // Arrange
            var sutHttpClientFactory = new HttpClientFactory();
            // Act
            var result1 = sutHttpClientFactory.CreateClient("");
            var result2 = sutHttpClientFactory.CreateClient("");

            // Assert
            result1.Should().NotBeSameAs(result2);
        }
    }
}
