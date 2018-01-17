using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.FSM
{
    /// <summary>
    /// Dummy GestureMachine which have only one state and passthrough all inputs.
    /// </summary>
    public class NullGestureMachine<A, B> : GestureMachine<A, DefaultActionContext>
        where A : GestureMachineConfig
        where B : ActionContext
    {
        public NullGestureMachine(A config) : base(config as A, new List<GestureDefinition>())
        { }
    }

    public static class NullGestureMachine
    {
        public static NullGestureMachine<A, DefaultActionContext> Create<A>(A config)
            where A : GestureMachineConfig
        {
            return new NullGestureMachine<A, DefaultActionContext>(config);
        }

        public static NullGestureMachine<DefaultGestureMachineConfig, DefaultActionContext> Create()
        {
            return Create(new DefaultGestureMachineConfig());
        }
    }

    /// <summary>
    /// FSM GestureMachine implimentation.
    /// </summary>
    public class GestureMachine<A, B>
        : IDisposable
        where A : GestureMachineConfig
        where B : ActionContext
    {
        public readonly StateGlobal Global;
        public IState State { get; private set; }
        public IEnumerable<GestureDefinition> GestureDefinition { get; private set; }

        private System.Timers.Timer timer = new System.Timers.Timer();
        private readonly object lockObject = new object();

        public GestureMachine(A config, IEnumerable<GestureDefinition> gestureDef)
        {
            var actionFactory = new ActionContextFactory<B>() as ActionContextFactory<ActionContext>;
            this.Global = new StateGlobal(config, actionFactory); // will work?
            this.State = new State0(Global, gestureDef);
            this.GestureDefinition = gestureDef;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(TryGestureTimeout);
            timer.Interval = Global.Config.GestureTimeout;
            timer.AutoReset = false;
        }

        public bool Input(Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            lock (lockObject)
            {
                var res = State.Input(evnt, point);   
                if (State.GetType() != res.NextState.GetType())
                {
                    Verbose.Print("The state of GestureMachine was changed: {0} -> {1}", State.GetType().Name, res.NextState.GetType().Name);

                    // Special side effect 1
                    if (res.NextState is State1 || res.NextState is State2)
                    {
                        Global.ResetStrokeWatcher();
                    }

                    // Reset timer for gesture timeout
                    if (State is State0 && res.NextState is State1)
                    {
                        timer.Stop();
                        // Reflect current config value
                        timer.Interval = Global.Config.GestureTimeout;
                        timer.Start();
                    }
                }
                State = res.NextState;
                return res.Event.IsConsumed;
            }
        }

        private void TryGestureTimeout(object sender, System.Timers.ElapsedEventArgs args)
        {
            lock (lockObject)
            {
                if (State is State1 && Global.StrokeWatcher.GetStorke().Count == 0)
                {
                    State = ((State1)State).Cancel();
                }
            }
        }

        public void Reset()
        {
            lock (lockObject)
            {
                State = State.Reset();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            lock (lockObject)
            {
                Global.Dispose();
            }
        }

        ~GestureMachine()
        {
            Dispose();
        }
    }
}
