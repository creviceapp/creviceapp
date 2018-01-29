using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Events.Mouse
{
    using Crevice.Core.Types;
    
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

    public class LogicalButtons : LogicalGroup
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

        public LogicalButtons(int offset)
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

    public class PhysicalButtons : PhysicalGroup
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

        public PhysicalButtons(LogicalButtons logicalButtons, int offset)
        {
            var id = offset;
            MoveEvent = new PhysicalMoveEvent(logicalButtons, this, id++);
            WheelDownEvent = new PhysicalWheelDownEvent(logicalButtons, this, id++);
            WheelUpEvent = new PhysicalWheelUpEvent(logicalButtons, this, id++);
            WheelLeftEvent = new PhysicalWheelLeftEvent(logicalButtons, this, id++);
            WheelRightEvent = new PhysicalWheelRightEvent(logicalButtons, this, id++);
            LeftButtonDownEvent = new PhysicalLeftButtonDownEvent(logicalButtons, this, id++);
            LeftButtonUpEvent = new PhysicalLeftButtonUpEvent(logicalButtons, this, id++);
            MiddleButtonDownEvent = new PhysicalMiddleButtonDownEvent(logicalButtons, this, id++);
            MiddleButtonUpEvent = new PhysicalMiddleButtonUpEvent(logicalButtons, this, id++);
            RightButtonDownEvent = new PhysicalRightButtonDownEvent(logicalButtons, this, id++);
            RightButtonUpEvent = new PhysicalRightButtonUpEvent(logicalButtons, this, id++);
            X1ButtonDownEvent = new PhysicalX1ButtonDownEvent(logicalButtons, this, id++);
            X1ButtonUpEvent = new PhysicalX1ButtonUpEvent(logicalButtons, this, id++);
            X2ButtonDownEvent = new PhysicalX2ButtonDownEvent(logicalButtons, this, id++);
            X2ButtonUpEvent = new PhysicalX2ButtonUpEvent(logicalButtons, this, id++);
        }
    }

    public class MoveEvent : LogicalFireEvent<LogicalButtons, MoveSwitch>
    {
        public MoveEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class WheelDownEvent : LogicalFireEvent<LogicalButtons, WheelDownSwitch>
    {
        public WheelDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class WheelUpEvent : LogicalFireEvent<LogicalButtons, WheelUpSwitch>
    {
        public WheelUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class WheelLeftEvent : LogicalFireEvent<LogicalButtons, WheelLeftSwitch>
    {
        public WheelLeftEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class WheelRightEvent : LogicalFireEvent<LogicalButtons, WheelRightSwitch>
    {
        public WheelRightEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class LeftButtonDownEvent : LogicalPressEvent<LogicalButtons, LeftButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, LeftButtonSwitch> OppositeReleaseEvent
            => LogicalGroup.LeftButtonUpEvent;

        public LeftButtonDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class LeftButtonUpEvent : LogicalReleaseEvent<LogicalButtons, LeftButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, LeftButtonSwitch> OppositePressEvent
            => LogicalGroup.LeftButtonDownEvent;

        public LeftButtonUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class MiddleButtonDownEvent : LogicalPressEvent<LogicalButtons, MiddleButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, MiddleButtonSwitch> OppositeReleaseEvent
            => LogicalGroup.MiddleButtonUpEvent;

        public MiddleButtonDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class MiddleButtonUpEvent : LogicalReleaseEvent<LogicalButtons, MiddleButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, MiddleButtonSwitch> OppositePressEvent
            => LogicalGroup.MiddleButtonDownEvent;

        public MiddleButtonUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class RightButtonDownEvent : LogicalPressEvent<LogicalButtons, RightButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, RightButtonSwitch> OppositeReleaseEvent
            => LogicalGroup.RightButtonUpEvent;

        public RightButtonDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class RightButtonUpEvent : LogicalReleaseEvent<LogicalButtons, RightButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, RightButtonSwitch> OppositePressEvent
            => LogicalGroup.RightButtonDownEvent;

        public RightButtonUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class X1ButtonDownEvent : LogicalPressEvent<LogicalButtons, X1ButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, X1ButtonSwitch> OppositeReleaseEvent
            => LogicalGroup.X1ButtonUpEvent;

        public X1ButtonDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class X1ButtonUpEvent : LogicalReleaseEvent<LogicalButtons, X1ButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, X1ButtonSwitch> OppositePressEvent
            => LogicalGroup.X1ButtonDownEvent;

        public X1ButtonUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class X2ButtonDownEvent : LogicalPressEvent<LogicalButtons, X2ButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, X2ButtonSwitch> OppositeReleaseEvent
            => LogicalGroup.X2ButtonUpEvent;

        public X2ButtonDownEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class X2ButtonUpEvent : LogicalReleaseEvent<LogicalButtons, X2ButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, X2ButtonSwitch> OppositePressEvent
            => LogicalGroup.X2ButtonDownEvent;

        public X2ButtonUpEvent(LogicalButtons eventGroup, int eventId) : base(eventGroup, eventId) { }
    }

    public class PhysicalMoveEvent : PhysicalFireEvent<LogicalButtons, PhysicalButtons, MoveSwitch>
    {
        public override LogicalFireEvent<LogicalButtons, MoveSwitch> LogicalEquivalentFireEvent
            => LogicalGroup.MoveEvent;

        public PhysicalMoveEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalWheelDownEvent : PhysicalFireEvent<LogicalButtons, PhysicalButtons, WheelDownSwitch>
    {
        public override LogicalFireEvent<LogicalButtons, WheelDownSwitch> LogicalEquivalentFireEvent
            => LogicalGroup.WheelDownEvent;

        public PhysicalWheelDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalWheelUpEvent : PhysicalFireEvent<LogicalButtons, PhysicalButtons, WheelUpSwitch>
    {
        public override LogicalFireEvent<LogicalButtons, WheelUpSwitch> LogicalEquivalentFireEvent
            => LogicalGroup.WheelUpEvent;

        public PhysicalWheelUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalWheelLeftEvent : PhysicalFireEvent<LogicalButtons, PhysicalButtons, WheelLeftSwitch>
    {
        public override LogicalFireEvent<LogicalButtons, WheelLeftSwitch> LogicalEquivalentFireEvent
            => LogicalGroup.WheelLeftEvent;

        public PhysicalWheelLeftEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalWheelRightEvent : PhysicalFireEvent<LogicalButtons, PhysicalButtons, WheelRightSwitch>
    {
        public override LogicalFireEvent<LogicalButtons, WheelRightSwitch> LogicalEquivalentFireEvent
            => LogicalGroup.WheelRightEvent;

        public PhysicalWheelRightEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalLeftButtonDownEvent : PhysicalPressEvent<LogicalButtons, PhysicalButtons, LeftButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, LeftButtonSwitch> LogicalEquivalentPressEvent
            => LogicalGroup.LeftButtonDownEvent;

        public override PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, LeftButtonSwitch> OppositePhysicalReleaseEvent
            => PhysicalGroup.LeftButtonUpEvent;

        public PhysicalLeftButtonDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalLeftButtonUpEvent : PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, LeftButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, LeftButtonSwitch> LogicalEquivalentReleaseEvent
            => LogicalGroup.LeftButtonUpEvent;

        public override PhysicalPressEvent<LogicalButtons, PhysicalButtons, LeftButtonSwitch> OppositePhysicalPressEvent
            => PhysicalGroup.LeftButtonDownEvent;

        public PhysicalLeftButtonUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalMiddleButtonDownEvent : PhysicalPressEvent<LogicalButtons, PhysicalButtons, MiddleButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, MiddleButtonSwitch> LogicalEquivalentPressEvent
            => LogicalGroup.MiddleButtonDownEvent;

        public override PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, MiddleButtonSwitch> OppositePhysicalReleaseEvent
            => PhysicalGroup.MiddleButtonUpEvent;

        public PhysicalMiddleButtonDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalMiddleButtonUpEvent : PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, MiddleButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, MiddleButtonSwitch> LogicalEquivalentReleaseEvent
            => LogicalGroup.MiddleButtonUpEvent;

        public override PhysicalPressEvent<LogicalButtons, PhysicalButtons, MiddleButtonSwitch> OppositePhysicalPressEvent
            => PhysicalGroup.MiddleButtonDownEvent;

        public PhysicalMiddleButtonUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalRightButtonDownEvent : PhysicalPressEvent<LogicalButtons, PhysicalButtons, RightButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, RightButtonSwitch> LogicalEquivalentPressEvent
            => LogicalGroup.RightButtonDownEvent;

        public override PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, RightButtonSwitch> OppositePhysicalReleaseEvent
            => PhysicalGroup.RightButtonUpEvent;

        public PhysicalRightButtonDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalRightButtonUpEvent : PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, RightButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, RightButtonSwitch> LogicalEquivalentReleaseEvent
            => LogicalGroup.RightButtonUpEvent;

        public override PhysicalPressEvent<LogicalButtons, PhysicalButtons, RightButtonSwitch> OppositePhysicalPressEvent
            => PhysicalGroup.RightButtonDownEvent;

        public PhysicalRightButtonUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalX1ButtonDownEvent : PhysicalPressEvent<LogicalButtons, PhysicalButtons, X1ButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, X1ButtonSwitch> LogicalEquivalentPressEvent
            => LogicalGroup.X1ButtonDownEvent;

        public override PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, X1ButtonSwitch> OppositePhysicalReleaseEvent
            => PhysicalGroup.X1ButtonUpEvent;

        public PhysicalX1ButtonDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }
        
    public class PhysicalX1ButtonUpEvent : PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, X1ButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, X1ButtonSwitch> LogicalEquivalentReleaseEvent
            => LogicalGroup.X1ButtonUpEvent;

        public override PhysicalPressEvent<LogicalButtons, PhysicalButtons, X1ButtonSwitch> OppositePhysicalPressEvent
            => PhysicalGroup.X1ButtonDownEvent;

        public PhysicalX1ButtonUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId)
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalX2ButtonDownEvent : PhysicalPressEvent<LogicalButtons, PhysicalButtons, X2ButtonSwitch>
    {
        public override LogicalPressEvent<LogicalButtons, X2ButtonSwitch> LogicalEquivalentPressEvent
            => LogicalGroup.X2ButtonDownEvent;

        public override PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, X2ButtonSwitch> OppositePhysicalReleaseEvent
            => PhysicalGroup.X2ButtonUpEvent;

        public PhysicalX2ButtonDownEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }

    public class PhysicalX2ButtonUpEvent : PhysicalReleaseEvent<LogicalButtons, PhysicalButtons, X2ButtonSwitch>
    {
        public override LogicalReleaseEvent<LogicalButtons, X2ButtonSwitch> LogicalEquivalentReleaseEvent
            => LogicalGroup.X2ButtonUpEvent;

        public override PhysicalPressEvent<LogicalButtons, PhysicalButtons, X2ButtonSwitch> OppositePhysicalPressEvent
            => PhysicalGroup.X2ButtonDownEvent;

        public PhysicalX2ButtonUpEvent(LogicalButtons logicalGroup, PhysicalButtons eventGroup, int eventId) 
            : base(logicalGroup, eventGroup, eventId) { }
    }
}
