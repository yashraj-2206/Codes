using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillup.XMLReadSearch
{
   public enum XmlFileExceptionCode
    {
        FileNotExist = 1000,
        NotXmlExtension,
        InvalidFile,
        EmptyFile,
        NoDevicePresent,
        InvalidDeviceInformation
    }

    public enum CommandeLineExceptionCode
    {
        InvalidCommandLineInput = 2000
    }
}
