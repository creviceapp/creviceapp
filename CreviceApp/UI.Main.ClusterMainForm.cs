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
    using Crevice.GestureMachine;

    public partial class ClusterMainForm : MainFormBase
    {
        internal GestureMachineCluster _gestureMachineCluster = null;
        public override IGestureMachine GestureMachine => _gestureMachineCluster;
        
        public ClusterMainForm(LauncherForm launcherForm)
            : base(launcherForm)
        {
            InitializeComponent();
        }

        public void Run(IReadOnlyList<GestureMachineProfile> gestureMachineProfiles)
        {
            if (_gestureMachineCluster != null)
            {
                throw new InvalidOperationException();
            }
            _gestureMachineCluster = new GestureMachineCluster(gestureMachineProfiles);
            _gestureMachineCluster.Run();
        }

        protected override void OnShown(EventArgs e)
        {
            Verbose.Print("CreviceApp was started.");
            UpdateTasktrayMessage(_gestureMachineCluster.Profiles);
            ShowInfoBalloon(_gestureMachineCluster);
            base.OnShown(e);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _gestureMachineCluster.Stop();
            Verbose.Print("CreviceApp was ended.");
            base.OnClosed(e);
        }
    }
}
