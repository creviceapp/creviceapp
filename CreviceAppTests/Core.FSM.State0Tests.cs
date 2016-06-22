using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Core.FSM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM.Tests
{
    [TestClass()]
    public class State0Tests
    {
        readonly UserActionExecutionContext ctx = new UserActionExecutionContext(new Point());

        [TestMethod()]
        public void MustHaveCleanGlobalValuesTest()
        {
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
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
            var S0 = new State0(new StateGlobal(), gestureDef);

            Assert.AreEqual(S0.T0.Count, 1);
            Assert.AreEqual(S0.T1.Count, 1);
            Assert.AreEqual(S0.T5.Count, 1);
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenTriggerInIgnoreListGivenTest()
        {
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            S0.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 0);
        }
        
        [TestMethod()]
        public void InputMustResetIgnoreListWhenTriggerOfPairInIgnoreListGivenTest()
        {
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            S0.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 1);

            var res = S0.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S0.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustExecuteTransition00WhenHavingIfButtonGestureDefinitionAndISingleActionGivenTest()
        {
            var list = new List<DSL.Def.AcceptableInIfButtonClause>()
            {
                DSL.Def.Constant.WheelUp,
                DSL.Def.Constant.WheelDown,
                DSL.Def.Constant.WheelLeft,
                DSL.Def.Constant.WheelRight
            };
            Assert.IsTrue(list.TrueForAll(x => TestDef.Constant.AcceptablesInIfButtonClause.Contains(x)));

            foreach (var a in  list)
            {
                var countDown = new CountdownEvent(1);
                var gestureDef = new List<GestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    (ctx) => { countDown.Signal(); })
                };
                var S0 = new State0(new StateGlobal(), gestureDef);
                foreach (var b in TestDef.Constant.AcceptablesInIfButtonClause)
                {
                    countDown.Reset();
                    var ev = Helper.Convert(b);
                    var res = S0.Input(ev, new Point());
                    Assert.IsTrue(res.NextState is State0);
                    Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
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

        [TestMethod()]
        public void InputMustExecuteTransition01WhenHavingIfButtonGestureDefinitionAndIDoubleActionGivenTest()
        {
            var list = new List<DSL.Def.AcceptableInIfButtonClause>()
            {
                DSL.Def.Constant.LeftButton,
                DSL.Def.Constant.MiddleButton,
                DSL.Def.Constant.RightButton,
                DSL.Def.Constant.X1Button,
                DSL.Def.Constant.X2Button
            };
            Assert.IsTrue(list.TrueForAll(x => TestDef.Constant.AcceptablesInIfButtonClause.Contains(x)));

            foreach (var a in list)
            {
                var gestureDef = new List<GestureDefinition>() {
                new IfButtonGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    (ctx) => { })
                };
                var S0 = new State0(new StateGlobal(), gestureDef);
                foreach (var b in TestDef.Constant.AcceptablesInIfButtonClause)
                {
                    var res = S0.Input(Helper.Convert(b), new Point());
                    if (a == b)
                    {
                        Assert.IsTrue(res.NextState is State1);
                        Assert.IsTrue(res.StrokeWatcher.IsResetRequested);
                        Assert.AreEqual(gestureDef[0], ((State1)res.NextState).T5.ToList()[0]);
                    }
                    else
                    {
                        Assert.IsTrue(res.NextState is State0);
                        Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                    }
                }
            }
        }

        [TestMethod()]
        public void InputMustExecuteTransition01WhenHavingOnButtonIfButtonGestureDefinitionAndIDoubleActionGivenTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                var gestureDef = new List<GestureDefinition>() {
                new OnButtonIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    a,
                    DSL.Def.Constant.WheelUp,
                    (ctx) => { })
                };
                var S0 = new State0(new StateGlobal(), gestureDef);
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    var res = S0.Input((Def.Event.IEvent)Helper.Convert(b), new Point());
                    if (a == b)
                    {
                        Assert.IsTrue(res.NextState is State1);
                        Assert.IsTrue(res.StrokeWatcher.IsResetRequested);
                        Assert.IsTrue(gestureDef.SequenceEqual(((State1)res.NextState).T2[Def.Constant.WheelUp]));
                    }
                    else
                    {
                        Assert.IsTrue(res.NextState is State0);
                        Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                    }
                }
            }
        }
        
        [TestMethod()]
        public void InputMustExecuteTransition10Test()
        {
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var res = S0.Reset();
            Assert.IsTrue(res is State0);
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