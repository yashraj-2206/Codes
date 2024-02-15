using System;

namespace FileTransferRate
{
    /// <summary>
    /// Class containing methods for validating proper input 
    /// and calculation and displaing the time taken to transfer a file
    /// </summary>
    public class Demo
    {
        /// <summary>
        /// Runs demo program code for calculating File transfer rate 
        /// </summary>
        public void Run()
        {

            //Displaing heading
            DisplayHeading("\tFILE TRANSFER TIME CALCULATION\n\tTransmission rate: " +
                $"{Constants.FILE_TRANSMISSION_RATE} bytes/sec");


            //user input for size of the file
            int fileSize = GetFileSize();

            // user input for unit of size of the file
            FileSizeUnits fileSizeUnit = GetFileSizeUnit();

            // calculation of time
            decimal timeInSec = GetFileTransferTime(fileSize, fileSizeUnit);

            // displaying the time
            DisplayFileTransferTime(fileSize, fileSizeUnit, timeInSec);

        }

        /// <summary>
        /// takes user input for file size and validates it
        /// </summary>
        /// <returns> size of file </returns>
        public int GetFileSize()
        {
            while (true)
            {
                int fileSize;

                Console.Write($"\nEnter the file size [Range: 0 to {int.MaxValue}]: ");

                bool isValidSize = int.TryParse(Console.ReadLine(), out fileSize);

                if (!isValidSize || fileSize < 0)
                {
                    Console.WriteLine("\nEntered file size is invalid.");
                }
                else
                {
                    return fileSize;
                }
            }
        }

        /// <summary>
        /// takes user input for file size unit and validates it
        /// </summary>
        /// <returns>unit of size of file</returns>
        public FileSizeUnits GetFileSizeUnit()
        {
            while (true)
            {
                Console.Write("\nEnter the file size unit [B or KB or MB]: ");
                string fileSizeUnit = Console.ReadLine().ToUpper();

                switch (fileSizeUnit)
                {
                    case Constants.BYTE_SYMBOL:
                        return FileSizeUnits.Byte;

                    case Constants.KILOBYTE_SYMBOL:
                        return FileSizeUnits.KiloByte;

                    case Constants.MEGABYTE_SYMBOL:
                        return FileSizeUnits.MegaByte;

                    default:
                        Console.WriteLine("\nEntered file size unit is invalid.");
                        break;
                }
            }
        }

        /// calculates the time taken to transfer the file in seconds
        /// </summary>
        /// <param name="fileSize"> size of the file </param>
        /// <param name="sizeUnit"> unit of size of the file </param>
        /// <returns> total time to transfer the file in seconds </returns>
        public decimal GetFileTransferTime(int fileSize, FileSizeUnits sizeUnit)
        {
            switch (sizeUnit)
            {
                case FileSizeUnits.Byte:
                    return (decimal)fileSize / Constants.FILE_TRANSMISSION_RATE;

                case FileSizeUnits.KiloByte:
                    return (decimal)fileSize * Constants.BYTES_IN_KILOBYTE / Constants.FILE_TRANSMISSION_RATE;

                case FileSizeUnits.MegaByte:
                    return (decimal)fileSize * Constants.BYTES_IN_MEGABYTE / Constants.FILE_TRANSMISSION_RATE;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Displays the calculated time in D,H,M,S,Ms pattern
        /// </summary>
        /// <param name="fileSize"> size of the file </param>
        /// <param name="fileSizeUnit"> unit of the size of the file </param>
        /// <param name="timeInSec">time taken to transfer the file in seconds </param>
        public void DisplayFileTransferTime(int fileSize, FileSizeUnits fileSizeUnit, decimal timeInSec)
        {
            DisplayHeading("\tCALCULATION RESULT");

            Console.WriteLine("\tFile transfer time calculation operation copleted successfully.");
            Console.WriteLine($"\tTotal timed required to transfer file of size {fileSize} {fileSizeUnit}:\n");

            Console.WriteLine($"\t\tDays           : {(int)(timeInSec / Constants.SECONDS_IN_A_DAY)}");
            Console.WriteLine($"\t\tHours          : {(int)(timeInSec / Constants.SECONDS_IN_AN_HOUR) % Constants.HOURS_IN_A_DAY}");
            Console.WriteLine($"\t\tMinutes        : {(int)((timeInSec / Constants.SECONDS_IN_A_MINUTE) % Constants.MINUTES_IN_AN_HOUR)}");
            Console.WriteLine($"\t\tSeconds        : {(int)((timeInSec) % Constants.SECONDS_IN_A_MINUTE)}");
            Console.WriteLine($"\t\tMilliseconds   : {(int)((timeInSec%1) * Constants.MILLISECONDS_IN_A_SECOND)}");

        }

        /// <summary>
        /// to display paterend heading
        /// </summary>
        /// <param name="message"> message to display in heading </param>
        public void DisplayHeading(string message)
        {
            Console.WriteLine(Constants.BORDER);
            Console.WriteLine(message);
            Console.WriteLine(Constants.BORDER);
        }
    }
}
