using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{

    using WinAPI.WindowsHookEx;

    [TestClass()]
    public class State0Tests
    {
        readonly UserActionExecutionContext ctx = new UserActionExecutionContext(new Point());

        [TestMethod()]
        public void MustHaveCleanGlobalValuesTest()
        {
            var S0 = new State0(new GlobalValues(), new List<GestureDefinition>());
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void MustHaveGivenArgumentsTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    (ctx) => { }),
            };
            var S0 = new State0(new GlobalValues(), gestureDef);

            Assert.AreEqual(S0.T0.Count, 1);
            Assert.AreEqual(S0.T1.Count, 1);
            Assert.AreEqual(S0.T5.Count, 1);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var S0 = new State0(new GlobalValues(), new List<GestureDefinition>());
            S0.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 0);
        }
        
        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var S0 = new State0(new GlobalValues(), new List<GestureDefinition>());
            S0.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustNotExecuteTransitionTest()
        {
            // todo: round robin test
            var gestureDef = new List<GestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
            };
            var S0 = new State0(new GlobalValues(), new List<GestureDefinition>());
            var res = S0.Input(Def.Constant.LeftButtonDown, new Point());
            Assert.IsTrue(res.NextState is State0);
        }

        [TestMethod()]
        public void InputMustExecuteTransition01Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
            };
            var S0 = new State0(new GlobalValues(), gestureDef);
            var res = S0.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsTrue(res.NextState is State1);
       }

        [TestMethod()]
        public void FilterByWhenClauseTest()
        {
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.LeftButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { }),
                 new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return false; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
            };
            var filtered = State0.FilterByWhenClause(ctx, gestureDef).ToList();
            Assert.AreEqual(filtered.Count(), 1);
            Assert.AreEqual(filtered[0].onButton, DSL.Def.Constant.LeftButton);
        }
    }
}