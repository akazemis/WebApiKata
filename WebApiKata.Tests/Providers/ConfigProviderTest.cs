using DataAccess.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using WebApiKata.Config;
using WebApiKata.Providers;
using Xunit;

namespace WebApiKata.Tests.Providers
{
    public class ConfigProviderTest
    {
        const string TokenValue = "TestToken";
        const string BaseUrlValue = "TestBaseUrl";

        [Theory]
        [InlineData(ConfigKeys.BaseUrl, BaseUrlValue)]
        [InlineData(ConfigKeys.Token, TokenValue)]
        public void GetConfigValue_WhenConfigKeyIsValid_ReturnsThePertinentConfigValue(string configKey, string expectedConfigValue)
        {
            // Arrange
            var mockOptions = new Mock<IOptions<ExternalApiSettings>>();
            mockOptions
                .Setup(o => o.Value)
                .Returns(new ExternalApiSettings() { Token = TokenValue, BaseUrl = BaseUrlValue });

            var sutConfigProvider = new ConfigProvider(mockOptions.Object);
            // Act
            var result = sutConfigProvider.GetConfigValue(configKey);

            // Assert
            result.Should().Be(expectedConfigValue);
        }

        [Fact]
        public void GetConfigValue_WhenConfigKeyIsInvalid_ThrowsProperArgumentException()
        {
            // Arrange
            var configKey = "InvalidKey";
            var mockOptions = new Mock<IOptions<ExternalApiSettings>>();
            mockOptions
                .Setup(o => o.Value)
                .Returns(new ExternalApiSettings() { Token = TokenValue, BaseUrl = BaseUrlValue });

            var sutConfigProvider = new ConfigProvider(mockOptions.Object);
            // Act
            Action action = () => sutConfigProvider.GetConfigValue(configKey);

            // Assert
            var expectedErrorMessage = $"ConfigKey '{configKey}' is not valid.";
            action.Should()
                  .ThrowExactly<ArgumentException>()
                  .WithMessage(expectedErrorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("       ")]
        public void GetConfigValue_WhenConfigKeyIsNullOrEmptyOrWhiteSpace_ThrowsProperArgumentException(string configKey)
        {
            // Arrange
            var mockOptions = new Mock<IOptions<ExternalApiSettings>>();
            mockOptions
                .Setup(o => o.Value)
                .Returns(new ExternalApiSettings() { Token = TokenValue, BaseUrl = BaseUrlValue });

            var sutConfigProvider = new ConfigProvider(mockOptions.Object);
            // Act
            Action action = () => sutConfigProvider.GetConfigValue(configKey);

            // Assert
            var expectedErrorMessage = "ConfigKey cannot be null or empty or only whitespace.";
            action.Should()
                  .ThrowExactly<ArgumentException>()
                  .WithMessage(expectedErrorMessage);
        }
    }
}
