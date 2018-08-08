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
        public readonly GestureMachineConfig Core = new GestureMachineConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();

        public readonly CallbackManager.CallbackContainer Callback;

        public UserConfig(CallbackManager.CallbackContainer callback)
        {
            Callback = callback;
        }
    }
}
