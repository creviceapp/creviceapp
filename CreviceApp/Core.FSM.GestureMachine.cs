using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public class GestureMachine : IDisposable
    {
        public GlobalValues Global;
        public IState State;

        private object lockObject = new object();

        public GestureMachine(IEnumerable<GestureDefinition> gestureDef)
        {
            this.Global = new GlobalValues();
            this.State = new State0(Global, gestureDef);
        }

        public bool Input(Def.Event.IEvent evnt, Point point)
        {
            lock (lockObject)
            {
                var res = State.Input(evnt, point);   
                if (State.GetType() != res.NextState.GetType())
                {
                    Debug.Print("The state of GestureMachine was changed: {0} -> {1}", State.GetType().Name, res.NextState.GetType().Name);
                }
                if (res.StrokeWatcher.IsResetRequested)
                {
                    Global.ResetStrokeWatcher();
                }
                State = res.NextState;
                return res.Event.IsConsumed;
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
