using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Linq;
    using System.Drawing;
    using Crevice.Core.Events;
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    
    [TestClass]
    public class StrokeTest
    {
        [TestMethod]
        public void StrokeResetTest()
        {
            var root = new TestRootElement();
            root.When((ctx) => { return true; })
                .On(TestEvents.LogicalDoubleThrowKeys[0])
                    .On(TestEvents.LogicalDoubleThrowKeys[1])
                        .Do((ctx) => { });
            var callback = new TestCallbackManager(
                enableStrokeResetCallback: true,
                enableStrokeUpdatedCallback: true,
                enableStateChangedCallback: true);
            using (var gm = new TestGestureMachine(root, callback))
            {
                gm.Config.StrokeWatchInterval = 0;
                gm.Config.GestureTimeout = 0;

                Assert.AreEqual(callback.OnStrokeResetCDE.Wait(10000), true);
                callback.OnStrokeResetCDE.Reset();

                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();

                gm.Input(TestEvents.PhysicalDoubleThrowKeys0[0].PressEvent);

                Assert.AreEqual(callback.OnStrokeResetCDE.Wait(10000), true);
                callback.OnStrokeResetCDE.Reset();

                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();

                // move
                gm.Input(new NullEvent(), new Point(100, 100));
                gm.Input(new NullEvent(), new Point(100, 150));
                Assert.AreEqual(callback.OnStrokeUpdatedCDE.Wait(10000), true);
                callback.OnStrokeUpdatedCDE.Reset();

                // move
                gm.Input(new NullEvent(), new Point(100, 150));
                gm.Input(new NullEvent(), new Point(150, 150));
                Assert.AreEqual(callback.OnStrokeUpdatedCDE.Wait(10000), true);
                callback.OnStrokeUpdatedCDE.Reset();

                var strokeSequence = gm.StrokeWatcher.GetStrokeSequence();
                Assert.AreEqual(strokeSequence, new StrokeSequence { StrokeDirection.Down, StrokeDirection.Right });

                gm.Input(TestEvents.PhysicalDoubleThrowKeys0[0].ReleaseEvent);
                Assert.AreEqual(callback.OnStateChangedCDE.Wait(10000), true);
                callback.OnStateChangedCDE.Reset();
            }
        }

        [TestMethod]
        public void ZeroIntervalPointProcessorDoesNotStartBackgroundTask()
        {
            var scheduler = new CountingTaskScheduler();
            using (new RecordingPointProcessor(new TaskFactory(scheduler), 0))
            {
                Assert.AreEqual(0, scheduler.QueuedTaskCount);
            }
        }

        [TestMethod]
        public void ZeroIntervalPointProcessorProcessesPointsSynchronously()
        {
            using (var processor = new RecordingPointProcessor(Task.Factory, 0))
            {
                var point = new Point(12, 34);

                processor.Process(point);

                Assert.AreEqual(1, processor.ProcessedPoints.Count);
                Assert.AreEqual(point, processor.ProcessedPoints[0]);
            }
        }

        [TestMethod]
        public void ZeroIntervalPointProcessorDisposeCanBeCalledRepeatedly()
        {
            var processor = new RecordingPointProcessor(Task.Factory, 0);

            processor.Dispose();
            processor.Dispose();
        }

        private sealed class RecordingPointProcessor : PointProcessor
        {
            public readonly List<Point> ProcessedPoints = new List<Point>();

            public RecordingPointProcessor(TaskFactory taskFactory, int watchInterval)
                : base(taskFactory, watchInterval)
            { }

            internal override void OnProcess(Point point)
            {
                ProcessedPoints.Add(point);
            }
        }

        private sealed class CountingTaskScheduler : TaskScheduler
        {
            private int queuedTaskCount;

            public int QueuedTaskCount => queuedTaskCount;

            protected override IEnumerable<Task> GetScheduledTasks()
                => Enumerable.Empty<Task>();

            protected override void QueueTask(Task task)
            {
                System.Threading.Interlocked.Increment(ref queuedTaskCount);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
                => false;
        }
    }
}
