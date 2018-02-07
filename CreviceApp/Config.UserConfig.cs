using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crevice.Config
{
    using Crevice.GestureMachine;

    public class UserConfig
    {
        public readonly Core.FSM.GestureMachineConfig Core = new Core.FSM.GestureMachineConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();

        public readonly CallbackManager.CallbackReceiver Callback;

        public UserConfig(CallbackManager.CallbackReceiver callbackReceiver)
        {
            Callback = callbackReceiver;
        }
    }
}
