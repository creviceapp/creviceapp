using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp
{
    public class Verbose
    {
        private static Verbose instance;
        private Verbose() { }

        
        public static Verbose Output
        {
            get
            {
                if (instance == null)
                {
                    instance = new Verbose();
                }
                return instance;
            }
        }

        private bool enable = false;

        public void Enable()
        {
            enable = true;
        }
        
        public static void Print(string message)
        {
            Debug.Print(message);
            if (Output.enable)
            {
                Console.WriteLine(message);
            }
        }

        public static void Print(string template, params object[] objects)
        {
            Print(string.Format(template, objects));
        }
    }
}
