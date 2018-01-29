using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestState0 = Crevice.Core.FSM.State0<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestStateN = Crevice.Core.FSM.StateN<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    [TestClass]
    public class GestureMachineTest
    {
        [TestMethod]
        public void ConsumeGivenPhysicalReleaseEventWhenItInIgnoreList()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, false);
                }

                gm.invalidReleaseEvents.IgnoreNext(TestEvents.Physical.TestPhysicalReleaseEventA);

                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, true);
                }

                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, false);
                }
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            var root = new TestRootElement();
            var when = root.When((ctx) => { return true; });
            when
                .On(TestEvents.Logical.TestPressEventA)
                    .On(TestEvents.Logical.TestPressEventB)
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventB], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    Assert.IsTrue(s2 != gm.CurrentState);
                    var s3 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 2);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventB], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
        }

        [TestMethod]
        public void OnGestureCancelledTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
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
                            .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(gm.OnGestureCancelledCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 1);
                }
            }
        }

        [TestMethod]
        public void OnGestureTimeoutTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
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
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(StrokeDirection.Up)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Release((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    //gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, false);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
        }
    }
}
