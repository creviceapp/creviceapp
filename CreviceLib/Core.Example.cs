using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Example
{
    using Crevice.Core.Keys;
    using Crevice.Core.Context;
    using Crevice.Core.Callback;
    using Crevice.Core.FSM;

    public class SimpleKeySetA : PhysicalDoubleThrowKeySet
    {
        public SimpleKeySetA(int maxSize)
            : base(new LogicalDoubleThrowKeySet(maxSize))
        { }
    }

    public class SimpleKeySetB : PhysicalSingleThrowKeySet
    {
        public SimpleKeySetB(int maxSize)
            : base(new LogicalSingleThrowKeySet(maxSize))
        { }
    }
    
    public class SimpleGestureMachineConfig : GestureMachineConfig
    { }
    
    public class SimpleRootElement : DSL.RootElement<EvaluationContext, ExecutionContext>
    { }

    public class SimpleContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext();

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext();
    }

    class SimpleCallbackManager : CallbackManager<SimpleGestureMachineConfig, SimpleContextManager, EvaluationContext, ExecutionContext>
    { }

    public class SimpleGestureMachine : GestureMachine<SimpleGestureMachineConfig, SimpleContextManager, EvaluationContext, ExecutionContext>
    {
        public SimpleGestureMachine()
            : base(new SimpleGestureMachineConfig(), new SimpleCallbackManager(), new SimpleContextManager()) { }
    }
}
