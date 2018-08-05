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
                Verbose.Enable();
            }

            Verbose.Print($"CLIOption.NoGUI: {cliOption.NoGUI}");
            Verbose.Print($"CLIOption.NoCache: {cliOption.NoCache}");
            Verbose.Print($"CLIOption.Verbose: {cliOption.Verbose}");
            Verbose.Print($"CLIOption.Version: {cliOption.Version}");
            Verbose.Print($"CLIOption.ProcessPriority: {cliOption.ProcessPriority}");
            Verbose.Print($"CLIOption.ScriptFile: {cliOption.ScriptFile}");

            Verbose.Print($"Setting ProcessPriority to {cliOption.ProcessPriority}");
            SetProcessPriority(cliOption.ProcessPriority);

            var config = new GlobalConfig(cliOption);
            PrepareUserScript(config);
            
            var launcherForm = new UI.LauncherForm(config);
            var mainForm = new UI.ReloadableMainForm(launcherForm);
            launcherForm.MainForm = mainForm;
            Application.Run(launcherForm);
        }

        private static void PrepareUserScript(GlobalConfig config)
        {
            if (!System.IO.File.Exists(config.UserScriptFile))
            {
                config.WriteUserScriptFile(Encoding.UTF8.GetString(Properties.Resources.DefaultUserScript));
            }
        }

        private static void SetProcessPriority(ProcessPriorityClass priority)
        {
            Process.GetCurrentProcess().PriorityClass = priority;
        }
    }
}
