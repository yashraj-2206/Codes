using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Skillup.XMLReadSearch
{

    [XmlRoot("Devices")]
    public class DeviceElements
    {
        [XmlElement("Dev")]
        public List<Device> DeviceList { get; set; }
    }

    public class Device 
    { 
        [XmlAttribute("SrNo")]
        public string SerialNumber { get; set; }

        public string Address { get; set; }

        public string DevName { get; set; }

        public string ModelName { get; set; }

        public string Type { get; set; }

        public CommSetting CommSetting { get; set; }

    }

    public class CommSetting
    {
        public int PortNo { get; set; }

        public bool UseSSL { get; set; }

        public string Password { get; set; }

        
    }
}
