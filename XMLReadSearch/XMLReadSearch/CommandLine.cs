namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Class for command line validations
    /// </summary>
    public static class CommandLine
    {
        /// <summary>
        /// Validates the user command line input
        /// </summary>
        /// <param name="args"> The command line input</param>
        /// <returns> Boolean value true if input is valid and false othervise </returns>
        public static void ValidateArgument(string[] args)
        {
            if (args.Length != 1)
            {
                throw new CommandLineException($"Error: Invalid input. Program usage is as below.\n[{System.AppDomain.CurrentDomain.FriendlyName}][XML file path]", (int)CommandeLineExceptionCode.InvalidCommandLineInput);
            }
        }
    }
}
