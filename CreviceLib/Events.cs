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

        private static Mouse.PhysicalButtons physicalButtons = new Mouse.PhysicalButtons(1100);
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

        public class Buttons
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
                MoveEvent = new MoveEvent(id++);
                WheelDownEvent = new WheelDownEvent(id++);
                WheelUpEvent = new WheelUpEvent(id++);
                WheelLeftEvent = new WheelLeftEvent(id++);
                WheelRightEvent = new WheelRightEvent(id++);
                LeftButtonDownEvent = new LeftButtonDownEvent(id++);
                LeftButtonUpEvent = new LeftButtonUpEvent(id++);
                MiddleButtonDownEvent = new MiddleButtonDownEvent(id++);
                MiddleButtonUpEvent = new MiddleButtonUpEvent(id++);
                RightButtonDownEvent = new RightButtonDownEvent(id++);
                RightButtonUpEvent = new RightButtonUpEvent(id++);
                X1ButtonDownEvent = new X1ButtonDownEvent(id++);
                X1ButtonUpEvent = new X1ButtonUpEvent(id++);
                X2ButtonDownEvent = new X2ButtonDownEvent(id++);
                X2ButtonUpEvent = new X2ButtonUpEvent(id++);
            }
        }

        public class PhysicalButtons
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

            public PhysicalButtons(int offset)
            {
                var id = offset;
                MoveEvent = new PhysicalMoveEvent(id++);
                WheelDownEvent = new PhysicalWheelDownEvent(id++);
                WheelUpEvent = new PhysicalWheelUpEvent(id++);
                WheelLeftEvent = new PhysicalWheelLeftEvent(id++);
                WheelRightEvent = new PhysicalWheelRightEvent(id++);
                LeftButtonDownEvent = new PhysicalLeftButtonDownEvent(id++);
                LeftButtonUpEvent = new PhysicalLeftButtonUpEvent(id++);
                MiddleButtonDownEvent = new PhysicalMiddleButtonDownEvent(id++);
                MiddleButtonUpEvent = new PhysicalMiddleButtonUpEvent(id++);
                RightButtonDownEvent = new PhysicalRightButtonDownEvent(id++);
                RightButtonUpEvent = new PhysicalRightButtonUpEvent(id++);
                X1ButtonDownEvent = new PhysicalX1ButtonDownEvent(id++);
                X1ButtonUpEvent = new PhysicalX1ButtonUpEvent(id++);
                X2ButtonDownEvent = new PhysicalX2ButtonDownEvent(id++);
                X2ButtonUpEvent = new PhysicalX2ButtonUpEvent(id++);
            }
        }

        public class MoveEvent : FireEvent<MoveSwitch>
        {
            public MoveEvent(int eventId) : base(eventId) { }
        }

        public class WheelDownEvent : FireEvent<WheelDownSwitch>
        {
            public WheelDownEvent(int eventId) : base(eventId) { }
        }

        public class WheelUpEvent : FireEvent<WheelUpSwitch>
        {
            public WheelUpEvent(int eventId) : base(eventId) { }
        }

        public class WheelLeftEvent : FireEvent<WheelLeftSwitch>
        {
            public WheelLeftEvent(int eventId) : base(eventId) { }
        }

        public class WheelRightEvent : FireEvent<WheelRightSwitch>
        {
            public WheelRightEvent(int eventId) : base(eventId) { }
        }

        public class LeftButtonDownEvent : PressEvent<LeftButtonSwitch>
        {
            public override ReleaseEvent<LeftButtonSwitch> OppositeReleaseEvent
                => Events.Buttons.LeftButtonUpEvent;

            public LeftButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class LeftButtonUpEvent : ReleaseEvent<LeftButtonSwitch>
        {
            public override PressEvent<LeftButtonSwitch> OppositePressEvent
                => Events.Buttons.LeftButtonDownEvent;

            public LeftButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class MiddleButtonDownEvent : PressEvent<MiddleButtonSwitch>
        {
            public override ReleaseEvent<MiddleButtonSwitch> OppositeReleaseEvent
                => Events.Buttons.MiddleButtonUpEvent;

            public MiddleButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class MiddleButtonUpEvent : ReleaseEvent<MiddleButtonSwitch>
        {
            public override PressEvent<MiddleButtonSwitch> OppositePressEvent
                => Events.Buttons.MiddleButtonDownEvent;

            public MiddleButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class RightButtonDownEvent : PressEvent<RightButtonSwitch>
        {
            public override ReleaseEvent<RightButtonSwitch> OppositeReleaseEvent
                => Events.Buttons.RightButtonUpEvent;

            public RightButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class RightButtonUpEvent : ReleaseEvent<RightButtonSwitch>
        {
            public override PressEvent<RightButtonSwitch> OppositePressEvent
                => Events.Buttons.RightButtonDownEvent;

            public RightButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class X1ButtonDownEvent : PressEvent<X1ButtonSwitch>
        {
            public override ReleaseEvent<X1ButtonSwitch> OppositeReleaseEvent
                => Events.Buttons.X1ButtonUpEvent;

            public X1ButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class X1ButtonUpEvent : ReleaseEvent<X1ButtonSwitch>
        {
            public override PressEvent<X1ButtonSwitch> OppositePressEvent
                => Events.Buttons.X1ButtonDownEvent;

            public X1ButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class X2ButtonDownEvent : PressEvent<X2ButtonSwitch>
        {
            public override ReleaseEvent<X2ButtonSwitch> OppositeReleaseEvent
                => Events.Buttons.X2ButtonUpEvent;

            public X2ButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class X2ButtonUpEvent : ReleaseEvent<X2ButtonSwitch>
        {
            public override PressEvent<X2ButtonSwitch> OppositePressEvent
                => Events.Buttons.X2ButtonDownEvent;

            public X2ButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalMoveEvent : PhysicalFireEvent<MoveSwitch>
        {
            public override FireEvent<MoveSwitch> LogicalEquivalentFireEvent
                => Events.Buttons.MoveEvent;

            public PhysicalMoveEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalWheelDownEvent : PhysicalFireEvent<WheelDownSwitch>
        {
            public override FireEvent<WheelDownSwitch> LogicalEquivalentFireEvent
                => Events.Buttons.WheelDownEvent;

            public PhysicalWheelDownEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalWheelUpEvent : PhysicalFireEvent<WheelUpSwitch>
        {
            public override FireEvent<WheelUpSwitch> LogicalEquivalentFireEvent
                => Events.Buttons.WheelUpEvent;

            public PhysicalWheelUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalWheelLeftEvent : PhysicalFireEvent<WheelLeftSwitch>
        {
            public override FireEvent<WheelLeftSwitch> LogicalEquivalentFireEvent
                => Events.Buttons.WheelLeftEvent;

            public PhysicalWheelLeftEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalWheelRightEvent : PhysicalFireEvent<WheelRightSwitch>
        {
            public override FireEvent<WheelRightSwitch> LogicalEquivalentFireEvent
                => Events.Buttons.WheelRightEvent;

            public PhysicalWheelRightEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalLeftButtonDownEvent : PhysicalPressEvent<LeftButtonSwitch>
        {
            public override PressEvent<LeftButtonSwitch> LogicalEquivalentPressEvent
                => Events.Buttons.LeftButtonDownEvent;

            public override PhysicalReleaseEvent<LeftButtonSwitch> OppositePhysicalReleaseEvent
                => Events.PhysicalButtons.LeftButtonUpEvent;

            public PhysicalLeftButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalLeftButtonUpEvent : PhysicalReleaseEvent<LeftButtonSwitch>
        {
            public override ReleaseEvent<LeftButtonSwitch> LogicalEquivalentReleaseEvent
                => Events.Buttons.LeftButtonUpEvent;

            public override PhysicalPressEvent<LeftButtonSwitch> OppositePhysicalPressEvent
                => Events.PhysicalButtons.LeftButtonDownEvent;

            public PhysicalLeftButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalMiddleButtonDownEvent : PhysicalPressEvent<MiddleButtonSwitch>
        {
            public override PressEvent<MiddleButtonSwitch> LogicalEquivalentPressEvent
                => Events.Buttons.MiddleButtonDownEvent;

            public override PhysicalReleaseEvent<MiddleButtonSwitch> OppositePhysicalReleaseEvent
                => Events.PhysicalButtons.MiddleButtonUpEvent;

            public PhysicalMiddleButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalMiddleButtonUpEvent : PhysicalReleaseEvent<MiddleButtonSwitch>
        {
            public override ReleaseEvent<MiddleButtonSwitch> LogicalEquivalentReleaseEvent
                => Events.Buttons.MiddleButtonUpEvent;

            public override PhysicalPressEvent<MiddleButtonSwitch> OppositePhysicalPressEvent
                => Events.PhysicalButtons.MiddleButtonDownEvent;

            public PhysicalMiddleButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalRightButtonDownEvent : PhysicalPressEvent<RightButtonSwitch>
        {
            public override PressEvent<RightButtonSwitch> LogicalEquivalentPressEvent
                => Events.Buttons.RightButtonDownEvent;

            public override PhysicalReleaseEvent<RightButtonSwitch> OppositePhysicalReleaseEvent
                => Events.PhysicalButtons.RightButtonUpEvent;

            public PhysicalRightButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalRightButtonUpEvent : PhysicalReleaseEvent<RightButtonSwitch>
        {
            public override ReleaseEvent<RightButtonSwitch> LogicalEquivalentReleaseEvent
                => Events.Buttons.RightButtonUpEvent;

            public override PhysicalPressEvent<RightButtonSwitch> OppositePhysicalPressEvent
                => Events.PhysicalButtons.RightButtonDownEvent;

            public PhysicalRightButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalX1ButtonDownEvent : PhysicalPressEvent<X1ButtonSwitch>
        {
            public override PressEvent<X1ButtonSwitch> LogicalEquivalentPressEvent
                => Events.Buttons.X1ButtonDownEvent;

            public override PhysicalReleaseEvent<X1ButtonSwitch> OppositePhysicalReleaseEvent
                => Events.PhysicalButtons.X1ButtonUpEvent;

            public PhysicalX1ButtonDownEvent(int eventId) : base(eventId) { }
        }
        
        public class PhysicalX1ButtonUpEvent : PhysicalReleaseEvent<X1ButtonSwitch>
        {
            public override ReleaseEvent<X1ButtonSwitch> LogicalEquivalentReleaseEvent
                => Events.Buttons.X1ButtonUpEvent;

            public override PhysicalPressEvent<X1ButtonSwitch> OppositePhysicalPressEvent
                => Events.PhysicalButtons.X1ButtonDownEvent;

            public PhysicalX1ButtonUpEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalX2ButtonDownEvent : PhysicalPressEvent<X2ButtonSwitch>
        {
            public override PressEvent<X2ButtonSwitch> LogicalEquivalentPressEvent
                => Events.Buttons.X2ButtonDownEvent;

            public override PhysicalReleaseEvent<X2ButtonSwitch> OppositePhysicalReleaseEvent
                => Events.PhysicalButtons.X2ButtonUpEvent;

            public PhysicalX2ButtonDownEvent(int eventId) : base(eventId) { }
        }

        public class PhysicalX2ButtonUpEvent : PhysicalReleaseEvent<X2ButtonSwitch>
        {
            public override ReleaseEvent<X2ButtonSwitch> LogicalEquivalentReleaseEvent
                => Events.Buttons.X2ButtonUpEvent;

            public override PhysicalPressEvent<X2ButtonSwitch> OppositePhysicalPressEvent
                => Events.PhysicalButtons.X2ButtonDownEvent;

            public PhysicalX2ButtonUpEvent(int eventId) : base(eventId) { }
        }
    }
}
