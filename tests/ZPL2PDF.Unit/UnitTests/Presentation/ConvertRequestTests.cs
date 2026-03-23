using System.Text.Json;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Presentation.Api.Models;

namespace ZPL2PDF.Tests.UnitTests.Presentation
{
    public class ConvertRequestTests
    {
        private static ConvertRequest Deserialize(string json)
        {
            return JsonSerializer.Deserialize<ConvertRequest>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }

        [Fact]
        public void Deserialize_WhenOnlyZplProvided_UsesDefaults()
        {
            // Arrange
            var json = """
            {
              "zpl": "^XA^FO10,10^A0N,30,30^FDHi^FS^XZ"
            }
            """;

            // Act
            var request = Deserialize(json);

            // Assert
            request.Format.Should().Be("pdf");
            request.Renderer.Should().Be("offline");
            request.Unit.Should().Be("mm");
            request.Dpi.Should().BeNull();
        }

        [Fact]
        public void Deserialize_WhenRendererProvided_SetsRenderer()
        {
            // Arrange
            var json = """
            {
              "zpl": "^XA^FO10,10^A0N,30,30^FDHi^FS^XZ",
              "format": "pdf",
              "renderer": "labelary"
            }
            """;

            // Act
            var request = Deserialize(json);

            // Assert
            request.Format.Should().Be("pdf");
            request.Renderer.Should().Be("labelary");
        }
    }
}

