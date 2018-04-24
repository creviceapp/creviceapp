using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Stroke
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Drawing;
    using System.Threading.Tasks;

    using Crevice.Core.Callback;

    public class StrokeWatcher : PointProcessor
    {
        internal readonly IStrokeCallbackManager Callbacks;
        internal readonly TaskFactory taskFactory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly List<Point> buffer = new List<Point>();

        internal readonly BlockingCollection<Point> queue
            = new BlockingCollection<Point>();

        internal override void OnProcess(Point point)
            => queue.Add(point);

        private readonly object _lockObject = new object();

        public StrokeWatcher(
            IStrokeCallbackManager callbacks,
            TaskFactory taskFactory,
            int initialStrokeThreshold,
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            int watchInterval) : base(watchInterval)
        {
            this.Callbacks = callbacks;
            this.taskFactory = taskFactory;
            this.initialStrokeThreshold = initialStrokeThreshold;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            StartBackgroundTask();
        }

        private void StartBackgroundTask() =>
            taskFactory.StartNew(() =>
            {
                try
                {
                    foreach (var point in queue.GetConsumingEnumerable())
                    {
                        lock (_lockObject)
                        {
                            if (_disposed) break;

                            buffer.Add(point);

                            if (buffer.Count < 2) continue;

                            if (strokes.Count == 0)
                            {
                                if (Stroke.CanCreate(initialStrokeThreshold, buffer.First(), buffer.Last()))
                                {
                                    var stroke = new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, buffer);
                                    strokes.Add(stroke);
                                    buffer.Clear();
                                }
                            }
                            else
                            {
                                var stroke = strokes.Last();
                                var _strokePointsCount = stroke.Points.Count;

                                var res = stroke.Input(buffer);
                                if (stroke != res)
                                {
                                    strokes.Add(res);
                                    buffer.Clear();
                                }
                                else if (_strokePointsCount != stroke.Points.Count)
                                {
                                    buffer.Clear();
                                }
                            }

                            if (StrokeIsEstablished)
                            {
                                Callbacks.OnStrokeUpdated(this, GetStorkes());
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });

        public bool StrokeIsEstablished => strokes.Any();

        public IReadOnlyList<Point> GetBufferedPoints()
        {
            lock (_lockObject)
            {
                return buffer.ToList();
            }
        }

        public IReadOnlyList<Stroke> GetStorkes()
        {
            lock (_lockObject)
            {
                return strokes.Select(s => s.Freeze()).ToList();
            }
        }

        public StrokeSequence GetStrokeSequence()
        {
            lock (_lockObject)
            {
                return new StrokeSequence(strokes.Select(x => x.Direction));
            }
        }

        internal bool _disposed { get; private set; } = false;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lockObject)
                {
                    _disposed = true;
                    queue.CompleteAdding();
                    base.Dispose(true);
                }
            }
        }

        ~StrokeWatcher() => Dispose(false);
    }
}
