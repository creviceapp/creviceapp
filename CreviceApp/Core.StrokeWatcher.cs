using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.Stroke
{
    using WinAPI.WindowsHookEx;

    public abstract class PointProcessor
    {
        internal readonly int watchInterval;

        internal long lastProcessTime = 0;
        internal LowLevelMouseHook.POINT lastProcess;

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
        internal readonly BlockingCollection<LowLevelMouseHook.POINT> queue = new BlockingCollection<LowLevelMouseHook.POINT>();
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
                
        public void Queue(LowLevelMouseHook.POINT point)
        {
            if (MustBeProcessed(currentTime: timeGetTime()))
            {
                queue.Add(point);
            }
        }

        private readonly List<LowLevelMouseHook.POINT> buffer = new List<LowLevelMouseHook.POINT>();

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
                            var res = Stroke.Create(initialStrokeThreshold, strokeDirectionChangeThreshold, strokeExtensionThreshold, buffer);
                            if (res != null)
                            {
                                Debug.Print("Stroke[0]: {0}", Enum.GetName(typeof(Def.Direction), res.Direction));
                                strokes.Add(res);
                            }
                        }
                        else
                        {
                            var s = strokes.Last();
                            var res = s.Input(buffer);
                            if (s != res)
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
                Debug.Print("StrokeWatcher was released: {0}", GetHashCode());
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
