using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Drawing;
    using System.Threading.Tasks;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Callback;
    using Crevice.Core.Stroke;
    using Crevice.Core.Helpers;

    public interface IGestureMachine
    {
        bool Input(IPhysicalEvent evnt);

        bool Input(IPhysicalEvent evnt, Point? point);

        void Reset();
    }

    public abstract class GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext>
        : IGestureMachine, IDisposable
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly TConfig Config;
        public readonly CallbackManager<TConfig, TContextManager, TEvalContext, TExecContext> CallbackManager;
        public readonly TContextManager ContextManager;

        protected readonly object lockObject = new object();

        private readonly System.Timers.Timer gestureTimeoutTimer = new System.Timers.Timer();

        internal readonly EventCounter<PhysicalReleaseEvent> invalidEvents = new EventCounter<PhysicalReleaseEvent>();

        public IReadOnlyRootElement<TEvalContext, TExecContext> RootElement { get; internal set; }

        public StrokeWatcher StrokeWatcher { get; internal set; }

        private IState _currentState = null;
        public IState CurrentState
        {
            get => _currentState;

            internal set
            {
                if (_currentState != value)
                {
                    ResetStrokeWatcher();
                    CallbackManager.OnStrokeReset();
                    if (value is State0<TConfig, TContextManager, TEvalContext, TExecContext>)
                    {
                        StopGestureTimeoutTimer();
                    }
                    else if (value is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
                    {
                        ResetGestureTimeoutTimer();
                    }
                    var lastState = _currentState;
                    _currentState = value;
                    CallbackManager.OnStateChanged(lastState, _currentState);
                }
            }
        }

        internal virtual TaskFactory StrokeWatcherTaskFactory => Task.Factory;
        internal virtual TaskFactory LowPriorityTaskFactory => Task.Factory;

        public GestureMachine(
            TConfig config,
            CallbackManager<TConfig, TContextManager, TEvalContext, TExecContext> callbackManager,
            TContextManager contextManager,
            IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            : this(config, callbackManager, contextManager)
        {
            Run(rootElement);
        }

        public GestureMachine(
            TConfig config,
            CallbackManager<TConfig, TContextManager, TEvalContext, TExecContext> callbackManager,
            TContextManager contextManager)
        {
            Config = config;
            CallbackManager = callbackManager;
            ContextManager = contextManager;

            SetupGestureTimeoutTimer();
        }

        public bool IsRunning { get; internal set; } = false;

        public virtual void Run(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
        {
            lock (lockObject)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException();
                }
                CurrentState = new State0<TConfig, TContextManager, TEvalContext, TExecContext>(this, rootElement);
                RootElement = rootElement;
                IsRunning = true;
            }
        }

        public virtual bool Input(IPhysicalEvent evnt) => Input(evnt, null);

        public virtual bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (lockObject)
            {
                if (point.HasValue && CurrentState is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
                {
                    StrokeWatcher.Queue(point.Value);
                }
                
                if (evnt is NullEvent)
                {
                    return false;
                }
                else if (evnt is PhysicalReleaseEvent releaseEvent && invalidEvents[releaseEvent] > 0)
                {
                    invalidEvents.CountDown(releaseEvent);
                    return true;
                }

                var result = CurrentState.Input(evnt);
                CurrentState = result.NextState;
                return result.EventIsConsumed;
            }
        }

        private void SetupGestureTimeoutTimer()
        {
            gestureTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TryTimeout);
            gestureTimeoutTimer.AutoReset = false;
        }

        private void StopGestureTimeoutTimer()
        {
            gestureTimeoutTimer.Stop();
        }

        private void ResetGestureTimeoutTimer()
        {
            gestureTimeoutTimer.Stop();
            if (Config.GestureTimeout > 0)
            {
                gestureTimeoutTimer.Interval = Config.GestureTimeout;
                gestureTimeoutTimer.Start();
            }
        }

        private void ReleaseGestureTimeoutTimer() => LazyRelease(gestureTimeoutTimer);

        private StrokeWatcher CreateStrokeWatcher()
            => new StrokeWatcher(
                this.CallbackManager,
                StrokeWatcherTaskFactory,
                Config.StrokeStartThreshold,
                Config.StrokeDirectionChangeThreshold,
                Config.StrokeExtensionThreshold,
                Config.StrokeWatchInterval);

        private void ReleaseStrokeWatcher() => LazyRelease(StrokeWatcher);

        private void LazyRelease(IDisposable disposable)
        {
            if (disposable != null)
            {
                LowPriorityTaskFactory.StartNew(() => {
                    disposable.Dispose();
                });
            }
        }

        private void ResetStrokeWatcher()
        {
            var strokeWatcher = StrokeWatcher;
            StrokeWatcher = CreateStrokeWatcher();
            LazyRelease(strokeWatcher);
        }

        private void TryTimeout(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (lockObject)
            {
                if (CurrentState is StateN<TConfig, TContextManager, TEvalContext, TExecContext> lastState)
                {
                    var state = CurrentState;
                    var _state = CurrentState.Timeout();
                    while (state != _state)
                    {
                        state = _state;
                        _state = state.Timeout();
                    }
                    if (CurrentState != state)
                    {
                        CurrentState = state;
                        CallbackManager.OnGestureTimeout(lastState);
                    }
                }
            }
        }

        public void Reset()
        {
            lock (lockObject)
            {
                var lastState = CurrentState;
                if (CurrentState is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
                {
                    var state = CurrentState;
                    var _state = CurrentState.Reset();
                    while (state != _state)
                    {
                        state = _state;
                        _state = state.Reset();
                    }
                    CurrentState = state;
                }
                CallbackManager.OnMachineReset(lastState);
            }
        }

        internal bool _disposed { get; private set; } = false;

        public void Dispose()
        {
            lock (lockObject)
            {
                GC.SuppressFinalize(this);
                _disposed = true;
                ReleaseGestureTimeoutTimer();
                ReleaseStrokeWatcher();
            }
        }

        ~GestureMachine()
        {
            Dispose();
        }
    }
}
