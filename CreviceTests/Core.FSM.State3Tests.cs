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
    [TestClass()]
    public class State3Tests
    {
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

        [TestMethod()]
        public void State3MustHaveGivenArgumentsTest()
        {
            var gestureDef = new List<OnButtonWithIfButtonGestureDefinition>() {
                new OnButtonWithIfButtonGestureDefinition(
                    (ctx) => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.LeftButton,
                    null,
                    (ctx) => { },
                    null)
            };
            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S2.T3, gestureDef);

            Assert.AreEqual(S3.Global, S1.Global);
            Assert.AreEqual(S3.S0, S0);
            Assert.AreEqual(S3.S2, S2);
            Assert.AreEqual(S3.primaryEvent, Def.Constant.RightButtonDown);
            Assert.AreEqual(S3.secondaryEvent, Def.Constant.LeftButtonDown);
            Assert.AreEqual(S3.T0, S2.T3);
            Assert.AreEqual(S3.T1.Count(), 1);
        }

        [TestMethod()]
        public void Transition3_0_RRTest()
        {
            using (var countDown = new CountdownEvent(2))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        var gestureDefA = new List<IfButtonGestureDefinition>() {
                            new IfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); })
                        };
                        var gestureDefB = new List<OnButtonWithIfButtonGestureDefinition>() {
                            new OnButtonWithIfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInOnClause,
                                b as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { countDown.Signal(); },
                                (ctx) => { countDown.Signal(); })
                        };
                        foreach (var c in TestDef.Constant.DoubleTriggerButtons)
                        {
                            if (a == c)
                            {
                                continue;
                            }
                            countDown.Reset();
                            using (var Global = GetDefaultGlobal())
                            {
                                var S0 = new State0(Global, new List<GestureDefinition>());
                                var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), gestureDefB, gestureDefA);
                                var S2 = S1.S2;
                                var S3 = new State3(Global, S0, S2, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), Helper.Convert(b as DSL.Def.AcceptableInOnClause), S2.T3, gestureDefB);
                                var ev = Helper.Convert(c as DSL.Def.AcceptableInOnClause).GetPair();
                                var res = S3.Input((Def.Event.IEvent)ev, new Point());
                                if (b == c)
                                {
                                    Assert.IsTrue(res.NextState is State2);
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                    Assert.IsTrue(countDown.Wait(50));
                                }
                                else
                                {
                                    Assert.IsTrue(res.NextState is State3);
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                    Assert.IsFalse(countDown.Wait(50));
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void Transition3_1_RRTest()
        {
            using (var countDown = new CountdownEvent(2))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        var gestureDefA = new List<IfButtonGestureDefinition>() {
                            new IfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { countDown.Signal(); })
                        };
                        var gestureDefB = new List<OnButtonWithIfButtonGestureDefinition>() {
                            new OnButtonWithIfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInOnClause,
                                b as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { countDown.Signal(); })
                        };
                        foreach (var c in TestDef.Constant.DoubleTriggerButtons)
                        {
                            if (b == c)
                            {
                                continue;
                            }
                            countDown.Reset();
                            using (var Global = GetDefaultGlobal())
                            {
                                var S0 = new State0(Global, new List<GestureDefinition>());
                                var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), gestureDefB, gestureDefA);
                                var S2 = S1.S2;
                                var S3 = new State3(Global, S0, S2, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), Helper.Convert(b as DSL.Def.AcceptableInOnClause), S2.T3, gestureDefB);
                                var ev = Helper.Convert(c as DSL.Def.AcceptableInOnClause).GetPair();
                                var res = S3.Input((Def.Event.IEvent)ev, new Point());
                                if (a == c)
                                {
                                    Assert.IsTrue(res.NextState is State0);
                                    Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(b as DSL.Def.AcceptableInOnClause).GetPair()));
                                    Assert.IsTrue(countDown.Wait(50));
                                }
                                else
                                {
                                    Assert.IsTrue(res.NextState is State3);
                                    Assert.IsTrue(Global.IgnoreNext.Count() == 0);
                                    Assert.IsFalse(countDown.Wait(50));
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
            using (var countDown = new CountdownEvent(2))
            {
                foreach (var a in TestDef.Constant.DoubleTriggerButtons)
                {
                    foreach (var b in TestDef.Constant.DoubleTriggerButtons)
                    {
                        var gestureDefA = new List<IfButtonGestureDefinition>() {
                            new IfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { countDown.Signal(); })
                        };
                        var gestureDefB = new List<OnButtonWithIfButtonGestureDefinition>() {
                            new OnButtonWithIfButtonGestureDefinition(
                                (ctx) => { return true; },
                                a as DSL.Def.AcceptableInOnClause,
                                b as DSL.Def.AcceptableInIfButtonClause,
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { Assert.Fail(); },
                                (ctx) => { countDown.Signal(); })
                        };
                        using (var Global = GetDefaultGlobal())
                        {
                            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
                            var S1 = new State1(Global, S0, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), gestureDefB, gestureDefA);
                            var S2 = S1.S2;
                            var S3 = new State3(Global, S0, S2, ctx, Helper.Convert(a as DSL.Def.AcceptableInOnClause), Helper.Convert(b as DSL.Def.AcceptableInOnClause), S2.T3, gestureDefB);
                            var res = S3.Reset();
                            Assert.IsTrue(res is State0);
                            Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(a as DSL.Def.AcceptableInOnClause).GetPair()));
                            Assert.IsTrue(Global.IgnoreNext.Contains(Helper.Convert(b as DSL.Def.AcceptableInOnClause).GetPair()));
                            Assert.IsTrue(countDown.Wait(50));
                        }
                    }
                }
            }
        }

        [TestMethod()]
        public void InputMustReturnConsumedResultWhenGivenTriggerIsInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonWithIfButtonGestureDefinition>();
            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S2.T3, gestureDef);

            S3.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 1);

            var res = S3.Input(Def.Constant.RightButtonUp, new Point());
            Assert.IsTrue(res.Event.IsConsumed);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 0);
        }

        [TestMethod()]
        public void InputMustResetIgnoreListWhenGivenTriggerIsPairOfTriggerInIgnoreListTest()
        {
            var gestureDef = new List<OnButtonWithIfButtonGestureDefinition>();
            var S0 = new State0(GetDefaultGlobal(), new List<GestureDefinition>());
            var S1 = new State1(S0.Global, S0, ctx, Def.Constant.RightButtonDown, new List<OnButtonGestureDefinition>(), new List<IfButtonGestureDefinition>());
            var S2 = S1.S2;
            var S3 = new State3(S1.Global, S0, S2, ctx, Def.Constant.RightButtonDown, Def.Constant.LeftButtonDown, S2.T3, gestureDef);

            S3.Global.IgnoreNext.Add(Def.Constant.RightButtonUp);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 1);

            var res = S3.Input(Def.Constant.RightButtonDown, new Point());
            Assert.IsFalse(res.Event.IsConsumed);
            Assert.AreEqual(S3.Global.IgnoreNext.Count, 0);
        }
    }
}