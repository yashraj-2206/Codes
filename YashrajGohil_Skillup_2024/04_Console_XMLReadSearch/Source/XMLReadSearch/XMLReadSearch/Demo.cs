using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLReadSearch
{
    public class Demo
    {
        public void Run(string args)
        {
            //if(CommandLineArguments.ValidateInput(args) && XML.ValidateXML(args))
            //{
           Dictionary<int, Dictionary<string, string>> dic = XML.ValidateXML(args);
            Console.WriteLine(dic);
            int option = 0;

                while (option != 3)
                {
                    DisplayOptions();
                    option = GetUserChoice();

                }

//            }



        }

        public void DisplayOptions()
        {
            Console.WriteLine("Please select option");
            Console.WriteLine("[1] Show all devices");
            Console.WriteLine("[2] Search devices by serial number");
            Console.WriteLine("[3] Exit");
        }

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
    }
}