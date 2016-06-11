using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    using WinAPI.WindowsHookEx;

    public class State0 : State
    {
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<GestureDefinition>> T0;

        public State0(
            GlobalValues Global,
            IDictionary<Def.Event.IDoubleActionSet, IEnumerable<GestureDefinition>> T0)
            : base(Global)
        {
            this.T0 = T0;
        }

        public override Result Input(Def.Event.IEvent evnt, LowLevelMouseHook.POINT point)
        {
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                if (T0.Keys.Contains(ev))
                {
                    var ctx = new UserActionExecutionContext(point.x, point.y);
                    var gestureDef = FilterByWhenClause(ctx, T0[ev]);
                    if (gestureDef.Count() > 0)
                    {
                        Debug.Print("[Transition 0]");
                        return Result.EventIsConsumed(nextState: new State1(Global, this, ctx, ev, gestureDef), resetStrokeWatcher: true);
                    }
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 8]");
            return this;
        }

        internal static IEnumerable<GestureDefinition> FilterByWhenClause(
            UserActionExecutionContext ctx, 
            IEnumerable<GestureDefinition> gestureDef)
        {
            // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
            // but executing it in sequential order here for simplicity.
            var cache = gestureDef
                .Select(x => x.whenFunc)
                .Distinct()
                .ToDictionary(x => x, x => EvaluateSafely(ctx, x));

            return gestureDef
                .Where(x => cache[x.whenFunc] == true)
                .ToList();
        }
    }
}
