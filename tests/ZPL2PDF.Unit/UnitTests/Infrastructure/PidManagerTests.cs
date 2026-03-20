using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for <see cref="PidManager"/> using <c>ZPL2PDF_PID_FOLDER</c> for isolation.
    /// </summary>
    public class PidManagerTests : IDisposable
    {
        private readonly string _pidRoot;
        private readonly string? _previousPidFolderEnv;

        public PidManagerTests()
        {
            _pidRoot = Path.Combine(Path.GetTempPath(), "ZPL2PDF_PidManagerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_pidRoot);

            _previousPidFolderEnv = Environment.GetEnvironmentVariable("ZPL2PDF_PID_FOLDER");
            Environment.SetEnvironmentVariable("ZPL2PDF_PID_FOLDER", _pidRoot);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("ZPL2PDF_PID_FOLDER", _previousPidFolderEnv);

            try
            {
                if (Directory.Exists(_pidRoot))
                {
                    Directory.Delete(_pidRoot, true);
                }
            }
            catch
            {
                // Best-effort cleanup.
            }
        }

        [Fact]
        public void Constructor_WithCustomFileName_ThrowsArgumentNullException_WhenNameInvalid()
        {
            Action act1 = () => _ = new PidManager(null!);
            Action act2 = () => _ = new PidManager("");
            Action act3 = () => _ = new PidManager("   ");

            act1.Should().Throw<ArgumentNullException>().WithParameterName("pidFileName");
            act2.Should().Throw<ArgumentNullException>().WithParameterName("pidFileName");
            act3.Should().Throw<ArgumentNullException>().WithParameterName("pidFileName");
        }

        [Fact]
        public void SavePidToFile_ThenGetPidFromFile_ReturnsSameValue()
        {
            var sut = new PidManager();
            const int expected = 424242;

            var saved = sut.SavePidToFile(expected);
            saved.Should().BeTrue();

            sut.GetPidFromFile().Should().Be(expected);
            sut.PidFileExists().Should().BeTrue();
        }

        [Fact]
        public void SavePidToFile_WithNestedRelativeFileName_CreatesParentDirectory()
        {
            var sut = new PidManager(Path.Combine("nested", "app.pid"));

            sut.SavePidToFile(99).Should().BeTrue();

            sut.GetPidFromFile().Should().Be(99);
            File.Exists(Path.Combine(_pidRoot, "nested", "app.pid")).Should().BeTrue();
        }

        [Fact]
        public void GetPidFromFile_WhenFileMissing_ReturnsZero()
        {
            var sut = new PidManager("missing.pid");

            sut.GetPidFromFile().Should().Be(0);
        }

        [Fact]
        public void GetPidFromFile_WhenContentNotInteger_ReturnsZero()
        {
            var sut = new PidManager();
            File.WriteAllText(Path.Combine(_pidRoot, "zpl2pdf.pid"), "not-a-number");

            sut.GetPidFromFile().Should().Be(0);
        }

        [Fact]
        public void GetPidFromFile_TrimsWhitespace()
        {
            var sut = new PidManager();
            File.WriteAllText(Path.Combine(_pidRoot, "zpl2pdf.pid"), "  12345  \n");

            sut.GetPidFromFile().Should().Be(12345);
        }

        [Fact]
        public void RemovePidFile_WhenFileExists_DeletesAndReturnsTrue()
        {
            var sut = new PidManager("to-remove.pid");
            sut.SavePidToFile(1).Should().BeTrue();

            sut.RemovePidFile().Should().BeTrue();

            sut.PidFileExists().Should().BeFalse();
            sut.GetPidFromFile().Should().Be(0);
        }

        [Fact]
        public void RemovePidFile_WhenFileMissing_ReturnsTrue()
        {
            var sut = new PidManager("never-created.pid");

            sut.RemovePidFile().Should().BeTrue();
        }
    }
}
