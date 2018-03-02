using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Logging
{
    public static class Verbose
    {
        public class ElapsedTimePrinter : IDisposable
        {
            public string Message { get; private set; }

            private readonly Stopwatch _stopwatch = new Stopwatch();

            public ElapsedTimePrinter(string message)
            {
                Message = string.Format("[{0}]", message);
                PrintStartMessage();
                _stopwatch.Start();
            }

            private void PrintStartMessage()
            {
                Print("{0} was started.", Message);
            }

            private void PrintFinishMessage()
            {
                Print("{0} was finished. ({1})", Message, _stopwatch.Elapsed);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _stopwatch.Stop();
                    PrintFinishMessage();
                }
            }

            ~ElapsedTimePrinter() => Dispose(false);
        }

        public static bool Enabled { get; private set; }

        public static void Enable()
        {
            Enabled = true;
        }
        
        public static void Print(string message)
        {
            Debug.Print(message);
            if (Enabled)
            {
                Console.WriteLine(message);
            }
        }

        public static void Print(string template, params object[] objects)
            => Print(string.Format(template, objects));

        public static ElapsedTimePrinter PrintElapsed(string message)
            => new ElapsedTimePrinter(message);

        public static ElapsedTimePrinter PrintElapsed(string template, params object[] objects)
            => new ElapsedTimePrinter(string.Format(template, objects));

        public static void Error(string message)
        {
            var errorMessage = string.Format("[Error]{0}", message);
            Debug.Print(errorMessage);
            if (Enabled)
            {
                Console.Error.WriteLine(errorMessage);
            }
        }

        public static void Error(string template, params object[] objects)
            => Error(string.Format(template, objects));
    }
}
