using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
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
                Dictionary<string, string> errorDevice = new XmlValidation().ValidateXml(xmlPath);

                if (errorDevice.Count != 0)
                {
                    DisplayErrorDevice(errorDevice);
                    return;
                }

                // Getting device data using deserialization
                XmlSerializer serializer = new XmlSerializer(typeof(DeviceElements));
                DeviceElements devices;

                using (Stream reader = new FileStream(xmlPath, FileMode.Open))
                {
                    devices = (DeviceElements)serializer.Deserialize(reader);
                }

                Dictionary<string, Device> allDevices = new Dictionary<string, Device>();


                // Storing all device data
                //Dictionary<string, Dictionary<string, string>> allDevices = new Dictionary<string, Dictionary<string, string>>();

                foreach (Device device in devices.DeviceList)
                {
                    allDevices.Add(device.SerialNumber, device);
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
                            string serialNumber = Console.ReadLine();

                            if (allDevices.ContainsKey(serialNumber))
                            {
                                DisplayDevice(allDevices[serialNumber]);
                            }
                            else
                            {
                                Console.WriteLine("\nDevice not found\n");
                            }

                            break;

                        case UserChoices.Exit:
                        default:
                            break;
                    }

                } while (choice != UserChoices.Exit);

            }
            catch (CommandLineException e)
            {
                Console.WriteLine($"{e.message}");

            }
            catch (XmlReaderException e)
            {
                Console.WriteLine($"\n{e.message} ({e.errorCode})");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nError: Unknown Error occured");
                Console.WriteLine($"Detail Error: {e.Message}");
                Console.WriteLine($"Stack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Displays menu options to user for action
        /// </summary>
        public void DisplayOptions()
        {
            Console.WriteLine("\nPlease select option");
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

                Console.WriteLine(Constants.INVALID_CHOICE_ERROR_MESSAGE);

            }
        }

        /// <summary>
        /// Display innformation and errors of the device containing invalid property
        /// </summary>
        /// <param name="errorDevice"> Data of the invalid device </param>
        public void DisplayErrorDevice(Dictionary<string, string> errorDevice)
        {
            Console.WriteLine($"\n{Constants.INVALID_DEVICE_ERROR_MESSAGE}\n");

            foreach (var info in errorDevice)
            {
                Console.WriteLine($"{info.Key}{new string(' ', 25 - info.Key.Length)}{info.Value}");
            }
        }

        /// <summary>
        /// Display all device data
        /// </summary>
        /// <param name="deviceData"> Dictionary containg data of all devices </param>
        public void DisplayAllDevices(Dictionary<string, Device> deviceData)
        {
            Console.WriteLine("\n[1] Show all devices");
            Console.WriteLine(new string('-', 140));
            Console.WriteLine($"No{new string(' ', 3)}Serial Number{new string(' ', 8)}IP Address{new string(' ', 11)}Device Name{new string(' ', 18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
            Console.WriteLine(new string('-', 140));
            int index = 1;

            foreach (var info in deviceData)
            {
                Device dev = info.Value;
                string password = new EncryptionManager().Decrypt(dev.CommSetting.Password);

                Console.Write($"{index}{new string(' ', 4 - index.ToString().Length)}{dev.SerialNumber}{new string(' ', 6)}{dev.Address}{new string(' ', 21 - dev.Address.Length)}");
                Console.Write($"{dev.DevName}{new string(' ', 29 - dev.DevName.Length)}");
                
                if (dev.ModelName != null)
                {
                    Console.Write($"{dev.ModelName}{new string(' ', 29 - dev.ModelName.Length)}");
                }
                else
                {
                    Console.Write(new string(' ',29));
                }

                Console.Write($"{dev.Type}{new string(' ', 4)}{dev.CommSetting.PortNo}{new string(' ', 8 - dev.CommSetting.PortNo.ToString().Length)}");
                Console.WriteLine($"{dev.CommSetting.UseSSL}{new string(' ', 8 - dev.CommSetting.UseSSL.ToString().Length)}{password}");

                index++;
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displaying a device
        /// </summary>
        /// <param name="dev"> Device to print </param>
        public void DisplayDevice(Device dev)
        {
            string password = new EncryptionManager().Decrypt(dev.CommSetting.Password);

            Console.WriteLine("\nDevice information is as below");
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"Serial Number{new string(' ', 9)}IP Address{new string(' ', 11)}Device Name{new string(' ', 18)}Model Name{new string(' ', 18)}Type{new string(' ', 3)}Port{new string(' ', 4)}SSL{new string(' ', 5)}Password");
            Console.WriteLine(new string('-', 135));

            Console.Write($"{dev.SerialNumber}{new string(' ', 6)}{dev.Address}{new string(' ', 21 - dev.Address.Length)}");
            Console.Write($"{dev.DevName}{new string(' ', 29 - dev.DevName.Length)}");

            if (dev.ModelName != null)
            {
                Console.Write($"{dev.ModelName}{new string(' ', 29 - dev.ModelName.Length)}");
            }
            else
            {
                Console.Write(new string(' ', 29));
            }

            Console.Write($"{dev.Type}{new string(' ', 4)}{dev.CommSetting.PortNo}{new string(' ', 8 - dev.CommSetting.PortNo.ToString().Length)}");
            Console.WriteLine($"{dev.CommSetting.UseSSL}{new string(' ', 8 - dev.CommSetting.UseSSL.ToString().Length)}{password}");
        }
    }
}