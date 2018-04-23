using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Stroke
{
    using System.Linq;
    using System.Drawing;
    using System.Threading.Tasks;

    using Crevice.Core.Callback;

    public class StrokeWatcher : QueuedPointProcessor, IDisposable
    {
        internal readonly IStrokeCallbackManager Callbacks;
        internal readonly TaskFactory taskFactory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly List<Point> buffer = new List<Point>();
        internal readonly Task task;

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
            this.task = CreateTask();
        }

        public virtual void Queue(Point point)
        {
            if (!_disposed)
            {
                Process(point);
            }
        }

        private Task CreateTask()
        {
            return taskFactory.StartNew(() =>
            {
                try
                {
                    foreach (var point in queue.GetConsumingEnumerable())
                    {
                        lock (_lockObject)
                        {
                            if (_disposed) break;

                            buffer.Add(point);
                            if (buffer.Count < 2)
                            {
                                continue;
                            }
                            if (strokes.Count == 0)
                            {
                                if (Stroke.CanCreate(initialStrokeThreshold, buffer.First(), buffer.Last()))
                                {
                                    var stroke = new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, buffer);
                                    strokes.Add(stroke);
                                    buffer.Clear();
                                    Callbacks.OnStrokeUpdated(this, GetStorkes());
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
                                    Callbacks.OnStrokeUpdated(this, GetStorkes());
                                }
                                else if (_strokePointsCount != stroke.Points.Count)
                                {
                                    buffer.Clear();
                                    Callbacks.OnStrokeUpdated(this, GetStorkes());
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
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

        public void Dispose()
        {
            lock (_lockObject)
            {
                _disposed = true;
                Dispose(true);
            }
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                queue.CompleteAdding();
            }
        }

        ~StrokeWatcher() => Dispose(false);
    }
}
