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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SetProcessPriority();

            WinAPI.Console.Console.AttachConsole();

            var Global = new AppGlobal();
            if (!Global.CLIOption.parseSuccess)
            {
                Console.WriteLine(Global.CLIOption.helpMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            } 

            if (Global.CLIOption.NoGUI)
            {
                Console.WriteLine("CreviceApp is running in NoGUI mode.");
            }
            
            Application.Run(Global.MainForm);
        }

        private static void SetProcessPriority()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }
    }
}
