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

            var Global = new AppGlobal();
            if (!Global.CLIOption.ParseSuccess)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(Global.CLIOption.HelpMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            } 

            if (Global.CLIOption.Version)
            {

                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(Global.CLIOption.VersionMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            }

            if (Global.CLIOption.Verbose)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine("CreviceApp is running in verbose mode.");
            }

            Debug.Print(string.Format("CLIOption.NoGUI: {0}", Global.CLIOption.NoGUI));
            Debug.Print(string.Format("CLIOption.NoCache: {0}", Global.CLIOption.NoCache));
            Debug.Print(string.Format("CLIOption.Verbose: {0}", Global.CLIOption.Verbose));
            Debug.Print(string.Format("CLIOption.Version: {0}", Global.CLIOption.Version));
            Debug.Print(string.Format("CLIOption.ProcessPriority: {0}", Global.CLIOption.ProcessPriority));
            Debug.Print(string.Format("CLIOption.ScriptFile: {0}", Global.CLIOption.ScriptFile));

            Debug.Print(string.Format("Setting ProcessPriority to {0}", Global.CLIOption.ProcessPriority));
            SetProcessPriority(Global.CLIOption.ProcessPriority);
            
            Application.Run(Global.MainForm);
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
