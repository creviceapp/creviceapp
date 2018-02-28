using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Linq;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.FSM;
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
                var history = History.Create(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent.Opposition, s0);
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
        public void On_Do_LogicalSingleThrow_Test()
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
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                        Assert.AreEqual((result.NextState as TestStateN).History, (s1 as TestStateN).History);
                        Assert.AreEqual((result.NextState as TestStateN).Ctx, (s1 as TestStateN).Ctx);
                        Assert.AreEqual((result.NextState as TestStateN).DoubleThrowElements, (s1 as TestStateN).DoubleThrowElements);
                        Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, true);
                    }
                    {
                        // Repeated start trigger.
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true); 
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, false);
                    }

                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                }
                {
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                        Assert.AreEqual((result.NextState as TestStateN).History, (s1 as TestStateN).History);
                        Assert.AreEqual((result.NextState as TestStateN).Ctx, (s1 as TestStateN).Ctx);
                        Assert.AreEqual((result.NextState as TestStateN).DoubleThrowElements, (s1 as TestStateN).DoubleThrowElements);
                        Assert.AreEqual((result.NextState as TestStateN).Depth == s1.Depth, true);
                        Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, true);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }

                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                }
            }
        }

        [TestMethod]
        public void On_Do_PhysicalSingleThrow_Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var doCalled = false;
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.PhysicalSingleThrowKeys0[0])
                    .Do((ctx) => { doCalled = true; });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                Assert.AreEqual(s1 is TestStateN, true);
                {
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                        Assert.AreEqual((result.NextState as TestStateN).History, (s1 as TestStateN).History);
                        Assert.AreEqual((result.NextState as TestStateN).Ctx, (s1 as TestStateN).Ctx);
                        Assert.AreEqual((result.NextState as TestStateN).DoubleThrowElements, (s1 as TestStateN).DoubleThrowElements);
                        Assert.AreEqual((result.NextState as TestStateN).Depth == s1.Depth, true);
                        Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, true);
                    }
                    { 
                        // Repeated start trigger.
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s0);
                        Assert.AreEqual(result.EventIsConsumed, true);
                        Assert.AreEqual(doCalled, false);
                    }

                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                }
                {
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }

                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                    {
                        doCalled = false;
                        var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                        Assert.AreEqual(result.NextState, s1);
                        Assert.AreEqual(result.EventIsConsumed, false);
                        Assert.AreEqual(doCalled, false);
                    }
                }

            }
        }
        
        [TestMethod]
        public void On_Press_LogicalDoubleThrow_Test()
        {
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
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
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
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void On_Press_PhysicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
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
                        .On(TestEvents.PhysicalDoubleThrowKeys0[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void On_Do_LogicalDoubleThrow_Test()
        {
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
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, true);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, true);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var doCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { doCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, true);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, true);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, true);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, true);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void On_Do_PhysicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var doCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                        .Do((ctx) => { doCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var doCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys0[1])
                        .Do((ctx) => { doCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, true);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(doCalled, true);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                    {
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }

                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                        {
                            doCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(doCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void On_Release_LogicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
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
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void On_Release_PhysicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
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
                        .On(TestEvents.PhysicalDoubleThrowKeys0[1])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth + 1);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            result = result.NextState.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s0, false);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual((result.NextState as TestStateN).Depth, s1.Depth);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false); // Should be false for the case when a button pressed before application to be started.
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void OnDecomposed_Press_LogicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
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
                        .OnDecomposed(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void OnDecomposed_Press_PhysicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var pressCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
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
                        .OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[1])
                        .Press((ctx) => { pressCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, true);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                    {
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }

                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                        {
                            pressCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(pressCalled, false);
                        }
                    }
                }
            }
        }
        
        [TestMethod]
        public void OnDecomposed_Release_LogicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
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
                        .OnDecomposed(TestEvents.LogicalDoubleThrowKeys[1])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void OnDecomposed_Release_PhysicalDoubleThrow_Test()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var releaseCalled = false;
                    root.When((ctx) => { return true; })
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            // Repeated start trigger.
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
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
                        .OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[1])
                        .Release((ctx) => { releaseCalled = true; });
                    var s0 = new TestState0(gm, root);
                    var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                    Assert.AreEqual(s1 is TestStateN, true);
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys0[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s0);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState == s1, false);
                            Assert.AreEqual(result.NextState.Depth == s1.Depth, true);
                            Assert.AreEqual((result.NextState as TestStateN).CanCancel, false);
                            Assert.AreEqual(result.EventIsConsumed, true);
                            Assert.AreEqual(releaseCalled, true);
                        }
                    }
                    {
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[0].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalSingleThrowKeys1[1].FireEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[0].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }

                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].PressEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                        {
                            releaseCalled = false;
                            var result = s1.Input(TestEvents.PhysicalDoubleThrowKeys1[1].ReleaseEvent);
                            Assert.AreEqual(result.NextState, s1);
                            Assert.AreEqual(result.EventIsConsumed, false);
                            Assert.AreEqual(releaseCalled, false);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void IgnoreRepeatedPressEvents()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var s1 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState;
                Assert.AreEqual(s1 == s0, false);
                var s2 = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent).NextState;
                Assert.AreEqual(s2 == s0, false);
                Assert.AreEqual(s2 == s1, false);
                {
                    var result = s2.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(result.NextState, s2);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s2.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                    Assert.AreEqual(result.NextState, s2);
                    Assert.AreEqual(result.EventIsConsumed, true);
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
                            .On(TestEvents.LogicalDoubleThrowKeys[2])
                                .On(TestEvents.LogicalDoubleThrowKeys[3])
                                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);

                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.Depth, 1);
                Assert.AreEqual(s1.History.Records.Count, 1);
                Assert.AreEqual(s1.History.Records[0].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(s1.History.Records[0].State, s0);

                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                var s2 = res1.NextState as TestStateN;
                Assert.AreEqual(s2.Depth, 2);
                Assert.AreEqual(s2.History.Records.Count, 2);
                Assert.AreEqual(s2.History.Records[0].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(s2.History.Records[1].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                Assert.AreEqual(s2.History.Records[1].State, s1);

                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys0[2].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                var s3 = res2.NextState as TestStateN;
                Assert.AreEqual(s3.Depth, 3);
                Assert.AreEqual(s3.History.Records.Count, 3);
                Assert.AreEqual(s3.History.Records[0].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(s3.History.Records[1].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                Assert.AreEqual(s3.History.Records[2].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent);
                Assert.AreEqual(s3.History.Records[2].State, s2);

                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys0[3].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                var s4 = res3.NextState as TestStateN;
                Assert.AreEqual(s4.Depth, 4);
                Assert.AreEqual(s4.History.Records.Count, 4);
                Assert.AreEqual(s4.History.Records[0].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(s4.History.Records[1].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                Assert.AreEqual(s4.History.Records[2].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent);
                Assert.AreEqual(s4.History.Records[3].ReleaseEvent, TestEvents.PhysicalDoubleThrowKeys0[3].ReleaseEvent);
                Assert.AreEqual(s4.History.Records[3].State, s3);
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
                            .On(TestEvents.LogicalDoubleThrowKeys[2])
                                .On(TestEvents.LogicalDoubleThrowKeys[3])
                                    .On(TestEvents.LogicalDoubleThrowKeys[4])
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState.Depth, 1);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState.Depth, 2);
                Assert.AreEqual(s1.AbnormalEndTriggers.Count, 0);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys0[2].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState.Depth, 3);
                Assert.AreEqual(s2.AbnormalEndTriggers.Count, 1);
                Assert.AreEqual(s2.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent), true);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys0[3].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState.Depth, 4);
                Assert.AreEqual(s3.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent), true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent), true);

                var s4 = res3.NextState as TestStateN;
                var res4 = s4.Input(TestEvents.PhysicalDoubleThrowKeys0[4].PressEvent);
                Assert.AreEqual(res4.EventIsConsumed, true);
                Assert.AreEqual(res4.NextState.Depth, 5);
                Assert.AreEqual(s4.AbnormalEndTriggers.Count, 3);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent), true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent), true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent), true);
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
                            .On(TestEvents.LogicalDoubleThrowKeys[2])
                                .On(TestEvents.LogicalDoubleThrowKeys[3])
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(s1.Depth, 1);
                Assert.AreEqual(s1.History.Records.Count, 1);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.PhysicalDoubleThrowKeys0[2].PressEvent);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.Depth, 2);
                Assert.AreEqual(s2.History.Records.Count, 2);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.PhysicalDoubleThrowKeys0[3].PressEvent);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.Depth, 3);
                Assert.AreEqual(s3.History.Records.Count, 3);

                var queryResult = s3.History.Query(TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                Assert.AreEqual(queryResult.FoundState, s1);
                Assert.AreEqual(queryResult.SkippedReleaseEvents.Count, 2);
                Assert.AreEqual(queryResult.SkippedReleaseEvents[0], TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent);
                Assert.AreEqual(queryResult.SkippedReleaseEvents[1], TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent);
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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Records.Count, 1);
                Assert.AreEqual(s1.NormalEndTrigger, TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Records.Count, 1);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
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
                    var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);
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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.Depth, 1);
                var result = s1.GetDoubleThrowElementsByTrigger(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].DoubleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetDecomposedElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .OnDecomposed(TestEvents.LogicalDoubleThrowKeys[1])
                        .Press((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.Depth, 1);
                var result = s1.GetDecomposedElementsByTrigger(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].DecomposedElements[0]);
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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetSingleThrowElementsByTrigger(TestEvents.PhysicalSingleThrowKeys0[0].FireEvent);
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
                var res0 = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetStrokeElementsByTrigger(new StrokeSequence() { StrokeDirection.Up });
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].StrokeElements[0]);
            }
        }

        [TestMethod]
        public void IsStateTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var s0 = new TestState0(gm, root);
                var history = History.Create(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent.Opposition, s0);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                var s1 = new TestStateN(gm, evalContext, history, dt, depth: 0);
                Assert.AreEqual(s1.IsState0, false);
                Assert.AreEqual(s1.IsStateN, true);
            }
        }

        [TestMethod]
        public void ToState0Test()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var s0 = new TestState0(gm, root);
                var history = History.Create(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent.Opposition, s0);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                var s1 = new TestStateN(gm, evalContext, history, dt, depth: 0);
                Assert.AreEqual(s1.AsState0() is null, true);
            }
        }

        [TestMethod]
        public void ToStateNTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var s0 = new TestState0(gm, root);
                var history = History.Create(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent.Opposition, s0);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                var s1 = new TestStateN(gm, evalContext, history, dt, depth: 0);
                Assert.AreEqual(s1.AsStateN() is TestStateN, true);
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            {
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 1);
                        var result = state.Reset();
                        Assert.AreEqual(result, s0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .On(TestEvents.LogicalDoubleThrowKeys[1])
                                    .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 2);
                        var result = state.Reset();
                        Assert.AreEqual(result == s0, false);
                        Assert.AreEqual(result.Depth, 1);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.LogicalDoubleThrowKeys[0])
                                .On(TestEvents.LogicalDoubleThrowKeys[1])
                                    .On(TestEvents.LogicalDoubleThrowKeys[2])
                                        .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[2].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 3);
                        var result = state.Reset();
                        Assert.AreEqual(result == s0, false);
                        Assert.AreEqual(result.Depth, 2);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
            {
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                                .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 1);
                        var result = state.Reset();
                        Assert.AreEqual(result, s0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                                .On(TestEvents.PhysicalDoubleThrowKeys0[1])
                                    .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 2);
                        var result = state.Reset();
                        Assert.AreEqual(result == s0, false);
                        Assert.AreEqual(result.Depth, 1);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
                {
                    var releaseCalled = false;
                    var root = new TestRootElement();
                    using (var gm = new TestGestureMachine(root))
                    {
                        var when = root.When((ctx) => { return true; });
                        when
                            .On(TestEvents.PhysicalDoubleThrowKeys0[0])
                                .On(TestEvents.PhysicalDoubleThrowKeys0[1])
                                    .On(TestEvents.PhysicalDoubleThrowKeys0[2])
                                        .Release((ctx) => { releaseCalled = true; });
                        var s0 = new TestState0(gm, root);
                        var state = s0.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[1].PressEvent).NextState.
                                    Input(TestEvents.PhysicalDoubleThrowKeys0[2].PressEvent).NextState as TestStateN;
                        Assert.AreEqual(state.Depth, 3);
                        var result = state.Reset();
                        Assert.AreEqual(result == s0, false);
                        Assert.AreEqual(result.Depth, 2);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[1].ReleaseEvent], 0);
                        Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys0[2].ReleaseEvent], 1);
                        Assert.AreEqual(releaseCalled, true);
                    }
                }
            }
        }
    }
}
