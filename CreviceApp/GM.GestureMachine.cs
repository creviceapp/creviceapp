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
    using Crevice.Element;

    public class NullGestureMachine : GestureMachine
    {
        public NullGestureMachine() : base(new RootElement()) { }
    }
    
    public class GestureMachine : GestureMachine<GestureMachineConfig, ContextManager, EvaluationContext, ExecutionContext>
    {
        public GestureMachine(
            RootElement rootElement)
            : base(new GestureMachineConfig(), new CustomCallbackManager(), new ContextManager(), rootElement)
        { }

        public GestureMachine(
            GestureMachineConfig gestureMachineConfig, 
            RootElement rootElement)
            : base(gestureMachineConfig, new CustomCallbackManager(), new ContextManager(), rootElement)
        { }
    
        public GestureMachine(
            GestureMachineConfig gestureMachineConfig, 
            CustomCallbackManager callbackManager,
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
