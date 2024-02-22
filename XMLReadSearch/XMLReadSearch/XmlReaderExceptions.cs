using System;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Xml reader exception class
    /// </summary>
    public class XmlReaderException : Exception
    {
        /// <summary>
        /// Dislaying the error message and code
        /// </summary>
        /// <param name="message"> Error messsage to display </param>
        /// <param name="errorCode"> Code of the error occured </param>
        public XmlReaderException(string message, int errorCode)
        {
            Console.WriteLine($"\n{message} ({errorCode})");
        }

    }
}
