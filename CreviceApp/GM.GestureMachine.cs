using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Threading;
    using System.Drawing;
    using Crevice.Core.FSM;
    using Crevice.Core.Events;
    using Crevice.Threading;
    using Crevice.UserScript.Keys;

    public class GestureMachine : GestureMachine<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {
        private readonly CallbackManager customCallbackManager;

        public GestureMachine(
            GestureMachineConfig gestureMachineConfig,
            CallbackManager callbackManager)
            : base(gestureMachineConfig, callbackManager, new ContextManager())
        {
            customCallbackManager = callbackManager;
        }

        private readonly LowLatencyScheduler _strokeWatcherScheduler = 
            new LowLatencyScheduler("StrokeWatcherTaskScheduler", ThreadPriority.Highest, 2);
        
        protected override TaskFactory StrokeWatcherTaskFactory 
            => new TaskFactory(_strokeWatcherScheduler);

        public override bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (lockObject)
            {
                if (point.HasValue)
                {
                    ContextManager.CursorPosition = point.Value;
                }

                if(evnt is PhysicalPressEvent pressEvent)
                {
                    var key = (pressEvent.PhysicalKey as PhysicalSystemKey);
                    if (key.IsKeyboardKey && customCallbackManager.TimeoutKeyboardKeys.Contains(key))
                    {
                        return false;
                    }
                }
                else if(evnt is PhysicalReleaseEvent releaseEvent)
                {
                    var key = (releaseEvent.PhysicalKey as PhysicalSystemKey);
                    if (key.IsKeyboardKey && customCallbackManager.TimeoutKeyboardKeys.Contains(key))
                    {
                        customCallbackManager.TimeoutKeyboardKeys.Remove(key);
                        return false;
                    }
                }

                return base.Input(evnt, point);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ContextManager.Dispose();
                _strokeWatcherScheduler.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
