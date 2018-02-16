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
    using Crevice.Config;
    using Crevice.GestureMachine;

    public partial class ClusterMainForm : MainFormBase
    {
        internal readonly GestureMachineCluster _gestureMachineCluster;
        public override IGestureMachine GestureMachine => _gestureMachineCluster;

        private readonly GlobalConfig _config;
        protected internal override GlobalConfig Config => _config; 

        public ClusterMainForm(CLIOption.Result cliOption, GestureMachineCluster gestureMachineCluster)
            : base()
        {
            _config = new GlobalConfig(cliOption, this);
            _gestureMachineCluster = gestureMachineCluster;
            InitializeComponent();
        }

        private string GetActivatedMessage(GestureMachineCluster gmCluster)
            => $"{gmCluster.Profiles.Select(p => p.RootElement.GestureCount).Sum()} Gestures Activated";
        
        protected override void OnShown(EventArgs e)
        {
            RegisterNotifyIcon(NotifyIcon1);
            _gestureMachineCluster.Run();
            UpdateTasktrayMessage(_gestureMachineCluster.Profiles);
            ShowInfoBalloon(_gestureMachineCluster);
            base.OnShown(e);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            NotifyIcon1.Visible = false;
            _gestureMachineCluster.Dispose();
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
