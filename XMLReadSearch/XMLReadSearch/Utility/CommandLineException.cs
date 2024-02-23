using System;
using System.Net.Http;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Represents an exception that occurs during command-line processing.
    /// </summary>
    public class CommandLineException : Exception
    {
        public string message { get; set; }
        public int errorCode { get; set; }
        public string innerExceptionMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineException"/> class with the specified error message and error code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errorCode">The error code associated with the exception.</param>
        public CommandLineException(string message, int errorCode) : base (message)
        {
            this.message = message;
            this.errorCode = errorCode;
        }

        public CommandLineException(string message, int errorCode, string innerExceptionMessage) : base(message) 
        {
            this.message = message;
            this.errorCode = errorCode;
            this.innerExceptionMessage = innerExceptionMessage;
        }
    }

}
