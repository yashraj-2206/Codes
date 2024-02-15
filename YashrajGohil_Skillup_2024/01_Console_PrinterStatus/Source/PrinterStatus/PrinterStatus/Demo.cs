using System;

namespace PrinterStatus
{
    /// <summary>
    /// Class containing methods for validating and prining the status of the printer
    /// </summary>
    internal class Demo
    {
        /// <summary>
        /// Runs demo program code
        /// </summary>
        public void Run()
        {
            try
            {
                Console.WriteLine("\n|===================================================|");
                Console.WriteLine("  Printer Status Check");
                Console.WriteLine("|===================================================|");

                int paperCount = GetPaperCount();
                PrintPaperStatus(paperCount);
            }
            catch (Exception e)
            {
                Console.WriteLine("Printer Status check operation failed.");
                Console.WriteLine($"Detail Error: {e.Message}");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Method for printing printer status based paper count
        /// </summary>
        /// <param name="paperCount">Count of paper in the paper tray </param>
        public void PrintPaperStatus(int paperCount)
        {
            if (Constants.NO_PAPER_COUNT <= paperCount && paperCount <= int.MaxValue)
            {
                Console.Write(Constants.STATUS_HEAD);
                if (paperCount == Constants.NO_PAPER_COUNT)
                {
                    Console.WriteLine("\"No paper\"");
                }
                else if (Constants.NO_PAPER_COUNT <= paperCount && paperCount <= Constants.MIN_PAPER_COUNT)
                {
                    Console.WriteLine("\"Printer not ready to Print due to Low paper\"");
                }
                else if (Constants.MIN_PAPER_COUNT < paperCount && paperCount <= int.MaxValue)
                {
                    Console.WriteLine("\"Printer is ready to Print\"");
                }
            }

        }

        /// <summary>
        /// Takes user input count of papers in the tray
        /// </summary>
        /// <returns>The count of papers in the tray</returns>
        public int GetPaperCount()
        {
            int userPaperCount;
 
            while (true)
            {
                Console.Write($"\nEnter the paper count [Range: {Constants.NO_PAPER_COUNT} to {int.MaxValue}]: ");

                try
                {
                    userPaperCount = int.Parse(Console.ReadLine());
                    Console.WriteLine();
                    break;
                }
                catch
                {
                    Console.WriteLine();
                    Console.WriteLine($"{Constants.STATUS_HEAD}[ERROR] Entered paper count is invalid.");
                }
            }
            return userPaperCount;
        } 
    }
}
