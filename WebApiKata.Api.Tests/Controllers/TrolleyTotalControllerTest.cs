using AutoMapper;
using WebApiKata.Interfaces;
using WebApiKata.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using WebApiKata.Api.Controllers;
using WebApiKata.Api.Exceptions;
using WebApiKata.Api.ResourceModels;
using Xunit;

namespace WebApiKata.Tests.Controllers
{
    public class TrolleyTotalControllerTest
    {
        [Fact]
        public async void CalculateTrolleyTotal_WhenInputIsValid_ReturnsCorrectResultFromService()
        {
            // Arrange
            var sutFactory = new SutTrolleyTotalControllerFactory();
            var valueReturnedFromService = 12.3m;
            var trolleyInfoModel = new TrolleyInfoModel();
            var trolleyInfo = new TrolleyInfo();

            sutFactory.MockMapper
                      .Setup(o => o.Map<TrolleyInfoModel, TrolleyInfo>(trolleyInfoModel))
                      .Returns(trolleyInfo);

            sutFactory.MockTrolleyCalculator
                      .Setup(o => o.CalculateTrolley(trolleyInfo))
                      .Returns(Task.FromResult(valueReturnedFromService));

            var sutTrolleyTotalController = sutFactory.Create();

            // Act
            var result = await sutTrolleyTotalController.CalculateTrolleyTotal(trolleyInfoModel);

            // Assert
            result.Should().Be(valueReturnedFromService);
        }

        [Fact]
        public void CalculateTrolleyTotal_WhenInputIsNull_ThrowsBadRequestException()
        {
            // Arrange
            var sutFactory = new SutTrolleyTotalControllerFactory();
            TrolleyInfoModel trolleyInfoModel = null;

            var sutTrolleyTotalController = sutFactory.Create();

            // Act
            Func<Task> action= async () => await sutTrolleyTotalController.CalculateTrolleyTotal(trolleyInfoModel);

            // Assert
            action.Should()
                  .ThrowExactly<BadApiRequestException>()
                  .WithMessage("trolleyInfoModel is required.");

        }

        class SutTrolleyTotalControllerFactory
        {
            public Mock<ILogger<TrolleyTotalController>> MockLogger { get; set; } = new Mock<ILogger<TrolleyTotalController>>();
            public Mock<IMapper> MockMapper { get; set; } = new Mock<IMapper>();
            public Mock<ITrolleyCalculator> MockTrolleyCalculator { get; set; } = new Mock<ITrolleyCalculator>();

            public TrolleyTotalController Create()
            {
                return new TrolleyTotalController(MockLogger.Object, MockMapper.Object, MockTrolleyCalculator.Object);
            }

        }
    }
}
