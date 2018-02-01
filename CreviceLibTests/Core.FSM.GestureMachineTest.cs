using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Drawing;
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestState0 = Crevice.Core.FSM.State0<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestStateN = Crevice.Core.FSM.StateN<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    [TestClass]
    public class GestureMachineTest
    {
        class StrokeWatcherMock : StrokeWatcher
        {
            public StrokeWatcherMock() : base(Task.Factory, 0, 0, 0, 0) { }

            internal new readonly List<Point> queue = new List<Point>();
            public override void Queue(Point point)
            {
                queue.Add(point);
            }
        }

        [TestMethod]
        public void PypassGivenPointToStrokeWatcherWhenCurrentStateIsStateN()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, null);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, new Point(0, 0));
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    var strokeWatcherMock = new StrokeWatcherMock();
                    gm.StrokeWatcher = strokeWatcherMock;
                    Assert.AreEqual(strokeWatcherMock.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, null);
                    Assert.AreEqual(strokeWatcherMock.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, new Point(0, 0));
                    Assert.AreEqual(strokeWatcherMock.queue.Count, 1);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, new Point(1, 1));
                    Assert.AreEqual(strokeWatcherMock.queue.Count, 2);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalReleaseEventWhenItInIgnoreList()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                {
                    var result = gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result, false);
                }

                gm.invalidReleaseEvents.IgnoreNext(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);

                {
                    var result = gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(result, true);
                }

                {
                    var result = gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
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
                .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                    .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
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
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    Assert.IsTrue(s2 != gm.CurrentState);
                    var s3 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 2);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent], 1);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                            .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalSingleThrowKeys[0].FireEvent)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(StrokeDirection.Up)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Press((ctx) => { })
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Do((ctx) => { })
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                        .Release((ctx) => { })
                            .On(TestEvents.LogicalDoubleThrowKeys[1].PressEvent)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalSingleThrowKeys[0].FireEvent)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    //gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
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
                        .On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent)
                            .On(TestEvents.LogicalSingleThrowKeys[0].FireEvent)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, false);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
        }
    }
}
