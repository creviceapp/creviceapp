using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace CreviceApp.Logging
{
    public class CustomConsoleTraceListener : ConsoleTraceListener
    {
        public override void Write(string message)
        {
            base.Write(Format(message));
        }

        public override void WriteLine(string message)
        {
            base.WriteLine(Format(message));
        }

        private string Format(string message)
        {
            return string.Format("{0} | {1:00} | {2}", DateTime.Now.ToString("o"), Thread.CurrentThread.ManagedThreadId, message);
        }
    }
}
