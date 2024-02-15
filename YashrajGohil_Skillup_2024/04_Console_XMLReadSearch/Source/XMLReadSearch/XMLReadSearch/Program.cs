using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReadSearch
{
    /// <summary>
    /// Main entry class
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Main entry function
        /// </summary>
        /// <param name="args"> For command line arguments </param>
        static void Main(string[] args)
        {
            Demo demo = new Demo();
            demo.Run(@"C:\Projects\Skillup\GIT\YashrajGohil_Skillup_2024\04_Console_XMLReadSearch\Source\XMLReadSearch\XMLReadSearch\DeviceInfo.xml");
            //demo.Run(@"C:\Users\SD-KS-2024-1\Desktop\Yashrajsinh_Gohil_Appointment_Letter.xml");
            //demo.Run(@"C:\Projects\Skillup\GIT\YashrajGohil_Skillup_2024\04_Console_XMLReadSearch\Source\XMLReadSearch\XMLReadSearch\Empty.xml");

        }
    }
}
