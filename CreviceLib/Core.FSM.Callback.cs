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
        public class CallbackReceiver
        {
            private readonly object Key;

            private delegate void EventInvoker();

            public CallbackReceiver(object key)
            {
                Key = key;
            }

            private void CheckKeyAndInvoke(object key, EventInvoker invoker)
            {
                if (Key == key)
                {
                    invoker();
                }
            }

            public event StrokeResetEventHandler StrokeReset;
            public virtual void OnStrokeReset(object key, StrokeResetEventArgs e)
                => CheckKeyAndInvoke(key, () => { StrokeReset?.Invoke(this, e); });

            public event StrokeUpdatedEventHandler StrokeUpdated;
            public virtual void OnStrokeUpdated(object key, StrokeUpdatedEventArgs e)
                => CheckKeyAndInvoke(key, () => { StrokeUpdated?.Invoke(this, e); });

            public event StateChangedEventHandler StateChanged;
            public virtual void OnStateChanged(object key, StateChangedEventArgs e)
                => CheckKeyAndInvoke(key, () => { StateChanged?.Invoke(this, e); });

            public event GestureCancelledEventHandler GestureCancelled;
            public virtual void OnGestureCancelled(object key, GestureCancelledEventArgs e)
                => CheckKeyAndInvoke(key, () => { GestureCancelled?.Invoke(this, e); });

            public event GestureTimeoutEventHandler GestureTimeout;
            public virtual void OnGestureTimeout(object key, GestureTimeoutEventArgs e)
                => CheckKeyAndInvoke(key, () => { GestureTimeout?.Invoke(this, e); });

            public event MachineResetEventHandler MachineReset;
            public virtual void OnMachineReset(object key, MachineResetEventArgs e)
                => CheckKeyAndInvoke(key, () => { MachineReset?.Invoke(this, e); });
        }

        private readonly object Key = new object();
        public readonly CallbackReceiver Receiver;

        public CallbackManager()
        {
            Receiver = new CallbackReceiver(Key);
        }

        #region Event StrokeReset
        public class StrokeResetEventArgs : EventArgs
        {

        }

        public delegate void StrokeResetEventHandler(object sender, StrokeResetEventArgs e);

        public virtual void OnStrokeReset() 
            => Receiver.OnStrokeReset(Key, new StrokeResetEventArgs());
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
            => Receiver.OnStrokeUpdated(Key, new StrokeUpdatedEventArgs(strokes));
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
            => Receiver.OnStateChanged(Key, new StateChangedEventArgs(lastState, currentState));
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
            => Receiver.OnGestureCancelled(Key, new GestureCancelledEventArgs(stateN));
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
            => Receiver.OnGestureTimeout(Key, new GestureTimeoutEventArgs(stateN));
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
            => Receiver.OnMachineReset(Key, new MachineResetEventArgs(state));
        #endregion
    }
}
