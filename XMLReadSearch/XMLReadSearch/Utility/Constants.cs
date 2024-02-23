using System.Runtime.CompilerServices;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Constants
    /// </summary>
    public class Constants
    {
        // Minimum size of an empty file
        public const int EMPTY_FILE_SIZE = 0;

        // Length of serial number
        public const int SERIAL_NUMBER_LENGTH = 16;

        // Minimum length of ip address
        public const int MIN_IP_ADDRESS_LENGTH = 1;

        // Maximum length of ip address
        public const int MAX_IP_ADDRESS_LENGTH = 15;

        // Minimum length of device name
        public const int MIN_DEVICE_NAME_LENGTH = 0;

        // Maximum length of device name
        public const int MAX_DEVICE_NAME_LENGTH = 24;

        // Minimum length of model name
        public const int MIN_MODEL_NAME_LENGTH = 0;

        // Maximum length of model name
        public const int MAX_MODEL_NAME_LENGTH = 24;

        // Minimum length of port number
        public const int MIN_PORT_NUMBER_VALUE = 1;

        // Maximum length of port number
        public const int MAX_PORT_NUMBER_VALUE = 65535;

        // Minimum length of password
        public const int MIN_PASSWORD_LENGTH = 0;

        // Maximum length of password
        public const int MAX_PASSWORD_LENGTH = 64;

        /// <summary>
        /// Message indicating that node or attribute is not present.
        /// </summary>
        public const string NOT_PRESENT_MESSAGE = "(Not present)";

        /// <summary>
        /// Message indicating that node or attribute value is null.
        /// </summary>
        public const string EMPTY_MESSAGE = "(Empty)";

        /// <summary>
        /// Message indicating that a duplicate value exists.
        /// </summary>
        public const string DUPLICATE_MESSAGE = "(Duplicate)";

        /// <summary>
        /// Message indicating that a duplicate tag exists.
        /// </summary>
        public const string DUPLICATE_TAG_MESSAGE = "(Duplicate Tag)";

        /// <summary>
        /// Message indicating the presence of unsupported characters.
        /// </summary>
        public const string INVALID_CHARACTER_MESSAGE = "(Not supported characters)";

        /// <summary>
        /// Message indicating an invalid length or size.
        /// </summary>
        public const string LENGTH_MISMATCH_MESSAGE = "(Invalid length)";

        /// <summary>
        /// Message indicating a format that is not supported.
        /// </summary>
        public const string FORMAT_ISSUE_MESSAGE = "(Not supported format)";

        /// <summary>
        /// Represents the file extension ".xml".
        /// </summary>
        public const string XML_EXTENSION = ".xml";

        /// <summary>
        /// Xsd file name
        /// </summary>
        public const string XSD_NAME = "XMLValidatingSchema.xsd";

        /// <summary>
        /// Error message indicating that the file doesn't exist.
        /// </summary>
        public const string FILE_NOT_EXISTS_ERROR_MESSAGE = "Error: File not exist. Please provide a valid file path.";

        /// <summary>
        /// Error message indicating that the provided file is not an XML file due to incorrect file extension.
        /// </summary>
        public const string INVALID_XML_FILE_ERROR_MESSAGE = "Error: Given file is not an XML file. The file extension is wrong.";

        /// <summary>
        /// Error message indicating that the XML file is empty. Device data is not present in the file.
        /// </summary>
        public const string EMPTY_FILE_ERROR_MESSAGE = "Error: The XML file is empty. Device data is not present in the file.";

        /// <summary>
        /// Error message indicating a general file format error, specifically when the file is not in XML format.
        /// </summary>
        public const string FILE_FORMAT_ERROR_MESSAGE = "Error: File format error.Given file is not an XML file.";

        /// <summary>
        /// Error message indicating invalid device information. Provides details for reference.
        /// </summary>
        public const string INVALID_DEVICE_ERROR_MESSAGE = "Error: Invalid device information. Please refer below details.";

        /// <summary>
        /// Error message indicating invalid input choice. Prompt to select from available options.
        /// </summary>
        public const string INVALID_CHOICE_ERROR_MESSAGE = "Error: Invalid input. Please select from above options.";

        /// <summary>
        /// Regular expression pattern for validating serial numbers. It allows uppercase letters and digits.
        /// </summary>
        public const string PATTERN_FOR_SERIAL_NUMBER = @"^[A-Z0-9]+$";

        /// <summary>
        /// Regular expression pattern for validating IP addresses. It allows only digits and periods.
        /// </summary>
        public const string CHARACTER_PATTERN_FOR_IP_ADDRESS = @"^[0-9.]+$";

        /// <summary>
        /// Regular expression pattern for validating the format of an IP address.
        /// </summary>
        public const string FORMAT_PATTERN_FOR_IP_ADDRESS = @"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";

        /// <summary>
        /// Error message indicating an invalid child element 'Address'.
        /// </summary>
        public const string INVALID_ADDRESS_ELEMENT = "invalid child element 'Address'";

        /// <summary>
        /// Error message indicating an invalid child element 'DevName'.
        /// </summary>
        public const string INVALID_DEV_NAME_ELEMENT = "invalid child element 'DevName'";

        /// <summary>
        /// Error message indicating an invalid child element 'Modelname'.
        /// </summary>
        public const string INVALID_MODEL_NAME_ELEMENT = "invalid child element 'Modelname'";

        /// <summary>
        /// Error message indicating an invalid child element 'Type'.
        /// </summary>
        public const string INVALID_TYPE_ELEMENT = "invalid child element 'Type'";

        /// <summary>
        /// Error message indicating an invalid child element 'PortNo'.
        /// </summary>
        public const string INVALID_PORT_NUMBER_ELEMENT = "invalid child element 'PortNo'";

        /// <summary>
        /// Error message indicating an invalid child element 'UseSSL'.
        /// </summary>
        public const string INVALID_USE_SSL_ELEMENT = "invalid child element 'UseSSL'";

        /// <summary>
        /// Error message indicating an invalid child element 'Password'.
        /// </summary>
        public const string INVALID_PASSWORD_ELEMENT = "invalid child element 'Password'";

        public const string XML_TAG_NAME_FOR_DEVICES = "Devices";
        public const string XML_TAG_NAME_FOR_DEV = "Dev";
        public const string XML_TAG_NAME_FOR_IP_ADDRESS = "Address";
        public const string XML_TAG_NAME_FOR_DEVICE_NAME = "DevName";
        public const string XML_TAG_NAME_FOR_MODEL_NAME = "ModelName";
        public const string XML_TAG_NAME_FOR_TYPE = "Type";
        public const string XML_TAG_NAME_FOR_COMMSETTING = "CommSetting";
        public const string XML_TAG_NAME_FOR_PORT_NO = "PortNo";
        public const string XML_TAG_NAME_FOR_USE_SSL = "UseSSL";
        public const string XML_TAG_NAME_FOR_PASSWORD = "Password";
        public const string XML_DEV_ATTRIBUTE_NAME_FOR_SR_NO = "SrNo";
     

    }
}
