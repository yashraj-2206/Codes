using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System;
using System.Net.Security;
using Microsoft.SqlServer.Server;
using System.Xml.Linq;
using System.ComponentModel;
using System.Threading;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Security.Cryptography;

namespace XMLReadSearch
{
    public class XML
    {
        public static Dictionary<int, Dictionary<string, string>> ValidateXML(string xmlPath)
        {
            bool isError = false;
            if (!File.Exists(xmlPath))
            {
                Console.WriteLine("Error: File not exist. Please provide a valid file path.");
                isError = true;
            }
            else if (Path.GetExtension(xmlPath).ToLower() != ".xml")
            {
                Console.WriteLine("Error: Given file is not an XML file. The file extension is wrong.");
                isError = true;
            }
            else if (new FileInfo(xmlPath).Length <= 3)
            {
                Console.WriteLine("Error: The XML file is empty. Device data is not present in the file.");
                isError = true;
            }
            else
            {
                try
                {
                    XDocument xmlDoc = XDocument.Load(xmlPath);
                    if (xmlDoc.Descendants("Dev").Count() == 0)
                    {
                        Console.WriteLine("Error: The XML file is empty. Device data is not present in the file.");
                        isError = true;
                    }
                }
                catch
                {
                    Console.WriteLine("Error: File format error.Give file is not an XML file.");
                    isError = true;
                }
            }



            int count = 0;
            Dictionary<string, string> currentDevElements = null;
            Dictionary<int, Dictionary<string, string>> xmlData = new Dictionary<int, Dictionary<string, string>>();

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Constants.XML_FILE_PATH);
            XmlNodeList nodeList = xdoc.SelectNodes("//Devices/Dev");
            XmlNode devnode = nodeList[0];

            XmlReaderSettings deviceSettings = new XmlReaderSettings();
            deviceSettings.ValidationType = ValidationType.Schema;
            deviceSettings.Schemas.Add(null, Constants.XSD_FILE_PATH);
            deviceSettings.ValidationEventHandler += (sender, e) => deviceSettingsValidationEventhandler(sender, e, count, devnode);

            XmlReader device = XmlReader.Create(xmlPath, deviceSettings);

            while (device.Read())
            {
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
                /* else if (device.NodeType == XmlNodeType.EndElement)
                 {
                     Console.WriteLine(device.Value);
                 }*/

            }
            return xmlData;
        }

