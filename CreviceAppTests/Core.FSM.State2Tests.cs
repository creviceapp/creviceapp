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
        public void Transition08_Transition09_RRTest()
        {
            using (var countDown = new CountdownEvent(1))
            {
                foreach (var a in TestDef.Constant.AcceptablesInOnClause)
                {
                    foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                    {
                        var gestureDef = new List<OnButtonIfButtonGestureDefinition>() {
                            new OnButtonIfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a,
                                (DSL.Def.AcceptableInIfButtonClause)b,
                                (ctx) => { countDown.Signal(); })
                        };
                        foreach (var c in TestDef.Constant.AcceptablesInOnClause)
                        {
                            countDown.Reset();
                            using (var Global = new StateGlobal())
                            {
                                var S0 = new State0(Global, new List<GestureDefinition>());
                                var S1 = new State1(Global, S0, ctx, Helper.Convert(a), gestureDef, new List<IfButtonGestureDefinition>());
                                var S2 = new State2(Global, S0, S1, ctx, Helper.Convert(a), Helper.Convert(b), gestureDef);
                                var ev = Helper.Convert(c).GetPair();
                                var res = S2.Input((Def.Event.IEvent)ev, new Point());
                                if (b == c)
                                {
                                    Assert.IsTrue(res.NextState is State1);
                                    Assert.IsTrue(res.StrokeWatcher.IsResetRequested);
                                    Assert.IsTrue(countDown.Wait(50));
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                }
                                else if (a == c)
                                {
                                    Assert.IsTrue(res.NextState is State0);
                                    Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                                    Assert.IsFalse(countDown.Wait(50));
                                    Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(b).GetPair()));
                                }
                                else
                                {
                                    Assert.IsTrue(res.NextState is State2);
                                    Assert.IsFalse(res.StrokeWatcher.IsResetRequested);
                                    Assert.IsFalse(countDown.Wait(50));
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition12_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                        var S2 = new State2(Global, S0, S1, ctx, Helper.Convert(a), Helper.Convert(b), new List<OnButtonIfButtonGestureDefinition>());
                        var res = S2.Reset();
                        Assert.IsTrue(res is State0);
                        Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(a).GetPair()));
                        Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(b).GetPair()));
                    }
                }
            }
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