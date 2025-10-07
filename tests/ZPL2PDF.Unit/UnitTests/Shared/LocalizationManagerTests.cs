using System;
using System.Globalization;
using Xunit;
using ZPL2PDF.Shared.Localization;

namespace ZPL2PDF.UnitTests.Shared
{
    /// <summary>
    /// Unit tests for LocalizationManager
    /// </summary>
    public class LocalizationManagerTests
    {
        [Fact]
        public void Initialize_WithDefaultCulture_ShouldWork()
        {
            // Act
            LocalizationManager.Initialize();
            
            // Assert
            Assert.NotNull(LocalizationManager.CurrentCulture);
            Assert.True(LocalizationManager.SupportedCultures.Length > 0);
        }

        [Fact]
        public void Initialize_WithSpecificCulture_ShouldSetCulture()
        {
            // Arrange
            var culture = new CultureInfo("pt-BR");
            
            // Act
            LocalizationManager.Initialize(culture);
            
            // Assert
            Assert.Equal(culture, LocalizationManager.CurrentCulture);
        }

        [Fact]
        public void GetString_WithValidKey_ShouldReturnLocalizedString()
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo("en-US"));
            
            // Act
            var result = LocalizationManager.GetString(ResourceKeys.APPLICATION_NAME);
            
            // Assert
            Assert.Equal("ZPL2PDF", result);
        }

        [Fact]
        public void GetString_WithParameters_ShouldFormatString()
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo("en-US"));
            var pid = 12345;
            
            // Act
            var result = LocalizationManager.GetString(ResourceKeys.DAEMON_STARTED_SUCCESS, pid);
            
            // Assert
            Assert.Equal($"Daemon started successfully! PID: {pid}", result);
        }

        [Fact]
        public void GetString_WithPortugueseCulture_ShouldReturnPortugueseString()
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo("pt-BR"));
            
            // Act
            var result = LocalizationManager.GetString(ResourceKeys.DAEMON_STARTED_SUCCESS, 12345);
            
            // Assert
            Assert.Equal("Daemon iniciado com sucesso! PID: 12345", result);
        }

        [Fact]
        public void GetString_WithInvalidKey_ShouldReturnKey()
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo("en-US"));
            
            // Act
            var result = LocalizationManager.GetString("INVALID_KEY");
            
            // Assert
            Assert.Equal("INVALID_KEY", result);
        }

        [Fact]
        public void GetString_WithUnsupportedCulture_ShouldFallbackToEnglish()
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo("xx-XX")); // Unsupported culture
            
            // Act
            var result = LocalizationManager.GetString(ResourceKeys.APPLICATION_NAME);
            
            // Assert
            Assert.Equal("ZPL2PDF", result); // Should fallback to English
        }

        [Fact]
        public void SupportedCultures_ShouldContainExpectedCultures()
        {
            // Act
            var supportedCultures = LocalizationManager.SupportedCultures;
            
            // Assert
            Assert.Contains("en-US", supportedCultures);
            Assert.Contains("pt-BR", supportedCultures);
            Assert.Contains("es-ES", supportedCultures);
            Assert.Contains("fr-FR", supportedCultures);
            Assert.Contains("de-DE", supportedCultures);
            Assert.Contains("it-IT", supportedCultures);
            Assert.Contains("ja-JP", supportedCultures);
            Assert.Contains("zh-CN", supportedCultures);
        }

        [Theory]
        [InlineData("en-US", "Daemon started successfully! PID: {0}")]
        [InlineData("pt-BR", "Daemon iniciado com sucesso! PID: {0}")]
        [InlineData("es-ES", "Daemon iniciado con éxito! PID: {0}")]
        [InlineData("fr-FR", "Daemon démarré avec succès! PID: {0}")]
        public void GetString_DifferentCultures_ShouldReturnCorrectLanguage(string cultureName, string expectedPattern)
        {
            // Arrange
            LocalizationManager.Initialize(new CultureInfo(cultureName));
            var pid = 99999;
            
            // Act
            var result = LocalizationManager.GetString(ResourceKeys.DAEMON_STARTED_SUCCESS, pid);
            
            // Assert
            Assert.Equal(expectedPattern.Replace("{0}", pid.ToString()), result);
        }
    }
}
