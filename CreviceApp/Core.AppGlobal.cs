using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    public class AppGlobal
    {
        public readonly CLIOption.Result CLIOption;
        public readonly Core.Config.UserConfig UserConfig;
        public readonly MainForm MainForm;

        public AppGlobal()
        {
            // StreamWriter sw = new StreamWriter(@"C:\Crevice\debug.log");
            // Debug.Listeners.Add(new TextWriterTraceListener(sw));

            this.CLIOption = CreviceApp.CLIOption.ParseEnvironmentCommandLine();
            this.UserConfig = new Core.Config.UserConfig();
            this.MainForm = new MainForm(this);
        }
    }
}
