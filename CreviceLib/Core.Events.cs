using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Events
{
    class Events
    {
        private static Mouse.LogicalButtons logicalButtons = new Mouse.LogicalButtons(1000);
        public static Mouse.LogicalButtons LogicalButtons => logicalButtons;

        private static Mouse.PhysicalButtons physicalButtons = new Mouse.PhysicalButtons(LogicalButtons, 1100);
        public static Mouse.PhysicalButtons PhysicalButtons => physicalButtons;
    }
}
