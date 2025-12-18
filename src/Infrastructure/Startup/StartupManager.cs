using System.Runtime.InteropServices;

namespace ZPL2PDF.Infrastructure.Startup
{
    /// <summary>
    /// Factory for creating platform-specific startup managers.
    /// </summary>
    public static class StartupManagerFactory
    {
        /// <summary>
        /// Creates a startup manager for the current platform.
        /// </summary>
        /// <returns>Platform-specific startup manager, or null if not supported.</returns>
        public static IStartupManager? Create()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new Windows.WindowsStartupManager();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new Linux.LinuxStartupManager();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new MacOS.MacStartupManager();
            }

            return null;
        }
    }
}

