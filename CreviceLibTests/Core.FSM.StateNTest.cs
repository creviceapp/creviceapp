using System;
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
                var history = s0.CreateHistory(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt, depth: 0);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, true);
                }
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt, depth: 0, canCancel: false);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenPhysicalSingleThrowDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalSingleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual((result.NextState as TestStateN).History, (s1 as TestStateN).History);
                    Assert.AreEqual((result.NextState as TestStateN).Ctx, (s1 as TestStateN).Ctx);
                    Assert.AreEqual((result.NextState as TestStateN).DoubleThrowElements, (s1 as TestStateN).DoubleThrowElements);
                    Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenLogicalSingleThrowDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.PhysicalSingleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual((result.NextState as TestStateN).History, (s1 as TestStateN).History);
                    Assert.AreEqual((result.NextState as TestStateN).Ctx, (s1 as TestStateN).Ctx);
                    Assert.AreEqual((result.NextState as TestStateN).DoubleThrowElements, (s1 as TestStateN).DoubleThrowElements);
                    Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, true);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowPressDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                    }
                    {
                        pressCalled = false;
                        var (eventIsConsumed, s2) = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(s2 == s0, false);
                        Assert.AreEqual(s2 == s1, false);
                        Assert.AreEqual((s1 as TestStateN).Depth, 1);
                        Assert.AreEqual((s2 as TestStateN).Depth, 2);
                        Assert.AreEqual(eventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                    }
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                    }
                    {
                        pressCalled = false;
                        var (eventIsConsumed, s2) = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                        Assert.AreEqual(s2, s1);
                        Assert.AreEqual(eventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                    }
                    {
                        pressCalled = false;
                        var (eventIsConsumed, s2) = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(s2, s1);
                        Assert.AreEqual(eventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                    }
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowPressDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                    }
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                    }
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                    }
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(result.NextState == s1, false);
                        Assert.AreEqual((s1 as TestStateN).Depth, 1);
                        Assert.AreEqual((result.NextState as TestStateN).Depth, 2);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                    }
                    {
                        pressCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                    }
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.PhysicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowReleaseDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[0])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        releaseCalled = false;
                        var (eventIsConsumed, s2) = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(s2 == s0, false);
                        Assert.AreEqual(s2 == s1, false);
                        Assert.AreEqual((s1 as TestStateN).Depth, 1);
                        Assert.AreEqual((s2 as TestStateN).Depth, 2);
                        Assert.AreEqual(eventIsConsumed, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[1])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
        }


        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowPressDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.PhysicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasPressExecutors, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    pressCalled = false;
                    doCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowPressDoDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var pressCalled = false;
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { pressCalled = true; })
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasPressExecutors, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, true);
                    Assert.AreEqual(doCalled, false);
                }
                {
                    doCalled = false;
                    pressCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(pressCalled, false);
                    Assert.AreEqual(doCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowDoReleaseDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.PhysicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasReleaseExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowDoReleaseDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                var releaseCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { doCalled = true; })
                    .Release((ctx) => { releaseCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result.NextState, s1);
                    Assert.AreEqual(result.EventIsConsumed, false);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result.NextState == s1, false);
                    Assert.AreEqual(result.NextState is TestStateN, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasDoExecutors, true);
                    Assert.AreEqual((result.NextState as TestStateN).HasReleaseExecutors, true);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
                {
                    doCalled = false;
                    releaseCalled = false;
                    var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                    Assert.AreEqual(doCalled, false);
                    Assert.AreEqual(releaseCalled, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowPressReleaseDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var (eventIsConsumed, s2) = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(s2 == s0, false);
                        Assert.AreEqual(s2 == s1, false);
                        Assert.AreEqual((s1 as TestStateN).Depth, 1);
                        Assert.AreEqual((s2 as TestStateN).Depth, 2);
                        Assert.AreEqual(eventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        // todo
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowPressReleaseDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(result.NextState is TestStateN, true);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        // todo
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                        Assert.AreEqual(result.NextState is TestStateN, true);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowPressDoReleaseDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                {
                    var pressCalled = false;
                    var doCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; })
                        .Do((ctx) => { doCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(result.NextState is TestStateN, true);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        // Todo
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                }
                {
                    var pressCalled = false;
                    var doCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; })
                        .Do((ctx) => { doCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
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
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowPressDoReleaseDefinisitonExists()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var doCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; })
                        .Do((ctx) => { doCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                        Assert.AreEqual(result.NextState is TestStateN, true);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, true);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        // todo
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    var doCalled = false;
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; })
                        .Do((ctx) => { doCalled = true; })
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, false);
                    }
                    {
                        pressCalled = false;
                        doCalled = false;
                        releaseCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
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
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(pressCalled, false);
                        Assert.AreEqual(doCalled, false);
                        Assert.AreEqual(releaseCalled, true);
                    }
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .On(TestEvents.LogicalDoubleThrowKeys[1])
                                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.History[0].Item1, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                Assert.AreEqual(s1.History[0].Item2, s0);

                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                var s2 = res1.NextState as TestStateN;
                Assert.AreEqual(s2.History.Count, 2);
                Assert.AreEqual(s2.History[1].Item1, TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                Assert.AreEqual(s2.History[1].Item2, s1);

                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                var s3 = res2.NextState as TestStateN;
                Assert.AreEqual(s3.History.Count, 3);
                Assert.AreEqual(s3.History[2].Item1, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                Assert.AreEqual(s3.History[2].Item2, s2);

                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                var s4 = res3.NextState as TestStateN;
                Assert.AreEqual(s4.History.Count, 4);
                Assert.AreEqual(s4.History[3].Item1, TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .On(TestEvents.LogicalDoubleThrowKeys[1])
                                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.AbnormalEndTriggers.Count, 0);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.AbnormalEndTriggers.Count, 1);
                Assert.AreEqual(s2.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent), true);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent), true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent), true);

                var s4 = res3.NextState as TestStateN;
                var res4 = s4.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res4.EventIsConsumed, true);
                Assert.AreEqual(res4.NextState is TestStateN, true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent), true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent), true);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .On(TestEvents.LogicalDoubleThrowKeys[1])
                                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.History.Count, 2);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.History.Count, 3);

                var (foundState, skippedReleaseEvents) = s3.FindStateFromHistory(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                Assert.AreEqual(foundState, s1);
                Assert.AreEqual(skippedReleaseEvents.Count, 2);
                Assert.AreEqual(skippedReleaseEvents[0], TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                Assert.AreEqual(skippedReleaseEvents[1], TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.NormalEndTrigger, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { })
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { })
                        .Do((ctx) => { })
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { })
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { })
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { })
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Do((ctx) => { })
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { })
                        .Do((ctx) => { })
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestState0, true);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetDoubleThrowElements(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetSingleThrowElements(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
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
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(StrokeDirection.Up)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                            .On(TestEvents.LogicalDoubleThrowKeys[1])
                                .On(TestEvents.LogicalDoubleThrowKeys[0])
                                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                            .On(TestEvents.LogicalDoubleThrowKeys[1])
                                .On(TestEvents.LogicalDoubleThrowKeys[0])
                                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                    Assert.AreEqual(res1.NextState is TestStateN, true);

                    var s2 = res1.NextState as TestStateN;
                    var result = s2.Reset();
                    Assert.AreEqual(s1, result);
                }
            }
        }
    }
}
