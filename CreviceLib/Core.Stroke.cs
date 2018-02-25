using System;
using System.Collections.Generic;
using System.Text;

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
            if (WatchInterval <= 0)
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
}
