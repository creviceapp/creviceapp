﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.FSM
{
    /// <summary>
    /// Dummy GestureMachine which have only one state and passthrough all inputs.
    /// </summary>
    public class NullGestureMachine 
        : GestureMachine<UserActionContext.UserActionEvaluationContext, UserActionContext.UserActionExecutionContext>
    {
        public NullGestureMachine() : base(new Config.UserConfig(), new List<GestureDefinition>())
        {

        }
    }

    /// <summary>
    /// FSM GestureMachine implimentation.
    /// </summary>
    public class GestureMachine<E, X> 
        : IDisposable
        where E : UserActionContext.UserActionEvaluationContext
        where X : UserActionContext.UserActionExecutionContext
    {
        public StateGlobal<E, X> Global { get; private set; }
        public IState State { get; private set; }
        public IEnumerable<GestureDefinition> GestureDefinition { get; private set; }

        private System.Timers.Timer timer = new System.Timers.Timer();
        private readonly object lockObject = new object();

        public GestureMachine(Config.UserConfig<E, X> userConfig, IEnumerable<GestureDefinition> gestureDef)
        {
            this.Global = new StateGlobal<E, X>(userConfig);
            this.State = new State0(Global, gestureDef);
            this.GestureDefinition = gestureDef;

            timer.Elapsed += new System.Timers.ElapsedEventHandler(TryGestureTimeout);
            timer.Interval = Global.Config.Gesture.Timeout;
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
                        timer.Interval = Global.Config.Gesture.Timeout;
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
