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
        public readonly GestureConfig Gesture = new GestureConfig();
        public readonly UserInterfaceConfig UI = new UserInterfaceConfig();
    }
}
