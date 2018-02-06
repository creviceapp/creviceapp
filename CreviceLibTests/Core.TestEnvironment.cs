using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Context;
    using Crevice.Core.Keys;
    using Crevice.Core.FSM;
    using Crevice.Core.Callback;
    using Crevice.Core.Stroke;
    
    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    class TestGestureMachineConfig : GestureMachineConfig
    { }

    class TestContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext();

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext();
    }

    class TestCallbackManager : CallbackManager<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext>
    {
        public readonly bool EnableStrokeResetCallback;
        public readonly bool EnableStrokeUpdatedCallback;
        public readonly bool EnableStateChangedCallback;
        public readonly bool EnableGestureTimeoutCallback;
        public readonly bool EnableGestureCancelledCallback;
        public readonly bool EnableMachineResetCallback;

        public TestCallbackManager(
            bool enableStrokeResetCallback = false,
            bool enableStrokeUpdatedCallback = false,
            bool enableStateChangedCallback = false,
            bool enableGestureTimeoutCallback = false,
            bool enableGestureCancelledCallback = false,
            bool enableMachineResetCallback = false)
        {
            EnableStrokeResetCallback = enableStrokeResetCallback;
            EnableStrokeUpdatedCallback = enableStrokeUpdatedCallback;
            EnableStateChangedCallback = enableStateChangedCallback;
            EnableGestureTimeoutCallback = enableGestureTimeoutCallback;
            EnableGestureCancelledCallback = enableGestureCancelledCallback;
            EnableMachineResetCallback = enableMachineResetCallback;
        }

        public readonly System.Threading.CountdownEvent OnStrokeResetCDE = new System.Threading.CountdownEvent(1);
        public int OnStrokeResetCallCount { get; private set; } = 0;

        public override void OnStrokeReset()
        {
            if (EnableStrokeResetCallback)
            {
                OnStrokeResetCallCount += 1;
                OnStrokeResetCDE.Signal();
                base.OnStrokeReset();
            }
        }

        public readonly System.Threading.CountdownEvent OnStrokeUpdatedCDE = new System.Threading.CountdownEvent(1);
        public int OnStrokeUpdatedCallCount { get; private set; } = 0;

        public override void OnStrokeUpdated(IReadOnlyList<Stroke> strokes)
        {
            if (EnableStrokeUpdatedCallback)
            {
                OnStrokeUpdatedCallCount += 1;
                OnStrokeUpdatedCDE.Signal();
                base.OnStrokeUpdated(strokes);
            }
        }

        public readonly System.Threading.CountdownEvent OnStateChangedCDE = new System.Threading.CountdownEvent(1);
        public int OnStateChangedCallCount { get; private set; } = 0;

        public override void OnStateChanged(
            IState lastState, IState currentState)
        {
            if (EnableStateChangedCallback)
            {
                OnStateChangedCallCount += 1;
                OnStateChangedCDE.Signal();
                base.OnStateChanged(lastState, currentState);
            }
        }

        public readonly System.Threading.CountdownEvent OnGestureTimeoutCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureTimeoutCallCount { get; private set; } = 0;

        public override void OnGestureTimeout(
            StateN<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            if (EnableGestureTimeoutCallback)
            {
                OnGestureTimeoutCallCount += 1;
                OnGestureTimeoutCDE.Signal();
                base.OnGestureTimeout(stateN);
            }
        }

        public readonly System.Threading.CountdownEvent OnGestureCancelledCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureCancelledCallCount { get; private set; } = 0;

        public override void OnGestureCancelled(
            StateN<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            if (EnableGestureCancelledCallback)
            {
                OnGestureCancelledCallCount += 1;
                OnGestureCancelledCDE.Signal();
                base.OnGestureCancelled(stateN);
            }
        }

        public readonly System.Threading.CountdownEvent OnMachineResetCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineResetCallCount { get; private set; } = 0;

        public override void OnMachineReset(
            IState state)
        {
            if (EnableMachineResetCallback)
            {
                OnMachineResetCallCount += 1;
                OnMachineResetCDE.Signal();
                base.OnMachineReset(state);
            }
        }
    }

    class TestGestureMachine : GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext>
    {
        public TestGestureMachine(
            TestRootElement rootElement)
            : this(rootElement, new TestCallbackManager())
        { }

        public TestGestureMachine(
            TestRootElement rootElement,
            TestCallbackManager callbackManger)
            : base(new TestGestureMachineConfig(), callbackManger, new TestContextManager(),  rootElement)
        { }
    }

    public class TestEvents
    {
        public static LogicalSingleThrowKeySet LogicalSingleThrowKeys = new LogicalSingleThrowKeySet(2);
        public static LogicalDoubleThrowKeySet LogicalDoubleThrowKeys = new LogicalDoubleThrowKeySet(2);
        public static PhysicalSingleThrowKeySet PhysicalSingleThrowKeys = new PhysicalSingleThrowKeySet(LogicalSingleThrowKeys);
        public static PhysicalDoubleThrowKeySet PhysicalDoubleThrowKeys = new PhysicalDoubleThrowKeySet(LogicalDoubleThrowKeys);
    }
}
