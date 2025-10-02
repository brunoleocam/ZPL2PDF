namespace ZPL2PDF.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface for configuration management service
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// Loads configuration from file
        /// </summary>
        /// <returns>True if loaded successfully, False otherwise</returns>
        bool LoadConfig();

        /// <summary>
        /// Saves configuration to file
        /// </summary>
        /// <returns>True if saved successfully, False otherwise</returns>
        bool SaveConfig();

        /// <summary>
        /// Gets the default watch folder path
        /// </summary>
        /// <returns>Default watch folder path</returns>
        string GetDefaultWatchFolder();

        /// <summary>
        /// Gets a configuration value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value or default</returns>
        string GetValue(string key);

        /// <summary>
        /// Sets a configuration value
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="value">Configuration value</param>
        void SetValue(string key, string value);

        /// <summary>
        /// Gets a configuration value as integer
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value as integer</returns>
        int GetIntValue(string key, int defaultValue = 0);

        /// <summary>
        /// Gets a configuration value as double
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value as double</returns>
        double GetDoubleValue(string key, double defaultValue = 0.0);

        /// <summary>
        /// Gets a configuration value as boolean
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value as boolean</returns>
        bool GetBoolValue(string key, bool defaultValue = false);
    }
}
