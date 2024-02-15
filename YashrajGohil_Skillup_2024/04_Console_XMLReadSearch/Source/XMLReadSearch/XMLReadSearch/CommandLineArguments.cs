using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReadSearch
{
    public class CommandLineArguments
    {
        public static bool ValidateInput(string args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Error invalid input. Program usage is as below.");
                Console.WriteLine($"[{System.AppDomain.CurrentDomain.FriendlyName}][XML file path]");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
