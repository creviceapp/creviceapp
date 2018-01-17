using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("CreviceTests")]

namespace Crevice.Core.FSM
{

    /*
    public class ExternalInterface<T>
        where T : ActionContext
    {
        public readonly ActionContextFactory<T> Factory = new ActionContextFactory<T>();
    }
    */

    public class StateGlobal
        : IDisposable
    {
        public readonly TaskFactory StrokeWatcherTaskFactory;
        public readonly TaskFactory LowPriorityTaskFactory;
        public readonly TaskFactory UserActionTaskFactory;

        public readonly GestureMachineConfig Config;
        public readonly ActionContextFactory<ActionContext> ActionContextFactory;

        public readonly HashSet<Def.Event.IDoubleActionRelease> IgnoreNext = new HashSet<Def.Event.IDoubleActionRelease>();

        public Stroke.StrokeWatcher StrokeWatcher { get; internal set; }

        public StateGlobal(
            GestureMachineConfig config,
            ActionContextFactory<ActionContext> actionContextFactory,
            TaskFactory strokeWatcherTaskFactory,
            TaskFactory userActionTaskFactory,
            TaskFactory lowPriorityTaskFactory)
        {
            this.Config = config;
            this.ActionContextFactory = actionContextFactory;
            this.StrokeWatcherTaskFactory = strokeWatcherTaskFactory;
            this.LowPriorityTaskFactory = lowPriorityTaskFactory;
            this.UserActionTaskFactory = userActionTaskFactory;
            ResetStrokeWatcher();
        }

        public StateGlobal(
            GestureMachineConfig config,
            ActionContextFactory<ActionContext> actionContextFactory) : this(
                config, 
                actionContextFactory,
                Task.Factory,
                Task.Factory,
                Task.Factory
                )
        { }
        
        private Stroke.StrokeWatcher NewStrokeWatcher()
        {
            return new Stroke.StrokeWatcher(
                StrokeWatcherTaskFactory,
                Config.StrokeStartThreshold,
                Config.StrokeDirectionChangeThreshold,
                Config.StrokeExtensionThreshold,
                Config.StrokeWatchInterval);
        }

        public void ResetStrokeWatcher()
        {
            var _StrokeWatcher = StrokeWatcher;
            StrokeWatcher = NewStrokeWatcher();
            if (_StrokeWatcher != null)
            {
                Verbose.Print("StrokeWatcher was reset; {0} -> {1}", _StrokeWatcher.GetHashCode(), StrokeWatcher.GetHashCode());
                LowPriorityTaskFactory.StartNew(() => {
                    _StrokeWatcher.Dispose();
                });
            }
        }
                
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            StrokeWatcher.Dispose();
        }

        ~StateGlobal()
        {
            Dispose();
        }
    }
}
