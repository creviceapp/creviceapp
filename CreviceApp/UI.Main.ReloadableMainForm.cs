using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.UI
{
    using Crevice.Core.FSM;
    using Crevice.Logging;
    using Crevice.Config;
    using Crevice.UserScript;
    using Crevice.GestureMachine;

    public partial class ReloadableMainForm : MainFormBase
    {
        private readonly FileSystemWatcher _userScriptWatcher;

        internal readonly ReloadableGestureMachine _reloadableGestureMachine;
        public override IGestureMachine GestureMachine => _reloadableGestureMachine;

        private readonly GlobalConfig _config;
        protected internal override GlobalConfig Config => _config; 

        public ReloadableMainForm(CLIOption.Result cliOption)
            : base()
        {
            _config = new GlobalConfig(cliOption, this);
            _reloadableGestureMachine = new ReloadableGestureMachine(Config);
            _userScriptWatcher = new UserScript.DirectoryWatcher(this, Config.UserDirectory, "*.csx");
            InitializeComponent();
        }
        
        private void ReloadGestureMachine(object source, FileSystemEventArgs e)
            => Task.Run(() => 
            {
                _reloadableGestureMachine.HotReload();
            });

        private void SetupUserScriptWatcher()
        {
            _userScriptWatcher.Changed += new FileSystemEventHandler(ReloadGestureMachine);
            _userScriptWatcher.Created += new FileSystemEventHandler(ReloadGestureMachine);
            _userScriptWatcher.Renamed += new RenamedEventHandler(ReloadGestureMachine);
            _userScriptWatcher.EnableRaisingEvents = true;
        }

        protected override void OnShown(EventArgs e)
        {
            Verbose.Print("CreviceApp was started.");
            RegisterNotifyIcon(NotifyIcon1);
            SetupUserScriptWatcher();
            base.OnShown(e);
            _reloadableGestureMachine.HotReload();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NotifyIcon1.Visible = false;
            _userScriptWatcher.EnableRaisingEvents = false;
            _userScriptWatcher.Dispose();
            _reloadableGestureMachine.Dispose();
            Verbose.Print("CreviceApp was ended.");
        }

        public override void UpdateTasktrayMessage(string message)
            => UpdateTasktrayMessage(NotifyIcon1, message);

        public override void ShowBalloon(string text, string title, ToolTipIcon icon, int timeout)
            => ShowBalloon(NotifyIcon1, text, title, icon, timeout);

        private void NotifyIcon1_Click(object sender, EventArgs e)
            => OpenLauncherForm();

        private void NotifyIcon1_BalloonTipClicked(object sender, EventArgs e)
            => OpenLastErrorMessageWithNotepad();
    }
}
