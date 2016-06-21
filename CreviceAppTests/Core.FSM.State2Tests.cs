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
        readonly UserActionExecutionContext ctx = new UserActionExecutionContext(new Point());

        [TestMethod()]
        public void State2MustHaveGivenArgumentsTest()
        {
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            Assert.AreEqual(S2.Global, S1.Global);
            Assert.AreEqual(S2.S0, S0);
            Assert.AreEqual(S2.S1, S1);
            Assert.AreEqual(S2.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S2.secondaryEvent, Def.Constant.LeftButtonDown);
            Assert.AreEqual(S2.T3.Count(), 1);
        }

        [TestMethod()]
        public void InputMustExecuteNoTransitionTest()
        {
            // todo: round robin test
            var executed = false;
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { executed = true; })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);
            var res = S2.Input(Def.Constant.MiddleButtonDown, new Point());
            Thread.Sleep(100);
            Assert.IsFalse(executed);
            Assert.IsTrue(res.NextState is State2);
        }

        [TestMethod()]
        public void InputMustExecuteTransition08Test()
        {
            var executed = false;
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { executed = true; })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            var res = S2.Input(Def.Constant.LeftButtonUp, new Point());
            Thread.Sleep(100);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            Assert.IsTrue(executed);
            Assert.IsTrue(res.NextState is State1);
        }

        [TestMethod()]
        public void InputMustExecuteTransition09Test()
        {
            var executed = false;
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { executed = true; })
            };
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
            var res = S2.Input(Def.Constant.RightButtonUp, new Point());
            Thread.Sleep(100);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);
            Assert.IsTrue(S2.Global.IgnoreNext.Contains(Def.Constant.LeftButtonUp));
            Assert.IsFalse(executed);
            Assert.IsTrue(res.NextState is State0);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            S2.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);

            var res = S2.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = new State2(S1.Global, S0, S1, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            S2.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 1);

            var res = S2.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S2.Global.IgnoreNext.Count, 0);
        }
    }
}