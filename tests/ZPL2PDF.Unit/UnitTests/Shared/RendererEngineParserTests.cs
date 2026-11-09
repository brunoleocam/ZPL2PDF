using FluentAssertions;
using Xunit;
using ZPL2PDF;
using ZPL2PDF.Shared.Constants;

namespace ZPL2PDF.Tests.UnitTests.Shared
{
    public class RendererEngineParserTests
    {
        [Theory]
        [InlineData("offline", RendererEngine.Offline)]
        [InlineData(" Labelary ", RendererEngine.Labelary)]
        [InlineData("AUTO", RendererEngine.Auto)]
        public void TryParse_WithValidValue_ReturnsEngine(string value, RendererEngine expected)
        {
            RendererEngineParser.TryParse(value, out var engine).Should().BeTrue();
            engine.Should().Be(expected);
        }

        [Theory]
        [InlineData("labelry")]
        [InlineData("")]
        [InlineData(null)]
        public void TryParse_WithInvalidValue_ReturnsFalse(string? value)
        {
            RendererEngineParser.TryParse(value, out _).Should().BeFalse();
        }

        [Theory]
        [InlineData(203, RendererEngine.Offline, 8)]
        [InlineData(300, RendererEngine.Offline, 12)]
        [InlineData(400, RendererEngine.Offline, 16)]  // offline supports 16 dpmm
        [InlineData(400, RendererEngine.Labelary, 12)] // Labelary only supports 6/8/12/24
        [InlineData(100, RendererEngine.Labelary, 6)]
        [InlineData(400, RendererEngine.Auto, 12)]     // auto follows Labelary
        public void ToDpmm_MapsPerEngine(int dpi, RendererEngine engine, int expectedDpmm)
        {
            RendererEngineParser.ToDpmm(dpi, engine).Should().Be(expectedDpmm);
        }

        [Fact]
        public void ToEffectiveDpi_RoundTripsThroughOfflineDpmm()
        {
            foreach (var engine in new[] { RendererEngine.Offline, RendererEngine.Labelary, RendererEngine.Auto })
            {
                var effective = RendererEngineParser.ToEffectiveDpi(203, engine);
                RendererEngineParser.ToDpmm(effective, RendererEngine.Offline).Should().Be(8);
            }
        }

        [Fact]
        public void ParseConversionMode_WithInvalidRenderer_FlagsInvalidRenderer()
        {
            var parser = new ArgumentParser();
            var result = parser.ParseConversionMode(new[] { "-z", "^XA^XZ", "-o", "/tmp", "--renderer", "labelry" }, 0);
            result.InvalidRenderer.Should().Be("labelry");
            result.RendererEngine.Should().Be(RendererEngine.Offline);
        }

        [Fact]
        public void ProcessArguments_WithInvalidRenderer_FailsWithExitCode1()
        {
            var processor = new ArgumentProcessor();
            processor.ProcessArguments(new[] { "-z", "^XA^XZ", "-o", "/tmp", "--renderer", "labelry" });
            processor.ExitCode.Should().Be(1);
            processor.Mode.Should().Be(OperationMode.Help);
        }
    }
}
