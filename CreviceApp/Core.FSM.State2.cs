using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    using WinAPI.WindowsHookEx;

    public class State2 : State
    {
        internal readonly State0 S0;
        internal readonly State1 S1;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly Def.Event.IDoubleActionSet secondaryEvent;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1;

        public State2(
            GlobalValues Global,
            State0 S0,
            State1 S1,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            Def.Event.IDoubleActionSet secondaryEvent,
        IDictionary<Def.Event.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1
            ) : base(Global)
        {
            this.S0 = S0;
            this.S1 = S1;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.secondaryEvent = secondaryEvent;
            this.T1 = T1;
        }

        public override Result Input(Def.Event.IEvent evnt, LowLevelMouseHook.POINT point)
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
                    Debug.Print("[Transition 6]");
                    Global.UserActionTaskFactory.StartNew(() => {
                        foreach (var gDef in T1[secondaryEvent])
                        {
                            ExecuteSafely(ctx, gDef.doFunc);
                        }
                    });
                    return Result.EventIsConsumed(nextState: S1, resetStrokeWatcher: true);
                }
                else if (ev == primaryEvent.GetPair())
                {
                    Debug.Print("[Transition 7]");
                    IgnoreNext(secondaryEvent.GetPair());
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 10]");
            IgnoreNext(primaryEvent.GetPair());
            IgnoreNext(secondaryEvent.GetPair());
            return S0;
        }
    }
}
