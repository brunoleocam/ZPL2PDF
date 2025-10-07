using System;
using ZPL2PDF.Shared.Localization;

namespace ZPL2PDF
{
    /// <summary>
    /// Responsible for displaying help information
    /// </summary>
    public class HelpDisplay
    {
        /// <summary>
        /// Shows the help message
        /// </summary>
        public void ShowHelp()
        {
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_TITLE));
            Console.WriteLine();
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_DEFAULT_BEHAVIOR));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_DEFAULT_USAGE));
            Console.WriteLine($"  {LocalizationManager.GetString(ResourceKeys.HELP_DEFAULT_DESCRIPTION)}");
            Console.WriteLine($"  {LocalizationManager.GetString(ResourceKeys.HELP_DAEMON_DIRECT_OPTIONS)}");
            Console.WriteLine();
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CONVERSION_MODE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CONVERSION_USAGE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAMETERS));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_INPUT_FILE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_ZPL_CONTENT));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_OUTPUT_FOLDER));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_OUTPUT_NAME));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_WIDTH));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_HEIGHT));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_DENSITY));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_PARAM_UNIT));
            Console.WriteLine();
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_DAEMON_MODE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_DAEMON_USAGE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_COMMANDS));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CMD_START));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CMD_STOP));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CMD_STATUS));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_CMD_RUN));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_DAEMON_OPTIONS));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_OPT_LISTEN_FOLDER));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_OPT_WIDTH_FIXED));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_OPT_HEIGHT_FIXED));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_OPT_UNIT_DAEMON));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_OPT_DENSITY_DAEMON));
            Console.WriteLine();
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_LANGUAGE_PARAM));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_SET_LANGUAGE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_RESET_LANGUAGE));
            Console.WriteLine(LocalizationManager.GetString(ResourceKeys.HELP_SHOW_LANGUAGE));
            Console.WriteLine();
            Console.WriteLine($"  -help                      {LocalizationManager.GetString(ResourceKeys.HELP_SHOW_HELP_MESSAGE)}");
        }
    }
}
