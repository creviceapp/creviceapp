using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
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


        [TestMethod()]
        public void InputMustExecuteNoTransitionTest()
        {
            // todo: round robin test
            var executed = false;
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => { executed = true; })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();
            var res = S1.Input(Def.Constant.LeftButtonDown, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsFalse(executed);
            Assert.IsTrue(res.NextState is State1);
        }
        
        [TestMethod()]
        public void InputMustExecuteTransition1Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    () => { })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();
            var res = S1.Input(Def.Constant.LeftButtonDown, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsFalse(S1.PrimaryTriggerIsRestorable);
            Assert.IsTrue(res.NextState is State2);
        }

        [TestMethod()]
        public void InputMustExecuteTransition2Test()
        {
            var executed = false;
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => { executed = true; })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();
            var res = S1.Input(Def.Constant.WheelUp, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsFalse(S1.PrimaryTriggerIsRestorable);
            Assert.IsTrue(executed);
            Assert.IsTrue(res.NextState is State1);
        }

        [TestMethod()]
        public void InputMustExecuteTransition3Test()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void InputMustExecuteTransition4Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => { })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();
            S1.PrimaryTriggerIsRestorable = true;
            var res = S1.Input(Def.Constant.RightButtonUp, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsTrue(res.NextState is State0);
            Assert.AreEqual(mouseEvents.Count, 2);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        [TestMethod()]
        public void InputMustExecuteTransition5Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => { })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();
            S1.PrimaryTriggerIsRestorable = false;
            var res = S1.Input(Def.Constant.RightButtonUp, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsTrue(res.NextState is State0);
            Assert.AreEqual(mouseEvents.Count, 0);
        }

        [TestMethod()]
        public void State1MustHaveGivenArgumentsTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    () => { }),
                new StrokeGestureDefinition(
                    () => { return false; },
                    DSL.Def.Constant.RightButton,
                    new Def.Stroke(new List<Def.Direction>() { Def.Direction.Up }),
                    () => { })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);

            Assert.AreEqual(S1.Global, S0.Global);
            Assert.AreEqual(S1.S0, S0);
            Assert.AreEqual(S1.primaryTrigger, Def.Constant.RightButtonDown);
            Assert.AreEqual(S1.T1.Count, 1);
            Assert.AreEqual(S1.T2.Count, 1);
            Assert.AreEqual(S1.T3.Count, 1);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var gestureDef = new List<GestureDefinition>();
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.LeftButtonDown, gestureDef);

            S1.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 1);

            var res = S1.Input(Def.Constant.RightButtonUp, new LowLevelMouseHook.POINT());
            Assert.IsTrue(res.Trigger.IsConsumed);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<GestureDefinition>();
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.LeftButtonDown, gestureDef);
            S1.Global.ResetStrokeWatcher();

            S1.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 1);

            var res = S1.Input(Def.Constant.RightButtonDown, new LowLevelMouseHook.POINT());
            Assert.IsFalse(res.Trigger.IsConsumed);
            Assert.AreEqual(S1.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void RestorePrimaryTriggerTest()
        {
            var gestureDef = new List<GestureDefinition>();
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            {
                var S1 = new State1(S0.Global, S0, Def.Constant.LeftButtonDown, gestureDef);
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryTrigger();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, Def.Constant.MiddleButtonDown, gestureDef);
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryTrigger();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryTrigger();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
            }
            {
                var S1 = new State1(S0.Global, S0, Def.Constant.X1ButtonDown, gestureDef);
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryTrigger();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton1);
            }
            {
                var S1 = new State1(S0.Global, S0, Def.Constant.X2ButtonDown, gestureDef);
                mouseEvents.Clear();
                Assert.AreEqual(mouseEvents.Count, 0);
                S1.RestorePrimaryTrigger();
                Assert.AreEqual(mouseEvents.Count, 2);
                Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
                Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
                Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
                Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
            }
        }
    }
}