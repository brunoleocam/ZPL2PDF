using System;

namespace ZPL2PDF
{
    /// <summary>
    /// Represents daemon configuration and status information
    /// </summary>
    public class DaemonInfo
    {
        /// <summary>
        /// Gets or sets the listen folder path
        /// </summary>
        public string ListenFolder { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label width
        /// </summary>
        public string LabelWidth { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the label height
        /// </summary>
        public string LabelHeight { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unit of measurement
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the print density (DPI)
        /// </summary>
        public string PrintDensity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the start time
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the process ID
        /// </summary>
        public int ProcessId { get; set; } = 0;
    }
}
