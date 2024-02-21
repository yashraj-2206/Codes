using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Containing demo program 
    /// </summary>
    public class Demo
    {
        /// <summary>
        /// Runs the demo program
        /// </summary>
        /// <param name="args"> Xml path provided in command line</param>
        public void Run(string[] args)
        {
            try
            {

               // Console.Write(new AES_Cryptography().encrypt("Acty@123"));
                // Starts program only if command line input is valid
                CommandLine.ValidateArgument(args);

                string xmlPath = args[0];

                if (new Xml().IsValidXML(args[0]).Count != 0)
                {
                    Dictionary<string, string> errorDevice = new Xml().IsValidXML(args[0]);
                    DisplayErrorDevice(errorDevice);
                }
                else
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DeviceElements));
                    DeviceElements deviceData;

                    using (Stream reader = new FileStream(xmlPath, FileMode.Open))
                    {
                        deviceData = (DeviceElements)serializer.Deserialize(reader);

                    }



                    Dictionary<string, Dictionary<string, string>> allDevices = new Dictionary<string, Dictionary<string, string>>();

                    foreach (Device device in deviceData.DeviceList)
                    {
                        Dictionary<string, string> dev = new Dictionary<string, string>();

                        dev.Add("Address", device.Address);
                        dev.Add("Device Name", device.DevName);
                        dev.Add("Model Name", device.ModelName);
                        dev.Add("Type", device.Type);
                        dev.Add("Port Number", device.CommSetting.PortNo.ToString());
                        dev.Add("UseSSL", device.CommSetting.UseSSL.ToString());
                        dev.Add("Password", device.CommSetting.Password);

                        allDevices.Add(device.SerialNumber, dev);
                    }

                    DisplayAllDevices(allDevices);
                    
                    int option = 0;

                    while (option != 3)
                    {
                        DisplayOptions();
                        option = GetUserChoice();
                        switch (option)
                        {
                            case 1:
                                DisplayAllDevices(allDevices);
                                break;
                            case 2:
                                Console.WriteLine("Enrter serial number of the device");
                                SearchDeviceBySerialNumber(Console.ReadLine(), deviceData);
                                break;
                            case 3:
                            default:
                                break;
                        }

                    }

                }
            }
            catch (CommandLineException)
            {
            }
            catch (XmlReaderException)
            {
            }
            catch (Exception e)
            {
                Console.WriteLine("Printer Status check operation failed.");
                Console.WriteLine($"Detail Error: {e.Message}");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Displays menu options to show or search for a device
        /// </summary>
        public void DisplayOptions()
        {
            Console.WriteLine("Please select option");
            Console.WriteLine("[1] Show all devices");
            Console.WriteLine("[2] Search devices by serial number");
            Console.WriteLine("[3] Exit");
        }

        /// <summary>
        /// Validating user input for choices
        /// </summary>
        /// <returns> User choice to operate on dvice xml</returns>
        public int GetUserChoice()
        {
            while (true)
            {

                if (int.TryParse(Console.ReadLine(), out int choice) && 0 < choice && choice < 4)
                {
                    return choice;
                }

                Console.WriteLine("Error: invalid input. Please select from above options.");

            }
        }

        public void DisplayErrorDevice(Dictionary<string, string> errorDevice)
        {
            Console.WriteLine("Error: Invalid device information. Please refer below details.\n");

            foreach (var info in errorDevice)
            {
                Console.WriteLine($"{info.Key}{new string (' ', 25 - info.Key.Length)}{info.Value}");
            }
        }

        public void DisplayAllDevices(Dictionary<string,Dictionary<string,string>> deviceData)
        {
            Console.WriteLine(new string('-',135));
            Console.WriteLine($"No{new string(' ', 2)}Serial Number{new string(' ',8)}IP Address{new string(' ',11)}Device Name{new string(' ',18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
            Console.WriteLine(new string('-',135));
            int index = 1;
            foreach (var info in deviceData)
            {
                Dictionary<string,string> device = info.Value;
                Console.WriteLine($"{index}{new string(' ', 2)}{info.Key}{new string(' ', 6)}{device["Address"]}{new string(' ', 21 - device["Address"].Length)}{device["Device Name"]}{new string(' ',29-device["Device Name"].Length)}{device["Model Name"]}{new string(' ', 29 - device["Model Name"].Length)}{device["Type"]}{new string(' ', 4)}{device["Port Number"]}{new string(' ',8 - device["Port Number"].Length)}{device["UseSSL"]}{new string(' ',8 - device["UseSSL"].Length)}{new AES_Cryptography().Decrypt(device["Password"])}");
                index++;
            }
            
        }

        public void SearchDeviceBySerialNumber(string deviceSerialNumber, DeviceElements device)
        {
            Device dev = device.DeviceList.Find(x => x.SerialNumber == deviceSerialNumber);
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"Serial Number{new string(' ', 8)}IP Address{new string(' ', 11)}Device Name{new string(' ', 18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"{dev.SerialNumber}{new string(' ', 6)}{dev.Address}{new string(' ', 21 - dev.Address.Length)}{dev.DevName}{new string(' ', 29 - dev.DevName.Length)}{dev.ModelName}{new string(' ', 29 - dev.ModelName.Length)}{dev.Type}{new string(' ', 4)}{dev.CommSetting.PortNo}{new string(' ', 8 - dev.CommSetting.PortNo.ToString().Length)}{dev.CommSetting.UseSSL}{new string(' ', 8 - dev.CommSetting.UseSSL.ToString().Length)}{dev.CommSetting.Password}");

        }
        /*
        /// <summary>
        /// Displays all devices present in xml
        /// </summary>
        /// <param name="deviceData"> Data of devices from xml </param>
        public void displayDevice(Dictionary<int, Dictionary<string, string>> deviceData)
        {
            Console.WriteLine($"\n{new string('-', 100)}");
            Console.WriteLine("No\tSerial Number\t  \tIP Address\tDevice Name\tModel Name\tType\tPort\tUseSSL\tPassword");
            Console.WriteLine($"{new string('-', 100)}\n");


            foreach (var dataOfDevices in deviceData)
            {
                // index of the device
                int deviceIndex = dataOfDevices.Key;

                // Get the device of index as deviceIndex
                Dictionary<string, string> innerDictionary = dataOfDevices.Value;

                Console.Write($"{deviceIndex + 1}\t");
                Console.Write($"{innerDictionary["SrNo"]}\t");
                Console.Write($"{innerDictionary["Address"]}\t\t");
                Console.Write($"{innerDictionary["DevName"]}\t\t");
                Console.Write($"{innerDictionary["ModelName"]}\t");
                Console.Write($"{innerDictionary["Type"]}\t");
                Console.Write($"{innerDictionary["PortNo"]}\t");

                if (innerDictionary["UseSSL"] == "true")
                {
                    Console.Write("Yes\t");
                }
                else
                {
                    Console.Write("No\t");
                }

                Console.Write($"{innerDictionary["Password"]}");

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Display a device having serial number as per user intrest
        /// </summary>
        /// <param name="serialNo"> Serial Number of the desired device data </param>
        /// <param name="deviceData"> Dictionary containing all device data </param>
        public void displayDevice(string serialNo, Dictionary<int, Dictionary<string, string>> deviceData)
        {
            bool isCorrectSerialNumber = false;
            Console.WriteLine("Device information is below");

            foreach (var outerKey in deviceData)
            {
                // Display only for the desired serial number
                if (deviceData[outerKey.Key]["SrNo"] == serialNo)
                {
                    Dictionary<string, string> innerDictionary = deviceData[outerKey.Key];

                    Console.WriteLine($"{new string('-', 100)}");
                    Console.WriteLine("Serial Number\t  \tIP Address\tDevice Name\tModel Name\tType\tPort\tUseSSL\tPassword");
                    Console.WriteLine($"{new string('-', 100)}\n");

                    Console.Write($"{innerDictionary["SrNo"]}\t");
                    Console.Write($"{innerDictionary["Address"]}\t\t");
                    Console.Write($"{innerDictionary["DevName"]}\t\t");
                    Console.Write($"{innerDictionary["ModelName"]}\t");
                    Console.Write($"{innerDictionary["Type"]}\t");
                    Console.Write($"{innerDictionary["PortNo"]}\t");
                    if (innerDictionary["UseSSL"] == "true")
                    {
                        Console.Write("Yes\t");
                    }
                    else
                    {
                        Console.Write("No\t");
                    }
                    Console.Write($"{innerDictionary["Password"]}");


                    isCorrectSerialNumber = true;
                    break;
                }
            }

            // Print if no device have enteered serial number
            if (!isCorrectSerialNumber)
            {
                Console.WriteLine("No device found with this serial number");
            }
            Console.WriteLine();
        }*/
    }
}