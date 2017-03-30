using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Trace.Listeners.Clear();
            //Trace.Listeners.Add(new Logging.CustomConsoleTraceListener());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetProcessPriority();
            RunApplication();
        }

        private static void SetProcessPriority()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        private static void RunApplication()
        {
            var Global = new AppGlobal();
            Application.Run(Global.MainForm);
        }
    }
}
