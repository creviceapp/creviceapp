using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public class StateGlobal : IDisposable
    {
        private readonly SingleThreadScheduler StrokeWatcherScheduler;
        private readonly SingleThreadScheduler LowPriorityScheduler;
        private readonly SingleThreadScheduler UserActionScheduler;
        public readonly TaskFactory StrokeWatcherTaskFactory;
        public readonly TaskFactory LowPriorityTaskFactory;
        public readonly TaskFactory UserActionTaskFactory;

        public readonly Config.UserConfig Config;

        public readonly HashSet<Def.Event.IDoubleActionRelease> IgnoreNext = new HashSet<Def.Event.IDoubleActionRelease>();
                
        public Stroke.StrokeWatcher StrokeWatcher { get; internal set; }

        public StateGlobal() : this(new Config.UserConfig())
        {

        }

        public StateGlobal(Config.UserConfig userConfig)
        {
            this.StrokeWatcherScheduler = new SingleThreadScheduler(ThreadPriority.AboveNormal);
            this.LowPriorityScheduler = new SingleThreadScheduler(ThreadPriority.Lowest);
            this.UserActionScheduler = new SingleThreadScheduler();
            this.StrokeWatcherTaskFactory = new TaskFactory(StrokeWatcherScheduler);
            this.LowPriorityTaskFactory = new TaskFactory(LowPriorityScheduler);
            this.UserActionTaskFactory = new TaskFactory(UserActionScheduler);
            this.Config = userConfig;
            this.StrokeWatcher = NewStrokeWatcher();
        }

        private Stroke.StrokeWatcher NewStrokeWatcher()
        {
            return new Stroke.StrokeWatcher(
                StrokeWatcherTaskFactory,
                Config.Gesture.InitialStrokeThreshold,
                Config.Gesture.StrokeDirectionChangeThreshold,
                Config.Gesture.StrokeExtensionThreshold,
                Config.Gesture.WatchInterval);
        }

        public void ResetStrokeWatcher()
        {
            var _StrokeWatcher = StrokeWatcher;
            StrokeWatcher = NewStrokeWatcher();
            Verbose.Print("StrokeWatcher was reset; {0} -> {1}", _StrokeWatcher.GetHashCode(), StrokeWatcher.GetHashCode());
            LowPriorityTaskFactory.StartNew(() => {
                _StrokeWatcher.Dispose();
            });
        }
                
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            StrokeWatcher.Dispose();
            StrokeWatcherScheduler.Dispose();
            LowPriorityScheduler.Dispose();
            UserActionScheduler.Dispose();
        }

        ~StateGlobal()
        {
            Dispose();
        }
    }
}
