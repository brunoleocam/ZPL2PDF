using System;
using System.IO;
using FluentAssertions;
using Xunit;
using ZPL2PDF;

namespace ZPL2PDF.Tests.UnitTests.Presentation
{
    /// <summary>
    /// Unit tests for ArgumentProcessor
    /// </summary>
    public class ArgumentProcessorTests : IDisposable
    {
        private readonly string _testDirectory;

        public ArgumentProcessorTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "ZPL2PDF_ArgumentProcessorTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Act
            var processor = new ArgumentProcessor();

            // Assert
            processor.Should().NotBeNull();
            processor.Mode.Should().Be(OperationMode.Conversion);
            processor.DaemonCommand.Should().Be(string.Empty);
            processor.ListenFolderPath.Should().Be(string.Empty);
            processor.InputFilePath.Should().Be(string.Empty);
        }

        #endregion

        #region ProcessArguments Tests

        [Fact]
        public void ProcessArguments_WithEmptyArgs_SetsDaemonMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new string[0];

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Daemon);
            processor.DaemonCommand.Should().Be("start");
            processor.ListenFolderPath.Should().NotBeEmpty();
        }

        [Fact]
        public void ProcessArguments_WithHelpArg_SetsHelpMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "-help" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Help);
        }

        [Fact]
        public void ProcessArguments_WithConversionArgs_SetsConversionMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var args = new[] { "-i", testFile, "-o", _testDirectory };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Conversion);
            processor.InputFilePath.Should().Be(testFile);
        }

        [Fact]
        public void ProcessArguments_WithDaemonStartArgs_SetsDaemonMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "start" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Daemon);
            processor.DaemonCommand.Should().Be("start");
        }

        [Fact]
        public void ProcessArguments_WithDaemonStopArgs_SetsDaemonMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "stop" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Daemon);
            processor.DaemonCommand.Should().Be("stop");
        }

        [Fact]
        public void ProcessArguments_WithDaemonStatusArgs_SetsDaemonMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "status" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Daemon);
            processor.DaemonCommand.Should().Be("status");
        }

        [Fact]
        public void ProcessArguments_WithServerStartArgs_SetsServerModeAndProperties()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "server", "start", "--port", "9101", "-o", _testDirectory };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Server);
            processor.ServerCommand.Should().Be("start");
            processor.ServerPort.Should().Be(9101);
            processor.ServerOutputFolder.Should().Be(_testDirectory);
            processor.ServerForeground.Should().BeFalse();
        }

        [Fact]
        public void ProcessArguments_WithServerStartAndForeground_SetsServerForeground()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "server", "start", "--port", "9102", "-o", _testDirectory, "--foreground" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Server);
            processor.ServerCommand.Should().Be("start");
            processor.ServerPort.Should().Be(9102);
            processor.ServerOutputFolder.Should().Be(_testDirectory);
            processor.ServerForeground.Should().BeTrue();
        }

        [Fact]
        public void ProcessArguments_WithServerStop_SetsServerMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "server", "stop" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Server);
            processor.ServerCommand.Should().Be("stop");
        }

        [Fact]
        public void ProcessArguments_WithServerStatus_SetsServerMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "server", "status" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Server);
            processor.ServerCommand.Should().Be("status");
        }

        [Fact]
        public void ProcessArguments_WithZplContentArgs_SetsConversionMode()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "-z", "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ", "-o", _testDirectory };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Conversion);
            processor.ZplContent.Should().Be("^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
        }

        #endregion

        #region Properties Tests

        [Fact]
        public void Mode_CanBeRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();

            // Act & Assert
            processor.Mode.Should().Be(OperationMode.Conversion);
        }

        [Fact]
        public void DaemonCommand_CanBeRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();

            // Act & Assert
            processor.DaemonCommand.Should().Be(string.Empty);
        }

        [Fact]
        public void ListenFolderPath_CanBeRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();

            // Act & Assert
            processor.ListenFolderPath.Should().Be(string.Empty);
        }

        [Fact]
        public void InputFilePath_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testPath = @"C:\Test\file.txt";

            // Act
            processor.InputFilePath = testPath;

            // Assert
            processor.InputFilePath.Should().Be(testPath);
        }

        [Fact]
        public void ZplContent_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testZpl = "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ";

            // Act
            processor.ZplContent = testZpl;

            // Assert
            processor.ZplContent.Should().Be(testZpl);
        }

        [Fact]
        public void OutputFolderPath_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testPath = @"C:\Test\Output";

            // Act
            processor.OutputFolderPath = testPath;

            // Assert
            processor.OutputFolderPath.Should().Be(testPath);
        }

        [Fact]
        public void OutputFileName_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testName = "test_output.pdf";

            // Act
            processor.OutputFileName = testName;

            // Assert
            processor.OutputFileName.Should().Be(testName);
        }

        [Fact]
        public void Width_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testWidth = 100.0;

            // Act
            processor.Width = testWidth;

            // Assert
            processor.Width.Should().Be(testWidth);
        }

        [Fact]
        public void Height_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testHeight = 200.0;

            // Act
            processor.Height = testHeight;

            // Assert
            processor.Height.Should().Be(testHeight);
        }

        [Fact]
        public void Unit_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testUnit = "mm";

            // Act
            processor.Unit = testUnit;

            // Assert
            processor.Unit.Should().Be(testUnit);
        }

        [Fact]
        public void Dpi_CanBeSetAndRetrieved()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testDpi = 203;

            // Act
            processor.Dpi = testDpi;

            // Assert
            processor.Dpi.Should().Be(testDpi);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void ProcessArguments_WithNullArgs_ThrowsNullReferenceException()
        {
            // Arrange
            var processor = new ArgumentProcessor();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => processor.ProcessArguments(null!));
        }

        [Fact]
        public void ProcessArguments_WithInvalidArgs_HandlesGracefully()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var args = new[] { "invalid", "args", "here" };

            // Act & Assert
            processor.ProcessArguments(args);
        }

        [Fact]
        public void ProcessArguments_WithMixedArgs_ProcessesCorrectly()
        {
            // Arrange
            var processor = new ArgumentProcessor();
            var testFile = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFile, "^XA^FO50,50^A0N,50,50^FDTest Label^FS^XZ");
            var args = new[] { "-i", testFile, "-o", _testDirectory, "-w", "100", "-h", "200", "-u", "mm" };

            // Act
            processor.ProcessArguments(args);

            // Assert
            processor.Mode.Should().Be(OperationMode.Conversion);
            processor.InputFilePath.Should().Be(testFile);
            processor.OutputFolderPath.Should().Be(_testDirectory);
            processor.Width.Should().Be(100.0);
            processor.Height.Should().Be(200.0);
            processor.Unit.Should().Be("mm");
        }

        #endregion

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
