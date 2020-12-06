using WebApiKata.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace WebApiKata.Services.Tests
{
    public class ExternalApiPathProviderTest
    {
        [Theory]
        [InlineData("http://baseurl.com/", ExternalApiPathName.GetShopperHistory, "http://baseurl.com/api/resource/shopperhistory")]
        [InlineData("https://apihost", ExternalApiPathName.GetShopperHistory, "https://apihost/api/resource/shopperhistory")]
        [InlineData("https://apihost", ExternalApiPathName.GetProducts, "https://apihost/api/resource/products")]
        [InlineData("https://apihost.com", ExternalApiPathName.CalculateTrolley, "https://apihost.com/api/resource/trolleycalculator")]
        public void GetApiPath_WhenApiPathNameIsValid_ReturnsCorrectUrl(string baseUrl, ExternalApiPathName apiPathName, string expectedUrl)
        {
            // Arrange
            var mockConfigProvider = new Mock<IConfigProvider>();
            mockConfigProvider
                .Setup(o => o.GetConfigValue(ConfigKeys.BaseUrl))
                .Returns(baseUrl);

            var sutExternalApiPathProvider = new ExternalApiPathProvider(mockConfigProvider.Object);

            // Act
            var result = sutExternalApiPathProvider.GetApiPath(apiPathName);

            // Assert
            result.Should().Be(expectedUrl);
        }
    }
}
