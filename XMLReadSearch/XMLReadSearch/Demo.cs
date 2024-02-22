using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Schema;
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
        /// <param name="args"> Xml path provided in command line </param>
        public void Run(string[] args)
        {
            try
            {

                // Validating command line input
                CommandLine.ValidateArgument(args);

                string xmlPath = args[0];

                // Xml validation
                Dictionary<string, string> errorDevice = new XmlValidation().IsValidXML(args[0]);

                if (errorDevice.Count != 0)
                {
                    DisplayErrorDevice(errorDevice);
                }
                else
                {
                    // Getting device data using deserialization
                    XmlSerializer serializer = new XmlSerializer(typeof(DeviceElements));
                    DeviceElements devices;

                    using (Stream reader = new FileStream(xmlPath, FileMode.Open))
                    {
                        devices = (DeviceElements)serializer.Deserialize(reader);
                    }

                    // Storing all device data
                    Dictionary<string, Dictionary<string, string>> allDevices = new Dictionary<string, Dictionary<string, string>>();

                    foreach (Device device in devices.DeviceList)
                    {
                        Dictionary<string, string> dev = new Dictionary<string, string>();

                        dev.Add("Address", device.Address);
                        dev.Add("Device Name", device.DevName);
                        
                        if (device.ModelName != null)
                        {
                            dev.Add("Model Name", device.ModelName);
                        }
                        else
                        {
                            dev.Add("Model Name", " ");
                        }

                        dev.Add("Type", device.Type);
                        dev.Add("Port Number", device.CommSetting.PortNo.ToString());
                        dev.Add("UseSSL", device.CommSetting.UseSSL.ToString());
                        dev.Add("Password", device.CommSetting.Password);

                        allDevices.Add(device.SerialNumber, dev);
                    }

                    // Displaying options to the user and calling appropriate functions
                    UserChoices choice;

                    do
                    {
                        DisplayOptions();
                        choice = GetUserChoice();

                        switch (choice)
                        {
                            case UserChoices.ShowAllDevices:
                                DisplayAllDevices(allDevices);
                                break;
                            case UserChoices.SearchForDevice:
                                Console.WriteLine("\n[2] Search devices by serial number");
                                Console.WriteLine("Enter serial number of the device");
                                SearchDeviceBySerialNumber(Console.ReadLine(), allDevices);
                                break;
                            case UserChoices.Exit:
                                Console.WriteLine("\nProgram terminated");
                                break;
                            default:
                                break;
                        }

                    } while (choice != UserChoices.Exit);

                }
            }
            catch (CommandLineException)
            {
            }
            catch (XmlReaderException)
            {
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Detail Error: {e.Message}");
            }
            catch (XmlSchemaException)
            {
                Console.WriteLine("Invalid xsd file");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown Error occured");
                Console.WriteLine($"Detail Error: {e.Message}");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Displays menu options to user for action
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
        /// <returns> User choice to operate on dvice xml </returns>
        public UserChoices GetUserChoice()
        {
            while (true)
            {

                if (int.TryParse(Console.ReadLine(), out int choice) && 0 < choice && choice < 4)
                {
                    return (UserChoices)choice;
                }

                Console.WriteLine("Error: invalid input. Please select from above options.");

            }
        }

        /// <summary>
        /// Display innformation and errors of the device containing invalid property
        /// </summary>
        /// <param name="errorDevice"> Data of the invalid device </param>
        public void DisplayErrorDevice(Dictionary<string, string> errorDevice)
        {
            Console.WriteLine("\nError: Invalid device information. Please refer below details.\n");

            foreach (var info in errorDevice)
            {
                Console.WriteLine($"{info.Key}{new string(' ', 25 - info.Key.Length)}{info.Value}");
            }
        }

        /// <summary>
        /// Display all device data
        /// </summary>
        /// <param name="deviceData"> Dictionary containg all data </param>
        public void DisplayAllDevices(Dictionary<string, Dictionary<string, string>> deviceData)
        {
            Console.WriteLine("\n[1] Show all devices");
            Console.WriteLine(new string('-', 140));
            Console.WriteLine($"No{new string(' ', 3)}Serial Number{new string(' ', 8)}IP Address{new string(' ', 11)}Device Name{new string(' ', 18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
            Console.WriteLine(new string('-', 140));
            int index = 1;

            foreach (var info in deviceData)
            {
                Dictionary<string, string> device = info.Value;
                Console.WriteLine($"{index}{new string(' ', 4 - index.ToString().Length)}{info.Key}{new string(' ', 6)}{device["Address"]}{new string(' ', 21 - device["Address"].Length)}{device["Device Name"]}{new string(' ', 29 - device["Device Name"].Length)}{device["Model Name"]}{new string(' ', 29 - device["Model Name"].Length)}{device["Type"]}{new string(' ', 4)}{device["Port Number"]}{new string(' ', 8 - device["Port Number"].Length)}{device["UseSSL"]}{new string(' ', 8 - device["UseSSL"].Length)}{new AES_Cryptography().Decrypt(device["Password"])}");
                index++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Search the device by serial number and display it
        /// </summary>
        /// <param name="deviceSerialNumber"> Serial number of the device to print </param>
        /// <param name="deviceData"> Dictionary containing all device data </param>
        public void SearchDeviceBySerialNumber(string deviceSerialNumber, Dictionary<string, Dictionary<string, string>> deviceData)
        {
            if (deviceData.ContainsKey(deviceSerialNumber))
            {
                Dictionary<string, string> device = deviceData[deviceSerialNumber];
                Console.WriteLine("\nDevice information is as below");
                Console.WriteLine(new string('-', 135));
                Console.WriteLine($"Serial Number{new string(' ', 9)}IP Address{new string(' ', 11)}Device Name{new string(' ', 18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
                Console.WriteLine(new string('-', 135));
                Console.WriteLine($"{deviceSerialNumber}{new string(' ', 6)}{device["Address"]}{new string(' ', 21 - device["Address"].Length)}{device["Device Name"]}{new string(' ', 29 - device["Device Name"].Length)}{device["Model Name"]}{new string(' ', 29 - device["Model Name"].Length)}{device["Type"]}{new string(' ', 4)}{device["Port Number"]}{new string(' ', 8 - device["Port Number"].Length)}{device["UseSSL"]}{new string(' ', 8 - device["UseSSL"].Length)}{new AES_Cryptography().Decrypt(device["Password"])}\n");
            }
            else
            {
                Console.WriteLine("\nDevice not found\n");
            }
        }
    }
}