using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Stroke
{
    using System.Drawing;

    public enum StrokeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public abstract class PointProcessor : IDisposable
    {
        private readonly System.Timers.Timer timer;

        private readonly int watchInterval;

        private int lastProcessedTickCount = 0;

        private object _lockObject = new object();

        private Point? skippedPoint = null;

        public PointProcessor(int watchInterval)
        {
            this.watchInterval = watchInterval;
            this.timer = new System.Timers.Timer(1);
            this.timer.AutoReset = true;
            this.timer.Elapsed += (sender, e) => 
            {
                lock (_lockObject)
                {
                    if (skippedPoint.HasValue)
                    {
                        OnProcess(skippedPoint.Value);
                        skippedPoint = null;
                    }
                }
            };
            this.timer.Start();
        }

        private bool MustBeProcessed(int currentTickCount) =>
            watchInterval <= 0 ||
            currentTickCount < lastProcessedTickCount ||
            lastProcessedTickCount + watchInterval < currentTickCount;

        public virtual void Process(Point point)
        {
            var current = Environment.TickCount;
            lock (_lockObject)
            {
                if (MustBeProcessed(currentTickCount: current))
                {
                    lastProcessedTickCount = current;
                    OnProcess(point);
                    skippedPoint = null;
                }
                else
                {
                    skippedPoint = point;
                }
            }
        }

        internal abstract void OnProcess(Point point);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Stop();
                timer.Dispose();
            }
        }

        ~PointProcessor() => Dispose(false);
    }
}
