using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillup.XMLReadSearch
{
    public class CommandLineException : Exception
    {
        public CommandLineException(string message,int errorCode)
        {
            Console.WriteLine($"{message} ({errorCode})");
        }
    }
}
