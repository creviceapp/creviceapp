﻿using System;
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

    public class NullGestureMachine : GestureMachine
    {
        public NullGestureMachine()
            : base(new GestureMachineConfig(), new CallbackManager())
        { }
    }

    public class GestureMachine : GestureMachine<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {
        public GestureMachine(
            GestureMachineConfig gestureMachineConfig,
            CallbackManager callbackManager)
            : base(gestureMachineConfig, callbackManager, new ContextManager())
        { }
        
        private readonly TaskFactory _strokeWatcherTaskFactory
            = LowLatencyScheduler.CreateTaskFactory(
                "StrokeWatcherTaskScheduler", 
                ThreadPriority.AboveNormal, 
                1);
        protected override TaskFactory StrokeWatcherTaskFactory 
            => _strokeWatcherTaskFactory;

        public override bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (lockObject)
            {
                if (point.HasValue)
                {
                    ContextManager.CursorPosition = point.Value;
                }
                return base.Input(evnt, point);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ContextManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
