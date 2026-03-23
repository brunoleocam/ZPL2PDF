using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for <see cref="LabelRenderer"/> (dimensions, render pipeline, font wiring).
    /// </summary>
    public class LabelRendererTests : IDisposable
    {
        private const string MinimalZpl = "^XA^FO10,10^A0N,30,30^FDHi^FS^XZ";

        private readonly string? _tempFontsDir;

        public LabelRendererTests()
        {
            _tempFontsDir = Path.Combine(Path.GetTempPath(), "ZPL2PDF_LabelRendererTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempFontsDir);
        }

        public void Dispose()
        {
            try
            {
                if (_tempFontsDir != null && Directory.Exists(_tempFontsDir))
                {
                    Directory.Delete(_tempFontsDir, true);
                }
            }
            catch
            {
                // Best-effort cleanup.
            }
        }

        private static void AssertValidPng(byte[] png)
        {
            png.Should().NotBeNull();
            png.Length.Should().BeGreaterThan(50);
            png.Take(8).Should().Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        }

        [Fact]
        public void RenderLabels_WhenLabelsNull_ThrowsArgumentNullException()
        {
            var sut = new LabelRenderer(80, 40, 203, "mm");

            Action act = () => sut.RenderLabels(null!);

            act.Should().Throw<ArgumentNullException>().WithParameterName("labels");
        }

        [Fact]
        public void RenderLabels_WhenEmpty_ReturnsEmptyList()
        {
            var sut = new LabelRenderer(80, 40, 203, "mm");

            var result = sut.RenderLabels(new List<string>());

            result.Should().NotBeNull().And.BeEmpty();
        }

        [Theory]
        [InlineData("mm", 80, 40)]
        [InlineData("cm", 8, 4)]
        [InlineData("in", 3.15, 1.57)]
        public void RenderLabels_WithSupportedUnits_ProducesPng(string unit, double w, double h)
        {
            var sut = new LabelRenderer(w, h, 203, unit);

            var result = sut.RenderLabels(new List<string> { MinimalZpl });

            result.Should().HaveCount(1);
            AssertValidPng(result[0]);
        }

        [Fact]
        public void RenderLabels_WithUnknownUnit_UsesFallbackDimensions_WithoutThrowing()
        {
            var sut = new LabelRenderer(10, 10, 203, "px");

            var result = sut.RenderLabels(new List<string> { MinimalZpl });

            result.Should().HaveCount(1);
            AssertValidPng(result[0]);
        }

        [Fact]
        public void Constructor_WithLabelDimensions_ProducesPng()
        {
            var dims = new LabelDimensions
            {
                WidthMm = 80,
                HeightMm = 40,
                Dpi = 203
            };

            var sut = new LabelRenderer(dims);

            var result = sut.RenderLabels(new List<string> { MinimalZpl });

            result.Should().HaveCount(1);
            AssertValidPng(result[0]);
        }

        [Fact]
        public void RenderLabels_WithFontsDirectoryOnly_DoesNotThrow()
        {
            var sut = new LabelRenderer(80, 40, 203, "mm", fontsDirectory: _tempFontsDir);

            var result = sut.RenderLabels(new List<string> { MinimalZpl });

            result.Should().HaveCount(1);
            AssertValidPng(result[0]);
        }

        /// <summary>
        /// Exercises <see cref="LabelRenderer"/> font mapping when a known system font file exists (optional per machine/CI image).
        /// </summary>
        [Fact]
        public void RenderLabels_WithFontMapping_WhenSystemFontExists_ProducesPng()
        {
            var systemTtf = TryGetExistingSystemSansFont();
            if (systemTtf == null)
            {
                return;
            }

            var fontsDir = Path.GetDirectoryName(systemTtf)!;
            var mappings = new[] { ("A", systemTtf) };

            var sut = new LabelRenderer(80, 40, 203, "mm", fontsDir, mappings);

            var zpl = "^XA^FO10,10^A0N,30,30^FDA^FS^XZ";
            var result = sut.RenderLabels(new List<string> { zpl });

            result.Should().HaveCount(1);
            AssertValidPng(result[0]);
        }

        private static string? TryGetExistingSystemSansFont()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var dir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                var arial = Path.Combine(dir, "arial.ttf");
                if (File.Exists(arial))
                {
                    return arial;
                }
            }
            else
            {
                var candidates = new[]
                {
                    "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
                    "/usr/share/fonts/TTF/DejaVuSans.ttf"
                };

                foreach (var c in candidates)
                {
                    if (File.Exists(c))
                    {
                        return c;
                    }
                }
            }

            return null;
        }
    }
}
