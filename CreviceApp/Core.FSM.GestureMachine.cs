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
        public StateGlobal Global { get; private set; }
        public IState State { get; private set; }
        public IEnumerable<GestureDefinition> GestureDefinition { get; private set; }

        private object lockObject = new object();

        public GestureMachine(Config.UserConfig userConfig, IEnumerable<GestureDefinition> gestureDef)
        {
            this.Global = new StateGlobal(userConfig);
            this.State = new State0(Global, gestureDef);
            this.GestureDefinition = gestureDef;
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
