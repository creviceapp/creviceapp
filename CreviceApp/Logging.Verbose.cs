using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Logging
{
    using System.Collections.Concurrent;

    public static class Verbose
    {
        internal enum MessageType
        {
            Standard,
            Error
        }

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

        internal static BlockingCollection<(MessageType, string)> queue = null;

        private static void SetupQueue()
        {
            var bc = new BlockingCollection<(MessageType, string)>();
            Task.Run(() =>
            {
                foreach (var (type, message) in bc.GetConsumingEnumerable())
                {
                    switch (type)
                    {
                        case MessageType.Standard:
                            try
                            {
                                Console.Out.Write(message);
                            }
                            catch (System.IO.IOException) { }
                            catch (UnauthorizedAccessException) { }
                            break;
                        case MessageType.Error:
                            try
                            {
                                Console.Error.Write(message);
                            }
                            catch (System.IO.IOException) { }
                            catch (UnauthorizedAccessException) { }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            });
            queue = bc;
        }

        private static readonly object _lockObject = new object();
        private static bool _enabled = false;
        public static bool Enabled
        {
            get => _enabled;
            set
            {
                lock (_lockObject)
                {
                    if (value)
                    {
                        if (queue == null)
                        {
                            SetupQueue();
                        }
                        _enabled = true;
                        return;
                    }
                    if (queue != null)
                    {
                        queue.CompleteAdding();
                    }
                    queue = null;
                    _enabled = false;
                }
            }
        }
        
        public static void Print(string message, bool omitNewline = false)
        {
            var standardMessage = message + (omitNewline ? "" : "\r\n");
            Debug.Write(standardMessage);
            lock (_lockObject)
            {
                if (Enabled)
                {
                    queue.Add((MessageType.Standard, standardMessage));
                }
            }
        }

        public static void Error(string message, bool omitPrefix = false, bool omitNewline = false)
        {
            var errorMessage = (omitPrefix ? "" : "[Error] ") + message + (omitNewline ? "" : "\r\n");
            Debug.Write(errorMessage);
            lock (_lockObject)
            {
                if (Enabled)
                {
                    queue.Add((MessageType.Error, errorMessage));
                }
            }
        }

        public static ElapsedTimePrinter PrintElapsed(string message)
            => new ElapsedTimePrinter(message);
    }
}
