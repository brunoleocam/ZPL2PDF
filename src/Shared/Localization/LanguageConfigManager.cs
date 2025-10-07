using System;
using System.Globalization;

namespace ZPL2PDF.Shared.Localization
{
    /// <summary>
    /// Manages language configuration persistence
    /// </summary>
    public static class LanguageConfigManager
    {
        private const string ENV_VAR_NAME = "ZPL2PDF_LANGUAGE";

        /// <summary>
        /// Sets the language permanently via environment variable
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "pt-BR", "en-US")</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetLanguagePermanently(string languageCode)
        {
            try
            {
                // Validate language code
                if (!IsValidLanguageCode(languageCode))
                {
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.INVALID_LANGUAGE_CODE, languageCode));
                    Console.WriteLine(LocalizationManager.GetString(ResourceKeys.SUPPORTED_LANGUAGES_LIST));
                    foreach (var culture in LocalizationManager.SupportedCultures)
                    {
                        Console.WriteLine($"  - {culture}");
                    }
                    return false;
                }

                // Set environment variable for user (persistent)
                Environment.SetEnvironmentVariable(ENV_VAR_NAME, languageCode, EnvironmentVariableTarget.User);
                
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.LANGUAGE_SET_SUCCESS, languageCode));
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.RESTART_REQUIRED));
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.ERROR_SETTING_LANGUAGE, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Resets language configuration to system default
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool ResetLanguage()
        {
            try
            {
                // Remove environment variable
                Environment.SetEnvironmentVariable(ENV_VAR_NAME, null, EnvironmentVariableTarget.User);
                
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.LANGUAGE_RESET_SUCCESS));
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.RESTART_REQUIRED));
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.ERROR_RESETTING_LANGUAGE, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Shows current language configuration and detection order
        /// </summary>
        public static void ShowLanguageConfiguration()
        {
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.CURRENT_LANGUAGE_CONFIG));
            Console.WriteLine();

            // Check environment variable
            var envLang = Environment.GetEnvironmentVariable(ENV_VAR_NAME);
            if (!string.IsNullOrEmpty(envLang))
            {
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.ENV_VAR_LANGUAGE, envLang));
            }
            else
            {
                Console.WriteLine(LocalizationManager.GetString(ResourceKeys.ENV_VAR_NOT_SET));
            }

            // Show current culture
            var currentCulture = LocalizationManager.CurrentCulture ?? CultureInfo.CurrentUICulture;
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.CURRENT_LANGUAGE, currentCulture.Name));
            
            // Show system language
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.SYSTEM_LANGUAGE, CultureInfo.CurrentUICulture.Name));
            
            Console.WriteLine();
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.LANGUAGE_PRIORITY_ORDER));
        }

        /// <summary>
        /// Validates if a language code is supported
        /// </summary>
        /// <param name="languageCode">Language code to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        private static bool IsValidLanguageCode(string languageCode)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
                return false;

            try
            {
                var culture = new CultureInfo(languageCode);
                return Array.Exists(LocalizationManager.SupportedCultures, 
                    c => c.Equals(languageCode, StringComparison.OrdinalIgnoreCase));
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
