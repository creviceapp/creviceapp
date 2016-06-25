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

    public class State1 : State
    {
        internal readonly State0 S0;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly IDictionary<Def.Event.ISingleAction, IEnumerable<OnButtonIfButtonGestureDefinition>> T2;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonIfButtonGestureDefinition>> T3;
        internal readonly IDictionary<Def.Stroke, IEnumerable<OnButtonIfStrokeGestureDefinition>> T4;
        internal readonly IEnumerable<IfButtonGestureDefinition> T5;

        private readonly SingleInputSender InputSender = new SingleInputSender();

        internal bool PrimaryEventIsRestorable { get; set; } = true;

        public State1(
            StateGlobal Global,
            State0 S0,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            IEnumerable<OnButtonGestureDefinition> T1,
            IEnumerable<IfButtonGestureDefinition> T5
            ) : base(Global)
        {
            this.S0 = S0;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.T2 = Transition.Gen2(T1);
            this.T3 = Transition.Gen3(T1);
            this.T4 = Transition.Gen4(T1);
            this.T5 = T5;
        }

        public override Result Input(Def.Event.IEvent evnt, Point point)
        {
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            Global.StrokeWatcher.Queue(point);

            if (evnt is Def.Event.ISingleAction)
            {
                var ev = evnt as Def.Event.ISingleAction;
                if (T2.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 02]");
                    PrimaryEventIsRestorable = false;
                    ExecuteUserActionInBackground(ctx, T2[ev]);
                    return Result.EventIsConsumed(nextState: this, resetStrokeWatcher: true);
                }
            }
            else if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                if (T3.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 03]");
                    PrimaryEventIsRestorable = false;
                    return Result.EventIsConsumed(nextState: new State2(Global, S0, this, ctx, primaryEvent, ev, T3[ev]));
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
                        if (T4.Keys.Contains(stroke))
                        {
                            Debug.Print("[Transition 04]");
                            ExecuteUserActionInBackground(ctx, T4[stroke]);
                        }
                    }
                    else
                    {
                        if (PrimaryEventIsRestorable)
                        {
                            if (T5.Count() > 0)
                            {
                                Debug.Print("[Transition 05]");
                                ExecuteUserActionInBackground(ctx, T5);
                            }
                            else
                            {
                                Debug.Print("[Transition 06]");
                                RestorePrimaryEvent();
                            }
                        }
                        else
                        {
                            Debug.Print("[Transition 07]");
                        }
                    }
                    
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 11]");
            IgnoreNext(primaryEvent.GetPair());
            return S0;
        }

        internal void RestorePrimaryEvent()
        {
            if (primaryEvent == Def.Constant.LeftButtonDown)
            {
                InputSender.LeftClick();
            }
            else if (primaryEvent == Def.Constant.MiddleButtonDown)
            {
                InputSender.MiddleClick();
            }
            else if (primaryEvent == Def.Constant.RightButtonDown)
            {
                InputSender.RightClick();
            }
            else if (primaryEvent == Def.Constant.X1ButtonDown)
            {
                InputSender.X1Click();
            }
            else if (primaryEvent == Def.Constant.X2ButtonDown)
            {
                InputSender.X2Click();
            }
        }
    }
}
