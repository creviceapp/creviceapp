using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public class State2 : State
    {
        internal readonly State0 S0;
        internal readonly State1 S1;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly Def.Event.IDoubleActionSet secondaryEvent;
        internal readonly IEnumerable<OnButtonIfButtonGestureDefinition> T3;

        public State2(
            StateGlobal Global,
            State0 S0,
            State1 S1,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            Def.Event.IDoubleActionSet secondaryEvent,
            IEnumerable<OnButtonIfButtonGestureDefinition> T3
            ) : base(Global)
        {
            this.S0 = S0;
            this.S1 = S1;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.secondaryEvent = secondaryEvent;
            this.T3 = T3;
        }

        public override Result Input(Def.Event.IEvent evnt, Point point)
        {
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            if (evnt is Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Def.Event.IDoubleActionRelease;
                if (ev == secondaryEvent.GetPair())
                {
                    Debug.Print("[Transition 08]");
                    ExecuteUserActionInBackground(ctx, T3);
                    return Result.EventIsConsumed(nextState: S1, resetStrokeWatcher: true);
                }
                else if (ev == primaryEvent.GetPair())
                {
                    Debug.Print("[Transition 09]");
                    IgnoreNext(secondaryEvent.GetPair());
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 12]");
            IgnoreNext(primaryEvent.GetPair());
            IgnoreNext(secondaryEvent.GetPair());
            return S0;
        }
    }
}
