using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    using WinAPI.WindowsHookEx;
    using WinAPI.SendInput;

    public class State1 : State
    {
        internal readonly State0 S0;
        internal readonly UserActionExecutionContext ctx;
        internal readonly Def.Event.IDoubleActionSet primaryEvent;
        internal readonly IDictionary<Def.Event.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1;
        internal readonly IDictionary<Def.Event.ISingleAction, IEnumerable<ButtonGestureDefinition>> T2;
        internal readonly IDictionary<Def.Stroke, IEnumerable<StrokeGestureDefinition>> T3;

        private readonly SingleInputSender InputSender = new SingleInputSender();

        internal bool PrimaryEventIsRestorable { get; set; } = true;

        public State1(
            GlobalValues Global,
            State0 S0,
            UserActionExecutionContext ctx,
            Def.Event.IDoubleActionSet primaryEvent,
            IEnumerable<GestureDefinition> gestureDef
            ) : base(Global)
        {
            this.S0 = S0;
            this.ctx = ctx;
            this.primaryEvent = primaryEvent;
            this.T1 = Transition.Gen1(gestureDef);
            this.T2 = Transition.Gen2(gestureDef);
            this.T3 = Transition.Gen3(gestureDef);
        }

        public override Result Input(Def.Event.IEvent evnt, LowLevelMouseHook.POINT point)
        {
            if (MustBeIgnored(evnt))
            {
                return Result.EventIsConsumed(nextState: this);
            }

            Global.StrokeWatcher.Queue(point);

            if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                if (T1.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 1]");
                    PrimaryEventIsRestorable = false;
                    return Result.EventIsConsumed(nextState: new State2(Global, S0, this, ctx, primaryEvent, ev, T1));
                }
            }
            else if (evnt is Def.Event.ISingleAction)
            {
                var ev = evnt as Def.Event.ISingleAction;
                if (T2.Keys.Contains(ev))
                {
                    Debug.Print("[Transition 2]");
                    PrimaryEventIsRestorable = false;
                    Global.UserActionTaskFactory.StartNew(() => {
                        foreach (var gDef in T2[ev])
                        {
                            ExecuteSafely(ctx, gDef.doFunc);
                        }
                    });
                    return Result.EventIsConsumed(nextState: this, resetStrokeWatcher: true);
                }
            }
            else if (evnt is Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Def.Event.IDoubleActionRelease;
                if (ev == primaryEvent.GetPair())
                {
                    var stroke = Global.StrokeWatcher.GetStorke();
                    Debug.Print("Stroke: {0}", stroke.ToString());
                    if (T3.Keys.Contains(stroke))
                    {
                        Debug.Print("[Transition 3]");
                        Global.UserActionTaskFactory.StartNew(() => {
                            foreach (var gDef in T3[stroke])
                            {
                                ExecuteSafely(ctx, gDef.doFunc);
                            }
                        });
                    }
                    else
                    {
                        if (PrimaryEventIsRestorable && stroke.Count == 0)
                        {
                            Debug.Print("[Transition 4]");
                            RestorePrimaryEvent();
                        }
                        else
                        {
                            Debug.Print("[Transition 5]");
                        }
                    }
                    return Result.EventIsConsumed(nextState: S0);
                }
            }
            return base.Input(evnt, point);
        }

        public override IState Reset()
        {
            Debug.Print("[Transition 9]");
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
