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
        void OnStrokeUpdate(IReadOnlyList<Stroke> strokes);
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

            public event StrokeUpdateEventHandler StrokeUpdated;
            public virtual void OnStrokeUpdate(object key, StrokeUpdateEventArgs e)
                => CheckKeyAndInvoke(key, () => { StrokeUpdated?.Invoke(this, e); });

            public event StateChangeEventHandler StateChanged;
            public virtual void OnStateChange(object key, StateChangeEventArgs e)
                => CheckKeyAndInvoke(key, () => { StateChanged?.Invoke(this, e); });

            public event GestureCancelEventHandler GestureCancelled;
            public virtual void OnGestureCancel(object key, GestureCancelEventArgs e)
                => CheckKeyAndInvoke(key, () => { GestureCancelled?.Invoke(this, e); });

            public event GestureTimeoutEventHandler GestureTimeout;
            public virtual void OnGestureTimeout(object key, GestureTimeoutEventArgs e)
                => CheckKeyAndInvoke(key, () => { GestureTimeout?.Invoke(this, e); });

            public event MachineResetEventHandler MachineReset;
            public virtual void OnMachineReset(object key, MachineResetEventArgs e)
                => CheckKeyAndInvoke(key, () => { MachineReset?.Invoke(this, e); });

            public event MachineStartEventHandler MachineStart;
            public virtual void OnMachineStart(object key, MachineStartEventArgs e)
                => CheckKeyAndInvoke(key, () => { MachineStart?.Invoke(this, e); });

            public event MachineStopEventHandler MachineStop;
            public virtual void OnMachineStop(object key, MachineStopEventArgs e)
                => CheckKeyAndInvoke(key, () => { MachineStop?.Invoke(this, e); });
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

        #region Event StrokeUpdate
        public class StrokeUpdateEventArgs : EventArgs
        {
            public readonly IReadOnlyList<Stroke> Strokes;

            public StrokeUpdateEventArgs(IReadOnlyList<Stroke> strokes)
            {
                Strokes = strokes;
            }
        }

        public delegate void StrokeUpdateEventHandler(object sender, StrokeUpdateEventArgs e);

        public virtual void OnStrokeUpdate(IReadOnlyList<Stroke> strokes) 
            => Receiver.OnStrokeUpdate(Key, new StrokeUpdateEventArgs(strokes));
        #endregion

        #region Event StateChange
        public class StateChangeEventArgs : EventArgs
        {
            public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> LastState;
            public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> CurrentState;

            public StateChangeEventArgs(
                State<TConfig, TContextManager, TEvalContext, TExecContext> lastState, 
                State<TConfig, TContextManager, TEvalContext, TExecContext> currentState)
            {
                LastState = lastState;
                CurrentState = currentState;
            }
        }

        public delegate void StateChangeEventHandler(object sender, StateChangeEventArgs e);

        public virtual void OnStateChange(
            State<TConfig, TContextManager, TEvalContext, TExecContext> lastState, 
            State<TConfig, TContextManager, TEvalContext, TExecContext> currentState) 
            => Receiver.OnStateChange(Key, new StateChangeEventArgs(lastState, currentState));
        #endregion

        #region Event GestureCancel
        public class GestureCancelEventArgs : EventArgs
        {
            public readonly StateN<TConfig, TContextManager, TEvalContext, TExecContext> LastState;

            public GestureCancelEventArgs(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN)
            {
                LastState = stateN;
            }
        }

        public delegate void GestureCancelEventHandler(object sender, GestureCancelEventArgs e);

        public virtual void OnGestureCancel(StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN) 
            => Receiver.OnGestureCancel(Key, new GestureCancelEventArgs(stateN));
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
            
        #region Event MachineStart
        public class MachineStartEventArgs : EventArgs
        {

        }

        public delegate void MachineStartEventHandler(object sender, MachineStartEventArgs e);

        public virtual void OnMachineStart() 
            => Receiver.OnMachineStart(Key, new MachineStartEventArgs());
        #endregion
            
        #region Event MachineStop
        public class MachineStopEventArgs : EventArgs
        {

        }

        public delegate void MachineStopEventHandler(object sender, MachineStopEventArgs e);

        public virtual void OnMachineStop() 
            => Receiver.OnMachineStop(Key, new MachineStopEventArgs());
        #endregion
    }
}
