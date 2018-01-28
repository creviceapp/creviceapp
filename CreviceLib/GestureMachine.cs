using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Crevice
{
    using Crevice.Core;

    public class GestureMachineConfig
    {
        // ms
        public int GestureTimeout { get; set; } = 1000;
        // px
        public int StrokeStartThreshold { get; set; } = 10;
        // px
        public int StrokeDirectionChangeThreshold { get; set; } = 20;
        // px
        public int StrokeExtensionThreshold { get; set; } = 10;
        // ms
        public int StrokeWatchInterval { get; set; } = 10;
    }

    public abstract class GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext>
        : IIsDisposed, IDisposable
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly TConfig Config;
        public readonly TContextManager ContextManager;
        public readonly RootElement<TEvalContext, TExecContext> RootElement;

        private readonly object LockObject = new object();

        private readonly System.Timers.Timer GestureTimeoutTimer = new System.Timers.Timer();

        internal readonly InvalidReleaseEventManager InvalidReleaseEvents = new InvalidReleaseEventManager();

        public StrokeWatcher StrokeWatcher { get; private set; } = null;

        private IState _currentState = null;
        public IState CurrentState
        {
            get => _currentState;

            internal set
            {
                if (_currentState != value)
                {
                    if (value is State0<TConfig, TContextManager, TEvalContext, TExecContext>)
                    {
                        ReleaseStrokeWatcher();
                        StopGestureTimeoutTimer();
                    }
                    else if (value is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
                    {
                        ResetStrokeWatcher();
                        ResetGestureTimeoutTimer();
                    }
                    _currentState = value;
                }
            }
        }

        public virtual TaskFactory StrokeWatcherTaskFactory => Task.Factory;
        public virtual TaskFactory LowPriorityTaskFactory => Task.Factory;

        public GestureMachine(
            TConfig config,
            TContextManager contextManager,
            RootElement<TEvalContext, TExecContext> rootElement)
        {
            Config = config;
            ContextManager = contextManager;
            RootElement = rootElement;

            SetupGestureTimeoutTimer();

            CurrentState = new State0<TConfig, TContextManager, TEvalContext, TExecContext>(
                this,
                rootElement);
        }

        public bool Input(IPhysicalEvent evnt) => Input(evnt, null);

        public bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (LockObject)
            {
                if (evnt is IReleaseEvent releaseEvent && InvalidReleaseEvents[releaseEvent] > 0)
                {
                    InvalidReleaseEvents.CountDown(releaseEvent);
                    return true;
                }

                if (point.HasValue && CurrentState is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
                {
                    StrokeWatcher.Queue(point.Value);
                }

                var (eventIsConsumed, nextState) = CurrentState.Input(evnt);
                CurrentState = nextState;
                return eventIsConsumed;
            }
        }

        private void SetupGestureTimeoutTimer()
        {
            GestureTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TryTimeout);
            GestureTimeoutTimer.Interval = Config.GestureTimeout;
            GestureTimeoutTimer.AutoReset = false;
        }

        private void StopGestureTimeoutTimer()
        {
            GestureTimeoutTimer.Stop();
        }

        private void ResetGestureTimeoutTimer()
        {
            GestureTimeoutTimer.Stop();
            GestureTimeoutTimer.Interval = Config.GestureTimeout;
            GestureTimeoutTimer.Start();
        }

        private void ReleaseGestureTimeoutTimer() => LazyRelease(GestureTimeoutTimer);

        private StrokeWatcher CreateStrokeWatcher()
            => new StrokeWatcher(
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
            lock (LockObject)
            {
                if (CurrentState is StateN<TConfig, TContextManager, TEvalContext, TExecContext>)
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
                        OnGestureTimeout();
                    }
                }
            }
        }

        // StateChanged
        //      でStateが来たときに扱いやすいように

        // StrokeReset
        // StrokeUpdated

        public event EventHandler GestureCancelled;
        internal virtual void OnGestureCancelled() => GestureCancelled?.Invoke(this, EventArgs.Empty);

        public event EventHandler GestureTimeout;
        internal virtual void OnGestureTimeout() => GestureTimeout?.Invoke(this, EventArgs.Empty);

        public event EventHandler MachineReset;
        internal virtual void OnMachineReset() => MachineReset?.Invoke(this, EventArgs.Empty);

        public void Reset()
        {
            lock (LockObject)
            {
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
                OnMachineReset();
            }
        }

        public bool IsDisposed { get; private set; } = false;

        public void Dispose()
        {
            lock (LockObject)
            {
                GC.SuppressFinalize(this);
                IsDisposed = true;
                ReleaseGestureTimeoutTimer();
                ReleaseStrokeWatcher();
            }
        }

        ~GestureMachine()
        {
            Dispose();
        }
    }

    public class InvalidReleaseEventManager
    {
        public class NaturalNumberCounter<T>
        {
            private readonly Dictionary<T, int> Dictionary = new Dictionary<T, int>();

            public int this[T key]
            {
                get
                {
                    return Dictionary.TryGetValue(key, out int count) ? count : 0;
                }
                set
                {
                    if (value < 0)
                    {
                        throw new InvalidOperationException("n >= 0");
                    }
                    Dictionary[key] = value;
                }
            }

            public void CountDown(T key)
            {
                Dictionary[key] = this[key] - 1;
            }

            public void CountUp(T key)
            {
                Dictionary[key] = this[key] + 1;
            }
        }

        private readonly NaturalNumberCounter<IReleaseEvent> InvalidReleaseEvents = new NaturalNumberCounter<IReleaseEvent>();

        public int this[IReleaseEvent key]
        {
            get => InvalidReleaseEvents[key];
        }

        public void IgnoreNext(IReleaseEvent releaseEvent) => InvalidReleaseEvents.CountUp(releaseEvent);

        public void IgnoreNext(IEnumerable<IReleaseEvent> releaseEvents)
        {
            foreach (var releaseEvent in releaseEvents)
            {
                IgnoreNext(releaseEvent);
            }
        }

        public void CountDown(IReleaseEvent key) => InvalidReleaseEvents.CountDown(key);
    }
}
