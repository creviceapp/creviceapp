using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    public class GestureMachineConfig
    {
        // ms
        public int GestureTimeout { get; set; } = 1000;
        // px
        public int StrokeStartThreshold { get; set; } = 10;
        // px
        public int StrokeDirectionChangeThreshold { get; set; } = 20;
        // px
        public int StrokeExtensionThreshold { get; set; } = 10;
        // ms
        public int StrokeWatchInterval { get; set; } = 10;
    }
}
