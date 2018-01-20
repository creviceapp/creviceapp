using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Future
{
    public class Events
    {
        public readonly MoveEventEvent Move;
        public readonly LeftButtonDownEvent LeftButtonDownEvent;
        public readonly LeftButtonUpEvent LeftButtonUpEvent;
        public readonly MiddleButtonDownEvent MiddleButtonDownEvent;
        public readonly MiddleButtonUpEvent MiddleButtonUpEvent;
        public readonly RightButtonDownEvent RightButtonDownEvent;
        public readonly RightButtonUpEvent RightButtonUpEvent;
        public readonly WheelDownEvent WheelDownEvent;
        public readonly WheelUpEvent WheelUpEvent;
        public readonly WheelLeftEvent WheelLeftEvent;
        public readonly WheelRightEvent WheelRightEvent;
        public readonly X1ButtonDownEvent X1ButtonDownEvent;
        public readonly X1ButtonUpEvent X1ButtonUpEvent;
        public readonly X2ButtonDownEvent X2ButtonDownEvent;
        public readonly X2ButtonUpEvent X2ButtonUpEvent;

        public readonly PhysicalLeftButtonDownEvent LeftButtonDownP0Event;
        public readonly PhysicalLeftButtonUpEvent LeftButtonUpP0Event;

        public Events(int maxEquivalents = 1000)
        {
            /* ただひとつの条件は同一の論理物理グループが同一のEventIdに割り当てられること。
             * 例えば全て同じEventIdに割り当てたとしても、速度面以外に問題はない。
             */

            var ME = maxEquivalents;
            /*
             *    0            : System reserved. (Assigned to Stroke events.)
             *    1 ... ME-1   : System reserved.
             * ME              : Assigned to the logical Move event, to which physical Move events 
             *                   following this EventId will be judged as equivalent.
             * ME+1 ... ME*2-1 : Reserved for physical Move event.
             *
             * ...
            */

            // 0 is assigned to the logical Stroke event.

            var id = ME;
            Move = new MoveEventEvent(id);

            id += ME;
            LeftButtonDownEvent = new LeftButtonDownEvent(id);
            // A physical event, a quivalent to the logical event, should have requential relative EventId to it.
            LeftButtonDownP0Event = new PhysicalLeftButtonDownEvent(id + 1);

            id += ME;
            LeftButtonUpEvent = new LeftButtonUpEvent(id);
            LeftButtonUpP0Event = new PhysicalLeftButtonUpEvent(id + 1);

            id += ME;
            MiddleButtonDownEvent = new MiddleButtonDownEvent(id);

            id += ME;
            MiddleButtonUpEvent = new MiddleButtonUpEvent(id);

            id += ME;
            RightButtonDownEvent = new RightButtonDownEvent(id);

            id += ME;
            RightButtonUpEvent = new RightButtonUpEvent(id);

            id += ME;
            WheelDownEvent = new WheelDownEvent(id);

            id += ME;
            WheelUpEvent = new WheelUpEvent(id);

            id += ME;
            WheelLeftEvent = new WheelLeftEvent(id);

            id += ME;
            WheelRightEvent = new WheelRightEvent(id);

            id += ME;
            X1ButtonDownEvent = new X1ButtonDownEvent(id);

            id += ME;
            X1ButtonUpEvent = new X1ButtonUpEvent(id);

            id += ME;
            X2ButtonDownEvent = new X2ButtonDownEvent(id);

            id += ME;
            X2ButtonUpEvent = new X2ButtonUpEvent(id);
        }

        private static Events singleton = new Events();
        public static Events Constants
        {
            get { return singleton; }
        }
    }

    public class MoveEventEvent : FireEvent<MoveSwich>
    {
        public MoveEventEvent(int eventId) : base(eventId) { }
    }

    public class LeftButtonDownEvent : PressEvent<LeftButtonSwitch>
    {
        public LeftButtonDownEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<LeftButtonSwitch> OppositeEvent { get { return Events.Constants.LeftButtonUpEvent; } }
    }

    public class LeftButtonUpEvent : ReleaseEvent<LeftButtonSwitch>
    {
        public LeftButtonUpEvent(int eventId) : base(eventId) { }
        public override PressEvent<LeftButtonSwitch> OppositeEvent { get { return Events.Constants.LeftButtonDownEvent; } }
    }
    
    public class MiddleButtonDownEvent : PressEvent<MiddleButtonSwitch>
    {
        public MiddleButtonDownEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<MiddleButtonSwitch> OppositeEvent { get { return Events.Constants.MiddleButtonUpEvent; } }
    }

    public class MiddleButtonUpEvent : ReleaseEvent<MiddleButtonSwitch>
    {
        public MiddleButtonUpEvent(int eventId) : base(eventId) { }
        public override PressEvent<MiddleButtonSwitch> OppositeEvent { get { return Events.Constants.MiddleButtonDownEvent; } }
    }

    public class RightButtonDownEvent : PressEvent<RightButtonSwitch>
    {
        public RightButtonDownEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<RightButtonSwitch> OppositeEvent { get { return Events.Constants.RightButtonUpEvent; } }
    }

    public class RightButtonUpEvent : ReleaseEvent<RightButtonSwitch>
    {
        public RightButtonUpEvent(int eventId) : base(eventId) { }
        public override PressEvent<RightButtonSwitch> OppositeEvent { get { return Events.Constants.RightButtonDownEvent; } }
    }

    public class WheelDownEvent : FireEvent<MoveSwich>
    {
        public WheelDownEvent(int eventId) : base(eventId) { }
    }

    public class WheelUpEvent : FireEvent<MoveSwich>
    {
        public WheelUpEvent(int eventId) : base(eventId) { }
    }

    public class WheelLeftEvent : FireEvent<MoveSwich>
    {
        public WheelLeftEvent(int eventId) : base(eventId) { }
    }

    public class WheelRightEvent : FireEvent<MoveSwich>
    {
        public WheelRightEvent(int eventId) : base(eventId) { }
    }
    public class X1ButtonDownEvent : PressEvent<X1ButtonSwitch>
    {
        public X1ButtonDownEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<X1ButtonSwitch> OppositeEvent { get { return Events.Constants.X1ButtonUpEvent; } }
    }

    public class X1ButtonUpEvent : ReleaseEvent<X1ButtonSwitch>
    {
        public X1ButtonUpEvent(int eventId) : base(eventId) { }
        public override PressEvent<X1ButtonSwitch> OppositeEvent { get { return Events.Constants.X1ButtonDownEvent; } }
    }

    public class X2ButtonDownEvent : PressEvent<X2ButtonSwitch>
    {
        public X2ButtonDownEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<X2ButtonSwitch> OppositeEvent { get { return Events.Constants.X2ButtonUpEvent; } }
    }

    public class X2ButtonUpEvent : ReleaseEvent<X2ButtonSwitch>
    {
        public X2ButtonUpEvent(int eventId) : base(eventId) { }
        public override PressEvent<X2ButtonSwitch> OppositeEvent { get { return Events.Constants.X2ButtonDownEvent; } }
    }

    public class PhysicalLeftButtonDownEvent : PhysicalPressEvent<LeftButtonSwitch>
    {
        public PhysicalLeftButtonDownEvent(int eventId) : base(eventId) { }
        public override PressEvent<LeftButtonSwitch> LogicalEquivalent { get { return Events.Constants.LeftButtonDownEvent; } }
        public override PhysicalReleaseEvent<LeftButtonSwitch> OppositeEvent { get { return Events.Constants.LeftButtonUpP0Event; } }
    }

    public class PhysicalLeftButtonUpEvent : PhysicalReleaseEvent<LeftButtonSwitch>
    {
        public PhysicalLeftButtonUpEvent(int eventId) : base(eventId) { }
        public override ReleaseEvent<LeftButtonSwitch> LogicalEquivalent { get { return Events.Constants.LeftButtonUpEvent; } }
        public override PhysicalPressEvent<LeftButtonSwitch> OppositeEvent { get { return Events.Constants.LeftButtonDownP0Event; } }
    }
}
