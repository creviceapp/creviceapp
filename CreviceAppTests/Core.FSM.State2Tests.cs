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
    public class State2Tests
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
        public void State2MustHaveGivenArgumentsTest()
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
            var S2 = S1.S2;

            Assert.AreEqual(S2.Global, S0.Global);
            Assert.AreEqual(S2.S0, S0);
            Assert.AreEqual(S2.ctx, ctx);
            Assert.AreEqual(S2.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S2.T0.Count(), 1);
            Assert.AreEqual(S2.T1.Count(), 1);
            Assert.AreEqual(S2.T2.Count(), 1);
            Assert.AreEqual(S2.T3.Count(), 1);
        }

        [TestMethod()]
        public void State2MustBypassInputToStrokeWatcher_RRTest()
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
                        var S2 = S1.S2;
                        S2.Input(Helper.Convert(b), new Point(0, 0));
                        S2.Input(Def.Constant.Move, new Point(100, 100));
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
        public void Transition2_0_RRTest()
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
                        var S2 = S1.S2;
                        var ev = Helper.Convert(b);
                        var res = S2.Input(ev, new Point());
                        Assert.IsTrue(res.NextState is State2);
                        if (a == b)
                        {
                            Assert.IsTrue(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition2_1_RRTest()
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
                        var S2 = S1.S2;
                        var ev = Helper.Convert(b);
                        var res = S2.Input(ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State3);
                            Assert.IsTrue(S2.T1[(Def.Event.IDoubleActionSet)ev].SequenceEqual(((State3)res.NextState).T0));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State2);
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition2_2_RRTest()
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
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(Global, new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), gestureDef, new List<IfButtonGestureDefinition>());
                        var S2 = S1.S2;
                        Global.StrokeWatcher.strokes.Add(new Stroke.Stroke(
                            Global.Config.Gesture.StrokeDirectionChangeThreshold,
                            Global.Config.Gesture.StrokeExtensionThreshold,
                            Def.Direction.Down));
                        var ev = Helper.Convert(b).GetPair();
                        var res = S2.Input((Def.Event.IEvent)ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.IsTrue(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State2);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition2_3_RRTest()
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
                        var S2 = S1.S2;
                        var ev = Helper.Convert(b).GetPair();
                        var res = S2.Input((Def.Event.IEvent)ev, new Point());
                        if (a == b)
                        {
                            Assert.IsTrue(res.NextState is State0);
                            Assert.AreEqual(mouseEvents.Count, 0);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                        else
                        {
                            Assert.IsTrue(res.NextState is State2);
                            Assert.AreEqual(mouseEvents.Count, 0);
                            Assert.IsFalse(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition2_4_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                using (var Global = new StateGlobal())
                {
                    var S0 = new State0(Global, new List<GestureDefinition>());
                    var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                    var S2 = S1.S2;
                    var res = S2.Reset();
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
                var S2 = S1.S2;

                Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
                Assert.AreEqual(Global.IgnoreNext.Count, 1);

                var res = S2.Input(Def.Constant.RightButtonUp, new Point());
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
                var S2 = S1.S2;

                Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
                Assert.AreEqual(Global.IgnoreNext.Count, 1);

                var res = S2.Input(Def.Constant.RightButtonDown, new Point());
                Assert.IsFalse(res.Event.IsConsumed);
                Assert.AreEqual(Global.IgnoreNext.Count, 0);
            }
        }
    }
}