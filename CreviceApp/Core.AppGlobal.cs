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
    public class AppGlobal
    {
        public readonly Core.Config.UserConfig UserConfig;
        public readonly MainForm MainForm;

        public AppGlobal()
        {
            this.UserConfig = new Core.Config.UserConfig();
            this.MainForm = new MainForm(this);
        }
    }
}
