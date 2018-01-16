using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.FSM
{
    public class State3 : State
    {
        internal readonly State0 S0;
        internal readonly State2 S2;
        internal readonly UserActionExecutionContextBase ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly Def.Event.IDoubleActionSet secondaryEvent;
        internal readonly IEnumerable<IfButtonGestureDefinition> T0;
        internal readonly IEnumerable<OnButtonWithIfButtonGestureDefinition> T1;

        public State3(
            StateGlobal Global,
            State0 S0,
            State2 S2,
            UserActionExecutionContextBase ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            Def.Event.IDoubleActionSet secondaryEvent,
            IEnumerable<IfButtonGestureDefinition> T0,
            IEnumerable<OnButtonWithIfButtonGestureDefinition> T1
            ) : base(Global)
        {
            this.S0 = S0;
            this.S2 = S2;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.secondaryEvent = secondaryEvent;
            this.T0 = T0;
            this.T1 = T1;
        }

        public override Result Input(Def.Event.IEvent evnt, System.Drawing.Point point)
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
                    Verbose.Print("[Transition 3_0]");
                    ExecuteUserDoFuncInBackground(ctx, T1);
                    ExecuteUserAfterFuncInBackground(ctx, T1);
                    return Result.EventIsConsumed(nextState: S2);
                }
                else if (ev == primaryEvent.GetPair())
                {
                    Verbose.Print("[Transition 3_1]");
                    IgnoreNext(secondaryEvent.GetPair());
                    ExecuteUserAfterFuncInBackground(ctx, T1);
                    ExecuteUserAfterFuncInBackground(ctx, T0);
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Verbose.Print("[Transition 3_2]");
            IgnoreNext(primaryEvent.GetPair());
            IgnoreNext(secondaryEvent.GetPair());
            ExecuteUserAfterFuncInBackground(ctx, T1);
            ExecuteUserAfterFuncInBackground(ctx, T0);
            return S0;
        }
    }
}
