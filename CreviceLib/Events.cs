using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Events
{
    using Crevice.Core;
    
    class Events
    {
        private static Mouse.Buttons buttons = new Mouse.Buttons(1000);
        public static Mouse.Buttons Buttons => buttons;

        private static Mouse.PhysicalButtons physicalButtons = new Mouse.PhysicalButtons(Buttons, 1100);
        public static Mouse.PhysicalButtons PhysicalButtons => physicalButtons;
    }

    namespace Mouse
    {
        public class MoveSwitch : SingleThrowSwitch { }
        public class WheelDownSwitch : SingleThrowSwitch { }
        public class WheelUpSwitch : SingleThrowSwitch { }
        public class WheelLeftSwitch : SingleThrowSwitch { }
        public class WheelRightSwitch : SingleThrowSwitch { }
        public class LeftButtonSwitch : DoubleThrowSwitch { }
        public class MiddleButtonSwitch : DoubleThrowSwitch { }
        public class RightButtonSwitch : DoubleThrowSwitch { }
        public class X1ButtonSwitch : DoubleThrowSwitch { }
        public class X2ButtonSwitch : DoubleThrowSwitch { }

        public class Buttons : EventLogicalGroup
        {
            public readonly MoveEvent MoveEvent;
            public readonly WheelDownEvent WheelDownEvent;
            public readonly WheelUpEvent WheelUpEvent;
            public readonly WheelLeftEvent WheelLeftEvent;
            public readonly WheelRightEvent WheelRightEvent;
            public readonly LeftButtonDownEvent LeftButtonDownEvent;
            public readonly LeftButtonUpEvent LeftButtonUpEvent;
            public readonly MiddleButtonDownEvent MiddleButtonDownEvent;
            public readonly MiddleButtonUpEvent MiddleButtonUpEvent;
            public readonly RightButtonDownEvent RightButtonDownEvent;
            public readonly RightButtonUpEvent RightButtonUpEvent;
            public readonly X1ButtonDownEvent X1ButtonDownEvent;
            public readonly X1ButtonUpEvent X1ButtonUpEvent;
            public readonly X2ButtonDownEvent X2ButtonDownEvent;
            public readonly X2ButtonUpEvent X2ButtonUpEvent;

            public Buttons(int offset)
            {
                var id = offset;
                MoveEvent = new MoveEvent(this, id++);
                WheelDownEvent = new WheelDownEvent(this, id++);
                WheelUpEvent = new WheelUpEvent(this, id++);
                WheelLeftEvent = new WheelLeftEvent(this, id++);
                WheelRightEvent = new WheelRightEvent(this, id++);
                LeftButtonDownEvent = new LeftButtonDownEvent(this, id++);
                LeftButtonUpEvent = new LeftButtonUpEvent(this, id++);
                MiddleButtonDownEvent = new MiddleButtonDownEvent(this, id++);
                MiddleButtonUpEvent = new MiddleButtonUpEvent(this, id++);
                RightButtonDownEvent = new RightButtonDownEvent(this, id++);
                RightButtonUpEvent = new RightButtonUpEvent(this, id++);
                X1ButtonDownEvent = new X1ButtonDownEvent(this, id++);
                X1ButtonUpEvent = new X1ButtonUpEvent(this, id++);
                X2ButtonDownEvent = new X2ButtonDownEvent(this, id++);
                X2ButtonUpEvent = new X2ButtonUpEvent(this, id++);
            }
        }

        public class PhysicalButtons : EventPhysicalGroup
        {
            public readonly PhysicalMoveEvent MoveEvent;
            public readonly PhysicalWheelDownEvent WheelDownEvent;
            public readonly PhysicalWheelUpEvent WheelUpEvent;
            public readonly PhysicalWheelLeftEvent WheelLeftEvent;
            public readonly PhysicalWheelRightEvent WheelRightEvent;
            public readonly PhysicalLeftButtonDownEvent LeftButtonDownEvent;
            public readonly PhysicalLeftButtonUpEvent LeftButtonUpEvent;
            public readonly PhysicalMiddleButtonDownEvent MiddleButtonDownEvent;
            public readonly PhysicalMiddleButtonUpEvent MiddleButtonUpEvent;
            public readonly PhysicalRightButtonDownEvent RightButtonDownEvent;
            public readonly PhysicalRightButtonUpEvent RightButtonUpEvent;
            public readonly PhysicalX1ButtonDownEvent X1ButtonDownEvent;
            public readonly PhysicalX1ButtonUpEvent X1ButtonUpEvent;
            public readonly PhysicalX2ButtonDownEvent X2ButtonDownEvent;
            public readonly PhysicalX2ButtonUpEvent X2ButtonUpEvent;

            public PhysicalButtons(Buttons buttons, int offset)
            {
                var id = offset;
                MoveEvent = new PhysicalMoveEvent(buttons, this, id++);
                WheelDownEvent = new PhysicalWheelDownEvent(buttons, this, id++);
                WheelUpEvent = new PhysicalWheelUpEvent(buttons, this, id++);
                WheelLeftEvent = new PhysicalWheelLeftEvent(buttons, this, id++);
                WheelRightEvent = new PhysicalWheelRightEvent(buttons, this, id++);
                LeftButtonDownEvent = new PhysicalLeftButtonDownEvent(buttons, this, id++);
                LeftButtonUpEvent = new PhysicalLeftButtonUpEvent(buttons, this, id++);
                MiddleButtonDownEvent = new PhysicalMiddleButtonDownEvent(buttons, this, id++);
                MiddleButtonUpEvent = new PhysicalMiddleButtonUpEvent(buttons, this, id++);
                RightButtonDownEvent = new PhysicalRightButtonDownEvent(buttons, this, id++);
                RightButtonUpEvent = new PhysicalRightButtonUpEvent(buttons, this, id++);
                X1ButtonDownEvent = new PhysicalX1ButtonDownEvent(buttons, this, id++);
                X1ButtonUpEvent = new PhysicalX1ButtonUpEvent(buttons, this, id++);
                X2ButtonDownEvent = new PhysicalX2ButtonDownEvent(buttons, this, id++);
                X2ButtonUpEvent = new PhysicalX2ButtonUpEvent(buttons, this, id++);
            }
        }

        public class MoveEvent : FireEvent<Buttons, MoveSwitch>
        {
            public MoveEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class WheelDownEvent : FireEvent<Buttons, WheelDownSwitch>
        {
            public WheelDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class WheelUpEvent : FireEvent<Buttons, WheelUpSwitch>
        {
            public WheelUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class WheelLeftEvent : FireEvent<Buttons, WheelLeftSwitch>
        {
            public WheelLeftEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class WheelRightEvent : FireEvent<Buttons, WheelRightSwitch>
        {
            public WheelRightEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class LeftButtonDownEvent : PressEvent<Buttons, LeftButtonSwitch>
        {
            public override ReleaseEvent<Buttons, LeftButtonSwitch> OppositeReleaseEvent
                => LogicalGroup.LeftButtonUpEvent;

            public LeftButtonDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class LeftButtonUpEvent : ReleaseEvent<Buttons, LeftButtonSwitch>
        {
            public override PressEvent<Buttons, LeftButtonSwitch> OppositePressEvent
                => LogicalGroup.LeftButtonDownEvent;

            public LeftButtonUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class MiddleButtonDownEvent : PressEvent<Buttons, MiddleButtonSwitch>
        {
            public override ReleaseEvent<Buttons, MiddleButtonSwitch> OppositeReleaseEvent
                => LogicalGroup.MiddleButtonUpEvent;

            public MiddleButtonDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class MiddleButtonUpEvent : ReleaseEvent<Buttons, MiddleButtonSwitch>
        {
            public override PressEvent<Buttons, MiddleButtonSwitch> OppositePressEvent
                => LogicalGroup.MiddleButtonDownEvent;

            public MiddleButtonUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class RightButtonDownEvent : PressEvent<Buttons, RightButtonSwitch>
        {
            public override ReleaseEvent<Buttons, RightButtonSwitch> OppositeReleaseEvent
                => LogicalGroup.RightButtonUpEvent;

            public RightButtonDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class RightButtonUpEvent : ReleaseEvent<Buttons, RightButtonSwitch>
        {
            public override PressEvent<Buttons, RightButtonSwitch> OppositePressEvent
                => LogicalGroup.RightButtonDownEvent;

            public RightButtonUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class X1ButtonDownEvent : PressEvent<Buttons, X1ButtonSwitch>
        {
            public override ReleaseEvent<Buttons, X1ButtonSwitch> OppositeReleaseEvent
                => LogicalGroup.X1ButtonUpEvent;

            public X1ButtonDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class X1ButtonUpEvent : ReleaseEvent<Buttons, X1ButtonSwitch>
        {
            public override PressEvent<Buttons, X1ButtonSwitch> OppositePressEvent
                => LogicalGroup.X1ButtonDownEvent;

            public X1ButtonUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class X2ButtonDownEvent : PressEvent<Buttons, X2ButtonSwitch>
        {
            public override ReleaseEvent<Buttons, X2ButtonSwitch> OppositeReleaseEvent
                => LogicalGroup.X2ButtonUpEvent;

            public X2ButtonDownEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class X2ButtonUpEvent : ReleaseEvent<Buttons, X2ButtonSwitch>
        {
            public override PressEvent<Buttons, X2ButtonSwitch> OppositePressEvent
                => LogicalGroup.X2ButtonDownEvent;

            public X2ButtonUpEvent(Buttons eventGroup, int eventId) : base(eventGroup, eventId) { }
        }

        public class PhysicalMoveEvent : PhysicalFireEvent<Buttons, PhysicalButtons, MoveSwitch>
        {
            public override FireEvent<Buttons, MoveSwitch> LogicalEquivalentFireEvent
                => LogicalGroup.MoveEvent;

            public PhysicalMoveEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalWheelDownEvent : PhysicalFireEvent<Buttons, PhysicalButtons, WheelDownSwitch>
        {
            public override FireEvent<Buttons, WheelDownSwitch> LogicalEquivalentFireEvent
                => LogicalGroup.WheelDownEvent;

            public PhysicalWheelDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalWheelUpEvent : PhysicalFireEvent<Buttons, PhysicalButtons, WheelUpSwitch>
        {
            public override FireEvent<Buttons, WheelUpSwitch> LogicalEquivalentFireEvent
                => LogicalGroup.WheelUpEvent;

            public PhysicalWheelUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalWheelLeftEvent : PhysicalFireEvent<Buttons, PhysicalButtons, WheelLeftSwitch>
        {
            public override FireEvent<Buttons, WheelLeftSwitch> LogicalEquivalentFireEvent
                => LogicalGroup.WheelLeftEvent;

            public PhysicalWheelLeftEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalWheelRightEvent : PhysicalFireEvent<Buttons, PhysicalButtons, WheelRightSwitch>
        {
            public override FireEvent<Buttons, WheelRightSwitch> LogicalEquivalentFireEvent
                => LogicalGroup.WheelRightEvent;

            public PhysicalWheelRightEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalLeftButtonDownEvent : PhysicalPressEvent<Buttons, PhysicalButtons, LeftButtonSwitch>
        {
            public override PressEvent<Buttons, LeftButtonSwitch> LogicalEquivalentPressEvent
                => LogicalGroup.LeftButtonDownEvent;

            public override PhysicalReleaseEvent<Buttons, PhysicalButtons, LeftButtonSwitch> OppositePhysicalReleaseEvent
                => PhysicalGroup.LeftButtonUpEvent;

            public PhysicalLeftButtonDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalLeftButtonUpEvent : PhysicalReleaseEvent<Buttons, PhysicalButtons, LeftButtonSwitch>
        {
            public override ReleaseEvent<Buttons, LeftButtonSwitch> LogicalEquivalentReleaseEvent
                => LogicalGroup.LeftButtonUpEvent;

            public override PhysicalPressEvent<Buttons, PhysicalButtons, LeftButtonSwitch> OppositePhysicalPressEvent
                => PhysicalGroup.LeftButtonDownEvent;

            public PhysicalLeftButtonUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalMiddleButtonDownEvent : PhysicalPressEvent<Buttons, PhysicalButtons, MiddleButtonSwitch>
        {
            public override PressEvent<Buttons, MiddleButtonSwitch> LogicalEquivalentPressEvent
                => LogicalGroup.MiddleButtonDownEvent;

            public override PhysicalReleaseEvent<Buttons, PhysicalButtons, MiddleButtonSwitch> OppositePhysicalReleaseEvent
                => PhysicalGroup.MiddleButtonUpEvent;

            public PhysicalMiddleButtonDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalMiddleButtonUpEvent : PhysicalReleaseEvent<Buttons, PhysicalButtons, MiddleButtonSwitch>
        {
            public override ReleaseEvent<Buttons, MiddleButtonSwitch> LogicalEquivalentReleaseEvent
                => LogicalGroup.MiddleButtonUpEvent;

            public override PhysicalPressEvent<Buttons, PhysicalButtons, MiddleButtonSwitch> OppositePhysicalPressEvent
                => PhysicalGroup.MiddleButtonDownEvent;

            public PhysicalMiddleButtonUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalRightButtonDownEvent : PhysicalPressEvent<Buttons, PhysicalButtons, RightButtonSwitch>
        {
            public override PressEvent<Buttons, RightButtonSwitch> LogicalEquivalentPressEvent
                => LogicalGroup.RightButtonDownEvent;

            public override PhysicalReleaseEvent<Buttons, PhysicalButtons, RightButtonSwitch> OppositePhysicalReleaseEvent
                => PhysicalGroup.RightButtonUpEvent;

            public PhysicalRightButtonDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalRightButtonUpEvent : PhysicalReleaseEvent<Buttons, PhysicalButtons, RightButtonSwitch>
        {
            public override ReleaseEvent<Buttons, RightButtonSwitch> LogicalEquivalentReleaseEvent
                => LogicalGroup.RightButtonUpEvent;

            public override PhysicalPressEvent<Buttons, PhysicalButtons, RightButtonSwitch> OppositePhysicalPressEvent
                => PhysicalGroup.RightButtonDownEvent;

            public PhysicalRightButtonUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalX1ButtonDownEvent : PhysicalPressEvent<Buttons, PhysicalButtons, X1ButtonSwitch>
        {
            public override PressEvent<Buttons, X1ButtonSwitch> LogicalEquivalentPressEvent
                => LogicalGroup.X1ButtonDownEvent;

            public override PhysicalReleaseEvent<Buttons, PhysicalButtons, X1ButtonSwitch> OppositePhysicalReleaseEvent
                => PhysicalGroup.X1ButtonUpEvent;

            public PhysicalX1ButtonDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }
        
        public class PhysicalX1ButtonUpEvent : PhysicalReleaseEvent<Buttons, PhysicalButtons, X1ButtonSwitch>
        {
            public override ReleaseEvent<Buttons, X1ButtonSwitch> LogicalEquivalentReleaseEvent
                => LogicalGroup.X1ButtonUpEvent;

            public override PhysicalPressEvent<Buttons, PhysicalButtons, X1ButtonSwitch> OppositePhysicalPressEvent
                => PhysicalGroup.X1ButtonDownEvent;

            public PhysicalX1ButtonUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalX2ButtonDownEvent : PhysicalPressEvent<Buttons, PhysicalButtons, X2ButtonSwitch>
        {
            public override PressEvent<Buttons, X2ButtonSwitch> LogicalEquivalentPressEvent
                => LogicalGroup.X2ButtonDownEvent;

            public override PhysicalReleaseEvent<Buttons, PhysicalButtons, X2ButtonSwitch> OppositePhysicalReleaseEvent
                => PhysicalGroup.X2ButtonUpEvent;

            public PhysicalX2ButtonDownEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }

        public class PhysicalX2ButtonUpEvent : PhysicalReleaseEvent<Buttons, PhysicalButtons, X2ButtonSwitch>
        {
            public override ReleaseEvent<Buttons, X2ButtonSwitch> LogicalEquivalentReleaseEvent
                => LogicalGroup.X2ButtonUpEvent;

            public override PhysicalPressEvent<Buttons, PhysicalButtons, X2ButtonSwitch> OppositePhysicalPressEvent
                => PhysicalGroup.X2ButtonDownEvent;

            public PhysicalX2ButtonUpEvent(Buttons logicalGroup, PhysicalButtons eventGroup, int eventId) : base(logicalGroup, eventGroup, eventId) { }
        }
    }
}
