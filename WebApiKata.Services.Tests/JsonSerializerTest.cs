using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace WebApiKata.Services.Tests
{
    public class JsonSerializerTest
    {
        [Fact]
        public void Serialize_WhenObjectIsNull_ReturnsNull()
        {
            // Arrange
            var sutJsonSerializer = new JsonSerializer();

            // Act
            SampleType obj = null;
            var result = sutJsonSerializer.Serialize(obj);

            // Assert
            result.Should().Be("null");
        }

        [Fact]
        public void Serialize_WhenObjectIsValid_ReturnsJsonSerializedInCamelCase()
        {
            // Arrange
            var sutJsonSerializer = new JsonSerializer();

            // Act
            var obj = new SampleType() { Id = 1, FirstName = "Test01" };
            var result = sutJsonSerializer.Serialize(obj);

            // Assert
            result.Should().Be("{\"id\":1,\"firstName\":\"Test01\"}");
        }

        [Fact]
        public void DeSerialize_WhenInputIsNullReference_ThrowsArgumentNullException()
        {
            // Arrange
            var sutJsonSerializer = new JsonSerializer();

            // Act
            string json = null;
            Action action= () => sutJsonSerializer.Deserialize<SampleType>(json);

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void DeSerialize_WhenInputIsNullString_ReturnsNull()
        {
            // Arrange
            var sutJsonSerializer = new JsonSerializer();

            // Act
            var json = "null";
            var result = sutJsonSerializer.Deserialize<SampleType>(json);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void DeSerialize_WhenInputIsValid_ReturnsCorrectObject()
        {
            // Arrange
            var sutJsonSerializer = new JsonSerializer();
            var expectedObject = new SampleType() { Id = 1, FirstName = "Test01" };
            var json = sutJsonSerializer.Serialize(expectedObject);

            // Act
            var result = sutJsonSerializer.Deserialize<SampleType>(json);

            // Assert
            result.Should().BeEquivalentTo(expectedObject);
        }

        // TODO: more tests needed here to cover more possible scenarios

        class SampleType
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
        }
    }
}
