using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Context;
    using Crevice.Core.Keys;
    using Crevice.Core.FSM;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    class TestGestureMachineConfig : GestureMachineConfig { }

    class TestContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext();

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext();
    }

    class TestGestureMachine : GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext>
    {
        public TestGestureMachine(TestRootElement rootElement) 
            : base(new TestGestureMachineConfig(), new TestContextManager(),  rootElement) { }

        public System.Threading.CountdownEvent OnGestureTimeoutCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureTimeoutCallCount { get; private set; } = 0;

        internal override void OnGestureTimeout(GestureTimeoutEventArgs e)
        {
            OnGestureTimeoutCallCount += 1;
            OnGestureTimeoutCDE.Signal();
            base.OnGestureTimeout(e);
        }

        public System.Threading.CountdownEvent OnGestureCancelledCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureCancelledCallCount { get; private set; } = 0;

        internal override void OnGestureCancelled(GestureCancelledEventArgs e)
        {
            OnGestureCancelledCallCount += 1;
            OnGestureCancelledCDE.Signal();
            base.OnGestureCancelled(e);
        }

        public System.Threading.CountdownEvent OnMachineResetCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineResetCallCount { get; private set; } = 0;

        internal override void OnMachineReset(MachineResetEventArgs e)
        {
            OnMachineResetCallCount += 1;
            OnMachineResetCDE.Signal();
            base.OnMachineReset(e);
        }
    }

    public class TestEvents
    {
        public static LogicalSingleThrowKeys LogicalSingleThrowKeys = new LogicalSingleThrowKeys(2);
        public static LogicalDoubleThrowKeys LogicalDoubleThrowKeys = new LogicalDoubleThrowKeys(2);
        public static PhysicalSingleThrowKeys PhysicalSingleThrowKeys = new PhysicalSingleThrowKeys(LogicalSingleThrowKeys);
        public static PhysicalDoubleThrowKeys PhysicalDoubleThrowKeys = new PhysicalDoubleThrowKeys(LogicalDoubleThrowKeys);
    }
}
