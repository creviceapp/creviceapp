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

        class StrokeWatcherMock : Stroke.StrokeWatcher
        {
            private StrokeWatcherMock(TaskFactory taskFactory) : base(taskFactory, 0, 0, 0, 0) { }
            public StrokeWatcherMock() : this(new TaskFactory(new Threading.SingleThreadScheduler())) { }

            internal new readonly List<Point> queue = new List<Point>();
            public override void Queue(Point point)
            {
                queue.Add(point);
            }
        }
        
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
        public void State1MustBypassInputToStrokeWatcher_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                var gestureDef = new List<OnButtonGestureDefinition>() {
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    new Def.Stroke(Def.Direction.Down),
                    (ctx) => { })
                };
                foreach (var b in TestDef.Constant.AcceptablesInIfButtonClause)
                {
                    using (var Global = new StateGlobal())
                    {
                        Global.StrokeWatcher.Dispose();
                        Global.StrokeWatcher = new StrokeWatcherMock();
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), gestureDef, new List<IfButtonGestureDefinition>());
                        S1.Input(Helper.Convert(b), new Point(0, 0));
                        S1.Input(Def.Constant.Move, new Point(100, 100));
                        Console.WriteLine(((StrokeWatcherMock)Global.StrokeWatcher).queue.Count());
                        Assert.IsTrue(((StrokeWatcherMock)Global.StrokeWatcher).queue.SequenceEqual(new List<Point>() {
                            new Point(0, 0),
                            new Point(100, 100)
                        }));
                    }
                }
            }
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
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
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
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                        var ev = Helper.Convert(b);
                        var res = S1.Input(ev, new Point());
                        if (a == b)
                        {
                            Assert.IsFalse(S1.PrimaryEventIsRestorable);
                            Assert.IsTrue(res.NextState is State2);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsTrue(S1.T3[(Def.Event.IDoubleActionSet)ev].SequenceEqual(((State2)res.NextState).T3));
                        }
                        else
                        {
                            Assert.IsTrue(S1.PrimaryEventIsRestorable);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsTrue(res.NextState is State1);
                        }
                        Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                    }
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
                    using(var Global = new StateGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), gestureDef, new List<IfButtonGestureDefinition>());
                        Global.StrokeWatcher.strokes.Add(new Stroke.Stroke(
                            Global.Config.Gesture.StrokeDirectionChangeThreshold, 
                            Global.Config.Gesture.StrokeExtensionThreshold,
                            Def.Direction.Down));
                        var ev = Helper.Convert(b).GetPair();
                        var res = S1.Input((Def.Event.IEvent)ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsTrue(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State1);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition05_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                var countDown = new CountdownEvent(1);
                var gestureDef = new List<IfButtonGestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    (DSL.Def.AcceptableInIfButtonClause)a,
                    (ctx) => { countDown.Signal(); })
                };
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    countDown.Reset();
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(new StateGlobal(), gestureDef);
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), gestureDef);
                        var ev = Helper.Convert(b).GetPair();
                        var res = S1.Input((Def.Event.IEvent)ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsTrue(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State1);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition06_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    mouseEvents.Clear();
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                        var ev = Helper.Convert(b).GetPair();
                        var res = S1.Input((Def.Event.IEvent)ev, new Point());
                        Thread.Sleep(100);
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.AreEqual(mouseEvents.Count, 2);
                            if (a == DSL.Def.Constant.LeftButton)
                            {
                                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
                            }
                            else if (a == DSL.Def.Constant.MiddleButton)
                            {
                                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
                            }
                            else if (a == DSL.Def.Constant.RightButton)
                            {
                                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
                            }
                            else if (a == DSL.Def.Constant.X1Button)
                            {
                                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
                                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton1);
                            }
                            else if (a == DSL.Def.Constant.X2Button)
                            {
                                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
                            }
                            else
                            {
                                throw new InvalidOperationException();
                            }
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State1);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.AreEqual(mouseEvents.Count, 0);
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition07_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                var countDown = new CountdownEvent(1);
                var defA = new List<OnButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    (DSL.Def.AcceptableInIfButtonClause)a,
                    (ctx) => { countDown.Signal(); }),
                new OnButtonIfStrokeGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Down }),
                    (ctx) => { countDown.Signal(); })
                };
                var defB = new List<IfButtonGestureDefinition>() {
                    new IfButtonGestureDefinition(
                        (ctx) => { return true; },
                        (DSL.Def.AcceptableInIfButtonClause)a,
                        (ctx) => { countDown.Signal(); })
                };
            foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    mouseEvents.Clear();
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), defA, defB);
                        S1.PrimaryEventIsRestorable = false;
                        var ev = Helper.Convert(b).GetPair();
                        var res = S1.Input((Def.Event.IEvent)ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.AreEqual(mouseEvents.Count, 0);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State1);
                            Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                            Assert.AreEqual(mouseEvents.Count, 0);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition11_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                using (var Global = new StateGlobal())
                {
                    var S0 = new State0(Global, new List<GestureDefinition>());
                    var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    var res = S1.Reset();
                    Assert.IsTrue(res is State0);
                    Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(a).GetPair()));
                }
            }
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenATriggerInIgnoreListGivenTest()
        {
            using (var Global = new StateGlobal())
            {
                var S0 = new State0(Global, new List<GestureDefinition>());
                var S1 = new State1(Global, S0, ctx, Def.Constant.LeftButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());

                Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
                Assert.AreEqual(Global.IgnoreNext.Count, 1);

                var res = S1.Input(Def.Constant.RightButtonUp, new Point());
                Assert.IsTrue(res.Event.IsConsumed);
                Assert.AreEqual(Global.IgnoreNext.Count, 0);
            }
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenAPairOfTriggerInIgnoreListGivenTest()
        {
            using (var Global = new StateGlobal())
            {
                var S0 = new State0(Global, new List<GestureDefinition>());
                var S1 = new State1(Global, S0, ctx, Def.Constant.LeftButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());

                Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
                Assert.AreEqual(Global.IgnoreNext.Count, 1);

                var res = S1.Input(Def.Constant.RightButtonDown, new Point());
                Assert.IsFalse(res.Event.IsConsumed);
                Assert.AreEqual(Global.IgnoreNext.Count, 0);
            }
        }

        [TestMethod()]
        public void RestorePrimaryTriggerTest()
        {
            using (var Global = new StateGlobal())
            {
                var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.LeftButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryEvent();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.MiddleButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryEvent();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryEvent();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.X1ButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
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
                    var S1 = new State1(Global, S0, ctx, Def.Constant.X2ButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
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
}