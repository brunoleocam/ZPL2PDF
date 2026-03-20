using System;
using System.Diagnostics;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for <see cref="ProcessManager"/> (process queries and safe kill paths).
    /// </summary>
    public class ProcessManagerTests
    {
        [Fact]
        public void IsProcessRunning_WithCurrentProcess_ReturnsTrue()
        {
            var sut = new ProcessManager();
            var pid = Process.GetCurrentProcess().Id;

            sut.IsProcessRunning(pid).Should().BeTrue();
        }

        [Fact]
        public void GetProcessInfo_WithCurrentProcess_ReturnsMatchingIdAndName()
        {
            var sut = new ProcessManager();
            using var current = Process.GetCurrentProcess();
            var pid = current.Id;

            var info = sut.GetProcessInfo(pid);

            info.Should().NotBeNull();
            info!.Id.Should().Be(pid);
            info.ProcessName.Should().NotBeNullOrEmpty();
            info.HasExited.Should().BeFalse();
        }

        [Fact]
        public void IsProcessRunning_WithNonExistentPid_ReturnsFalse()
        {
            var sut = new ProcessManager();

            sut.IsProcessRunning(int.MaxValue).Should().BeFalse();
        }

        [Fact]
        public void GetProcessInfo_WithNonExistentPid_ReturnsNull()
        {
            var sut = new ProcessManager();

            sut.GetProcessInfo(int.MaxValue).Should().BeNull();
        }

        [Fact]
        public void KillProcess_WithNonExistentPid_ReturnsTrue()
        {
            var sut = new ProcessManager();

            sut.KillProcess(int.MaxValue).Should().BeTrue();
        }

        /// <summary>
        /// When a runnable ZPL2PDF host is resolved (exe or dotnet + dll), <c>-help</c> should exit quickly.
        /// If the test output layout does not expose a host, the assertion is skipped (no failure).
        /// </summary>
        [Fact]
        public void StartBackgroundProcess_WithHelp_WhenHostResolved_ProcessEnds()
        {
            var sut = new ProcessManager();
            var pid = sut.StartBackgroundProcess("-help");

            if (pid == 0)
            {
                // No runnable binary next to tests / project output — not a failure.
                return;
            }

            try
            {
                var deadline = DateTime.UtcNow.AddSeconds(20);
                while (sut.IsProcessRunning(pid) && DateTime.UtcNow < deadline)
                {
                    Thread.Sleep(150);
                }

                sut.IsProcessRunning(pid).Should().BeFalse("help mode should terminate the child process");
            }
            finally
            {
                if (sut.IsProcessRunning(pid))
                {
                    sut.KillProcess(pid);
                }
            }
        }
    }
}
