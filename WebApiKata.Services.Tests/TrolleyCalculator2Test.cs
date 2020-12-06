using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiKata.Models;
using Xunit;

namespace WebApiKata.Services.Tests
{
    public class TrolleyCalculator2Test
    {
        [Theory]
        [MemberData(nameof(GetTrolleyInfoAndExpectedTotals))]
        public async void CalculateTrolley_WhenTrolleyInfoIsValid_ReturnsCorrectMinimumTotal(TrolleyInfo trolleyInfo, decimal expectedTotal)
        {
            // Arrange
            var sutTrolleyCalculator = new TrolleyCalculator2();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(trolleyInfo);

            // Assert
            result.Should().Be(expectedTotal);
        }

        [Fact]
        public async void CalculateTrolley_WhenTrolleyInfoIsNull_ReturnsZero()
        {
            // Arrange
            var sutTrolleyCalculator = new TrolleyCalculator2();

            // Act
            var result = await sutTrolleyCalculator.CalculateTrolley(null);

            // Assert
            result.Should().Be(0);
        }

        [Theory]
        [MemberData(nameof(GetTrolleyInfoWithNullProperty))]
        public void CalculateTrolley_WhenTrolleyInfoPropertyIsNull_ThrowArgumentException(TrolleyInfo trolleyInfo)
        {
            // Arrange
            var sutTrolleyCalculator = new TrolleyCalculator2();

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

        public static IEnumerable<object[]> GetTrolleyInfoAndExpectedTotals()
        {
            var productA = "Product A";
            yield return new object[]
            {
                // Input trolleyInfo
                new TrolleyInfo()
                {
                    Products = new List<TrolleyProduct>(){ new TrolleyProduct() { Name = productA, Price = 6 } },
                    Quantities = new List<TrolleyQuantity>() { new TrolleyQuantity() { Name = productA, Quantity = 50 } },
                    Specials = new List<TrolleySpecial>()
                    {
                        new TrolleySpecial()
                        {
                            Quantities = new List<TrolleyQuantity>()
                            {
                                new TrolleyQuantity()
                                {
                                    Name = productA,
                                    Quantity = 5
                                }
                            },
                            Total = 20
                        }
                    }
                },
                // Expected total
                200
            };
            yield return new object[]
            {
                // Input trolleyInfo
                new TrolleyInfo()
                {
                    Products = new List<TrolleyProduct>(){ new TrolleyProduct() { Name = productA, Price = 6 } },
                    Quantities = new List<TrolleyQuantity>() { new TrolleyQuantity() { Name = productA, Quantity = 4 } },
                    Specials = new List<TrolleySpecial>()
                    {
                        new TrolleySpecial()
                        {
                            Quantities = new List<TrolleyQuantity>()
                            {
                                new TrolleyQuantity()
                                {
                                    Name = productA,
                                    Quantity = 5
                                }
                            },
                            Total = 20
                        }
                    }
                },
                // Expected total
                24
            };

            yield return new object[]
            {
                 // Input trolleyInfo
                 new TrolleyInfo()
                 {
                     Products = new List<TrolleyProduct>(){ new TrolleyProduct() { Name = productA, Price = 0.1234567890123m } },
                     Quantities = new List<TrolleyQuantity>() { new TrolleyQuantity() { Name = productA, Quantity = 8 } },
                     Specials = new List<TrolleySpecial>()
                     {
                         new TrolleySpecial()
                         {
                             Quantities = new List<TrolleyQuantity>()
                             {
                                 new TrolleyQuantity()
                                 {
                                     Name = productA,
                                     Quantity = 6
                                 }
                             },
                             Total =  0.555666777999888m
                         }
                     }
                 },
                 // Expected total
                 0.802580356024488m
            };
        }
    }
}
