using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Linq;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestState0 = Crevice.Core.FSM.State0<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestStateN = Crevice.Core.FSM.StateN<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    [TestClass]
    public class State0Test
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                Assert.AreEqual(s0.Machine, gm);
                Assert.AreEqual(s0.RootElement, root);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.History[0].Item1, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                Assert.AreEqual(s1.History[0].Item2, s0);
            }
        }

        [TestMethod]
        public void DoesNotConsumeGivenInputWhenNoDefinition()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenPhysicalSingleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenLogicalSingleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.PhysicalSingleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.PhysicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(result.NextState is TestStateN);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(result.NextState is TestStateN);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }
        [TestMethod]
        public void CreateHistory()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                var result = s0.CreateHistory(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0].Item1, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                Assert.AreEqual(result[0].Item2, s0);
            }
        }


        [TestMethod]
        public void GetActiveDoubleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var result = s0.GetActiveDoubleThrowElements(evalContext, TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetActiveSingleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var result = s0.GetActiveSingleThrowElements(evalContext, TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.SingleThrowElements[0]);
            }
        }

        [TestMethod]
        public void SingleThrowTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var result = s0.SingleThrowTriggers;
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result.Contains(when.SingleThrowElements[0].Trigger), true);
            }
        }

        [TestMethod]
        public void DoubleThrowTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var result = s0.DoubleThrowTriggers;
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result.Contains(when.DoubleThrowElements[0].Trigger), true);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var result = s0.Reset();
                    Assert.AreEqual(s0, result);
                }
            }
        }
    }
}
