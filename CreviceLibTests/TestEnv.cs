using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using System.Collections.Generic;

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
        public readonly bool EnableStrokeUpdateCallback;
        public readonly bool EnableStateChangeCallback;
        public readonly bool EnableGestureTimeoutCallback;
        public readonly bool EnableGestureCancelCallback;
        public readonly bool EnableMachineResetCallback;
        public readonly bool EnableMachineStartCallback;
        public readonly bool EnableMachineStopCallback;

        public TestCallbackManager(
            bool enableStrokeResetCallback = false,
            bool enableStrokeUpdatedCallback = false,
            bool enableStateChangedCallback = false,
            bool enableGestureTimeoutCallback = false,
            bool enableGestureCancelledCallback = false,
            bool enableMachineResetCallback = false,
            bool enableMachineStartCallback = false,
            bool enableMachineStopCallback = false)
        {
            EnableStrokeResetCallback = enableStrokeResetCallback;
            EnableStrokeUpdateCallback = enableStrokeUpdatedCallback;
            EnableStateChangeCallback = enableStateChangedCallback;
            EnableGestureTimeoutCallback = enableGestureTimeoutCallback;
            EnableGestureCancelCallback = enableGestureCancelledCallback;
            EnableMachineResetCallback = enableMachineResetCallback;
            EnableMachineStartCallback = enableMachineStartCallback;
            EnableMachineStopCallback = enableMachineStopCallback;
        }

        public readonly System.Threading.CountdownEvent OnStrokeResetCDE = new System.Threading.CountdownEvent(1);
        public int OnStrokeResetCallCount { get; private set; } = 0;

        public override void OnStrokeReset(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine,
            StrokeWatcher lastStrokeWatcher,
            StrokeWatcher currentStrokeWatcher)

        {
            if (EnableStrokeResetCallback)
            {
                OnStrokeResetCallCount += 1;
                OnStrokeResetCDE.Signal();
                base.OnStrokeReset(testGestureMachine, lastStrokeWatcher, currentStrokeWatcher);
            }
        }

        public readonly System.Threading.CountdownEvent OnStrokeUpdatedCDE = new System.Threading.CountdownEvent(1);
        public int OnStrokeUpdatedCallCount { get; private set; } = 0;

        public override void OnStrokeUpdated(
            StrokeWatcher strokeWatcher, 
            IReadOnlyList<Stroke> strokes)
        {
            if (EnableStrokeUpdateCallback)
            {
                OnStrokeUpdatedCallCount += 1;
                OnStrokeUpdatedCDE.Signal();
                base.OnStrokeUpdated(strokeWatcher, strokes);
            }
        }

        public readonly System.Threading.CountdownEvent OnStateChangedCDE = new System.Threading.CountdownEvent(1);
        public int OnStateChangeCallCount { get; private set; } = 0;

        public override void OnStateChanged(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine,
            State<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> lastState, 
            State<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> currentState)
        {
            if (EnableStateChangeCallback)
            {
                OnStateChangeCallCount += 1;
                OnStateChangedCDE.Signal();
                base.OnStateChanged(testGestureMachine, lastState, currentState);
            }
        }

        public readonly System.Threading.CountdownEvent OnGestureTimeoutCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureTimeoutCallCount { get; private set; } = 0;

        public override void OnGestureTimeout(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine,
            StateN<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            if (EnableGestureTimeoutCallback)
            {
                OnGestureTimeoutCallCount += 1;
                OnGestureTimeoutCDE.Signal();
                base.OnGestureTimeout(testGestureMachine, stateN);
            }
        }

        public readonly System.Threading.CountdownEvent OnGestureCancelledCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureCancelCallCount { get; private set; } = 0;

        public override void OnGestureCanceled(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine,
            StateN<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> stateN)
        {
            if (EnableGestureCancelCallback)
            {
                OnGestureCancelCallCount += 1;
                OnGestureCancelledCDE.Signal();
                base.OnGestureCanceled(testGestureMachine, stateN);
            }
        }

        public readonly System.Threading.CountdownEvent OnMachineResetCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineResetCallCount { get; private set; } = 0;

        public override void OnMachineReset(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine,
            State<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> state)
        {
            if (EnableMachineResetCallback)
            {
                OnMachineResetCallCount += 1;
                OnMachineResetCDE.Signal();
                base.OnMachineReset(testGestureMachine, state);
            }
        }

        public readonly System.Threading.CountdownEvent OnMachineStartCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineStartCallCount { get; private set; } = 0;

        public override void OnMachineStart(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine)
        {
            if (EnableMachineStartCallback)
            {
                OnMachineStartCallCount += 1;
                OnMachineStartCDE.Signal();
                base.OnMachineStart(testGestureMachine);
            }
        }

        public readonly System.Threading.CountdownEvent OnMachineStopCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineStopCallCount { get; private set; } = 0;

        public override void OnMachineStop(
            GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext> testGestureMachine)
        {
            if (EnableMachineStopCallback)
            {
                OnMachineStopCallCount += 1;
                OnMachineStopCDE.Signal();
                base.OnMachineStop(testGestureMachine);
            }
        }
    }

    class TestGestureMachine : GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext>
    {
        // New and Run(rootElement) pattern.
        public TestGestureMachine()
            : this(new TestCallbackManager())
        { }

        public TestGestureMachine(
            TestCallbackManager callbackManger)
            : base(new TestGestureMachineConfig(), callbackManger, new TestContextManager())
        { }

        // New(rootElement) pattern.
        public TestGestureMachine(
            TestRootElement rootElement)
            : this(rootElement, new TestCallbackManager())
        { }
        
        public TestGestureMachine(
            TestRootElement rootElement,
            TestCallbackManager callbackManger)
            : base(new TestGestureMachineConfig(), callbackManger, new TestContextManager())
        {
            Run(rootElement);
        }
    }

    public class TestEvents
    {
        public static LogicalSingleThrowKeySet LogicalSingleThrowKeys = new LogicalSingleThrowKeySet(10);
        public static LogicalDoubleThrowKeySet LogicalDoubleThrowKeys = new LogicalDoubleThrowKeySet(10);
        public static PhysicalSingleThrowKeySet PhysicalSingleThrowKeys0 = new PhysicalSingleThrowKeySet(LogicalSingleThrowKeys);
        public static PhysicalDoubleThrowKeySet PhysicalDoubleThrowKeys0 = new PhysicalDoubleThrowKeySet(LogicalDoubleThrowKeys);
        public static PhysicalSingleThrowKeySet PhysicalSingleThrowKeys1 = new PhysicalSingleThrowKeySet(LogicalSingleThrowKeys);
        public static PhysicalDoubleThrowKeySet PhysicalDoubleThrowKeys1 = new PhysicalDoubleThrowKeySet(LogicalDoubleThrowKeys);
    }
}
