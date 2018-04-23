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
        void OnStrokeUpdated(IReadOnlyList<Stroke> strokes);
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
            public virtual void OnStrokeReset(StrokeResetEventArgs e)
                => Invoke(() => { StrokeReset?.Invoke(this, e); });

            public event StrokeUpdatedEventHandler StrokeUpdated;
            public virtual void OnStrokeUpdated(StrokeUpdatedEventArgs e)
                => Invoke(() => { StrokeUpdated?.Invoke(this, e); });

            public event StateChangedEventHandler StateChanged;
            public virtual void OnStateChanged(StateChangedEventArgs e)
                => Invoke(() => { StateChanged?.Invoke(this, e); });

            public event GestureCancelledEventHandler GestureCancelled;
            public virtual void OnGestureCancelled(GestureCancelledEventArgs e)
                => Invoke(() => { GestureCancelled?.Invoke(this, e); });

            public event GestureTimeoutEventHandler GestureTimeout;
            public virtual void OnGestureTimeout(GestureTimeoutEventArgs e)
                => Invoke(() => { GestureTimeout?.Invoke(this, e); });

            public event MachineResetEventHandler MachineReset;
            public virtual void OnMachineReset(MachineResetEventArgs e)
                => Invoke(() => { MachineReset?.Invoke(this, e); });

            public event MachineStartEventHandler MachineStart;
            public virtual void OnMachineStart(MachineStartEventArgs e)
                => Invoke(() => { MachineStart?.Invoke(this, e); });

            public event MachineStopEventHandler MachineStop;
            public virtual void OnMachineStop(MachineStopEventArgs e)
                => Invoke(() => { MachineStop?.Invoke(this, e); });
        }

        public readonly CallbackContainer Callback;

        public CallbackManager() : this(new CallbackContainer()) {}
        
        public CallbackManager(CallbackContainer callback)
        {
            this.Callback = callback;
        }

        #region Event StrokeReset
        public class StrokeResetEventArgs : EventArgs {}

        public delegate void StrokeResetEventHandler(object sender, StrokeResetEventArgs e);

        public virtual void OnStrokeReset() 
            => Callback.OnStrokeReset(new StrokeResetEventArgs());
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

        public virtual void OnStrokeUpdated(IReadOnlyList<Stroke> strokes) 
            => Callback.OnStrokeUpdated(new StrokeUpdatedEventArgs(strokes));
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
            State<TConfig, TContextManager, TEvalContext, TExecContext> lastState, 
            State<TConfig, TContextManager, TEvalContext, TExecContext> currentState) 
            => Callback.OnStateChanged(new StateChangedEventArgs(lastState, currentState));
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

        public virtual void OnGestureCancelled(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN) 
            => Callback.OnGestureCancelled(new GestureCancelledEventArgs(stateN));
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

        public virtual void OnGestureTimeout(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN) 
            => Callback.OnGestureTimeout(new GestureTimeoutEventArgs(stateN));
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

        public virtual void OnMachineReset(State<TConfig, TContextManager, TEvalContext, TExecContext> state)
            => Callback.OnMachineReset(new MachineResetEventArgs(state));
        #endregion
            
        #region Event MachineStart
        public class MachineStartEventArgs : EventArgs
        {

        }

        public delegate void MachineStartEventHandler(object sender, MachineStartEventArgs e);

        public virtual void OnMachineStart() 
            => Callback.OnMachineStart(new MachineStartEventArgs());
        #endregion
            
        #region Event MachineStop
        public class MachineStopEventArgs : EventArgs
        {

        }

        public delegate void MachineStopEventHandler(object sender, MachineStopEventArgs e);

        public virtual void OnMachineStop() 
            => Callback.OnMachineStop(new MachineStopEventArgs());
        #endregion
    }
}
