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
    public class State2Tests
    {
        [TestMethod()]
        public void InputMustExecuteNoTransitionTest()
        {
            // todo: round robin test
            var executed = false;
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    () => { executed = true; })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);
            var res = S2.Input(Def.Constant.MiddleButtonDown, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.IsFalse(executed);
            Assert.IsTrue(res.NextState is State2);
        }

        [TestMethod()]
        public void InputMustExecuteTransition6Test()
        {
            var executed = false;
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    () => { executed = true; })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            var res = S2.Input(Def.Constant.LeftButtonUp, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            Assert.IsTrue(executed);
            Assert.IsTrue(res.NextState is State1);
        }

        [TestMethod()]
        public void InputMustExecuteTransition7Test()
        {
            var executed = false;
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    () => { executed = true; })
            };
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.RightButtonDown, gestureDef);
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            var res = S2.Input(Def.Constant.RightButtonUp, new LowLevelMouseHook.POINT());
            Thread.Sleep(100);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);
            Assert.IsTrue(S2.Global.IgnoreNext.Contains(Def.Constant.LeftButtonUp));
            Assert.IsFalse(executed);
            Assert.IsTrue(res.NextState is State0);
        }

        [TestMethod()]
        public void State2MustHaveGivenArgumentsTest()
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
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);

            Assert.AreEqual(S2.Global, S1.Global);
            Assert.AreEqual(S2.S0, S0);
            Assert.AreEqual(S2.S1, S1);
            Assert.AreEqual(S2.primaryTrigger, Def.Constant.RightButtonDown);
            Assert.AreEqual(S2.secondaryTrigger, Def.Constant.LeftButtonDown);
            Assert.AreEqual(S2.T1.Count, 1);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var gestureDef = new List<GestureDefinition>();
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.LeftButtonDown, gestureDef);
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);

            S2.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);

            var res = S2.Input(Def.Constant.RightButtonUp, new LowLevelMouseHook.POINT());
            Assert.IsTrue(res.Trigger.IsConsumed);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<GestureDefinition>();
            var S0 = new State0(new GlobalValues(), Transition.Gen0(gestureDef));
            var S1 = new State1(S0.Global, S0, Def.Constant.LeftButtonDown, gestureDef);
            var S2 = new State2(S1.Global, S0, S1, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S1.T1);

            S2.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);

            var res = S2.Input(Def.Constant.RightButtonDown, new LowLevelMouseHook.POINT());
            Assert.IsFalse(res.Trigger.IsConsumed);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
        }
    }
}