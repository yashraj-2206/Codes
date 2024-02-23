using System;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Xml reader exception class
    /// </summary>
    public class XmlReaderException : Exception
    {
        public string message { get; set; }
        public int errorCode { get; set; }
        public string innerExceptionMessage { get; set; }
        /// <summary>
        /// Dislaying the error message and code
        /// </summary>
        /// <param name="message"> Error messsage to display </param>
        /// <param name="errorCode"> Code of the error occured </param>
        public XmlReaderException(string message, int errorCode) : base(message)
        {
            this.message = message;
            this.errorCode = errorCode;
        }

        public XmlReaderException(string message, string innerExceptionMessage) : base(message)
        {
            this.message = message;
            this.innerExceptionMessage = innerExceptionMessage;

        }

    }
}
