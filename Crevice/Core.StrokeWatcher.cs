using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Crevice.Core.Stroke
{
    public abstract class PointProcessor
    {
        public readonly int WatchInterval;

        private int lastTickCount;

        public PointProcessor(int watchInterval)
        {
            this.WatchInterval = watchInterval;
            this.lastTickCount = 0;
        }

        private bool MustBeProcessed(int currentTickCount)
        {
            if (WatchInterval == 0)
            {
                return true;
            }
            if (currentTickCount < lastTickCount || lastTickCount + WatchInterval < currentTickCount)
            {
                lastTickCount = currentTickCount;
                return true;
            }
            return false;
        }

        public bool Process(System.Drawing.Point point)
        {
            if (!MustBeProcessed(currentTickCount: Environment.TickCount))
            {
                return false;
            }
            OnProcess(point);
            return true;
        }

        internal abstract void OnProcess(System.Drawing.Point point);
    }

    public abstract class QueuedPointProcessor : PointProcessor
    {
        internal readonly BlockingCollection<System.Drawing.Point> queue = new BlockingCollection<System.Drawing.Point>();

        public QueuedPointProcessor(int watchInterval) : base(watchInterval) { }

        internal override void OnProcess(System.Drawing.Point point)
        {
            queue.Add(point);
        }
    }

    public class StrokeWatcher : QueuedPointProcessor, IDisposable
    {
        internal readonly TaskFactory taskFactory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;
                
        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly Task task;

        public StrokeWatcher(
            TaskFactory taskFactory,
            int initialStrokeThreshold,
            int strokeDirectionChangeThreshold, 
            int strokeExtensionThreshold,
            int watchInterval) : base(watchInterval)
        {
            this.taskFactory = taskFactory;
            this.initialStrokeThreshold = initialStrokeThreshold;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            this.task = Start();
        }
                
        public virtual void Queue(System.Drawing.Point point)
        {
            if (!IsDisposed)
            {
                Process(point);
            }
        }

        internal readonly List<System.Drawing.Point> buffer = new List<System.Drawing.Point>();

        private Task Start()
        {
            return taskFactory.StartNew(() =>
            {
                try
                {
                    foreach (var point in queue.GetConsumingEnumerable())
                    {
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
                                Verbose.Print("Stroke[0]: {0}", Enum.GetName(typeof(Def.Direction), stroke.Direction));
                                strokes.Add(stroke);
                            }
                        }
                        else
                        {
                            var stroke = strokes.Last();
                            var res = stroke.Input(buffer);
                            if (stroke != res)
                            {
                                Verbose.Print("Stroke[{0}]: {1}", strokes.Count, Enum.GetName(typeof(Def.Direction), res.Direction));
                                strokes.Add(res);
                            }
                        }
                    }
                }
                catch(OperationCanceledException) { }
            });
        }

        public Def.Stroke GetStorke()
        {
            return new Def.Stroke(strokes.Select(x => x.Direction));
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            IsDisposed = true;
            queue.CompleteAdding();
            Verbose.Print("StrokeWatcher(HashCode: 0x{0:X}) was released.", GetHashCode());
        }

        ~StrokeWatcher()
        {
            Dispose();
        }
    }
}
