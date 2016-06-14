using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.Stroke
{
    public abstract class PointProcessor
    {
        internal readonly int watchInterval;

        internal long lastProcessTime = 0;
        internal Point lastProcess;

        public PointProcessor(int watchInterval)
        {
            this.watchInterval = watchInterval;
        }

        internal bool MustBeProcessed(uint currentTime)
        {
            if (lastProcessTime + watchInterval < currentTime)
            {
                lastProcessTime = currentTime;
                return true;
            }
            else if (lastProcessTime > currentTime)
            {
                lastProcessTime = uint.MaxValue - lastProcessTime;
                return MustBeProcessed(currentTime);
            }
            return false;
        }
    }

    public class StrokeWatcher : PointProcessor, IDisposable
    {
        [DllImport("winmm.dll")]
        private static extern uint timeGetTime();
                
        internal readonly TaskFactory factory;
        internal readonly int initialStrokeThreshold;
        internal readonly int strokeDirectionChangeThreshold;
        internal readonly int strokeExtensionThreshold;
                
        internal readonly List<Stroke> strokes = new List<Stroke>();
        internal readonly BlockingCollection<Point> queue = new BlockingCollection<Point>();
        internal readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        internal readonly Task task;

        public StrokeWatcher(
            TaskFactory factory,
            int initialStrokeThreshold,
            int strokeDirectionChangeThreshold, 
            int strokeExtensionThreshold,
            int watchInterval) : base(watchInterval)
        {
            this.factory = factory;
            this.initialStrokeThreshold = initialStrokeThreshold;
            this.strokeDirectionChangeThreshold = strokeDirectionChangeThreshold;
            this.strokeExtensionThreshold = strokeExtensionThreshold;
            this.task = Start();
        }
                
        public void Queue(Point point)
        {
            if (MustBeProcessed(currentTime: timeGetTime()))
            {
                queue.Add(point);
            }
        }

        private readonly List<Point> buffer = new List<Point>();

        private Task Start()
        {
            return factory.StartNew(() =>
            {
                try
                {
                    foreach (var point in queue.GetConsumingEnumerable(tokenSource.Token))
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
                                Debug.Print("Stroke[0]: {0}", Enum.GetName(typeof(Def.Direction), stroke.Direction));
                                strokes.Add(stroke);
                            }
                        }
                        else
                        {
                            var stroke = strokes.Last();
                            var res = stroke.Input(buffer);
                            if (stroke != res)
                            {
                                Debug.Print("Stroke[{0}]: {1}", strokes.Count, Enum.GetName(typeof(Def.Direction), res.Direction));
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
                
        private async void AsyncDispose()
        {
            try
            {
                tokenSource.Cancel();
                await task;
            }
            catch (OperationCanceledException) { }
            finally
            {
                tokenSource.Dispose();
                queue.Dispose();
                Debug.Print("StrokeWatcher(0x{0:X}) was released", GetHashCode());
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            AsyncDispose();
        }

        ~StrokeWatcher()
        {
            Dispose();
        }
    }
}
