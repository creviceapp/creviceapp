using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Callback
{
    using Crevice.Core.Context;
    using Crevice.Core.FSM;
    using Crevice.Core.Stroke;

    public interface IStrokeCallbackManager
    {
        void OnStrokeUpdated(StrokeWatcher strokeWatcher, IReadOnlyList<Stroke> strokes);
    }

    public class CallbackManager<TConfig, TContextManager, TEvalContext, TExecContext>
        : IStrokeCallbackManager
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public class CallbackContainer
        {
            protected virtual void Invoke(Action action) => action();

            public event StrokeResetEventHandler StrokeReset;
            public virtual void OnStrokeReset(object o, StrokeResetEventArgs e)
                => Invoke(() => { StrokeReset?.Invoke(o, e); });

            public event StrokeUpdatedEventHandler StrokeUpdated;
            public virtual void OnStrokeUpdated(object o, StrokeUpdatedEventArgs e)
                => Invoke(() => { StrokeUpdated?.Invoke(o, e); });

            public event StateChangedEventHandler StateChanged;
            public virtual void OnStateChanged(object o, StateChangedEventArgs e)
                => Invoke(() => { StateChanged?.Invoke(o, e); });

            public event GestureCancelledEventHandler GestureCancelled;
            public virtual void OnGestureCancelled(object o, GestureCancelledEventArgs e)
                => Invoke(() => { GestureCancelled?.Invoke(o, e); });

            public event GestureTimeoutEventHandler GestureTimeout;
            public virtual void OnGestureTimeout(object o, GestureTimeoutEventArgs e)
                => Invoke(() => { GestureTimeout?.Invoke(o, e); });

            public event MachineResetEventHandler MachineReset;
            public virtual void OnMachineReset(object o, MachineResetEventArgs e)
                => Invoke(() => { MachineReset?.Invoke(o, e); });

            public event MachineStartEventHandler MachineStart;
            public virtual void OnMachineStart(object o, MachineStartEventArgs e)
                => Invoke(() => { MachineStart?.Invoke(o, e); });

            public event MachineStopEventHandler MachineStop;
            public virtual void OnMachineStop(object o, MachineStopEventArgs e)
                => Invoke(() => { MachineStop?.Invoke(o, e); });
        }

        public readonly CallbackContainer Callback;

        public CallbackManager() : this(new CallbackContainer()) {}
        
        public CallbackManager(CallbackContainer callback)
        {
            this.Callback = callback;
        }

        #region Event StrokeReset
        public class StrokeResetEventArgs : EventArgs
        {
            public readonly StrokeWatcher LastStrokeWatcher;
            public readonly StrokeWatcher CurrentStrokeWatcher;

            public StrokeResetEventArgs(StrokeWatcher lastStrokeWatcher, StrokeWatcher currentStrokeWatcher)
            {
                LastStrokeWatcher = lastStrokeWatcher;
                CurrentStrokeWatcher = currentStrokeWatcher;
            }
        }

        public delegate void StrokeResetEventHandler(object sender, StrokeResetEventArgs e);

        public virtual void OnStrokeReset(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine,
                StrokeWatcher lastStrokeWatcher,
                StrokeWatcher currentStrokeWatcher) 
            => Callback.OnStrokeReset(gestureMachine, new StrokeResetEventArgs(lastStrokeWatcher, currentStrokeWatcher));
        #endregion

        #region Event StrokeUpdated
        public class StrokeUpdatedEventArgs : EventArgs
        {
            public readonly IReadOnlyList<Stroke> Strokes;

            public StrokeUpdatedEventArgs(IReadOnlyList<Stroke> strokes)
            {
                Strokes = strokes;
            }
        }

        public delegate void StrokeUpdatedEventHandler(object sender, StrokeUpdatedEventArgs e);

        public virtual void OnStrokeUpdated(StrokeWatcher strokeWatcher, IReadOnlyList<Stroke> strokes) 
            => Callback.OnStrokeUpdated(strokeWatcher, new StrokeUpdatedEventArgs(strokes));
        #endregion

        #region Event StateChanged
        public class StateChangedEventArgs : EventArgs
        {
            public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> LastState;
            public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> CurrentState;

            public StateChangedEventArgs(
                State<TConfig, TContextManager, TEvalContext, TExecContext> lastState, 
                State<TConfig, TContextManager, TEvalContext, TExecContext> currentState)
            {
                LastState = lastState;
                CurrentState = currentState;
            }
        }

        public delegate void StateChangedEventHandler(object sender, StateChangedEventArgs e);

        public virtual void OnStateChanged(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine,
            State<TConfig, TContextManager, TEvalContext, TExecContext> lastState, 
            State<TConfig, TContextManager, TEvalContext, TExecContext> currentState) 
            => Callback.OnStateChanged(gestureMachine, new StateChangedEventArgs(lastState, currentState));
        #endregion

        #region Event GestureCancelled
        public class GestureCancelledEventArgs : EventArgs
        {
            public readonly StateN<TConfig, TContextManager, TEvalContext, TExecContext> LastState;

            public GestureCancelledEventArgs(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN)
            {
                LastState = stateN;
            }
        }

        public delegate void GestureCancelledEventHandler(object sender, GestureCancelledEventArgs e);

        public virtual void OnGestureCancelled(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine,
                StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN) 
            => Callback.OnGestureCancelled(gestureMachine, new GestureCancelledEventArgs(stateN));
        #endregion

        #region Event GestureTimeout
        public class GestureTimeoutEventArgs : EventArgs
        {
            public readonly StateN<TConfig, TContextManager, TEvalContext, TExecContext> LastState;

            public GestureTimeoutEventArgs(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN)
            {
                LastState = stateN;
            }
        }

        public delegate void GestureTimeoutEventHandler(object sender, GestureTimeoutEventArgs e);

        public virtual void OnGestureTimeout(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine,
                StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN) 
            => Callback.OnGestureTimeout(gestureMachine, new GestureTimeoutEventArgs(stateN));
        #endregion

        #region Event MachineReset
        public class MachineResetEventArgs : EventArgs
        {
            public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> LastState;

            public MachineResetEventArgs(State<TConfig, TContextManager, TEvalContext, TExecContext> lastState)
            {
                LastState = lastState;
            }
        }

        public delegate void MachineResetEventHandler(object sender, MachineResetEventArgs e);

        public virtual void OnMachineReset(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine,
                State<TConfig, TContextManager, TEvalContext, TExecContext> state)
            => Callback.OnMachineReset(gestureMachine, new MachineResetEventArgs(state));
        #endregion
            
        #region Event MachineStart
        public class MachineStartEventArgs : EventArgs {}

        public delegate void MachineStartEventHandler(object sender, MachineStartEventArgs e);

        public virtual void OnMachineStart(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine) 
            => Callback.OnMachineStart(gestureMachine, new MachineStartEventArgs());
        #endregion
            
        #region Event MachineStop
        public class MachineStopEventArgs : EventArgs {}

        public delegate void MachineStopEventHandler(object sender, MachineStopEventArgs e);

        public virtual void OnMachineStop(
                GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> gestureMachine) 
            => Callback.OnMachineStop(gestureMachine, new MachineStopEventArgs());
        #endregion
    }
}
