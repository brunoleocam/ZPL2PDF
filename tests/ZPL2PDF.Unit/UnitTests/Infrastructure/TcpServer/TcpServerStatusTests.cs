using FluentAssertions;
using Xunit;
using ZPL2PDF.Infrastructure.TcpServer;

namespace ZPL2PDF.Unit.UnitTests.Infrastructure.TcpServer
{
    /// <summary>
    /// Tests for TCP Server status and configuration
    /// </summary>
    public class TcpServerStatusTests
    {
        [Fact]
        public void TcpServerStatus_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var status = new TcpServerStatus();

            // Assert
            status.IsRunning.Should().BeFalse();
            status.Port.Should().Be(0);
            status.OutputDirectory.Should().BeEmpty();
            status.RendererName.Should().BeEmpty();
            status.TotalConnections.Should().Be(0);
        }

        [Fact]
        public void TcpServerStatus_CanSetValues()
        {
            // Arrange
            var status = new TcpServerStatus
            {
                IsRunning = true,
                Port = 9101,
                OutputDirectory = "C:\\Output",
                RendererName = "BinaryKits",
                TotalConnections = 5
            };

            // Assert
            status.IsRunning.Should().BeTrue();
            status.Port.Should().Be(9101);
            status.OutputDirectory.Should().Be("C:\\Output");
            status.RendererName.Should().Be("BinaryKits");
            status.TotalConnections.Should().Be(5);
        }

        [Fact]
        public void TcpPrinterServer_DefaultPort_Is9101()
        {
            // Assert
            TcpPrinterServer.DefaultPort.Should().Be(9101);
        }

        [Fact]
        public void TcpConnectionEventArgs_StoresValues()
        {
            // Arrange & Act
            var args = new TcpConnectionEventArgs("192.168.1.1:5000", 42);

            // Assert
            args.RemoteEndpoint.Should().Be("192.168.1.1:5000");
            args.ConnectionId.Should().Be(42);
        }

        [Fact]
        public void TcpLabelProcessedEventArgs_StoresValues()
        {
            // Arrange & Act
            var args = new TcpLabelProcessedEventArgs("C:\\Output\\label.pdf", 3, 42);

            // Assert
            args.OutputPath.Should().Be("C:\\Output\\label.pdf");
            args.LabelCount.Should().Be(3);
            args.ConnectionId.Should().Be(42);
        }

        [Fact]
        public void TcpErrorEventArgs_StoresValues()
        {
            // Arrange & Act
            var args = new TcpErrorEventArgs("Connection failed", "192.168.1.1:5000");

            // Assert
            args.ErrorMessage.Should().Be("Connection failed");
            args.RemoteEndpoint.Should().Be("192.168.1.1:5000");
        }

        [Fact]
        public void TcpErrorEventArgs_NullEndpoint_IsAllowed()
        {
            // Arrange & Act
            var args = new TcpErrorEventArgs("Error", null);

            // Assert
            args.RemoteEndpoint.Should().BeNull();
        }
    }
}

