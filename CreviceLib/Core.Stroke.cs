﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Core.Stroke
{
    using System.Collections.Concurrent;
    using System.Drawing;

    public enum StrokeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Stroke
    {

        public readonly StrokeDirection Direction;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        private readonly List<Point> points = new List<Point>();

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            StrokeDirection direction)
        {
            this.Direction = direction;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
        }

        public Stroke(
            int strokeDirectionChangeThreshold,
            int strokeExtensionThreshold,
            List<Point> input)
        {
            this.Direction = NextDirection(GetAngle(input.First(), input.Last()));
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            Absorb(input);
        }

        public virtual Stroke Input(List<Point> input)
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

        public void Absorb(IReadOnlyList<Point> points)
            => this.points.AddRange(points);

        private static StrokeDirection NextDirection(double angle)
        {
            if (-135 <= angle && angle < -45)
            {
                return StrokeDirection.Up;
            }
            else if (-45 <= angle && angle < 45)
            {
                return StrokeDirection.Right;
            }
            else if (45 <= angle && angle < 135)
            {
                return StrokeDirection.Down;
            }
            else // if (135 <= angle || angle < -135)
            {
                return StrokeDirection.Left;
            }
        }

        private bool IsSameDirection(StrokeDirection dir)
            => dir == Direction;

        private bool IsExtensionable(double angle)
            => Direction == NextDirection(angle);

        private Stroke CreateNew(StrokeDirection dir)
            => new Stroke(strokeDirectionChangeThreshold, strokeExtensionThreshold, dir);

        public static bool CanCreate(int initialStrokeThreshold, Point p0, Point p1)
        {
            var dx = Math.Abs(p0.X - p1.X);
            var dy = Math.Abs(p0.Y - p1.Y);
            if (dx > initialStrokeThreshold || dy > initialStrokeThreshold)
            {
                return true;
            }
            return false;
        }

        private static double GetAngle(Point p0, Point p1)
            => Math.Atan2(p1.Y - p0.Y, p1.X - p0.X) * 180 / Math.PI;
    }

    public abstract class PointProcessor
    {
        public readonly int WatchInterval;

        private int lastTickCount = 0;

        public PointProcessor(int watchInterval)
        {
            this.WatchInterval = watchInterval;
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

        public bool Process(Point point)
        {
            if (!MustBeProcessed(currentTickCount: Environment.TickCount))
            {
                return false;
            }
            OnProcess(point);
            return true;
        }

        internal abstract void OnProcess(Point point);
    }

    public abstract class QueuedPointProcessor : PointProcessor
    {
        internal readonly BlockingCollection<Point> queue
            = new BlockingCollection<Point>();

        public QueuedPointProcessor(int watchInterval) : base(watchInterval) { }

        internal override void OnProcess(Point point)
            => queue.Add(point);
    }

    public class StrokeWatcher : QueuedPointProcessor, IDisposable
    {
        internal readonly TaskFactory taskFactory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;

        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly List<Point> buffer = new List<Point>();
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
            this.task = CreateTask();
        }

        public virtual void Queue(Point point)
        {
            if (!IsDisposed)
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
                                // Verbose.Print("Stroke[0]: {0}", Enum.GetName(typeof(StrokeDirection), stroke.Direction));
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
                                // Verbose.Print("Stroke[{0}]: {1}", strokes.Count, Enum.GetName(typeof(StrokeDirection), res.Direction));
                                strokes.Add(res);
                            }
                            buffer.Clear();
                        }
                    }
                }
                catch (OperationCanceledException) { }
            });
        }

        public IReadOnlyList<StrokeDirection> GetStorkes()
            => strokes.Select(x => x.Direction).ToList();

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            IsDisposed = true;
            queue.CompleteAdding();
            // Verbose.Print("StrokeWatcher(HashCode: 0x{0:X}) was released.", GetHashCode());
        }

        ~StrokeWatcher() => Dispose();
    }
}