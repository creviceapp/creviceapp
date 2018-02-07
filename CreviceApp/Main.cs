using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice
{
    using Crevice.Logging;
    using Crevice.Config;

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

            var cliOption = CLIOption.ParseEnvironmentCommandLine();
            if (!cliOption.ParseSuccess)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(cliOption.HelpMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            } 

            if (cliOption.Version)
            {
                WinAPI.Console.Console.AttachConsole();
                Console.WriteLine(cliOption.VersionMessage);
                WinAPI.Console.Console.FreeConsole();
                return;
            }

            if (cliOption.Verbose)
            {
                WinAPI.Console.Console.AttachConsole();
                Verbose.Output.Enable();
            }

            Verbose.Print("CLIOption.NoGUI: {0}", cliOption.NoGUI);
            Verbose.Print("CLIOption.NoCache: {0}", cliOption.NoCache);
            Verbose.Print("CLIOption.Verbose: {0}", cliOption.Verbose);
            Verbose.Print("CLIOption.Version: {0}", cliOption.Version);
            Verbose.Print("CLIOption.ProcessPriority: {0}", cliOption.ProcessPriority);
            Verbose.Print("CLIOption.ScriptFile: {0}", cliOption.ScriptFile);

            Verbose.Print("Setting ProcessPriority to {0}", cliOption.ProcessPriority);
            SetProcessPriority(cliOption.ProcessPriority);

            var globalConfig = new GlobalConfig(cliOption);
            Application.Run(globalConfig.MainForm);
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
