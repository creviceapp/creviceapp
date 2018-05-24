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
            public readonly string Message;

            private readonly Stopwatch _stopwatch = new Stopwatch();

            public ElapsedTimePrinter(string message)
            {
                Message = $"[{message}]";
                PrintStartMessage();
                _stopwatch.Start();
            }

            private void PrintStartMessage()
            {
                Print($"{Message} was started.");
            }

            private void PrintFinishMessage()
            {
                Print($"{Message} was finished. ({_stopwatch.Elapsed})");
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
        
        public static void Print(string message, bool omitNewline = false)
        {
            Debug.Print(message);
            if (Enabled)
            {
                try
                {
                    if (omitNewline)
                    {
                        Console.Write(message);
                    }
                    else
                    {
                        Console.WriteLine(message);
                    }
                }
                catch (System.IO.IOException) {}
                catch (UnauthorizedAccessException) {}
            }
        }

        public static void Error(string message, bool omitPrefix = false, bool omitNewline = false)
        {
            var errorMessage = omitPrefix ? message : $"[Error] {message}";
            Debug.Print(errorMessage);
            if (Enabled)
            {
                if (omitNewline)
                {
                    Console.Error.Write(errorMessage);
                }
                else
                {
                    Console.Error.WriteLine(errorMessage);
                }
            }
        }

        public static ElapsedTimePrinter PrintElapsed(string message)
            => new ElapsedTimePrinter(message);
    }
}
