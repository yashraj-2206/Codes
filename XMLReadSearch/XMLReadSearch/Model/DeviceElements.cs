using System.Collections.Generic;
using System.Xml.Serialization;

namespace Skillup.XMLReadSearch
{
    /// <summary>
    /// Represents the root element.
    /// </summary>
    [XmlRoot("Devices")]
    public class DeviceElements
    {
        /// <summary>
        /// Gets or sets the list of devices.
        /// Each device is represented by a 'Dev' element.
        /// </summary>
        [XmlElement("Dev")]
        public List<Device> DeviceList { get; set; }
    }

    /// <summary>
    /// Represents a device element.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the serial number attribute of the device.
        /// </summary>
        [XmlAttribute("SrNo")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the address of the device.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        public string DevName { get; set; }

        /// <summary>
        /// Gets or sets the model name of the device.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the type of the device.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the communication settings of the device.
        /// </summary>
        public CommSetting CommSetting { get; set; }
    }

    /// <summary>
    /// Represents communication settings for a device.
    /// </summary>
    public class CommSetting
    {
        /// <summary>
        /// Gets or sets the port number used for communication.
        /// </summary>
        public int PortNo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SSL is enabled for communication.
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the password for authentication.
        /// </summary>
        public string Password { get; set; }
    }
}
