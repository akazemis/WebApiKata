using AutoMapper;
using WebApiKata.Interfaces;
using WebApiKata.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiKata.Api.Controllers;
using WebApiKata.Api.Exceptions;
using WebApiKata.Api.ResourceModels;
using Xunit;

namespace WebApiKata.Tests.Controllers
{
    public class SortControllerTest
    {
        [Theory]
        [InlineData(SortType.Ascending)]
        [InlineData(SortType.Descending)]
        [InlineData(SortType.Low)]
        [InlineData(SortType.High)]
        [InlineData(SortType.Recommended)]
        public async void GetSortedShopperHistory_WhenSortOptionsIsValid_ReturnResultFromService(SortType sortOption)
        {
            // Arrange
            var sutFactory = new SutSortControllerFactory();
            var productList = new List<Product>() { new Product() };
            var productModelList = new List<ProductModel>() { new ProductModel() };
            sutFactory
                .MockMapper
                .Setup(o => o.Map<List<Product>, List<ProductModel>>(productList))
                .Returns(productModelList);
            sutFactory
                .MockProductService
                .Setup(o => o.GetSortedProducts(sortOption))
                .ReturnsAsync(productList);

            var sutSortController = sutFactory.Create();

            // Act
            var result = await sutSortController.GetSortedShopperHistory(sortOption.ToString());

            // Assert
            result.Should().BeEquivalentTo(productModelList);
        }

        [Fact]
        public void GetSortedShopperHistory_WhenSortOptionsIsNull_ThrowsBadApiRequestException()
        {
            // Arrange
            var sutFactory = new SutSortControllerFactory();
            var sutSortController = sutFactory.Create();

            // Act
            Func<Task> action = async () => await sutSortController.GetSortedShopperHistory(null);

            // Assert
            action.Should().ThrowExactlyAsync<BadApiRequestException>()
                  .WithMessage("SortOption query parameter is required.");
        }

        [Theory]
        [InlineData("InvalidSortOption")]
        [InlineData("")]
        [InlineData(" ")]
        public void GetSortedShopperHistory_WhenSortOptionsInvalid_ThrowsBadApiRequestException(string sortOption)
        {
            // Arrange
            var sutFactory = new SutSortControllerFactory();
            var sutSortController = sutFactory.Create();

            // Act
            Func<Task> action = async () => await sutSortController.GetSortedShopperHistory(sortOption);

            // Assert
            action.Should().ThrowExactlyAsync<BadApiRequestException>()
                  .WithMessage($"SortOption '{sortOption}' is not supported.");

        }

        class SutSortControllerFactory
        {
            Mock<ILogger<SortController>> MockLogger { get; set; } = new Mock<ILogger<SortController>>();
            public Mock<IMapper> MockMapper { get; set; } = new Mock<IMapper>();
            public Mock<IProductService> MockProductService { get; set; } = new Mock<IProductService>();

            public SortController Create()
            {
                return new SortController(MockLogger.Object, MockMapper.Object, MockProductService.Object);
            }
        }
    }
}
