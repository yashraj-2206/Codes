using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System;


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
        public Dictionary<string, string> IsValidXML(string xmlPath)
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
                if (xDoc.Descendants(XmlTagNames.Dev.ToString()).Count() == 0)
                {
                    throw new XmlReaderException(Constants.EMPTY_FILE_ERROR_MESSAGE, (int)XmlFileExceptionCode.NoDevicePresent);
                }

                string xsdPath = @"C:\Projects\Skillup\GIT\YashrajGohil_Skillup_2024\04_Console_XMLReadSearch\Work\XMLValidatingSchema.xsd";
                //string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XMLValidatingSchema.xsd");

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

                   
                    XmlAttribute srno = node.Attributes[XmlAttributeNames.SrNo.ToString()];
                    if (srno != null)
                    {
                        string serialNumber = srno.InnerText;

                        // Checks if the Serial Number is Duplicate
                        if (listOfSrNo.Contains(serialNumber))
                        {
                            isDuplicateSrNo = true;
                            isCorrectXmlData = false;
                        }
                        else
                        {
                            listOfSrNo.Add(serialNumber);
                        }
                    }


                    XmlNode ip = node.SelectSingleNode(XmlTagNames.Address.ToString());

                    if (ip != null)
                    {
                        string ipAddress = ip.InnerText;

                        // Checks if the IP Address is Duplicate
                        if (listOfIP.Contains(ipAddress))
                        {
                            isDuplicateIP = true;
                            isCorrectXmlData = false;
                        }
                        else
                        {
                            listOfIP.Add(ipAddress);
                        }
                    }


                    if (!isCorrectXmlData)
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

        /// <summary>
        /// Sets the error messages to display in the device having error
        /// </summary>
        public void SetErrorMessages()
        {
            errorDeviceInfo.Add("Device Index", errorDeviceIndex.ToString());
            errorDeviceInfo.Add("SerialNumber:", GetSerialNumberErrorMessage(errorMessage));
            errorDeviceInfo.Add("IP Address:", GetIPAddressErrorMessage(errorMessage));
            errorDeviceInfo.Add("Device Name:", GetDevNameErrorMessage(errorMessage));
            errorDeviceInfo.Add("ModelName:", GetModelNameErrorMessage(errorMessage));
            errorDeviceInfo.Add("Type:", GetTypeErrorMessage(errorMessage));
            errorDeviceInfo.Add("Port No:", GetPortNoErrorMessage(errorMessage));
            errorDeviceInfo.Add("UseSSL:", GetUseSSLErrorMessage(errorMessage));
            errorDeviceInfo.Add("Password:", GetPasswordErrorMessage(errorMessage));
        }

        /// <summary>
        /// Validating Serial Number
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetSerialNumberErrorMessage(string errorMessage)
        {
            XmlAttribute attr = errorDeviceNode.Attributes[XmlAttributeNames.SrNo.ToString()];

            if (attr == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else
            {
                string valueOfSerialNumber = attr.InnerText;

                if (string.IsNullOrWhiteSpace(attr.InnerText))
                {
                    return Constants.EMPTY_MESSAGE;
                }
                else if (isDuplicateSrNo)
                {
                    return $"{valueOfSerialNumber}{Constants.DUPLICATE_MESSAGE}";
                }
                else if (!Regex.IsMatch(valueOfSerialNumber, Constants.PATTERN_FOR_SERIAL_NUMBER))
                {
                    return $"{valueOfSerialNumber}{Constants.INVALID_CHARACTER_MESSAGE}";
                }
                else if (attr.InnerText.Length != Constants.SERIAL_NUMBER_LENGTH)
                {
                    return $"{valueOfSerialNumber}{Constants.LENGTH_MISMATCH_MESSAGE}";
                }
                else
                {   
                    return $"{attr.Value}";
                }
            }
        }

        /// <summary>
        /// Validating IP Address
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetIPAddressErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.Address.ToString());
            if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else if (errorMessage.Contains(Constants.INVALID_ADDRESS_ELEMENT))
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else
            {
                string strValueOfIP = currentNode.InnerText;

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return Constants.EMPTY_MESSAGE;
                }
                else if (isDuplicateIP)
                {
                    return $"{strValueOfIP}{Constants.DUPLICATE_MESSAGE}";
                }
                else if (!Regex.IsMatch(currentNode.InnerText, Constants.CHARACTER_PATTERN_FOR_IP_ADDRESS))
                {
                    return $"{strValueOfIP}{Constants.INVALID_CHARACTER_MESSAGE}";
                }
                else if (currentNode.InnerText.Length > Constants.MAX_IP_ADDRESS_LENGTH || currentNode.InnerText.Length < Constants.MIN_IP_ADDRESS_LENGTH)
                {
                    return $"{strValueOfIP}{Constants.LENGTH_MISMATCH_MESSAGE}";
                }
                else if (!Regex.IsMatch(currentNode.InnerText, Constants.FORMAT_PATTERN_FOR_IP_ADDRESS))
                {
                    return $"{strValueOfIP}{Constants.FORMAT_ISSUE_MESSAGE}";
                }
                else
                {
                    return $"{strValueOfIP}";
                }
            }
        }

        /// <summary>
        /// Validating Device Name
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetDevNameErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.DevName.ToString());
            if (errorMessage.Contains(Constants.INVALID_DEV_NAME_ELEMENT) && errorDeviceNode.SelectSingleNode(XmlTagNames.Address.ToString()) != null)
            {

                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else
            {
                if (currentNode == null)
                {

                    return Constants.NOT_PRESENT_MESSAGE;
                }
                else
                {
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
            }
        }

        /// <summary>
        /// Validating Model Name
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetModelNameErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.ModelName.ToString());

            if (currentNode == null)
            {
                return string.Empty;
            }

            string strValueOfModelName = currentNode.InnerText;

            if (errorMessage.Contains(Constants.INVALID_MODEL_NAME_ELEMENT) && errorDeviceNode.SelectSingleNode(XmlTagNames.DevName.ToString()) != null)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else if (currentNode.InnerText.Length > Constants.MAX_MODEL_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_MODEL_NAME_LENGTH)
            {
                return $"{strValueOfModelName}{Constants.LENGTH_MISMATCH_MESSAGE}";
            }
            else
            {
                return $"{strValueOfModelName}";
            }
        }

        /// <summary>
        /// Validating Type
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetTypeErrorMessage(string errorMessage)
        {

            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.Type.ToString());

            if (errorMessage.Contains(Constants.INVALID_TYPE_ELEMENT))
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else
            {
                string strValueOfType = currentNode.InnerText;

                if (strValueOfType == string.Empty)
                {
                    return Constants.EMPTY_MESSAGE;
                }
                else if (Enum.TryParse<DeviceTypes>(strValueOfType, out _))
                {
                    return $"{strValueOfType}{Constants.INVALID_CHARACTER_MESSAGE}";
                }
                else
                {
                    return $"{strValueOfType}";
                }

            }
        }

        /// <summary>
        /// Validating Port No
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetPortNoErrorMessage(string errorMessage)
        {

            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.CommSetting.ToString()).SelectSingleNode(XmlTagNames.PortNo.ToString());

            if (errorMessage.Contains(Constants.INVALID_PORT_NUMBER_ELEMENT) && errorDeviceNode.SelectSingleNode(XmlTagNames.Type.ToString()) != null)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else
            {
                string strValueOfPortNumber = currentNode.InnerText;

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return $"{strValueOfPortNumber}{Constants.EMPTY_MESSAGE}";
                }
                else if (!(int.TryParse(strValueOfPortNumber, out int portNo) && (Constants.MIN_PORT_NUMBER_VALUE <= portNo && portNo <= Constants.MAX_PORT_NUMBER_VALUE)))
                {
                    return $"{strValueOfPortNumber}{Constants.FORMAT_ISSUE_MESSAGE}";
                }
                else
                {
                    return $"{strValueOfPortNumber}";
                }
            }
        }

        /// <summary>
        /// Validating UseSSL
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetUseSSLErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.CommSetting.ToString()).SelectSingleNode(XmlTagNames.UseSSL.ToString());

            if (errorMessage.Contains(Constants.INVALID_USE_SSL_ELEMENT) && errorDeviceNode.SelectSingleNode(XmlTagNames.CommSetting.ToString()).SelectSingleNode(XmlTagNames.Type.ToString()) != null)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else
            {
                string strValueOfUseSSL = currentNode.InnerText.ToUpper();
                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return $"{strValueOfUseSSL}{Constants.EMPTY_MESSAGE}";
                }
                else if (!bool.TryParse(strValueOfUseSSL, out _))
                {
                    return $"{strValueOfUseSSL}{Constants.FORMAT_ISSUE_MESSAGE}";
                }
                else
                {
                    return $"{strValueOfUseSSL}";
                }
            }
        }

        /// <summary>
        /// Validating Password
        /// </summary>
        /// <param name="errorMessage"> Error message occured while validating node </param>
        /// <returns> Message to be printed </returns>
        public string GetPasswordErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode(XmlTagNames.CommSetting.ToString()).SelectSingleNode(XmlTagNames.Password.ToString());

            if (errorMessage.Contains(Constants.INVALID_PASSWORD_ELEMENT) && errorDeviceNode.SelectSingleNode(XmlTagNames.CommSetting.ToString()).SelectSingleNode(XmlTagNames.UseSSL.ToString()) != null)
            {
                return Constants.DUPLICATE_TAG_MESSAGE;
            }
            else if (currentNode == null)
            {
                return Constants.NOT_PRESENT_MESSAGE;
            }
            else
            {
                string strValueOfPassword = currentNode.InnerText;

                if (currentNode.InnerText.Length > Constants.MAX_PASSWORD_LENGTH)
                {
                    return $"{strValueOfPassword}{Constants.LENGTH_MISMATCH_MESSAGE}";
                }
                else
                {
                    return $"{strValueOfPassword}";
                }
            }
        }
    }
}

