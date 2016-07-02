using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    using WinAPI.SendInput;

    public class State2 : State
    {
        internal readonly State0 S0;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly IDictionary<Def.Event.ISingleAction, IEnumerable<OnButtonIfButtonGestureDefinition>> T0;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonIfButtonGestureDefinition>> T1;
        internal readonly IDictionary<Def.Stroke, IEnumerable<OnButtonIfStrokeGestureDefinition>> T2;
        internal readonly IEnumerable<IfButtonGestureDefinition> T3;

        public State2(
            StateGlobal Global,
            State0 S0,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            IDictionary<Def.Event.ISingleAction, IEnumerable<OnButtonIfButtonGestureDefinition>> T0,
            IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonIfButtonGestureDefinition>> T1,
            IDictionary<Def.Stroke, IEnumerable<OnButtonIfStrokeGestureDefinition>> T2,
            IEnumerable<IfButtonGestureDefinition> T3
            ) : base(Global)
        {
            this.S0 = S0;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.T0 = T0;
            this.T1 = T1;
            this.T2 = T2;
            this.T3 = T3;
        }

        public override Result Input(Def.Event.IEvent evnt, Point point)
        {
            // Special side effect 3, 4
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }
            // Special side effect 2
            Global.StrokeWatcher.Queue(point);

            if (evnt is Def.Event.ISingleAction)
            {
                var ev = evnt as Def.Event.ISingleAction;
                if (T0.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 2_0]");
                    ExecuteUserActionInBackground(ctx, T0[ev]);
                    return Result.EventIsConsumed(nextState: this);
                }
            }
            else if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 2_1]");
                    return Result.EventIsConsumed(nextState: new State3(Global, S0, this, ctx, primaryEvent, ev, T1[ev]));
                }
            }
            else if (evnt is Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Def.Event.IDoubleActionRelease;
                if (ev == primaryEvent.GetPair())
                {
                    var stroke = Global.StrokeWatcher.GetStorke();
                    if (stroke.Count() > 0)
                    {
                        Debug.Print("Stroke: {0}", stroke.ToString());
                        if (T2.Keys.Contains(stroke))
                        {
                            Debug.Print("[Transition 2_2]");
                            ExecuteUserActionInBackground(ctx, T2[stroke]);
                        }
                    }
                    else
                    {
                        Debug.Print("[Transition 2_3]");
                    }

                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }
        
        public override IState Reset()
        {
            Debug.Print("[Transition 2_4]");
            IgnoreNext(primaryEvent.GetPair());
            return S0;
        }
    }
}
