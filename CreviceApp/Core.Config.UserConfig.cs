using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core.Config
{
    public class UserConfig
    {
        public readonly Crevice.Core.FSM.GestureMachineConfig Gesture = new Crevice.Core.FSM.GestureMachineConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();
    }
}
