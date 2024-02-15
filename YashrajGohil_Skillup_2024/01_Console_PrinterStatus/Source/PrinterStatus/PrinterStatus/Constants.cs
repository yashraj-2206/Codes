namespace PrinterStatus
{
    /// <summary>
    /// Constants
    /// </summary>
    internal class Constants 
    {
        /// <summary>
        /// Paper count for empty tray
        /// </summary>
        public const int NO_PAPER_COUNT = 0;

        /// <summary>
        /// Minimum paper count for printer to start printing
        /// </summary>
        public const int MIN_PAPER_COUNT = 10;

        /// <summary>
        /// Printing ahead of the current status of printer
        /// </summary>
        public const string STATUS_HEAD = "Printer Status: ";
    }
}
