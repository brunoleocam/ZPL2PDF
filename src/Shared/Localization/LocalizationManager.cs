using System;
using System.Globalization;
using System.Resources;

namespace ZPL2PDF.Shared.Localization
{
    /// <summary>
    /// Manages localization and multi-language support for ZPL2PDF
    /// </summary>
    public static class LocalizationManager
    {
        private static ResourceManager? _resourceManager;
        private static CultureInfo? _currentCulture;
        
        /// <summary>
        /// Initializes the localization manager with automatic language detection
        /// Priority: Environment Variable > Config File > System Detection
        /// </summary>
        public static void Initialize()
        {
            _currentCulture = DetectLanguageWithPriority(null);
            _resourceManager = new ResourceManager("ZPL2PDF.Resources.Messages", 
                typeof(LocalizationManager).Assembly);
        }

        /// <summary>
        /// Initializes the localization manager with configuration language
        /// Priority: Environment Variable > Config File > System Detection
        /// </summary>
        /// <param name="configLanguage">Language from configuration file</param>
        public static void InitializeWithConfig(string? configLanguage)
        {
            _currentCulture = DetectLanguageWithPriority(configLanguage);
            _resourceManager = new ResourceManager("ZPL2PDF.Resources.Messages", 
                typeof(LocalizationManager).Assembly);
        }
        
        /// <summary>
        /// Initializes the localization manager with a specific culture
        /// </summary>
        /// <param name="culture">The culture to use for localization</param>
        public static void Initialize(CultureInfo culture)
        {
            _currentCulture = culture;
            _resourceManager = new ResourceManager("ZPL2PDF.Resources.Messages", 
                typeof(LocalizationManager).Assembly);
        }

        /// <summary>
        /// Initializes the localization manager with a specific language code
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "pt-BR", "en-US")</param>
        public static void Initialize(string languageCode)
        {
            try
            {
                var culture = new CultureInfo(languageCode);
                Initialize(culture);
            }
            catch (CultureNotFoundException)
            {
                // Fallback to automatic detection if language code is invalid
                Initialize();
            }
        }
        
        /// <summary>
        /// Gets a localized string by key
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Localized string</returns>
        public static string GetString(string key, params object[] args)
        {
            if (_resourceManager == null)
            {
                Initialize();
            }
            
            if (_resourceManager == null || _currentCulture == null)
            {
                return key; // Fallback to key if initialization failed
            }
            
            try
            {
                var localizedString = _resourceManager.GetString(key, _currentCulture);
                
                if (string.IsNullOrEmpty(localizedString))
                {
                    // Try fallback to English
                    var fallbackCulture = new CultureInfo("en-US");
                    localizedString = _resourceManager.GetString(key, fallbackCulture);
                }
                
                if (string.IsNullOrEmpty(localizedString))
                {
                    return key; // Fallback to key
                }
                
                // Format string with arguments if provided
                if (args.Length > 0)
                {
                    return string.Format(localizedString, args);
                }
                
                return localizedString;
            }
            catch (Exception)
            {
                return key; // Fallback to key on error
            }
        }
        
        /// <summary>
        /// Detects language with priority: Env Var > Config File > System Detection
        /// </summary>
        /// <param name="configLanguage">Language from configuration file (optional)</param>
        /// <returns>CultureInfo for the detected language</returns>
        private static CultureInfo DetectLanguageWithPriority(string? configLanguage)
        {
            try
            {
                // Priority 1: Environment Variable ZPL2PDF_LANGUAGE
                var envLanguage = Environment.GetEnvironmentVariable("ZPL2PDF_LANGUAGE");
                if (!string.IsNullOrEmpty(envLanguage))
                {
                    try
                    {
                        var culture = new CultureInfo(envLanguage);
                        if (IsCultureSupported(culture.Name))
                        {
                            return culture;
                        }
                    }
                    catch (CultureNotFoundException)
                    {
                        // Invalid culture in environment variable, continue to next priority
                    }
                }

                // Priority 2: Configuration File
                if (!string.IsNullOrEmpty(configLanguage))
                {
                    try
                    {
                        var culture = new CultureInfo(configLanguage);
                        if (IsCultureSupported(culture.Name))
                        {
                            return culture;
                        }
                    }
                    catch (CultureNotFoundException)
                    {
                        // Invalid culture in config, continue to system detection
                    }
                }

                // Priority 3: System Detection
                return DetectSystemLanguage();
            }
            catch (Exception)
            {
                // Fallback to English on any error
                return new CultureInfo("en-US");
            }
        }

