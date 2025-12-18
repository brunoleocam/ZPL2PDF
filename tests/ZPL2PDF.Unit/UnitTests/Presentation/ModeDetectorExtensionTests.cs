using FluentAssertions;
using Xunit;

namespace ZPL2PDF.Unit.UnitTests.Presentation
{
    /// <summary>
    /// Tests for ModeDetector TCP Server mode detection
    /// </summary>
    public class ModeDetectorExtensionTests
    {
        private readonly ModeDetector _modeDetector;

        public ModeDetectorExtensionTests()
        {
            _modeDetector = new ModeDetector();
        }

        #region TCP Server Mode Detection Tests

        [Fact]
        public void DetectMode_ServerCommand_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "server" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        [Fact]
        public void DetectMode_ServerStartCommand_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "server", "start" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        [Fact]
        public void DetectMode_ServerStopCommand_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "server", "stop" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        [Fact]
        public void DetectMode_ServerStatusCommand_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "server", "status" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        [Fact]
        public void DetectMode_ServerWithPort_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "server", "start", "--port", "9100" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        [Fact]
        public void DetectMode_ServerCaseInsensitive_ReturnsTcpServer()
        {
            // Arrange
            var args = new[] { "SERVER" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.TcpServer);
        }

        #endregion

        #region Daemon Mode Still Works Tests

        [Fact]
        public void DetectMode_StartCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "start" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_StopCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "stop" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        [Fact]
        public void DetectMode_StatusCommand_ReturnsDaemon()
        {
            // Arrange
            var args = new[] { "status" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Daemon);
        }

        #endregion

        #region Conversion Mode Still Works Tests

        [Fact]
        public void DetectMode_InputFlag_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-i", "test.txt", "-o", "output" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DetectMode_ZplFlag_ReturnsConversion()
        {
            // Arrange
            var args = new[] { "-z", "^XA^XZ", "-o", "output" };

            // Act
            var result = _modeDetector.DetectMode(args);

            // Assert
            result.Should().Be(OperationMode.Conversion);
        }

        #endregion
    }
}

