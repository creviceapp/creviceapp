﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Linq;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestState0 = Crevice.Core.FSM.State0<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestStateN = Crevice.Core.FSM.StateN<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;


    [TestClass]
    public class StateNTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var s0 = new TestState0(gm, root);
                var history = s0.CreateHistory(TestEvents.Physical.TestPhysicalPressEventA);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, true);
                }
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt, canCancel: false);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, false);
                }
            }
        }

        [TestMethod]
        public void CreateHistoryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.History[0].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(s1.History[0].Item2, s0);

                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                var s2 = res1.NextState as TestStateN;
                Assert.AreEqual(s2.History.Count, 2);
                Assert.AreEqual(s2.History[1].Item1, TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(s2.History[1].Item2, s1);

                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                var s3 = res2.NextState as TestStateN;
                Assert.AreEqual(s3.History.Count, 3);
                Assert.AreEqual(s3.History[2].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(s3.History[2].Item2, s2);

                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                var s4 = res3.NextState as TestStateN;
                Assert.AreEqual(s4.History.Count, 4);
                Assert.AreEqual(s4.History[3].Item1, TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(s4.History[3].Item2, s3);
            }
        }

        [TestMethod]
        public void AbnormalEndTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                        .On(TestEvents.Logical.TestPressEventB)
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.AbnormalEndTriggers.Count, 0);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.AbnormalEndTriggers.Count, 1);
                Assert.AreEqual(s2.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventB), true);

                var s4 = res3.NextState as TestStateN;
                var res4 = s4.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res4.EventIsConsumed, true);
                Assert.AreEqual(res4.NextState is TestStateN, true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventB), true);
            }
        }

        [TestMethod]
        public void FindStateFromHistoryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.History.Count, 2);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.History.Count, 3);

                var (foundState, skippedReleaseEvents) = s3.FindStateFromHistory(TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(foundState, s1);
                Assert.AreEqual(skippedReleaseEvents.Count, 2);
                Assert.AreEqual(skippedReleaseEvents[0], TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(skippedReleaseEvents[1], TestEvents.Physical.TestPhysicalReleaseEventA);
            }
        }

        [TestMethod]
        public void NormalEndTriggerTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.NormalEndTrigger, TestEvents.Physical.TestPhysicalReleaseEventA);
            }
        }

        [TestMethod]
        public void IsNormalEndTriggerTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.IsNormalEndTrigger(TestEvents.Physical.TestPhysicalReleaseEventA), true);
            }
        }

        [TestMethod]
        public void LastStateTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.LastState, s0);
            }
        }

        [TestMethod]
        public void HasPressExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, true);
                }
            }
        }

        [TestMethod]
        public void HasDoExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, true);
                }
            }
        }

        [TestMethod]
        public void HasReleaseExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, true);
                }
            }
        }

        [TestMethod]
        public void GetDoubleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetDoubleThrowElements(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].DoubleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetSingleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestFireEventA)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetSingleThrowElements(TestEvents.Physical.TestPhysicalFireEventA);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].SingleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetStrokeElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(StrokeDirection.Up)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetStrokeElements(new List<StrokeDirection>() { StrokeDirection.Up });
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].StrokeElements[0]);
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                                .On(TestEvents.Logical.TestPressEventA)
                                    .On(TestEvents.Logical.TestPressEventB)
                                        .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    var result = s1.Reset();
                    Assert.AreEqual(s0, result);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                                .On(TestEvents.Logical.TestPressEventA)
                                    .On(TestEvents.Logical.TestPressEventB)
                                        .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.AreEqual(res1.NextState is TestStateN, true);

                    var s2 = res1.NextState as TestStateN;
                    var result = s2.Reset();
                    Assert.AreEqual(s1, result);
                }
            }
        }
    }
}
