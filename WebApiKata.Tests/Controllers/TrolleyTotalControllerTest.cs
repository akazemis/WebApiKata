using AutoMapper;
using DataAccess.Interfaces;
using DataAccess.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using WebApiKata.Controllers;
using WebApiKata.Exceptions;
using WebApiKata.ResourceModels;
using Xunit;

namespace WebApiKata.Tests.Controllers
{
    public class TrolleyTotalControllerTest
    {
        [Fact]
        public async void CalculateTrolleyTotal_WhenInputIsValid_ReturnsCorrectResultFromRepository()
        {
            // Arrange
            var sutFactory = new SutTrolleyTotalControllerFactory();
            var valueReturnedFromRepository = 12.3;
            var trolleyInfoModel = new TrolleyInfoModel();
            var trolleyInfo = new TrolleyInfo();

            sutFactory.MockMapper
                      .Setup(o => o.Map<TrolleyInfoModel, TrolleyInfo>(trolleyInfoModel))
                      .Returns(trolleyInfo);

            sutFactory.MockTrolleyRepository
                      .Setup(o => o.CalculateTrolley(trolleyInfo))
                      .Returns(Task.FromResult(valueReturnedFromRepository));

            var sutTrolleyTotalController = sutFactory.Create();

            // Act
            var result = await sutTrolleyTotalController.CalculateTrolleyTotal(trolleyInfoModel);

            // Assert
            result.Should().Be(valueReturnedFromRepository);
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
            public Mock<ITrolleyRepository> MockTrolleyRepository { get; set; } = new Mock<ITrolleyRepository>();

            public TrolleyTotalController Create()
            {
                return new TrolleyTotalController(MockLogger.Object, MockMapper.Object, MockTrolleyRepository.Object);
            }

        }
    }
}
