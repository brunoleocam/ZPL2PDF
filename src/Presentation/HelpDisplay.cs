using System;

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
            Console.WriteLine("ZPL2PDF - ZPL to PDF Converter");
            Console.WriteLine();
            Console.WriteLine("DEFAULT BEHAVIOR:");
            Console.WriteLine("Usage: ZPL2PDF.exe [options]");
            Console.WriteLine("  Running without arguments starts daemon mode (same as 'start' command)");
            Console.WriteLine("  You can also pass daemon options directly: ZPL2PDF.exe -l \"folder\" -w 10 -h 5");
            Console.WriteLine();
            Console.WriteLine("CONVERSION MODE:");
            Console.WriteLine("Usage: ZPL2PDF.exe -i <input_file_path> -o <output_folder_path> [-n <output_file_name>] | -z <zpl_content>");
            Console.WriteLine("Parameters:");
            Console.WriteLine("  -i <input_file_path>       Path to the input .txt or .prn file");
            Console.WriteLine("  -z <zpl_content>           ZPL content as a string");
            Console.WriteLine("  -o <output_folder_path>    Path to the folder where the PDF file will be saved");
            Console.WriteLine("  -n <output_file_name>      Name of the output PDF file (optional)");
            Console.WriteLine("  -w <width>                 Width of the label (accepts . or , as decimal separator)");
            Console.WriteLine("  -h <height>                Height of the label (accepts . or , as decimal separator)");
            Console.WriteLine("  -d <density>               Print density in DPI (integer only, e.g., 203, 300)");
            Console.WriteLine("  -u <unit>                  Unit of measurement for width and height ('in', 'cm', 'mm')");
            Console.WriteLine();
            Console.WriteLine("DAEMON MODE:");
            Console.WriteLine("Usage: ZPL2PDF.exe start [options] | stop | status | run [options]");
            Console.WriteLine("Commands:");
            Console.WriteLine("  start                      Start daemon mode in background");
            Console.WriteLine("  stop                       Stop daemon mode");
            Console.WriteLine("  status                     Check daemon status");
            Console.WriteLine("  run                        Run daemon mode in current process (for testing)");
            Console.WriteLine("Daemon Options:");
            Console.WriteLine("  -l <listen_folder>          Folder to monitor (default: Documents/ZPL2PDF Auto Converter)");
            Console.WriteLine("  -w <width>                 Fixed width for all conversions (accepts . or , as decimal separator)");
            Console.WriteLine("  -h <height>                Fixed height for all conversions (accepts . or , as decimal separator)");
            Console.WriteLine("  -u <unit>                  Unit of measurement ('in', 'cm', 'mm')");
            Console.WriteLine("  -d <density>               Print density in DPI (integer only, e.g., 203, 300)");
            Console.WriteLine();
            Console.WriteLine("  -help                      Show this help message");
        }
    }
}
