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



            int count = 1;
            Dictionary<string, string> currentDevElements = null;
            Dictionary<int, Dictionary<string, string>> xmlData = new Dictionary<int, Dictionary<string, string>>();

            XmlReaderSettings deviceSettings = new XmlReaderSettings();
            deviceSettings.ValidationType = ValidationType.Schema;
            deviceSettings.Schemas.Add(null, Constants.XSD_FILE_PATH);
            deviceSettings.ValidationEventHandler += (sender, e) => deviceSettingsValidationEventhandler(sender, e, count, currentDevElements);

            XmlReader device = XmlReader.Create(xmlPath, deviceSettings);



            while (device.Read())
            {
                if (device.NodeType == XmlNodeType.Element && device.Name == "Dev")
                {

                    currentDevElements = new Dictionary<string, string>();
                    do
                    {
                        Console.WriteLine(device.Value);

                        if (device.HasAttributes)
                        {
                            if (device.MoveToFirstAttribute())
                            {
                                do
                                {
                                    currentDevElements.Add(device.Name, device.Value);
                                } while (device.MoveToNextAttribute());

                            }
                        }
                        if (device.NodeType == XmlNodeType.Element && device.Name != "CommSetting")
                        {
                            currentDevElements[device.Name] = device.ReadElementContentAsString();
                        }
                        if (device.NodeType == XmlNodeType.EndElement && device.Name == "Dev")
                        {
                            xmlData.Add(count, currentDevElements);
                            count++;
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

        public static void deviceSettingsValidationEventhandler(object sender, ValidationEventArgs e, int index, Dictionary<string, string> deviceData)
        {
            Console.WriteLine($"{e.Message}");
            string errorMessage = e.Message;
            XmlReader read = (XmlReader)sender;
            Console.WriteLine(read.NamespaceURI);

            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(Constants.XML_FILE_PATH);
            XmlNode devnode = xdoc.SelectSingleNode($"//Dev[@index={index}]");
       

            Console.WriteLine($"Device index: {index}");
            Console.Write("Serial Number:  ");

            if (errorMessage.Contains("SrNo"))
            {
                if (e.Message.Contains("missing"))
                {
                    Console.WriteLine("(not present)");
                }
                else
                {
                    XmlAttribute attr = devnode.Attributes["SrNo"];

                    if (string.IsNullOrWhiteSpace(attr.Value))
                    {
                        Console.WriteLine("Empty");
                    }
                    else if (errorMessage.Contains("Pattern constraint failed"))
                    {
                        Console.WriteLine($"{attr.Value}(Not supported charecters)");
                    }
                    else if (attr.Value.Length != 16)
                    {
                        Console.WriteLine($"{attr.Value}(invalid length)");
                    }
                    else
                    {
                        Console.WriteLine("(empty)");
                    }
                }
            }
            else
            {
                Console.WriteLine(devnode.Attributes["SrNo"].Value);
            }

            Console.Write("IP Address:     ");
            if (errorMessage.Contains("Address"))
            {

                if (e.Message.Contains("missing"))
                {
                    Console.WriteLine("(not present)");
                }
                else
                {
                    XmlNode ipNode = devnode.SelectSingleNode("Address");

                    if (string.IsNullOrWhiteSpace(ipNode.Value))
                    {
                        Console.WriteLine("Empty");
                    }
                    else if (Regex.IsMatch(ipNode.Value, "[\\b(?:\\d{1,3}\\.){3}\\d{1,3}\\b]"))
                    {
                        Console.WriteLine($"{ipNode.Value}(Not supported charecters)");
                    }
                    else if (1 <= ipNode.Value.Length && ipNode.Value.Length <= 15)
                    {
                        Console.WriteLine($"{ipNode.Value}(invalid length)");
                    }
                }
            }
            else
            {
                Console.WriteLine(devnode.SelectSingleNode("Address").Value);
            }











            XmlReader device = (XmlReader)sender;
            Console.WriteLine(device.ReadContentAsString());
            Console.WriteLine($"Device index: {index}");
            foreach (string s in deviceData.Keys)
            {
                if (s != device.Name)
                {
                    Console.WriteLine($"{s}:    {deviceData[s]}");
                }

            }


















            /* foreach(XmlNode childnode in devnode) 
             {
                 Console.WriteLine($"{childnode.Name}: {childnode.ge}");
                 if (childnode.Attributes != null)
                 {
                     foreach (XmlAttribute attr in childnode.Attributes)
                     {
                         Console.WriteLine($"Attribute: {attr.Name} = {attr.Value}");
                     }
                 }
             } 


 */

        }

    }
}
