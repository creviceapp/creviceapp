using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crevice.Core.FSM
{
    using UserActionContext;

    public class StateGlobal<E, X> 
        : IDisposable
        where E : UserActionEvaluationContext
        where X : UserActionExecutionContext
    {
        private readonly Threading.SingleThreadScheduler StrokeWatcherScheduler;
        private readonly Threading.SingleThreadScheduler LowPriorityScheduler;
        private readonly Threading.SingleThreadScheduler UserActionScheduler;
        public readonly TaskFactory StrokeWatcherTaskFactory;
        public readonly TaskFactory LowPriorityTaskFactory;
        public readonly TaskFactory UserActionTaskFactory;

        public readonly Config.UserConfig<E, X> Config;

        public readonly UserActionEvaluationContextFactory<E> UserActionEvaluationContextFactory 
            = new UserActionEvaluationContextFactory<E>();

        public readonly UserActionExecutionContextFactory<X> UserActionExecutionContextFactory
            = new UserActionExecutionContextFactory<X>();

        public readonly HashSet<Def.Event.IDoubleActionRelease> IgnoreNext = new HashSet<Def.Event.IDoubleActionRelease>();
                
        public Stroke.StrokeWatcher StrokeWatcher { get; internal set; }

        public StateGlobal() : this(new Config.UserConfig<E, X>())
        {

        }

        public StateGlobal(Config.UserConfig<E, X> userConfig)
        {
            this.StrokeWatcherScheduler = new Threading.SingleThreadScheduler(ThreadPriority.AboveNormal);
            this.LowPriorityScheduler = new Threading.SingleThreadScheduler(ThreadPriority.Lowest);
            this.UserActionScheduler = new Threading.SingleThreadScheduler();
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