        /// <summary>
        /// Detects the system language and returns appropriate culture
        /// </summary>
        /// <returns>CultureInfo for the detected language</returns>
        private static CultureInfo DetectSystemLanguage()
        {
            try
            {
                // Try to get system culture
                var systemCulture = CultureInfo.CurrentCulture;
                
                // Check if we support this culture
                if (IsCultureSupported(systemCulture.Name))
                {
                    return systemCulture;
                }
                
                // Try to get parent culture (e.g., "pt-BR" -> "pt")
                var parentCulture = systemCulture.Parent;
                if (parentCulture != null && IsCultureSupported(parentCulture.Name))
                {
                    return parentCulture;
                }
                
                // Check environment variables on Linux
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    var langEnv = Environment.GetEnvironmentVariable("LANG");
                    if (!string.IsNullOrEmpty(langEnv))
                    {
                        var cultureName = ExtractCultureFromLang(langEnv);
                        if (!string.IsNullOrEmpty(cultureName) && IsCultureSupported(cultureName))
                        {
                            return new CultureInfo(cultureName);
                        }
                    }
                    
                    var lcAllEnv = Environment.GetEnvironmentVariable("LC_ALL");
                    if (!string.IsNullOrEmpty(lcAllEnv))
                    {
                        var cultureName = ExtractCultureFromLang(lcAllEnv);
                        if (!string.IsNullOrEmpty(cultureName) && IsCultureSupported(cultureName))
                        {
                            return new CultureInfo(cultureName);
                        }
                    }
                }
                
                // Default to English
                return new CultureInfo("en-US");
            }
            catch (Exception)
            {
                // Fallback to English on any error
                return new CultureInfo("en-US");
            }
        }
        
        /// <summary>
        /// Extracts culture name from LANG environment variable
        /// </summary>
        /// <param name="langValue">LANG environment variable value</param>
        /// <returns>Culture name or null</returns>
        private static string? ExtractCultureFromLang(string langValue)
        {
            if (string.IsNullOrEmpty(langValue))
                return null;
                
            // LANG format: "pt_BR.UTF-8" -> "pt-BR"
            var parts = langValue.Split('.');
            if (parts.Length > 0)
            {
                var culturePart = parts[0];
                // Replace underscore with dash for .NET culture format
                return culturePart.Replace('_', '-');
            }
            
            return null;
        }
        
        /// <summary>
        /// Checks if a culture is supported by the application
        /// </summary>
        /// <param name="cultureName">Culture name to check</param>
        /// <returns>True if supported, false otherwise</returns>
        private static bool IsCultureSupported(string cultureName)
        {
            var supportedCultures = new[]
            {
                "en-US",    // English
                "pt-BR",    // Portuguese (Brazil)
                "es-ES",    // Spanish (Spain)
                "fr-FR",    // French (France)
                "de-DE",    // German (Germany)
                "it-IT",    // Italian (Italy)
                "ja-JP",    // Japanese (Japan)
                "zh-CN"     // Chinese (Simplified)
            };
            
            return Array.Exists(supportedCultures, c => c.Equals(cultureName, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Gets the current culture being used
        /// </summary>
        public static CultureInfo? CurrentCulture => _currentCulture;
        
        /// <summary>
        /// Gets all supported cultures
        /// </summary>
        public static string[] SupportedCultures => new[]
        {
            "en-US", "pt-BR", "es-ES", "fr-FR", "de-DE", "it-IT", "ja-JP", "zh-CN"
        };
    }
}
