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
    [TestClass()]
    public class State3Tests
    {
        readonly UserActionExecutionContext ctx = new UserActionExecutionContext(new Point());

        [TestMethod()]
        public void State3MustHaveGivenArgumentsTest()
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
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            Assert.AreEqual(S3.Global, S1.Global);
            Assert.AreEqual(S3.S0, S0);
            Assert.AreEqual(S3.S2, S2);
            Assert.AreEqual(S3.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S3.secondaryEvent, Def.Constant.LeftButtonDown);
            Assert.AreEqual(S3.T0.Count(), 1);
        }

        [TestMethod()]
        public void Transition3_0_Transition3_1_RRTest()
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
                                var S2 = S1.S2;
                                var S3 = new State3(Global, S0, S2, ctx, Helper.Convert(a), Helper.Convert(b), gestureDef);
                                var ev = Helper.Convert(c).GetPair();
                                var res = S3.Input((Def.Event.IEvent)ev, new Point());
                                if (b == c)
                                {
                                    Assert.IsTrue(res.NextState is State2);
                                    Assert.IsTrue(countDown.Wait(50));
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                }
                                else if (a == c)
                                {
                                    Assert.IsTrue(res.NextState is State0);
                                    Assert.IsFalse(countDown.Wait(50));
                                    Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(b).GetPair()));
                                }
                                else
                                {
                                    Assert.IsTrue(res.NextState is State3);
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
        public void Transition3_2_RRTest()
        {
            foreach (var a in TestDef.Constant.AcceptablesInOnClause)
            {
                foreach (var b in TestDef.Constant.AcceptablesInOnClause)
                {
                    using (var Global = new StateGlobal())
                    {
                        var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
                        var S1 = new State1(Global, S0, ctx, Helper.Convert(a), new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
                        var S2 = S1.S2;
                        var S3 = new State3(Global, S0, S2, ctx, Helper.Convert(a), Helper.Convert(b), new List<OnButtonIfButtonGestureDefinition>());
                        var res = S3.Reset();
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
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            S3.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 1);

            var res = S3.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonIfButtonGestureDefinition>();
            var S0 = new State0(new StateGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, gestureDef);

            S3.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 1);

            var res = S3.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 0);
        }
    }
}