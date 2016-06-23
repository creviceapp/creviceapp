using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
    using WinAPI.WindowsHookEx;

    [TestClass()]
    public class State1Tests
    {

        static List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>> mouseEvents = new List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>>();
        static LowLevelMouseHook mouseHook = new LowLevelMouseHook((evnt, data) => {
            if (data.fromCreviceApp)
            {
                mouseEvents.Add(Tuple.Create(evnt, data));
            }
            return LowLevelMouseHook.Result.Cancel;
        });

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            mouseHook.SetHook();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            mouseHook.Unhook();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            mouseEvents.Clear();
        }

        readonly UserActionExecutionContext ctx = new UserActionExecutionContext(new Point());
        
        [TestMethod()]
        public void State1MustHaveGivenArgumentsTest()
        {
            var defA = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { })
            };
            var defB = new List<IfButtonGestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, defA, defB);

            Assert.AreEqual(S1.Global, S0.Global);
            Assert.AreEqual(S1.S0, S0);
            Assert.AreEqual(S1.ctx, ctx);
            Assert.AreEqual(S1.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S1.T3.Count(), 1);
            Assert.AreEqual(S1.T2.Count(), 1);
            Assert.AreEqual(S1.T4.Count(), 1);
            Assert.AreEqual(S1.T5.Count(), 1);
        }
        
        [TestMethod()]
        public void Transition02_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInIfButtonClause.Where(x => Helper.Convert(x) is Def.Event.ISingleAction))
            {
                var countDown = new CountdownEvent(1);
                var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    a,
                    (ctx) => { countDown.Signal(); })
                };
                foreach (var b in TestDef.Constant.AcceptablesInIfButtonClause)
                {
                    countDown.Reset();
                    var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                    var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                    var ev = Helper.Convert(b);
                    var res = S1.Input(ev, new Point());
                    if (a == b)
                    {
                        Assert.IsFalse(S1.PrimaryEventIsRestorable);
                        Assert.IsTrue(res.NextState is State1);
                        Assert.IsTrue(res.StrokeWatcher.IsResetRequested);
                        Assert.IsTrue(countDown.Wait(50));
                    }
                    else
                    {
                        Assert.IsTrue(S1.PrimaryEventIsRestorable);
                        Assert.IsTrue(res.NextState is State1);
                        Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                        Assert.IsFalse(countDown.Wait(50));
                    }
                }
            }
        }
        
        [TestMethod()]
        public void Transition03_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInIfButtonClause.Where(x => Helper.Convert(x) is Def.Event.IDoubleActionSet))
            {
                var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    a,
                    (ctx) => { })
                };
                foreach (var b in TestDef.Constant.AcceptablesInIfButtonClause)
                {
                    var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                    var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                    var ev = Helper.Convert(b);
                    var res = S1.Input(ev, new Point());
                    if (a == b)
                    {
                        Assert.IsFalse(S1.PrimaryEventIsRestorable);
                        Assert.IsTrue(res.NextState is State2);
                        Assert.IsTrue(S1.T3[(Def.Event.IDoubleActionSet)ev].SequenceEqual(((State2)res.NextState).T3));
                    }
                    else
                    {
                        Assert.IsTrue(S1.PrimaryEventIsRestorable);
                        Assert.IsTrue(res.NextState is State1);
                    }
                    Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                }
            }
        }

        [TestMethod()]
        public void Transition04_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                var countDown = new CountdownEvent(1);
                var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Down }),
                    (ctx) => { countDown.Signal(); })
                };
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    countDown.Reset();
                    var Config = new Config.UserConfig();
                    Config.Gesture.WatchInterval = 0;
                    using(var Global = new StateGlobal(Config))
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), gestureDef, new List<IfButtonGestureDefinition>());
                        S1.Input(Def.Constant.Move, new Point(0, 0));
                        S1.Input(Def.Constant.Move, new Point(0, 200));
                        Thread.Sleep(100);
                        var ev = Helper.Convert(b).GetPair();
                        var res = S1.Input((Def.Event.IEvent)ev, new Point(0, 400));
                        if (a == b)
                        {
                            Assert.IsFalse(S1.PrimaryEventIsRestorable);
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsTrue(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(S1.PrimaryEventIsRestorable);
                            Assert.IsTrue(res.NextState is State1);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void InputMustExecuteTransition06Test()
        {
            var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
            S1.Global.ResetStrokeWatcher();
            S1.PrimaryEventIsRestorable = true;
            var res = S1.Input(Def.Constant.RightButtonUp, new Point());
            Thread.Sleep(100);
            Assert.IsTrue(res.NextState is State0);
            Assert.AreEqual(mouseEvents.Count, 2);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        [TestMethod()]
        public void InputMustExecuteTransition07Test()
        {
            var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
            S1.Global.ResetStrokeWatcher();
            S1.PrimaryEventIsRestorable = false;
            var res = S1.Input(Def.Constant.RightButtonUp, new Point());
            Thread.Sleep(100);
            Assert.IsTrue(res.NextState is State0);
            Assert.AreEqual(mouseEvents.Count, 0);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.LeftButtonDown, gestureDef, new List<IfButtonGestureDefinition>());

            S1.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 1);

            var res = S1.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.LeftButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
            S1.Global.ResetStrokeWatcher();

            S1.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 1);

            var res = S1.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void RestorePrimaryTriggerTest()
        {
            var gestureDef = new List<OnButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            {
                var S1 = new State1(S0.Global, S0, ctx, Def.Constant.LeftButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryEvent();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, ctx, Def.Constant.MiddleButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryEvent();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryEvent();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, ctx, Def.Constant.X1ButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryEvent();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton1);
            }
            {
                var S1 = new State1(S0.Global, S0, ctx, Def.Constant.X2ButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryEvent();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
            }
        }
    }
}