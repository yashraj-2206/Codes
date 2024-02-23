using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System;
using System.Diagnostics.SymbolStore;
using System.Net;


namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Validating methods for xml
    /// </summary>
    public class XmlValidation
    {
        /// <summary>
        /// Boolean value for indicating error in xml
        /// </summary>
        private bool isCorrectXmlData = true;

        /// <summary>
        /// Dictionary storing the data and message of error device
        /// </summary>
        private Dictionary<string, string> errorDeviceInfo = new Dictionary<string, string>();

        /// <summary>
        /// Dev node for the error device
        /// </summary>
        private XmlNode errorDeviceNode;

        /// <summary>
        /// Index of the error device
        /// </summary>
        private int errorDeviceIndex = 1;

        /// <summary>
        /// For storing error message occuring while validation fail
        /// </summary>
        private string errorMessage = string.Empty;

        /// <summary>
        /// Indicates if the Serial Number is duplicate
        /// </summary>
        private bool isDuplicateSrNo = false;

        /// <summary>
        /// Indicates if the IP Address is duplicate
        /// </summary>
        private bool isDuplicateIP = false;

        /// <summary>
        /// Validatating input file
        /// </summary>
        /// <param name="xmlPath"> Path given in the command line </param>
        /// <returns> Dictionary containing data of the error device </returns>
        /// <exception cref="XmlReaderException"> Exception of invalid format of xml file </exception>
        public Dictionary<string, string> ValidateXml(string xmlPath)
        {
            try
            {
                if (!File.Exists(xmlPath))
                {
                    throw new XmlReaderException(Constants.FILE_NOT_EXISTS_ERROR_MESSAGE, (int)XmlFileExceptionCode.FileNotExist);
                }
                else if (Path.GetExtension(xmlPath).ToLower() != Constants.XML_EXTENSION)
                {
                    throw new XmlReaderException(Constants.INVALID_XML_FILE_ERROR_MESSAGE, (int)XmlFileExceptionCode.NotXmlExtension);
                }
                else if (new FileInfo(xmlPath).Length <= Constants.EMPTY_FILE_SIZE)
                {
                    throw new XmlReaderException(Constants.EMPTY_FILE_ERROR_MESSAGE, (int)XmlFileExceptionCode.EmptyFile);
                }

                XDocument xDoc = XDocument.Load(xmlPath);

                // Checking if there are devices or not
                if (xDoc.Descendants(Constants.XML_TAG_NAME_FOR_DEV).Count() == 0)
                {
                    throw new XmlReaderException(Constants.EMPTY_FILE_ERROR_MESSAGE, (int)XmlFileExceptionCode.NoDevicePresent);
                }

                string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Constants.XSD_NAME}");

                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add("", xsdPath);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                XmlNodeList nodeList = xmlDoc.SelectNodes("//Devices/Dev");
                xmlDoc.Schemas.Add(schemaSet);

                errorDeviceNode = nodeList[0];
                List<string> listOfSrNo = new List<string>();
                List<string> listOfIP = new List<string>();

                // Iterates devices  
                foreach (XmlNode node in nodeList)
                {
                    // Validates the device
                    xmlDoc.Validate(ValidationCallback, node);

                    if (!isCorrectXmlData || IsSrNoDuplicate(listOfSrNo) || IsIpAddressDuplicate(listOfIP))
                    {
                        SetErrorMessages();
                        return errorDeviceInfo;
                    }

                    errorDeviceNode = nodeList[errorDeviceIndex];
                    errorDeviceIndex++;
                }

                return errorDeviceInfo;
            }
            catch (XmlException)
            {
                throw new XmlReaderException(Constants.FILE_FORMAT_ERROR_MESSAGE, (int)XmlFileExceptionCode.InvalidFile);
            }
            catch (XmlReaderException e)
            {
                throw e;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new XmlReaderException($"Error: File access denied \nDetail Error: {e.Message}", e.Message);
            }
            catch (XmlSchemaException e)
            {
                throw new XmlReaderException("Error: Invalid xsd file", e.Message);
            }
            catch (Exception e)
            {
                throw new XmlReaderException("Error: Unknown error occured", e.Message);
            }
        }

        /// <summary>
        /// Indicates if the device has error
        /// </summary>
        /// <param name="sender"> The object invoking the Callback function </param>
        /// <param name="e"> Error message </param>
        public void ValidationCallback(object sender, ValidationEventArgs e)
        {
            errorMessage = e.Message;
            isCorrectXmlData = false;
        }

        public bool IsSrNoDuplicate(List<string> listOfSrNo)
        {
            XmlAttribute srno = errorDeviceNode.Attributes[Constants.XML_DEV_ATTRIBUTE_NAME_FOR_SR_NO];
            string serialNumber = srno.InnerText;

            // Checks if the Serial Number is Duplicate
            if (listOfSrNo.Contains(serialNumber))
            {
                isDuplicateSrNo = true;
                return true;
            }
            else
            {
                listOfSrNo.Add(serialNumber);
                return false;
            }
        }

        public bool IsIpAddressDuplicate(List<string> listOfIP)
        {
            XmlNode ip = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_IP_ADDRESS);

            string ipAddress = ip.InnerText;

            // Checks if the IP Address is Duplicate
            if (listOfIP.Contains(ipAddress))
            {
                isDuplicateIP = true;
                return true;
            }
            else
            {
                listOfIP.Add(ipAddress);
                return false;
            }
        }

        /// <summary>
        /// Sets the error messages to display in the device having error
        /// </summary>
        public void SetErrorMessages()
        {
            Console.WriteLine(errorMessage);
            errorDeviceInfo.Add("Device Index", errorDeviceIndex.ToString());
            errorDeviceInfo.Add("SerialNumber:", GetSerialNumberErrorMessage());
            errorDeviceInfo.Add("IP Address:", GetIPAddressErrorMessage());
            errorDeviceInfo.Add("Device Name:", GetDevNameErrorMessage());
            errorDeviceInfo.Add("ModelName:", GetModelNameErrorMessage());
            errorDeviceInfo.Add("Type:", GetTypeErrorMessage());
            errorDeviceInfo.Add("Port No:", GetPortNoErrorMessage());
            errorDeviceInfo.Add("UseSSL:", GetUseSSLErrorMessage());
            errorDeviceInfo.Add("Password:", GetPasswordErrorMessage());
        }

        /// <summary>
        /// Validating Serial Number
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetSerialNumberErrorMessage()
        {
            XmlAttribute attr = errorDeviceNode.Attributes[Constants.XML_DEV_ATTRIBUTE_NAME_FOR_SR_NO];

            if (attr == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            string valueOfSerialNumber = attr.InnerText;


            if (string.IsNullOrWhiteSpace(attr.InnerText))
            {
                return Constants.EMPTY_MESSAGE;
            }

            if (isDuplicateSrNo)
            {
                return $"{valueOfSerialNumber}{Constants.DUPLICATE_MESSAGE}";
            }

            if (!Regex.IsMatch(valueOfSerialNumber, Constants.PATTERN_FOR_SERIAL_NUMBER))
            {
                return $"{valueOfSerialNumber}{Constants.INVALID_CHARACTER_MESSAGE}";
            }

            if (attr.InnerText.Length != Constants.SERIAL_NUMBER_LENGTH)
            {
                return $"{valueOfSerialNumber}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }

            return $"{attr.InnerText}";


        }

        /// <summary>
        /// Validating IP Address
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetIPAddressErrorMessage()
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_IP_ADDRESS);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_IP_ADDRESS}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_IP_ADDRESS}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            string strValueOfIP = currentNode.InnerText;

            if (string.IsNullOrWhiteSpace(currentNode.InnerText))
            {
                return Constants.EMPTY_MESSAGE;
            }

            if (isDuplicateIP)
            {
                return $"{strValueOfIP}{Constants.DUPLICATE_MESSAGE}";
            }

            if (!Regex.IsMatch(currentNode.InnerText, Constants.CHARACTER_PATTERN_FOR_IP_ADDRESS))
            {
                return $"{strValueOfIP}{Constants.INVALID_CHARACTER_MESSAGE}";
            }

            if (currentNode.InnerText.Length > Constants.MAX_IP_ADDRESS_LENGTH || currentNode.InnerText.Length < Constants.MIN_IP_ADDRESS_LENGTH)
            {
                return $"{strValueOfIP}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }

            //if (!Regex.IsMatch(currentNode.InnerText, Constants.FORMAT_PATTERN_FOR_IP_ADDRESS))
            if (!IPAddress.TryParse(strValueOfIP, out _))
            {
                return $"{strValueOfIP}{Constants.FORMAT_ISSUE_MESSAGE}";
            }

            else
            {
                return $"{strValueOfIP}";
            }

        }

        /// <summary>
        /// Validating Device Name
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetDevNameErrorMessage()
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_DEVICE_NAME);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_DEVICE_NAME}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_DEVICE_NAME}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            if (currentNode.InnerText == null)
            {
                return "Empty";
            }

            string strValueOfDevName = currentNode.InnerText;

            if (currentNode.InnerText.Length > Constants.MAX_DEVICE_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_DEVICE_NAME_LENGTH)
            {
                return $"{strValueOfDevName}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }
            else
            {
                return $"{strValueOfDevName}";
            }


        }

        /// <summary>
        /// Validating Model Name
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetModelNameErrorMessage()
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_MODEL_NAME);

            if (currentNode == null)
            {
                return string.Empty;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_MODEL_NAME}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_MODEL_NAME}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            string strValueOfModelName = currentNode.InnerText;
            if (currentNode.InnerText.Length > Constants.MAX_MODEL_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_MODEL_NAME_LENGTH)
            {
                return $"{strValueOfModelName}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }

            return $"{strValueOfModelName}";


        }

        /// <summary>
        /// Validating Type
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetTypeErrorMessage()
        {

            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_TYPE);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_TYPE}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_TYPE}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            string strValueOfType = currentNode.InnerText;

            if (strValueOfType == string.Empty)
            {
                return Constants.EMPTY_MESSAGE;
            }

            if (!Enum.TryParse<DeviceTypes>(strValueOfType, out _))
            {
                return $"{strValueOfType}{Constants.INVALID_CHARACTER_MESSAGE}";
            }

            return $"{strValueOfType}";
        }

        /// <summary>
        /// Validating Port No
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetPortNoErrorMessage()
        {

            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_COMMSETTING).SelectSingleNode(Constants.XML_TAG_NAME_FOR_PORT_NO);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_PORT_NO}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_PORT_NO}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }


            string strValueOfPortNumber = currentNode.InnerText;

            if (string.IsNullOrWhiteSpace(currentNode.InnerText))
            {
                return $"{strValueOfPortNumber}{Constants.EMPTY_MESSAGE}";
            }

            if (!(int.TryParse(strValueOfPortNumber, out int portNo) && (Constants.MIN_PORT_NUMBER_VALUE <= portNo && portNo <= Constants.MAX_PORT_NUMBER_VALUE)))
            {
                return $"{strValueOfPortNumber}{Constants.FORMAT_ISSUE_MESSAGE}";
            }

            return $"{strValueOfPortNumber}";
        }

        /// <summary>
        /// Validating UseSSL
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetUseSSLErrorMessage()
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_COMMSETTING).SelectSingleNode(Constants.XML_TAG_NAME_FOR_USE_SSL);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if (errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_USE_SSL}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_USE_SSL}").Count != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            string strValueOfUseSSL = currentNode.InnerText.ToUpper();

            if (string.IsNullOrWhiteSpace(currentNode.InnerText))
            {
                return $"{strValueOfUseSSL}{Constants.EMPTY_MESSAGE}";
            }

            if (!bool.TryParse(strValueOfUseSSL, out _))
            {
                return $"{strValueOfUseSSL}{Constants.FORMAT_ISSUE_MESSAGE}";
            }

            return $"{strValueOfUseSSL}";


        }

        /// <summary>
        /// Validating Password
        /// </summary>
        /// <returns> Message to be printed </returns>
        public string GetPasswordErrorMessage()
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(Constants.XML_TAG_NAME_FOR_COMMSETTING).SelectSingleNode(Constants.XML_TAG_NAME_FOR_PASSWORD);

            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }

            if ((errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_PASSWORD}").Count + errorDeviceNode.SelectNodes($"{Constants.XML_TAG_NAME_FOR_COMMSETTING}/{Constants.XML_TAG_NAME_FOR_PASSWORD}").Count) != 1)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }

            string strValueOfPassword = currentNode.InnerText;

            if (currentNode.InnerText.Length > Constants.MAX_PASSWORD_LENGTH)
            {
                return $"{strValueOfPassword}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }

            return $"{strValueOfPassword}";
        }

        public void Validation()
        {
            
        }
        public bool IsNull(Object o)
        {
            if (o == null)
                return false;
            else
                return true;
        }
    }
}

