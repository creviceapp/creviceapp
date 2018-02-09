using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Drawing;
    using Crevice.Core.FSM;
    using Crevice.Core.Events;
    using Crevice.DSL;

    public class NullGestureMachine : GestureMachine
    {
        public NullGestureMachine() : base(new RootElement()) { }
    }
    
    public class GestureMachine : GestureMachine<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {

        public GestureMachine()
            : base(new GestureMachineConfig(), new CallbackManager(), new ContextManager())
        { }

        public GestureMachine(
            GestureMachineConfig gestureMachineConfig)
            : base(gestureMachineConfig, new CallbackManager(), new ContextManager())
        { }

        public GestureMachine(
            GestureMachineConfig gestureMachineConfig,
            CallbackManager callbackManager)
            : base(gestureMachineConfig, callbackManager, new ContextManager())
        { }

        public GestureMachine(
            RootElement rootElement)
            : base(new GestureMachineConfig(), new CallbackManager(), new ContextManager(), rootElement)
        { }

        public GestureMachine(
            GestureMachineConfig gestureMachineConfig, 
            RootElement rootElement)
            : base(gestureMachineConfig, new CallbackManager(), new ContextManager(), rootElement)
        { }
    
        public GestureMachine(
            GestureMachineConfig gestureMachineConfig, 
            CallbackManager callbackManager,
            RootElement rootElement)
            : base(gestureMachineConfig, callbackManager, new ContextManager(), rootElement)
        { }

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
    }
}
