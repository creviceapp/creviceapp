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
        public readonly Crevice.Core.FSM.GestureMachineConfig Core = new Crevice.Core.FSM.GestureMachineConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();

        public readonly CustomCallbackManager.CallbackReceiver Callback;

        public UserConfig(CustomCallbackManager.CallbackReceiver callbackReceiver)
        {
            Callback = callbackReceiver;
        }
    }
}
