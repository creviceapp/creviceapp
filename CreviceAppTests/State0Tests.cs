using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
    [TestClass()]
    public class State0Tests
    {
        [TestMethod()]
        public void State0MustHaveCleanGlobalValuesTest()
        {
            var S0 = new State0(new Dictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>>());
            Assert.AreEqual(S0.Global.ignoreNext.Count, 0);
        }

        [TestMethod()]
        public void State0MustHaveGivenT0Test()
        {
            var T0 = new Dictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>>();
            var S0 = new State0(T0);
            Assert.AreEqual(S0.T0, T0);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var S0 = new State0(new Dictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>>());
            S0.Global.ignoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.ignoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonUp);
            Assert.IsTrue(res.Trigger.IsConsumed);
            Assert.AreEqual(S0.Global.ignoreNext.Count, 0);
        }
        
        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var S0 = new State0(new Dictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>>());
            S0.Global.ignoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.ignoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonDown);
            Assert.IsFalse(res.Trigger.IsConsumed);
            Assert.AreEqual(S0.Global.ignoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustNotExecuteTransitionTest()
        {
            // todo: round robin test
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { })
            };
            var S0 = new State0(Transition.Gen0(gestureDef));
            var res = S0.Input(Def.Constant.LeftButtonDown);
            Assert.IsTrue(res.NextState is State0);
        }

        [TestMethod()]
        public void InputMustExecuteTransition0Test()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { })
            };
            var S0 = new State0(Transition.Gen0(gestureDef));
            var res = S0.Input(Def.Constant.RightButtonDown);
            Assert.IsTrue(res.NextState is State1);
       }

        [TestMethod()]
        public void FilterByWhenClauseTest()
        {
            var gestureDef = new List<GestureDefinition>() {
                new ButtonGestureDefinition(
                    () => { return true; },
                    Config.Def.Constant.LeftButton,
                    Config.Def.Constant.WheelUp,
                    () => { }),
                 new ButtonGestureDefinition(
                    () => { return false; },
                    Config.Def.Constant.RightButton,
                    Config.Def.Constant.WheelUp,
                    () => { })
            };
            var filtered = State0.FilterByWhenClause(gestureDef).ToList();
            Assert.AreEqual(filtered.Count(), 1);
            Assert.AreEqual(filtered[0].onButton, Config.Def.Constant.LeftButton);
        }
    }
}