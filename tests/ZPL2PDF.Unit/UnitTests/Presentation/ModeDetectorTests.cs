using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Presentation
{
    /// <summary>
    /// Unit tests for ModeDetector
    /// </summary>
    public class ModeDetectorTests
    {
        private readonly ModeDetector _modeDetector;

        public ModeDetectorTests()
        {
            _modeDetector = new ModeDetector();
        }

        #region DetectMode Tests

        [Fact]
        public void DetectMode_WithEmptyArgs_ReturnsDaemon()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithHelpArg_ReturnsHelp()
        {
            // Arrange
            var args = new[] { "-help" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Help);
        }

        [Fact]
        public void DetectMode_WithHelpArgCaseInsensitive_ReturnsHelp()
        {
            // Arrange
            var args = new[] { "-HELP" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Help);
        }

        [Fact]
        public void DetectMode_WithStartCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "start" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithStopCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "stop" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithStatusCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "status" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithRunCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "run" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithInputFileArg_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-i", "test.txt" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_WithZplContentArg_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-z", "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_WithOutputFolderArg_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-o", "C:\\Output" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_WithLArgWithoutStart_ReturnsHelp()
        {
            // Arrange
            var args = new[] { "-l", "C:\\Folder" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Help);
        }

        [Fact]
        public void DetectMode_WithConversionArgs_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "somefile.txt", "-o", "C:\\Output", "-n", "output.pdf" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_WithDaemonArgs_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "somefile.txt", "-w", "100", "-h", "200", "-u", "mm" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_WithUnclearArgs_ReturnsHelp()
        {
            // Arrange
            var args = new[] { "unclear", "args" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Help);
        }

        [Fact]
        public void DetectMode_WithServerArg_ReturnsServer()
        {
            // Arrange
            var args = new[] { "server" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Server);
        }

        [Fact]
        public void DetectMode_WithServerStart_ReturnsServer()
        {
            // Arrange
            var args = new[] { "server", "start" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Server);
        }

        [Fact]
        public void DetectMode_WithServerCaseInsensitive_ReturnsServer()
        {
            // Arrange
            var args = new[] { "SERVER", "START" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Server);
        }

        #endregion

        #region ExtractServerCommand Tests

        [Fact]
        public void ExtractServerCommand_WithServerStart_ReturnsStart()
        {
            // Arrange
            var args = new[] { "server", "start" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void ExtractServerCommand_WithServerStop_ReturnsStop()
        {
            // Arrange
            var args = new[] { "server", "stop" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("stop");
        }

        [Fact]
        public void ExtractServerCommand_WithServerStatus_ReturnsStatus()
        {
            // Arrange
            var args = new[] { "server", "status" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("status");
        }

        [Fact]
        public void ExtractServerCommand_WithServerOnly_ReturnsStartAsDefault()
        {
            // Arrange
            var args = new[] { "server" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void ExtractServerCommand_WithInvalidServerSubcommand_ReturnsStartAsDefault()
        {
            // Arrange
            var args = new[] { "server", "invalid" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void ExtractServerCommand_WithNonServerArgs_ReturnsStart()
        {
            // Arrange
            var args = new[] { "start" };

            // Act
            var result = _modeDetector.ExtractServerCommand(args);

            // Assert
            result.Should().Be("start");
        }

        #endregion

        #region ExtractDaemonCommand Tests

        [Fact]
        public void ExtractDaemonCommand_WithEmptyArgs_ReturnsStart()
        {
            // Arrange
            var args = new string[0];

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void ExtractDaemonCommand_WithStartCommand_ReturnsStart()
        {
            // Arrange
            var args = new[] { "start" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void ExtractDaemonCommand_WithStopCommand_ReturnsStop()
        {
            // Arrange
            var args = new[] { "stop" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("stop");
        }

        [Fact]
        public void ExtractDaemonCommand_WithStatusCommand_ReturnsStatus()
        {
            // Arrange
            var args = new[] { "status" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("status");
        }

        [Fact]
        public void ExtractDaemonCommand_WithRunCommand_ReturnsRun()
        {
            // Arrange
            var args = new[] { "run" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("run");
        }

        [Fact]
        public void ExtractDaemonCommand_WithInvalidCommand_ReturnsEmpty()
        {
            // Arrange
            var args = new[] { "invalid" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void ExtractDaemonCommand_WithConversionArgs_ReturnsEmpty()
        {
            // Arrange
            var args = new[] { "-i", "test.txt", "-o", "C:\\Output" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void DetectMode_WithNullArgs_ThrowsNullReferenceException()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _modeDetector.DetectMode(null!));
        }

        [Fact]
        public void ExtractDaemonCommand_WithNullArgs_ThrowsNullReferenceException()
        {
            // Act & Assert
            Assert.Throws<NullReferenceException>(() => _modeDetector.ExtractDaemonCommand(null!));
        }

        [Fact]
        public void DetectMode_WithMixedCaseArgs_HandlesCorrectly()
        {
            // Arrange
            var args = new[] { "-I", "test.txt", "-O", "C:\\Output" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void ExtractDaemonCommand_WithMixedCaseArgs_HandlesCorrectly()
        {
            // Arrange
            var args = new[] { "START" };

            // Act
            var result = _modeDetector.ExtractDaemonCommand(args);

            // Assert
            result.Should().Be("start");
        }

        [Fact]
        public void DetectMode_WithMultipleConversionArgs_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-i", "test.txt", "-o", "C:\\Output", "-n", "output.pdf", "-w", "100" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_WithMultipleDaemonArgs_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "-w", "100", "-h", "200", "-u", "mm", "-d", "203" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        #endregion
    }
}
