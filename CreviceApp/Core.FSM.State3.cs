using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public class State3 : State
    {
        internal readonly State0 S0;
        internal readonly State2 S2;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly Def.Event.IDoubleActionSet secondaryEvent;
        internal readonly IEnumerable<OnButtonIfButtonGestureDefinition> T0;

        public State3(
            StateGlobal Global,
            State0 S0,
            State2 S2,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            Def.Event.IDoubleActionSet secondaryEvent,
            IEnumerable<OnButtonIfButtonGestureDefinition> T0
            ) : base(Global)
        {
            this.S0 = S0;
            this.S2 = S2;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.secondaryEvent = secondaryEvent;
            this.T0 = T0;
        }

        public override Result Input(Def.Event.IEvent evnt, Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            if (evnt is Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Def.Event.IDoubleActionRelease;
                if (ev == secondaryEvent.GetPair())
                {
                    Debug.Print("[Transition 3_0]");
                    ExecuteUserActionInBackground(ctx, T0);
                    return Result.EventIsConsumed(nextState: S2);
                }
                else if (ev == primaryEvent.GetPair())
                {
                    Debug.Print("[Transition 3_1]");
                    IgnoreNext(secondaryEvent.GetPair());
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 3_2]");
            IgnoreNext(primaryEvent.GetPair());
            IgnoreNext(secondaryEvent.GetPair());
            return S0;
        }
    }
}
