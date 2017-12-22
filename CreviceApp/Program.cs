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
                Verbose.Output.Enable();
            }

            Verbose.Print("CLIOption.NoGUI: {0}", Global.CLIOption.NoGUI);
            Verbose.Print("CLIOption.NoCache: {0}", Global.CLIOption.NoCache);
            Verbose.Print("CLIOption.Verbose: {0}", Global.CLIOption.Verbose);
            Verbose.Print("CLIOption.Version: {0}", Global.CLIOption.Version);
            Verbose.Print("CLIOption.ProcessPriority: {0}", Global.CLIOption.ProcessPriority);
            Verbose.Print("CLIOption.ScriptFile: {0}", Global.CLIOption.ScriptFile);

            Verbose.Print("Setting ProcessPriority to {0}", Global.CLIOption.ProcessPriority);
            SetProcessPriority(Global.CLIOption.ProcessPriority);
            
            Application.Run(Global.MainForm);
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
