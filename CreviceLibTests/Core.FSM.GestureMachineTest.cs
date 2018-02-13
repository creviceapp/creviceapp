using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Threading;
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
            public StrokeWatcherMock(TestGestureMachine gm) : base(gm.CallbackManager, Task.Factory, 0, 0, 0, 0) { }

            internal new readonly List<Point> queue = new List<Point>();
            public override void Queue(Point point)
            {
                queue.Add(point);
            }
        }

        [TestMethod]
        public void ConstructorTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine())
                {
                    Assert.AreEqual(gm.IsRunning, false);
                    gm.Run(root);
                    Assert.AreEqual(gm.IsRunning, true);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                using (var gm = new TestGestureMachine(root))
                {
                    Assert.AreEqual(gm.IsRunning, true);
                }
            }
        }

        [TestMethod]
        public void RunTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine())
                {
                    Assert.AreEqual(gm.IsRunning, false);
                    gm.Run(root);
                    Assert.AreEqual(gm.IsRunning, true);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    Assert.AreEqual(gm.IsRunning, true);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RunThrowsInvalidOperationExceptionWhenCalledTwiceTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine())
            {
                gm.Run(root);
                gm.Run(root);
            }
        }

        [TestMethod]
        public void PypassesGivenPointToStrokeWatcherWhenCurrentStateIsStateN()
        {
            var root = new TestRootElement();
            root.When((ctx) => { return true; })
                .On(TestEvents.LogicalDoubleThrowKeys[0])
                .Do((ctx) => { });
            using (var gm = new TestGestureMachine(root))
            {
                {
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, null);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent, new Point(0, 0));
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.StrokeWatcher.queue.Count, 0);

                    var strokeWatcherMock = new StrokeWatcherMock(gm);
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
        public void ConsumesGivenPhysicalReleaseEventWhenItInIgnoreList()
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

                gm.invalidEvents.IgnoreNext(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);

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
        public void ConsumesNullEventAndReturnsFalse()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var result = gm.Input(new Crevice.Core.Events.NullEvent());
                Assert.AreEqual(result, false);
            }
        }

        [TestMethod]
        public void WhenEvaluatorTest()
        {
            {
                using(var cde = new CountdownEvent(1))
                {
                    var root = new TestRootElement();
                    root.When(ctx => { return false; })
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                        .Do(ctx => { cde.Signal(); });
                    using (var gm = new TestGestureMachine(root))
                    {
                        var result = gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result, false);
                        Assert.AreEqual(cde.Wait(100), false);
                    }
                }
            }
            {
                using (var cde = new CountdownEvent(1))
                {
                    var root = new TestRootElement();
                    root.When(ctx => { return true; })
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                        .Do(ctx => { cde.Signal(); });
                    using (var gm = new TestGestureMachine(root))
                    {
                        var result = gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                        Assert.AreEqual(result, true);
                        Assert.AreEqual(cde.Wait(100), true);
                    }
                }
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            var root = new TestRootElement();
            var when = root.When((ctx) => { return true; });
            when
                .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .On(TestEvents.LogicalDoubleThrowKeys[0])
                            .On(TestEvents.LogicalDoubleThrowKeys[1])
                            .Do((ctx) => { });
            {
                var callback = new TestCallbackManager(enableMachineResetCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    var s0 = gm.CurrentState;
                    Assert.AreEqual(callback.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(callback.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(callback.OnMachineResetCallCount, 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                var callback = new TestCallbackManager(enableMachineResetCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    Assert.AreEqual(callback.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(callback.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(callback.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                var callback = new TestCallbackManager(enableMachineResetCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    Assert.AreEqual(callback.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(callback.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(callback.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 1);
                    Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                var callback = new TestCallbackManager(enableMachineResetCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
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
                    Assert.AreEqual(callback.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(callback.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(callback.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent], 2);
                    Assert.AreEqual(gm.invalidEvents[TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
        }

        [TestMethod]
        public void OnStateChangedTest()
        {
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var callback = new TestCallbackManager(enableStateChangedCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 1);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 2);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 3);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableStateChangedCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 1);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 2);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[1].PressEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 3);

                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[1].ReleaseEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 4);
                    
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(callback.OnStateChangedCDE.Wait(1000), true);
                    callback.OnStateChangedCDE.Reset();
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnStateChangedCallCount, 5);
                }
            }
        }

        [TestMethod]
        public void OnGestureCancelledTest()
        {
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureCancelledCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(callback.OnGestureCancelledCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureCancelledCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
                    Assert.AreEqual(callback.OnGestureCancelledCDE.Wait(1000), true);
                    Assert.AreEqual(callback.OnGestureCancelledCallCount, 1);
                }
            }
        }

        [TestMethod]
        public void OnGestureTimeoutTest()
        {
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(500), false);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(5000), true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(5000), true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(StrokeDirection.Up)
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(5000), true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Press((ctx) => { })
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(500), false);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Do((ctx) => { })
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(500), false);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .Release((ctx) => { })
                        .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(500), false);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                            .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    //gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(5000), true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                root.When((ctx) => { return true; })
                    .On(TestEvents.LogicalDoubleThrowKeys[0])
                        .On(TestEvents.LogicalSingleThrowKeys[0])
                            .Do((ctx) => { });
                var callback = new TestCallbackManager(enableGestureTimeoutCallback: true);
                using (var gm = new TestGestureMachine(root, callback))
                {
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    gm.Input(TestEvents.PhysicalSingleThrowKeys[0].FireEvent);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, false);
                    Assert.AreEqual(callback.OnGestureTimeoutCDE.Wait(500), false);
                    Assert.AreEqual(callback.OnGestureTimeoutCallCount, 0);
                }
            }
        }
    }
}
