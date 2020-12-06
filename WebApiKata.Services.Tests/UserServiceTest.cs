using WebApiKata.Interfaces;
using WebApiKata.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace WebApiKata.Services.Tests
{
    public class UserServiceTest
    {
        [Fact]
        public async void GetUser_ReturnsCorrectUser()
        {
            // Arrange
            var token = "UserTokenFromConfig";

            var mockConfigProvider = new Mock<IConfigProvider>();
            mockConfigProvider
                .Setup(o => o.GetConfigValue(ConfigKeys.Token))
                .Returns(token);

            var sutUserService = new UserService(mockConfigProvider.Object);

            // Act
            var user = await sutUserService.GetUser();

            // Assert
            user.Should().BeEquivalentTo(new User() { Name = "Arvin Kardon", Token = token });
        }
    }
}
