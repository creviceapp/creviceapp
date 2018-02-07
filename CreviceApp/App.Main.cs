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

            var CLIOption = App.CLIOption.ParseEnvironmentCommandLine();
            if (!CLIOption.ParseSuccess)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(CLIOption.HelpMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            } 

            if (CLIOption.Version)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(CLIOption.VersionMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            }

            if (CLIOption.Verbose)
            {
                WinAPI.Console.Console.AttachConsole();
                Verbose.Output.Enable();
            }

            Verbose.Print("CLIOption.NoGUI: {0}", CLIOption.NoGUI);
            Verbose.Print("CLIOption.NoCache: {0}", CLIOption.NoCache);
            Verbose.Print("CLIOption.Verbose: {0}", CLIOption.Verbose);
            Verbose.Print("CLIOption.Version: {0}", CLIOption.Version);
            Verbose.Print("CLIOption.ProcessPriority: {0}", CLIOption.ProcessPriority);
            Verbose.Print("CLIOption.ScriptFile: {0}", CLIOption.ScriptFile);

            Verbose.Print("Setting ProcessPriority to {0}", CLIOption.ProcessPriority);
            SetProcessPriority(CLIOption.ProcessPriority);

            var AppConfig = new App.AppConfig(CLIOption);
            Application.Run(AppConfig.MainForm);
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
