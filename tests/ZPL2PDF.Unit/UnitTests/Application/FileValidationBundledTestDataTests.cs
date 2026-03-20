using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF.Application.Services;

namespace ZPL2PDF.Tests.UnitTests.Application
{
    /// <summary>
    /// Validates <see cref="FileValidationService"/> against files under <c>TestData/TestFiles</c> (copied to output).
    /// </summary>
    public class FileValidationBundledTestDataTests
    {
        private static string TestFilesDirectory =>
            Path.Combine(AppContext.BaseDirectory, "TestData", "TestFiles");

        private readonly FileValidationService _sut = new();

        [Fact]
        public void BundledTestDataDirectory_ShouldContainExpectedFiles()
        {
            Directory.Exists(TestFilesDirectory).Should().BeTrue(
                "TestData/TestFiles must be copied to output (see ZPL2PDF.Unit.csproj).");

            File.Exists(Path.Combine(TestFilesDirectory, "valid.txt")).Should().BeTrue();
            File.Exists(Path.Combine(TestFilesDirectory, "valid.prn")).Should().BeTrue();
            File.Exists(Path.Combine(TestFilesDirectory, "empty.txt")).Should().BeTrue();
            File.Exists(Path.Combine(TestFilesDirectory, "invalid.doc")).Should().BeTrue();
        }

        [Theory]
        [InlineData("valid.txt", true)]
        [InlineData("valid.prn", true)]
        [InlineData("empty.txt", true)]
        [InlineData("invalid.doc", false)]
        public void IsValidFile_ForBundledSample_MatchesExpected(string fileName, bool expectedValid)
        {
            var path = Path.Combine(TestFilesDirectory, fileName);
            File.Exists(path).Should().BeTrue($"Missing bundled file: {fileName}");

            _sut.IsValidFile(path).Should().Be(expectedValid);
        }

        [Theory]
        [InlineData("valid.txt", true)]
        [InlineData("valid.prn", true)]
        [InlineData("empty.txt", true)]
        [InlineData("invalid.doc", false)]
        public void IsValidExtension_ForBundledSample_MatchesExpected(string fileName, bool expectedValid)
        {
            var path = Path.Combine(TestFilesDirectory, fileName);

            _sut.IsValidExtension(path).Should().Be(expectedValid);
        }
    }
}
