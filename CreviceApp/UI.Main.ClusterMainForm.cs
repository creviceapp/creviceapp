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
    using Crevice.GestureMachine;

    public partial class ClusterMainForm : MainFormBase
    {
        internal readonly GestureMachineCluster _gestureMachineCluster;
        public override IGestureMachine GestureMachine => _gestureMachineCluster;

        private readonly GlobalConfig _config;
        protected internal override GlobalConfig Config => _config; 

        public ClusterMainForm(CLIOption.Result cliOption, IReadOnlyList<GestureMachineProfile> gestureMachineProfiles)
            : base()
        {
            _config = new GlobalConfig(cliOption, this);
            _gestureMachineCluster = new GestureMachineCluster(gestureMachineProfiles);
            _gestureMachineCluster.Run();
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            Verbose.Print("CreviceApp was started.");
            RegisterNotifyIcon(NotifyIcon1);
            UpdateTasktrayMessage(_gestureMachineCluster.Profiles);
            ShowInfoBalloon(_gestureMachineCluster);
            base.OnShown(e);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NotifyIcon1.Visible = false;
            _gestureMachineCluster.Stop();
            _gestureMachineCluster.Dispose();
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
