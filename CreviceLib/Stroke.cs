using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice
{
    using Crevice.Core;

    public class Stroke
    {
        public readonly StrokeEvent.Direction Direction;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        private readonly List<System.Drawing.Point> points = new List<System.Drawing.Point>();

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            StrokeEvent.Direction dir)
        {
            this.Direction = dir;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
        }

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            List<System.Drawing.Point> input)
        {
            this.Direction = NextDirection(GetAngle(input.First(), input.Last()));
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
        }

        public virtual Stroke Input(List<System.Drawing.Point> input)
        {
            var p0 = input.First();
            var p1 = input.Last();
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            var angle = GetAngle(p0, p1);
            if (dx > strokeDirectionChangeThreshold || dy > strokeDirectionChangeThreshold)
            {
                var dir = NextDirection(angle);
                if (IsSameDirection(dir))
                {
                    Absorb(input);
                    return this;
                }
                var stroke = CreateNew(dir);
                stroke.Absorb(input);
                return stroke;
            }

            if (dx > strokeExtensionThreshold || dy > strokeExtensionThreshold)
            {
                if (IsExtensionable(angle))
                {
                    Absorb(input);
                }
            }
            return this;
        }

        public void Absorb(List<System.Drawing.Point> points)
        {
            this.points.AddRange(points);
            points.Clear();
        }

        private static StrokeEvent.Direction NextDirection(double angle)
        {
            if (-135 <= angle && angle < -45)
            {
                return StrokeEvent.Direction.Up;
            }
            else if (-45 <= angle && angle < 45)
            {
                return StrokeEvent.Direction.Right;
            }
            else if (45 <= angle && angle < 135)
            {
                return StrokeEvent.Direction.Down;
            }
            else // if (135 <= angle || angle < -135)
            {
                return StrokeEvent.Direction.Left;
            }
        }

        private bool IsSameDirection(StrokeEvent.Direction dir)
        {
            return dir == Direction;
        }

        private bool IsExtensionable(double angle)
        {
            return Direction == NextDirection(angle);
        }

        private Stroke CreateNew(StrokeEvent.Direction dir)
        {
            return new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, dir);
        }

        public static bool CanCreate(int initialStrokeThreshold, System.Drawing.Point p0, System.Drawing.Point p1)
        {
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                return true;
            }
            return false;
        }

        private static double GetAngle(System.Drawing.Point p0, System.Drawing.Point p1)
        {
            return Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) * 180 / Math.PI;
        }
    }

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
        internal readonly System.Collections.Concurrent.BlockingCollection<System.Drawing.Point> queue
            = new System.Collections.Concurrent.BlockingCollection<System.Drawing.Point>();

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
                                // OnStroke~
                                var stroke = new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, buffer);
                                // Verbose.Print("Stroke[0]: {0}", Enum.GetName(typeof(StrokeEvent.Direction), stroke.Direction));
                                strokes.Add(stroke);
                            }
                        }
                        else
                        {
                            var stroke = strokes.Last();
                            var res = stroke.Input(buffer);
                            if (stroke != res)
                            {
                                // OnStroke~
                                // Verbose.Print("Stroke[{0}]: {1}", strokes.Count, Enum.GetName(typeof(StrokeEvent.Direction), res.Direction));
                                strokes.Add(res);
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public IReadOnlyList<StrokeEvent.Direction> GetStorkes()
            => strokes.Select(x => x.Direction).ToList();

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            IsDisposed = true;
            queue.CompleteAdding();
            // Verbose.Print("StrokeWatcher(HashCode: 0x{0:X}) was released.", GetHashCode());
        }

        ~StrokeWatcher()
        {
            Dispose();
        }
    }
}
