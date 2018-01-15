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

            var AppConfig = new App.AppConfig();
            if (!AppConfig.CLIOption.ParseSuccess)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(AppConfig.CLIOption.HelpMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            } 

            if (AppConfig.CLIOption.Version)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(AppConfig.CLIOption.VersionMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            }

            if (AppConfig.CLIOption.Verbose)
            {
                WinAPI.Console.Console.AttachConsole();
                Verbose.Output.Enable();
            }

            Verbose.Print("CLIOption.NoGUI: {0}", AppConfig.CLIOption.NoGUI);
            Verbose.Print("CLIOption.NoCache: {0}", AppConfig.CLIOption.NoCache);
            Verbose.Print("CLIOption.Verbose: {0}", AppConfig.CLIOption.Verbose);
            Verbose.Print("CLIOption.Version: {0}", AppConfig.CLIOption.Version);
            Verbose.Print("CLIOption.ProcessPriority: {0}", AppConfig.CLIOption.ProcessPriority);
            Verbose.Print("CLIOption.ScriptFile: {0}", AppConfig.CLIOption.ScriptFile);

            Verbose.Print("Setting ProcessPriority to {0}", AppConfig.CLIOption.ProcessPriority);
            SetProcessPriority(AppConfig.CLIOption.ProcessPriority);
            
            Application.Run(AppConfig.MainForm);
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
