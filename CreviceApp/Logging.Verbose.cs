using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Logging
{
    public class Verbose
    {
        public class ElapsedTimePrinter : IDisposable
        {
            public string Message { get; private set; }
            private Stopwatch stopwatch;

            public ElapsedTimePrinter(string message)
            {
                this.Message = string.Format("[{0}]", message);
                this.stopwatch = new Stopwatch();
                PrintStartMessage();
                stopwatch.Start();
            }

            private void PrintStartMessage()
            {
                Print("{0} was started.", Message);
            }

            private void PrintFinishMessage()
            {
                Print("{0} was finished. ({1})", Message, stopwatch.Elapsed);
            }

            public void Dispose()
            {
                stopwatch.Stop();
                PrintFinishMessage();
            }
        }

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

        // Todo: Verbose.Error()
        
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

        public static ElapsedTimePrinter PrintElapsed(string message)
        {
            return new ElapsedTimePrinter(message);
        }

        public static ElapsedTimePrinter PrintElapsed(string template, params object[] objects)
        {
            return new ElapsedTimePrinter(string.Format(template, objects));
        }
    }
}
