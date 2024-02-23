using System;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Class for command line operations
    /// </summary>
    public static class CommandLine
    {
        public static string Usage { get; set; } = $"[{System.AppDomain.CurrentDomain.FriendlyName}][XML file path]";
        /// <summary>
        /// Validates the user command line input
        /// </summary>
        /// <param name="args"> The command line input </param>
        public static void ValidateArgument(string[] args)
        {
            if (args.Length != 1)
            {
                throw new CommandLineException($"\nError: Invalid input. ({(int)CommandeLineExceptionCode.InvalidCommandLineInput}) Program usage is as below.\n{Usage}", (int)CommandeLineExceptionCode.InvalidCommandLineInput);
            }
        }
    }
}
