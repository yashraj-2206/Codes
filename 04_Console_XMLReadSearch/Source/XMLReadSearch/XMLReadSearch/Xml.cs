using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Validating methods for xml
    /// </summary>
    public class Xml
    {
        private bool isCorrectXmlData = true;
        private Dictionary<string, string> errorDeviceInfo = new Dictionary<string, string>();
        private XmlNode errorDeviceNode;
        private int errorDeviceIndex = 1;
        public Dictionary<string, string> IsValidXML(string xmlPath)
        {
            try
            {
                if (!File.Exists(xmlPath))
                {
                    throw new XmlReaderException("Error: File not exist. Please provide a valid file path.", (int)XmlFileExceptionCode.FileNotExist);
                }
                else if (Path.GetExtension(xmlPath).ToLower() != ".xml")
                {
                    throw new XmlReaderException("Error: Given file is not an XML file. The file extension is wrong.", (int)XmlFileExceptionCode.NotXmlExtension);
                }
                else if (new FileInfo(xmlPath).Length <= 0)
                {
                    throw new XmlReaderException("Error: The XML file is empty. Device data is not present in the file.", (int)XmlFileExceptionCode.EmptyFile);
                }

                XDocument xDoc = XDocument.Load(xmlPath);

                // Checking if there are devices or not
                if (xDoc.Descendants("Dev").Count() == 0)
                {
                    throw new XmlReaderException("Error: The XML file is empty. Device data is not present in the file.", (int)XmlFileExceptionCode.NoDevicePresent);
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
                foreach (XmlNode node in nodeList)
                {
                    xmlDoc.Validate(ValidationCallback, node);
                    if (!isCorrectXmlData)
                    {
                        return errorDeviceInfo;
                    }
                    errorDeviceNode = nodeList[errorDeviceIndex];
                    errorDeviceIndex++;
                }
                
                return errorDeviceInfo;
            }
            catch (XmlException)
            {
                throw new XmlReaderException("Error: File format error.Given file is not an XML file.", (int)XmlFileExceptionCode.InvalidFile);
            }
        }
        public void ValidationCallback(object sender, ValidationEventArgs e)
        {
            errorDeviceInfo.Add("Device Index", errorDeviceIndex.ToString());
            errorDeviceInfo.Add("SerialNumber:", GetSerialNumberErrorMessage(e.Message));
            errorDeviceInfo.Add("IP Address:", GetIPAddressErrorMessage(e.Message));
            errorDeviceInfo.Add("Device Name:", GetDevNameErrorMessage(e.Message));
            errorDeviceInfo.Add("ModelName:", GetModelNameErrorMessage(e.Message));
            errorDeviceInfo.Add("Type:", GetTypeErrorMessage(e.Message));
            errorDeviceInfo.Add("Port No:", GetPortNoErrorMessage(e.Message));
            errorDeviceInfo.Add("UseSSL:", GetUseSSLErrorMessage(e.Message));
            errorDeviceInfo.Add("Password:", GetPasswordErrorMessage(e.Message));

            isCorrectXmlData = false;
        }

        public string GetSerialNumberErrorMessage(string errorMessage)
        {

            if (errorMessage.Contains("SrNo"))
            {
                if (errorDeviceNode.Attributes["SrNo"] == null)
                {
                    return "(not present)";
                }
                else
                {
                    XmlAttribute attr = errorDeviceNode.Attributes["SrNo"];
                    string valueOfSerialNumber = attr.InnerText;

                    if (string.IsNullOrWhiteSpace(attr.InnerText))
                    {
                        return "Empty";
                    }
                    else if (errorMessage.Contains("uniqueSrNo"))
                    {
                        return $"{valueOfSerialNumber}(duplicate)";
                    }
                    else if (errorMessage.Contains("Pattern constraint failed"))
                    {
                        return $"{valueOfSerialNumber}(Not supported charecters)";
                    }
                    else if (attr.InnerText.Length != Constants.SERIAL_NUMBER_LENGTH)
                    {
                        return $"{valueOfSerialNumber}(invalid length)";
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

            }

            return $"{errorDeviceNode.Attributes["SrNo"].Value}";

        }

        public string GetIPAddressErrorMessage(string errorMessage)
        {

            if (errorDeviceNode.SelectSingleNode("Address") == null)
            {
                return $"(not present)";
            }
            else if (errorMessage.Contains("invalid child element 'Address'"))
            {
                return $"(duplicate Tag)";
            }
            else
            {
                XmlNode currentNode = errorDeviceNode.SelectSingleNode("Address");
                string strValueOfIP = currentNode.InnerText;

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return $"Empty";
                }
                else if (errorMessage.Contains("uniqueIP"))
                {
                    return $"(duplicate)";
                }
                else if (!Regex.IsMatch(currentNode.InnerText, @"^[0-9.]+$"))
                {
                    return $"{strValueOfIP}(Not supported characters)";
                }
                else if (currentNode.InnerText.Length > Constants.MAX_IP_ADDRESS_LENGTH || currentNode.InnerText.Length < Constants.MIN_IP_ADDRESS_LENGTH)
                {
                    return $"{strValueOfIP}(invalid length)";
                }
                else if (!Regex.IsMatch(currentNode.InnerText, @"^\b([0-2]?[0-9]{0,2}\.){3}[0-2]?[0-9]{0,2}\b$"))
                {
                    return $"{strValueOfIP}(Not supported format)";
                }
                else
                {
                    return $"{strValueOfIP}";
                }
            }
        }

        public string GetDevNameErrorMessage(string errorMessage)
        {
            if (errorMessage.Contains("invalid child element 'DevName'") && errorDeviceNode.SelectSingleNode("Address") != null)
            {
                
                return $"(duplicate Tag)"; 
            }
            else
            {
                if (errorDeviceNode.SelectSingleNode("DevName") == null)
                {
                    
                    return $"(not present)";
                }
                else
                {
                    XmlNode currentNode = errorDeviceNode.SelectSingleNode("DevName");
                    string strValueOfDevName = currentNode.InnerText;

                    if (currentNode.InnerText.Length > Constants.MAX_DEVICE_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_DEVICE_NAME_LENGTH)
                    {
                        return $"{strValueOfDevName}(invalid length)";
                    }
                    else
                    {
                        return $"{strValueOfDevName}";
                    }
                }
            }
        }

        public string GetModelNameErrorMessage(string errorMessage)
        {
            XmlNode currentNode = errorDeviceNode.SelectSingleNode("ModelName");
            string strValueOfModelName = currentNode.InnerText;
            if (errorDeviceNode.SelectSingleNode("ModelName") == null)
            {
                return string.Empty;
            }
            if (errorMessage.Contains("invalid child element 'ModelName'") && errorDeviceNode.SelectSingleNode("DevName") != null )
            {
                return $"(duplicate Tag)";
            }
            else if (currentNode.InnerText.Length > Constants.MAX_MODEL_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_MODEL_NAME_LENGTH)
            {
                return $"{strValueOfModelName}(invalid length)";
            }
            else
            {
                return $"{strValueOfModelName}";
            }
        }

        public string GetTypeErrorMessage(string errorMessage)
        {
            List<string> deviceTypes = new List<string> { "A3", "A4" };


            if (errorMessage.Contains("invalid child element 'Type'") && errorDeviceNode.SelectSingleNode("ModelName") != null)
            {
                return $"(duplicate Tag)";
            }
            else if (errorDeviceNode.SelectSingleNode("Type") == null)
            {
                return "(not present)";
            }
            else
            {
                XmlNode currentNode = errorDeviceNode.SelectSingleNode("Type");
                string strValueOfType = currentNode.InnerText;
                if (strValueOfType == "")
                {
                    return "Empty";
                }
                else if (!deviceTypes.Contains(strValueOfType))
                {
                    return $"{strValueOfType}(Not supported charecters)";
                }
                else
                {
                    return $"{strValueOfType}";
                }
            }
        }

        public string GetPortNoErrorMessage(string errorMessage)
        {

            

            if (errorMessage.Contains("invalid child element 'PortNo'") && errorDeviceNode.SelectSingleNode("Type") != null)
            {
                return "(duplicate Tag)";
            }
            else if (errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("PortNo") == null)
            {
                return "(not present)";
            }
            else
            {
                XmlNode currentNode = errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("PortNo");
                string strValueOfPortNumber = currentNode.InnerText;

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return $"{strValueOfPortNumber}Empty";
                }
                else if (!(int.TryParse(strValueOfPortNumber, out int portNo) && (Constants.MIN_PORT_NUMBER_VALUE <= portNo && portNo <= Constants.MAX_PORT_NUMBER_VALUE)))
                {
                    return $"{strValueOfPortNumber}(Not supported format)";
                }
                else
                {
                    return $"{strValueOfPortNumber}";
                }
            }
        }

        public string GetUseSSLErrorMessage(string errorMessage)
        {
            if (errorMessage.Contains("invalid child element 'UseSSL'") && errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("Type") != null)
            {
                return $"(duplicate Tag)";
            }
            else if (errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("UseSSL") == null)
            {
                return $"(not present)";
            }
            else
            {
                XmlNode currentNode = errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("UseSSL");
                string strValueOfUseSSL = currentNode.InnerText.ToUpper();

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    return $"{strValueOfUseSSL}Empty";
                }
                else if (!bool.TryParse(strValueOfUseSSL, out bool useSSL))
                {
                    return $"{strValueOfUseSSL}(Not supported format)";
                }
                else
                {
                    return $"{strValueOfUseSSL}";
                }
            }
        }

        public string GetPasswordErrorMessage(string errorMessage)
        {
            if (errorMessage.Contains("invalid child element 'Password'") && errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("UseSSL") != null)
            {
                return $"(duplicate Tag)";
            }
            else if (errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("Password") == null)
            {
                return $"(not present)";
            }
            else
            {
                XmlNode currentNode = errorDeviceNode.SelectSingleNode("CommSetting").SelectSingleNode("Password");
                string strValueOfPassword = currentNode.InnerText;

                if (currentNode.InnerText.Length > Constants.MAX_PASSWORD_LENGTH)
                {
                    return $"{strValueOfPassword}(invalid length)";
                }
                else
                {
                    return $"{strValueOfPassword}";
                }
            }
        }
        /*public bool IsValidNode(XmlNode node)
        {

        }*/
        /*
        /// <summary>
        /// Check for the format of the input file and validate its schema
        /// </summary>
        /// <param name="xmlPath"> Path provided in the command line </param>
        /// <returns> Dictionary containing all the device data in the xml </returns>
        public Dictionary<int, Dictionary<string, string>> ValidateXML(string xmlPath)
        {
            if (!File.Exists(xmlPath))
            {
                Console.WriteLine("Error: File not exist. Please provide a valid file path.");
            }
            else if (Path.GetExtension(xmlPath).ToLower() != ".xml")
            {
                Console.WriteLine("Error: Given file is not an XML file. The file extension is wrong.");
                Console.Read();
                System.Environment.Exit(1);
            }
            else if (new FileInfo(xmlPath).Length <= 3)
            {
                Console.WriteLine("Error: The XML file is empty. Device data is not present in the file.");
                Console.Read();
                System.Environment.Exit(1);
            }
            else
            {
                try
                {
                    XDocument xmlDoc = XDocument.Load(xmlPath);

                    // Checking if there are devices or not
                    if (xmlDoc.Descendants("Dev").Count() == 0)
                    {
                        Console.WriteLine("Error: The XML file is empty. Device data is not present in the file.");
                        Console.Read();
                        System.Environment.Exit(1);
                    }
                }
                catch
                {
                    Console.WriteLine("Error: File format error.Give file is not an XML file.");
                    System.Environment.Exit(1);
                }
            }

            int count = 0;
            Dictionary<string, string> currentDevElements = null;
            Dictionary<int, Dictionary<string, string>> xmlData = new Dictionary<int, Dictionary<string, string>>();

            // For representing the current node
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xmlPath);
            XmlNodeList nodeList = xdoc.SelectNodes("//Devices/Dev");
            XmlNode devnode = nodeList[0];

            string xsdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"XMLValidatingSchema.xsd");

            // Specify the setting and schema of the reader object
            XmlReaderSettings deviceSettings = new XmlReaderSettings();
            deviceSettings.ValidationType = ValidationType.Schema;
            deviceSettings.Schemas.Add(null,xsdPath);
            deviceSettings.ValidationEventHandler += (sender, e) => deviceSettingsValidationEventhandler(e, count, devnode);

            XmlReader device = XmlReader.Create(xmlPath, deviceSettings);

            // Reading the xml file
            while (device.Read())
            {
                // Storing the xml data
                if (device.NodeType == XmlNodeType.Element && device.Name == "Dev")
                {

                    currentDevElements = new Dictionary<string, string>();
                    do
                    {

                        if (device.HasAttributes)
                        {
                            device.MoveToFirstAttribute();

                            do
                            {
                                currentDevElements.Add(device.Name, device.Value);
                            } while (device.MoveToNextAttribute() && device.Name != "index");


                        }
                        if (device.NodeType == XmlNodeType.Element && device.Name != "CommSetting")
                        {
                            currentDevElements[device.Name] = device.ReadElementContentAsString();
                        }
                        else if (device.NodeType == XmlNodeType.EndElement && device.Name == "Dev")
                        {
                            xmlData.Add(count, currentDevElements);
                            count++;
                            devnode = nodeList[count];
                            break;
                        }

                    } while (device.Read());
                }
            }
            return xmlData;
        }*/
        /*
                /// <summary>
                /// Validation of the device where error occured
                /// </summary>
                /// <param name="e"> Error message object </param>
                /// <param name="index"> Index of the device having error </param>
                /// <param name="devnode"> The current Dev node </param>
                public static void ValidationEventhandler(ValidationEventArgs e, int index, XmlNode devnode)
                {

                    string errorMessage = e.Message;

                    Console.WriteLine(devnode.Name);
                    XmlNode currentNode = null;
                    bool isPrevNodePresent = true;
                    Console.WriteLine($"Device index: {index + 1}");

                    // Serial Number
                    string strSrNoTag = "Serial Number:   ";
                    if (errorMessage.Contains("SrNo"))
                    {
                        if (devnode.Attributes["SrNo"] == null)
                        {
                            Console.WriteLine($"{strSrNoTag}(not present)");
                        }
                        else
                        {
                            XmlAttribute attr = devnode.Attributes["SrNo"];
                            string strValueOfDevName = attr.InnerText;

                            if (string.IsNullOrWhiteSpace(attr.InnerText))
                            {
                                Console.WriteLine($"{strSrNoTag}Empty");
                            }
                            else if (errorMessage.Contains("uniqueSrNo"))
                            {
                                Console.WriteLine($"{strSrNoTag}{strValueOfDevName}(duplicate)");
                            }
                            else if (errorMessage.Contains("Pattern constraint failed"))
                            {
                                Console.WriteLine($"{strSrNoTag}{strValueOfDevName}(Not supported charecters)");
                            }
                            else if (attr.InnerText.Length != Constants.SERIAL_NUMBER_LENGTH)
                            {
                                Console.WriteLine($"{strSrNoTag}{strValueOfDevName}(invalid length)");
                            }
                            else
                            {
                                Console.WriteLine();
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine($"{strSrNoTag}{devnode.Attributes["SrNo"].InnerText}");
                    }

                    // IP Adress
                    string strIPTag = "IP Address:      ";

                    if (devnode.SelectSingleNode("Address") == null)
                    {
                        isPrevNodePresent = false;
                        Console.WriteLine($"{strIPTag}(not present)");
                    }
                    else if (errorMessage.Contains("invalid child element 'Address'"))
                    {
                        Console.WriteLine($"{strIPTag}(duplicate Tag)");
                    }
                    else
                    {
                        currentNode = devnode.SelectSingleNode("Address");
                        string strValueOfIP = currentNode.InnerText;

                        if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}Empty");
                        }
                        else if (errorMessage.Contains("uniqueIP"))
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}(duplicate)");
                        }
                        else if (!Regex.IsMatch(currentNode.InnerText, @"^[0-9.]+$"))
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}(Not supported characters)");
                        }
                        else if (currentNode.InnerText.Length > Constants.MAX_IP_ADDRESS_LENGTH || currentNode.InnerText.Length < Constants.MIN_IP_ADDRESS_LENGTH)
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}(invalid length)");
                        }
                        else if (!Regex.IsMatch(currentNode.InnerText, @"^\b(\d{1,3}\.){3}\d{1,3}\b$"))
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}(Not supported format)");
                        }
                        else
                        {
                            Console.WriteLine($"{strIPTag}{strValueOfIP}");
                        }


                    }

                    // deviceName
                    string strDevNameTag = "Device Name:     ";
                    if (errorMessage.Contains("invalid child element 'DevName'") && isPrevNodePresent)
                    {
                        isPrevNodePresent = true;
                        Console.WriteLine($"{strDevNameTag}(duplicate Tag)");
                    }
                    else
                    {
                        if (devnode.SelectSingleNode("DevName") == null)
                        {
                            isPrevNodePresent = false;
                            Console.WriteLine($"{strDevNameTag}(not present)");
                        }
                        else
                        {
                            currentNode = devnode.SelectSingleNode("DevName");
                            string strValueOfDevName = currentNode.InnerText;

                            if (Regex.IsMatch(strValueOfDevName, "@(^\x00-\x7F)*$"))
                            {
                                Console.WriteLine($"{strDevNameTag}{strValueOfDevName}(Not supported charecters)");
                            }
                            else if (currentNode.InnerText.Length > Constants.MAX_DEVICE_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_DEVICE_NAME_LENGTH)
                            {
                                Console.WriteLine($"{strDevNameTag}{strValueOfDevName}(invalid length)");
                            }
                            else
                            {
                                Console.WriteLine($"{strDevNameTag}{strValueOfDevName}");
                            }
                        }
                    }

                    // Model Name
                    string strModelNameTag = "Model Name:      ";


                    currentNode = devnode.SelectSingleNode("ModelName");
                    string strValueOfModelName = currentNode.InnerText;
                    if (devnode.SelectSingleNode("ModelName") == null)
                    {
                        isPrevNodePresent = false;
                    }
                    if (errorMessage.Contains("invalid child element 'ModelName'") && isPrevNodePresent)
                    {
                        Console.WriteLine($"{strModelNameTag}(duplicate Tag)");
                    }
                    else if (Regex.IsMatch(strValueOfModelName, "@[^\x00-\x7F]*"))
                    {
                        Console.WriteLine($"{strModelNameTag}{strValueOfModelName}(Not supported charecters)");
                    }
                    else if (currentNode.InnerText.Length > Constants.MAX_MODEL_NAME_LENGTH || currentNode.InnerText.Length < Constants.MIN_MODEL_NAME_LENGTH)
                    {
                        Console.WriteLine($"{strModelNameTag}{strValueOfModelName}(invalid length)");
                    }
                    else
                    {
                        Console.WriteLine($"{strModelNameTag}{strValueOfModelName}");
                    }


                    //Type
                    List<string> deviceTypes = new List<string> { "A3", "A4" };
                    string strTypeTag = "Type:            ";


                    if (errorMessage.Contains("invalid child element 'Type'") && isPrevNodePresent)
                    {
                        isPrevNodePresent = true;
                        Console.WriteLine($"{strTypeTag}(duplicate Tag)");
                    }
                    else if (devnode.SelectSingleNode("Type") == null)
                    {
                        isPrevNodePresent = false;
                        Console.WriteLine($"{strTypeTag}(not present)");
                    }
                    else
                    {
                        currentNode = devnode.SelectSingleNode("Type");
                        string strValueOfType = currentNode.InnerText;
                        if (strValueOfType == "")
                        {
                            Console.WriteLine($"{strTypeTag}Empty");
                        }
                        else if (!deviceTypes.Contains(strValueOfType))
                        {
                            Console.WriteLine($"{strTypeTag}{strValueOfType}(Not supported charecters)");
                        }
                        else
                        {
                            Console.WriteLine($"{strTypeTag}{strValueOfType}");
                        }
                    }

                    devnode = devnode.SelectSingleNode("CommSetting");

                    // PortNumber
                    string strPortNumberTag = "Port Number:     ";

                    if (errorMessage.Contains("invalid child element 'PortNo'") && isPrevNodePresent)
                    {
                        isPrevNodePresent = true;
                        Console.WriteLine($"{strSrNoTag}(duplicate Tag)");
                    }
                    else if (devnode.SelectSingleNode("PortNo") == null)
                    {
                        isPrevNodePresent = false;
                        Console.WriteLine($"{strPortNumberTag}(not present)");
                    }
                    else
                    {
                        currentNode = devnode.SelectSingleNode("PortNo");
                        string strValueOfPortNumber = currentNode.InnerText;

                        if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                        {
                            Console.WriteLine($"{strPortNumberTag}{strValueOfPortNumber}Empty");
                        }
                        else if (!(int.TryParse(strValueOfPortNumber, out int portNo) && (Constants.MIN_PORT_NUMBER_VALUE <= portNo && portNo <= Constants.MAX_PORT_NUMBER_VALUE)))
                        {
                            Console.WriteLine($"{strPortNumberTag}{strValueOfPortNumber}(Not supported format)");
                        }
                        else
                        {
                            Console.WriteLine($"{strPortNumberTag}{strValueOfPortNumber}");
                        }
                    }

                    // UseSSL
                    string strUseSSLTag = "UseSSL:          ";
                    if (errorMessage.Contains("invalid child element 'UseSSL'") && isPrevNodePresent)
                    {
                        Console.WriteLine($"{strUseSSLTag}(duplicate Tag)");
                    }
                    else if (devnode.SelectSingleNode("UseSSL") == null)
                    {
                        Console.WriteLine($"{strUseSSLTag}(not present)");
                    }
                    else
                    {
                        currentNode = devnode.SelectSingleNode("UseSSL");
                        string strValueOfUseSSL = currentNode.InnerText;

                        if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                        {
                            Console.WriteLine($"{strUseSSLTag}{strValueOfUseSSL}Empty");
                        }
                        else if (!bool.TryParse(strValueOfUseSSL, out bool useSSL))
                        {
                            Console.WriteLine($"{strUseSSLTag}{strValueOfUseSSL}(Not supported format)");
                        }
                        else
                        {
                            Console.WriteLine($"{strUseSSLTag}{strValueOfUseSSL}");
                        }
                    }

                    // Password
                    string strPasswordTag = "Password:        ";
                    if (errorMessage.Contains("invalid child element 'Password'") && isPrevNodePresent)
                    {
                        Console.WriteLine($"{strPasswordTag}(duplicate Tag)");
                    }
                    else if (devnode.SelectSingleNode("Password") == null)
                    {
                        Console.WriteLine($"{strPasswordTag}(not present)");
                    }
                    else
                    {
                        currentNode = devnode.SelectSingleNode("Password");
                        string strValueOfPassword = currentNode.InnerText;

                        if (Regex.IsMatch(strValueOfPassword, "@[^\x00-\x7F]*"))
                        {
                            Console.WriteLine($"{strPasswordTag}{strValueOfPassword}(Not supported charecters)");
                        }
                        else if (currentNode.InnerText.Length > Constants.MAX_PASSWORD_LENGTH)
                        {
                            Console.WriteLine($"{strPasswordTag}{strValueOfPassword}(invalid length)");
                        }
                        else
                        {
                            Console.WriteLine($"{strPasswordTag}{strValueOfPassword}");
                        }
                    }

                    Console.Read();
                    System.Environment.Exit(1);
                }

        */
    }
}

