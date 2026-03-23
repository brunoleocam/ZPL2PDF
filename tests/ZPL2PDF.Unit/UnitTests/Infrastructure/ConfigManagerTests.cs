using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ZPL2PDF.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Unit tests for <see cref="ConfigManager"/> using an isolated config directory (<c>ZPL2PDF_CONFIG_FOLDER</c>).
    /// </summary>
    public class ConfigManagerTests : IDisposable
    {
        private readonly string _isolatedConfigDir;
        private readonly string? _previousConfigFolderEnv;

        public ConfigManagerTests()
        {
            _isolatedConfigDir = Path.Combine(
                Path.GetTempPath(),
                "ZPL2PDF_ConfigManagerTests",
                Guid.NewGuid().ToString());
            Directory.CreateDirectory(_isolatedConfigDir);

            _previousConfigFolderEnv = Environment.GetEnvironmentVariable("ZPL2PDF_CONFIG_FOLDER");
            Environment.SetEnvironmentVariable("ZPL2PDF_CONFIG_FOLDER", _isolatedConfigDir);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("ZPL2PDF_CONFIG_FOLDER", _previousConfigFolderEnv);

            try
            {
                if (Directory.Exists(_isolatedConfigDir))
                {
                    Directory.Delete(_isolatedConfigDir, true);
                }
            }
            catch
            {
                // Best-effort cleanup on locked temp paths.
            }
        }

        private static string ConfigFilePath(string root) => Path.Combine(root, "zpl2pdf.json");

        [Fact]
        public void Constructor_WithIsolatedFolder_CreatesDefaultConfigFile_WhenMissing()
        {
            var sut = new ConfigManager();

            var jsonPath = ConfigFilePath(_isolatedConfigDir);
            File.Exists(jsonPath).Should().BeTrue();
            sut.Config.Should().NotBeNull();
            sut.Config.LabelWidth.Should().BeGreaterThan(0);
            sut.Config.LabelHeight.Should().BeGreaterThan(0);
            sut.Config.Dpi.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Constructor_WithInvalidJson_FallsBackToDefaultConfiguration()
        {
            File.WriteAllText(ConfigFilePath(_isolatedConfigDir), "{ not valid json");

            var sut = new ConfigManager();

            sut.Config.Should().NotBeNull();
            sut.ValidateConfig().Should().BeTrue();
        }

        [Fact]
        public void Constructor_LoadsExistingValidConfigFile()
        {
            var expected = new Zpl2PdfConfig
            {
                DefaultListenFolder = Path.Combine(_isolatedConfigDir, "watch"),
                LabelWidth = 77,
                LabelHeight = 55,
                Unit = "mm",
                Dpi = 203,
                LogLevel = "Warning",
                RetryDelay = 1500,
                MaxRetries = 7,
                AutoStart = true,
                Language = "en-US"
            };

            var json = JsonSerializer.Serialize(expected, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath(_isolatedConfigDir), json);

            var sut = new ConfigManager();

            sut.Config.DefaultListenFolder.Should().Be(expected.DefaultListenFolder);
            sut.Config.LabelWidth.Should().Be(77);
            sut.Config.LabelHeight.Should().Be(55);
            sut.Config.LogLevel.Should().Be("Warning");
            sut.Config.RetryDelay.Should().Be(1500);
            sut.Config.MaxRetries.Should().Be(7);
            sut.Config.AutoStart.Should().BeTrue();
            sut.Config.Language.Should().Be("en-US");
        }

        [Fact]
        public void GetConfig_ReturnsSameInstanceAsConfigProperty()
        {
            var sut = new ConfigManager();

            sut.GetConfig().Should().BeSameAs(sut.Config);
        }

        [Fact]
        public void GetDefaultListenFolder_ContainsDocumentsAndAppFolder()
        {
            var sut = new ConfigManager();
            var path = sut.GetDefaultListenFolder();

            path.Should().Contain("ZPL2PDF Auto Converter");
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path.Should().StartWith(documents);
        }

        [Fact]
        public void GetConfigFolder_UsesOsSpecificLayout()
        {
            var sut = new ConfigManager();
            var folder = sut.GetConfigFolder();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folder.Should().StartWith(appData);
                Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .Should()
                    .Be("ZPL2PDF");
            }
            else
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                folder.Should().StartWith(home);
                Path.GetFileName(folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .Should()
                    .Be("zpl2pdf");
            }
        }

        [Fact]
        public void GetPidFolder_UsesOsSpecificLayout()
        {
            var sut = new ConfigManager();
            var folder = sut.GetPidFolder();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var temp = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                folder.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .Should()
                    .Be(temp);
            }
            else
            {
                folder.Should().Be("/var/run");
            }
        }

        [Fact]
        public void UpdateConfig_WritesChangesToConfigFile()
        {
            var sut = new ConfigManager();
            var updated = new Zpl2PdfConfig
            {
                DefaultListenFolder = sut.Config.DefaultListenFolder,
                LabelWidth = 88,
                LabelHeight = 44,
                Unit = "cm",
                Dpi = 300,
                LogLevel = "Error",
                RetryDelay = 500,
                MaxRetries = 2,
                AutoStart = false
            };

            sut.UpdateConfig(updated);

            var json = File.ReadAllText(ConfigFilePath(_isolatedConfigDir));
            var roundTrip = JsonSerializer.Deserialize<Zpl2PdfConfig>(json);
            roundTrip.Should().NotBeNull();
            roundTrip!.Unit.Should().Be("cm");
            roundTrip.LabelWidth.Should().Be(88);
            roundTrip.Dpi.Should().Be(300);
            roundTrip.LogLevel.Should().Be("Error");
        }

        [Fact]
        public void EnsureFoldersExist_CreatesListenFolder_WhenAbsent()
        {
            var sut = new ConfigManager();
            var listen = Path.Combine(_isolatedConfigDir, "new_watch");
            sut.Config.DefaultListenFolder = listen;

            Directory.Exists(listen).Should().BeFalse();

            sut.EnsureFoldersExist();

            Directory.Exists(listen).Should().BeTrue();
        }

        [Fact]
        public void ValidateConfig_WithFreshManager_ReturnsTrue()
        {
            var sut = new ConfigManager();
            sut.ValidateConfig().Should().BeTrue();
        }

        [Fact]
        public void ValidateConfig_WithEmptyListenFolder_ReturnsFalse()
        {
            var sut = new ConfigManager();
            sut.Config.DefaultListenFolder = string.Empty;

            sut.ValidateConfig().Should().BeFalse();
        }

        [Fact]
        public void ValidateConfig_WithInvalidUnit_ReturnsFalse()
        {
            var sut = new ConfigManager();
            sut.Config.Unit = "px";

            sut.ValidateConfig().Should().BeFalse();
        }

        [Fact]
        public void ValidateConfig_WithInvalidLogLevel_ReturnsFalse()
        {
            var sut = new ConfigManager();
            sut.Config.LogLevel = "Verbose";

            sut.ValidateConfig().Should().BeFalse();
        }

        [Fact]
        public void ValidateConfig_WithNonPositiveLabelWidth_ReturnsFalse()
        {
            var sut = new ConfigManager();
            sut.Config.LabelWidth = 0;

            sut.ValidateConfig().Should().BeFalse();
        }

        [Fact]
        public void ShowConfig_DoesNotThrow()
        {
            var sut = new ConfigManager();

            var act = () => sut.ShowConfig();

            act.Should().NotThrow();
        }
    }
}