        public static void deviceSettingsValidationEventhandler(object sender, ValidationEventArgs e, int index, XmlNode devnode)
        {
            Console.WriteLine($"{e.Message}");
            string errorMessage = e.Message;
            XmlReader reader = (XmlReader)sender;
            /*            XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(Constants.XML_FILE_PATH);
                        XmlNodeList nodeList = xdoc.SelectNodes("//Devices/Dev");
                        XmlNode devnode = nodeList[index];*/
            Console.WriteLine(devnode.Name);
            XmlNode currentNode = null;
            Console.WriteLine($"Device index: {index + 1}");

            // Serial Number
            string strSrNoTag = "Serial Number:  ";
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
                    else if (errorMessage.Contains("duplicate"))
                    {
                        Console.WriteLine($"{strSrNoTag}{strValueOfDevName}(duplicate)");
                    }
                    else if (errorMessage.Contains("Pattern constraint failed"))
                    {
                        Console.WriteLine($"{strSrNoTag}{strValueOfDevName}(Not supported charecters)");
                    }
                    else if (attr.InnerText.Length != 16)
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
            string strIPTag = "IP Address:";

            if (errorMessage.Contains("invalid child element 'Address'"))
            {
                Console.WriteLine();
            }
            else if (devnode.SelectSingleNode("Address") == null)
            {
                Console.WriteLine($"{strIPTag}(not present)");
            }
            else
            {
                currentNode = devnode.SelectSingleNode("Address");
                string strValueOfIP = currentNode.InnerText;

                if (string.IsNullOrWhiteSpace(currentNode.InnerText))
                {
                    Console.WriteLine($"{strIPTag}{strValueOfIP}Empty");
                }
                else if (errorMessage.Contains("duplicate"))
                {
                    Console.WriteLine($"{strIPTag}{strValueOfIP}(duplicate)");
                }
                else if (!Regex.IsMatch(currentNode.InnerText, "[0-9]*[.]*"))
                {
                    Console.WriteLine($"{strIPTag}{strValueOfIP}(Not supported characters)");
                }
                else if (currentNode.InnerText.Length >= 15 && currentNode.InnerText.Length <= 1)
                {
                    Console.WriteLine($"{strIPTag}{strValueOfIP}(invalid length)");
                }
                else if (!Regex.IsMatch(currentNode.InnerText, "[\\b(?:\\d{1,3}\\.){3}\\d{1,3}\\b]"))
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
            if (errorMessage.Contains("invalid child element 'DevName'"))
            {
                Console.WriteLine();
            }
            else if (devnode.SelectSingleNode("DevName") == null)
            {
                Console.WriteLine("(not present)");
            }
            else
            {
                currentNode = devnode.SelectSingleNode("DevName");
                string strValueOfDevName = currentNode.InnerText;

                if (Regex.IsMatch(strValueOfDevName, "[^\x00-\x7F]"))
                {
                    Console.WriteLine($"{strDevNameTag}{strValueOfDevName}(Not supported charecters)");
                }
                else if (currentNode.InnerText.Length >= 24 && currentNode.InnerText.Length <= 0)
                {
                    Console.WriteLine($"{strDevNameTag}{strValueOfDevName}(invalid length)");
                }
                else
                {
                    Console.WriteLine($"{strDevNameTag}{strValueOfDevName}");
                }
            }

            // Model Name
            string strModelNameTag = "Model Name:     ";

            if (errorMessage.Contains("invalid child element 'ModelName'"))
            {
                Console.WriteLine();
            }
            else
            {
                currentNode = devnode.SelectSingleNode("ModelName");
                string strValueOfModelName = currentNode.InnerText;

                if (!Regex.IsMatch(strValueOfModelName, "[^\x00-\x7F]"))
                {
                    Console.WriteLine($"{strModelNameTag}{strValueOfModelName}(Not supported charecters)");
                }
                else if (currentNode.InnerText.Length >= 24 && currentNode.InnerText.Length <= 0)
                {
                    Console.WriteLine($"{strModelNameTag}{strValueOfModelName}(invalid length)");
                }
                else
                {
                    Console.WriteLine($"{strModelNameTag}{strValueOfModelName}");
                }
            }

            //Type
            List<string> deviceTypes = new List<string> { "A3", "A4" };
            string strTypeTag = "Type:     ";

            if (errorMessage.Contains("invalid child element 'Type'"))
            {
                Console.WriteLine();
            }
            else if (devnode.SelectSingleNode("Type") == null)
            {
                Console.WriteLine("(not present)");
            }
            else
            {
                currentNode = devnode.SelectSingleNode("Type");
                string strValueOfType = currentNode.InnerText;
                if (strValueOfType == null)
                {
                    Console.WriteLine($"{strSrNoTag}Empty");
                }
                if (!deviceTypes.Contains(strValueOfType))
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
            if (errorMessage.Contains("invalid child element 'PortNO'"))
            {
                Console.WriteLine();
            }
            else if (devnode.SelectSingleNode("PortNo") == null)
            {
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
                else if (int.TryParse(strValueOfPortNumber, out int PortNo) && 1 <= PortNo && PortNo <= 65535)
                {
                    Console.WriteLine($"{strPortNumberTag}{strValueOfPortNumber}(Not supported format)");
                }
                else
                {
                    Console.WriteLine(strPortNumberTag, strValueOfPortNumber);
                }
            }

            // UseSSL
            string strUseSSLTag = "UseSSL:     ";
            if (errorMessage.Contains("invalid child element 'UseSSL'"))
            {
                Console.WriteLine();
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
                else if (!bool.TryParse(strValueOfUseSSL, out bool PortNo))
                {
                    Console.WriteLine($"{strUseSSLTag}{strValueOfUseSSL}(Not supported format)");
                }
                else
                {
                    Console.WriteLine(strUseSSLTag, strValueOfUseSSL);
                }
            }

            // Password
            string strPasswordTag = "Password:     ";
            if (errorMessage.Contains("invalid child element 'Password'"))
            {
                Console.WriteLine();
            }
            else if (devnode.SelectSingleNode("Password") == null)
            {
                Console.WriteLine($"{strPasswordTag}(not present)");
            }
            else
            {
                currentNode = devnode.SelectSingleNode("Password");
                string strValueOfPassword = currentNode.InnerText;

                if (Regex.IsMatch(strValueOfPassword, "[^\x00-\x7F]"))
                {
                    Console.WriteLine($"{strPasswordTag}{strValueOfPassword}(Not supported charecters)");
                }
                else if (currentNode.InnerText.Length >= 64 && currentNode.InnerText.Length <= 0)
                {
                    Console.WriteLine($"{strPasswordTag}{strValueOfPassword}(invalid length)");
                }
                else
                {
                    Console.WriteLine($"{strPasswordTag}{strValueOfPassword}");
                }
            }

            Console.ReadLine();
        }

    }

}

