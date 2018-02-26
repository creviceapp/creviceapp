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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Records.Count, 1);
                Assert.AreEqual(s1.History.Records[0].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(s1.History.Records[0].State, s0);
            }
        }

        [TestMethod]
        public void NoDefinitionTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }
        
        [TestMethod]
        public void Do_LogicalSingleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_PhysicalSingleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.PhysicalSingleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_LogicalSingleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_PhysicalSingleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.PhysicalSingleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
                {
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState is TestState0, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Release_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState == s0, false);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Release_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState == s0, false);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Release_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Release_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_Release_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState is TestState0, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_Release_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_Release_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Do_Release_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                var when = root.When((ctx) => { return false; });
                when
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Release_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Release_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState == s0, false);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Release_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Release_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_Release_LogicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_Release_PhysicalDoubleThrow_WhenIsTrue_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, true);
                    Assert.AreEqual(releaseCalled, true);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_Release_LogicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void Press_Do_Release_PhysicalDoubleThrow_WhenIsFalse_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return false; })
                    .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    releaseCalled = false;
                    var result = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void GetInversedSingleThrowTriggerDictionaryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.LogicalSingleThrowKeys[0]).Do((ctx) => { });
                when.On(TestEvents.LogicalSingleThrowKeys[0]).Do((ctx) => { });
                var s0 = new TestState0(gm, root) as TestState0;
                var result = TestState0.GetInversedSingleThrowTrigger(root);
                var key = TestEvents.LogicalSingleThrowKeys[0].FireEvent;
                Assert.AreEqual(result[key].Count(), 1);
                Assert.AreEqual(result[key].First(), when);
            }
        }

        [TestMethod]
        public void GetInversedDoubleThrowTriggerDictionaryTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Do((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Do((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Do((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    when.On(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDoubleThrowTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
        }

        [TestMethod]
        public void GetInversedDecomposedTriggerDictionaryTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDecomposedTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDecomposedTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Press((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDecomposedTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]).Release((ctx) => { });
                    var s0 = new TestState0(gm, root) as TestState0;
                    var result = TestState0.GetInversedDecomposedTrigger(root);
                    var key = TestEvents.LogicalDoubleThrowKeys[0].PressEvent;
                    Assert.AreEqual(result[key].Count(), 1);
                    Assert.AreEqual(result[key].First(), when);
                }
            }
        }

        [TestMethod]
        public void IsStateTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                Assert.AreEqual(s0.IsState0, true);
                Assert.AreEqual(s0.IsStateN, false);
            }
        }

        [TestMethod]
        public void ToState0Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                Assert.AreEqual(s0.AsState0() is TestState0, true);
            }
        }

        [TestMethod]
        public void ToStateNTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                Assert.AreEqual(s0.AsStateN() is null, true);
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
