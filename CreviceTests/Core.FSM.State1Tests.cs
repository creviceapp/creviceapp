using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crevice.Core.FSM.Tests
{
    //using WinAPI.WindowsHookEx;

    [TestClass()]
    public class State1Tests
    {

        /*
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
        */

        readonly DefaultActionContext ctx = new DefaultActionContext(new Point());

        private static StateGlobal GetDefaultGlobal<T>()
        where T : ActionContext
        {
            var factory = new ActionContextFactory<T>();
            var config = new DefaultGestureMachineConfig();
            return new StateGlobal(config, factory as ActionContextFactory<ActionContext>);
        }

        private static StateGlobal GetDefaultGlobal()
        {
            return GetDefaultGlobal<DefaultActionContext>();
        }
        
        class StrokeWatcherMock : Stroke.StrokeWatcher
        {
            // Todo: release releases on Dispose().

            public StrokeWatcherMock() : base(Task.Factory, 0, 0, 0, 0) { }

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
                new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    null,
                    (ctx) => { },
                    null),
                 new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    null,
                    (ctx) => { },
                    null),
                new OnButtonWithIfStrokeGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    (ctx) => { })
            };
            var defB = new List<IfButtonGestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.LeftButton,
                    null,
                    (ctx) => { },
                    null)
            };
            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, defA, defB);

            Assert.AreEqual(S1.Global, S0.Global);
            Assert.AreEqual(S1.S0, S0);
            Assert.AreEqual(S1.ctx, ctx);
            Assert.AreEqual(S1.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S1.T0.Count(), 1);
            Assert.AreEqual(S1.T1.Count(), 1);
            Assert.AreEqual(S1.T2.Count(), 1);
            Assert.AreEqual(S1.T3.Count(), 1);
        }
        
        [TestMethod()]
        public void State1MustBypassInputToStrokeWatcher_RRTest()
        {
            foreach (var a in TestDef.Constant.DoubleTriggerButtons)
            {
                var gestureDef = new List<OnButtonGestureDefinition>() {
                    new OnButtonWithIfStrokeGestureDefinition(
                        (ctx) => { return true; },
                        a as DSL.Def.AcceptableInOnClause,
                        new Def.Stroke(Def.Direction.Down),
                        (ctx) => { Assert.Fail(); })
                };
                foreach (var b in TestDef.Constant.Buttons)
                {
                    using (var Global = GetDefaultGlobal())
                    {
                        Global.StrokeWatcher.Dispose();
                        Global.StrokeWatcher = new StrokeWatcherMock();
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), gestureDef, new List<IfButtonGestureDefinition>());
                        S1.Input(Helper.Convert(b as DSL.Def.AcceptableInIfButtonClause), new Point(0, 0));
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
        public void Transition1_0_RRTest()
        {
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.SingleTriggerButtons)
                {
                    var gestureDef = new List<OnButtonGestureDefinition>() {
                        new OnButtonWithIfButtonGestureDefinition(
                            (ctx) => { return true; },
                            DSL.Def.Constant.RightButton,
                            a as DSL.Def.AcceptableInIfButtonClause,
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { countDown.Signal(); },
                            (ctx) => { Assert.Fail(); })
                    };
                    foreach (var b in TestDef.Constant.Buttons)
                    {
                        countDown.Reset();
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(Global, new List<GestureDefinition>());
                            var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                            var ev = Helper.Convert(b as DSL.Def.AcceptableInIfButtonClause);
                            var res = S1.Input(ev, new Point());
                            if (a == b)
                            {
                                Assert.IsTrue(res.NextState is State2);
                                Assert.IsTrue(countDown.Wait(50));
                            }
                            else
                            {
                                Assert.IsTrue(res.NextState is State1);
                                Assert.IsFalse(countDown.Wait(50));
                            }
                        }
                    }
                }
            }
        }
        
        [TestMethod()]
        public void Transition1_1_RRTest()
        {
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    var gestureDef = new List<OnButtonGestureDefinition>() {
                        new OnButtonWithIfButtonGestureDefinition(
                            (ctx) => { return true; },
                            DSL.Def.Constant.RightButton,
                            a as DSL.Def.AcceptableInIfButtonClause,
                            (ctx) => { countDown.Signal(); },
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { Assert.Fail(); })
                    };
                    foreach (var b in TestDef.Constant.Buttons)
                    {
                        countDown.Reset();
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(Global, new List<GestureDefinition>());
                            var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, gestureDef, new List<IfButtonGestureDefinition>());
                            var ev = Helper.Convert(b as DSL.Def.AcceptableInIfButtonClause);
                            var res = S1.Input(ev, new Point());
                            if (a == b)
                            {
                                Assert.IsTrue(res.NextState is State3);
                                Assert.IsTrue(S1.T1[(Def.Event.IDoubleActionSet)ev].SequenceEqual(((State3)res.NextState).T1));
                                Assert.IsTrue(countDown.Wait(50));
                            }
                            else
                            {
                                Assert.IsTrue(res.NextState is State1);
                                Assert.IsFalse(countDown.Wait(50));
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition1_2_RRTest()
        {
            using (var countDown = new CountdownEvent(2))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    var gestureDefA = new List<OnButtonGestureDefinition>() {
                        new OnButtonWithIfStrokeGestureDefinition(
                            (ctx) => { return true; },
                            a as DSL.Def.AcceptableInOnClause,
                            new Def.Stroke(new List<Def.Direction>() { Def.Direction.Down }),
                            (ctx) => { countDown.Signal(); })
                    };
                    var gestureDefB = new List<IfButtonGestureDefinition>() {
                        new IfButtonGestureDefinition(
                            (ctx) => { return true; },
                            a as DSL.Def.AcceptableInIfButtonClause,
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { countDown.Signal(); })
                    };
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        countDown.Reset();
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(Global, new List<GestureDefinition>());
                            var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), gestureDefA, gestureDefB);
                            Global.StrokeWatcher.strokes.Add(new Stroke.Stroke(
                                Global.Config.StrokeDirectionChangeThreshold,
                                Global.Config.StrokeExtensionThreshold,
                                Def.Direction.Down));
                            var ev = Helper.Convert(b as DSL.Def.AcceptableInOnClause).GetPair();
                            var res = S1.Input((Def.Event.IEvent)ev, new Point());
                            if (a == b)
                            {
                                Assert.IsTrue(res.NextState is State0);
                                Assert.IsTrue(countDown.Wait(50));
                            }
                            else
                            {
                                Assert.IsTrue(res.NextState is State1);
                                Assert.IsFalse(countDown.Wait(50));
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition1_3_RRTest()
        {
            using (var countDown = new CountdownEvent(2))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    var gestureDef = new List<IfButtonGestureDefinition>() {
                        new IfButtonGestureDefinition(
                            (ctx) => { return true; },
                            a as DSL.Def.AcceptableInIfButtonClause,
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { countDown.Signal(); },
                            (ctx) => { countDown.Signal(); })
                    };
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        countDown.Reset();
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(GetDefaultGlobal(), gestureDef);
                            var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), new List<OnButtonGestureDefinition>(), gestureDef);
                            var ev = Helper.Convert(b as DSL.Def.AcceptableInOnClause).GetPair();
                            var res = S1.Input((Def.Event.IEvent)ev, new Point());
                            if (a == b)
                            {
                                Assert.IsTrue(res.NextState is State0);
                                Assert.IsTrue(countDown.Wait(50));
                            }
                            else
                            {
                                Assert.IsTrue(res.NextState is State1);
                                Assert.IsFalse(countDown.Wait(50));
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition1_4_RRTest()
        {
            Assert.Fail();
            /*
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        mouseEvents.Clear();
                        countDown.Reset();
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
                            var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                            var ev = Helper.Convert(b as DSL.Def.AcceptableInOnClause).GetPair();
                            var res = S1.Input((Def.Event.IEvent)ev, new Point());

                            Global.UserActionTaskFactory.StartNew(() =>
                            {
                                countDown.Signal();
                            });
                            countDown.Wait(50);
                            if (a == b)
                            {
                                Assert.IsTrue(res.NextState is State0);
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
                                Assert.AreEqual(mouseEvents.Count, 0);
                            }
                        }
                    }
                }
            }
            */
        }

        [TestMethod()]
        public void Transition1_5_RRTest()
        {
            Assert.Fail();
            /*
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    mouseEvents.Clear();
                    countDown.Reset();
                    using (var Global = GetDefaultGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                        var res = S1.Cancel();
                        Global.UserActionTaskFactory.StartNew(() =>
                        {
                            countDown.Signal();
                        });
                        countDown.Wait(50);
                        Assert.AreEqual(mouseEvents.Count, 1);
                        if (a == DSL.Def.Constant.LeftButton)
                        {
                            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                        }
                        else if (a == DSL.Def.Constant.MiddleButton)
                        {
                            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                        }
                        else if (a == DSL.Def.Constant.RightButton)
                        {
                            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                        }
                        else if (a == DSL.Def.Constant.X1Button)
                        {
                            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
                        }
                        else if (a == DSL.Def.Constant.X2Button)
                        {
                            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                        }
                        Assert.IsTrue(res is State0);
                    }
                }
            }
            */
        }

        [TestMethod()]
        public void Transition1_6_RRTest()
        {
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    countDown.Reset();
                    var gestureDef = new List<IfButtonGestureDefinition>() {
                        new IfButtonGestureDefinition(
                            (ctx) => { return true; },
                            a as DSL.Def.AcceptableInIfButtonClause,
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { Assert.Fail(); },
                            (ctx) => { countDown.Signal(); })
                    };
                    using (var Global = GetDefaultGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), new List<OnButtonGestureDefinition>(), gestureDef);
                        var res = S1.Reset();
                        Assert.IsTrue(res is State0);
                        Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(a as DSL.Def.AcceptableInOnClause).GetPair()));
                        Assert.IsTrue(countDown.Wait(50));
                    }
                }
            }
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenATriggerInIgnoreListGivenTest()
        {
            using (var Global = GetDefaultGlobal())
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
            using (var Global = GetDefaultGlobal())
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
        public void RestorePrimaryButtonDownTest()
        {
            Assert.Fail();
            /*
            using (var Global = GetDefaultGlobal())
            {
                var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.LeftButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonDownEvent()();
                    Assert.AreEqual(mouseEvents.Count, 1);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.MiddleButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonDownEvent()();
                    Assert.AreEqual(mouseEvents.Count, 1);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonDownEvent()();
                    Assert.AreEqual(mouseEvents.Count, 1);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.X1ButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonDownEvent()();
                    Assert.AreEqual(mouseEvents.Count, 1);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                    Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.X2ButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonDownEvent()();
                    Assert.AreEqual(mouseEvents.Count, 1);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                    Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                }
            }
            */
        }

        [TestMethod()]
        public void RestorePrimaryButtonClickTest()
        {
            Assert.Fail();
            /*
            using (var Global = GetDefaultGlobal())
            {
                var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.LeftButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonClickEvent()();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.MiddleButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonClickEvent()();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonClickEvent()();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
                }
                {
                    var S1 = new State1(Global, S0, ctx, Def.Constant.X1ButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    mouseEvents.Clear();
                    Assert.AreEqual(mouseEvents.Count, 0);
                    S1.RestorePrimaryButtonClickEvent()();
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
                    S1.RestorePrimaryButtonClickEvent()();
                    Assert.AreEqual(mouseEvents.Count, 2);
                    Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                    Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                    Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                    Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
                }
            }
            */
        }
    }
}