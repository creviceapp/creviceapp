using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.FSM
{
    #region State Definition
    // S0
    //
    // Inital state.

    // S1
    //
    // The state holding primary double action mouse button and the set action of the button is restorable.

    // S2
    //
    // The state holding primary double action mouse button and the set action of the button is not restorable.

    // S3
    //
    // The state holding primary and secondary double action mouse buttons.
    #endregion
        
    using GestureActionContext;

    public interface IState
    {
        Result Input(Def.Event.IEvent evnt, System.Drawing.Point point);
        IState Reset();
    }
    
    public abstract class State
        : IState
    {
        protected internal readonly StateGlobal Global;

        public State(StateGlobal Global)
        {
            this.Global = Global;
        }

        public virtual Result Input(Def.Event.IEvent evnt, System.Drawing.Point point)
        {
            return Result.EventIsRemained(nextState: this);
        }

        public virtual IState Reset()
        {
            throw new InvalidOperationException();
        }

        // TOdo move to Global

        protected internal void ExecuteUserBeforeFuncInBackground(
            ActionContext ctx,
            IEnumerable<IBeforeExecutable> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteBeforeFunc(ctx);
                }
            });
        }

        protected internal void ExecuteUserDoFuncInBackground(
            ActionContext ctx,
            IEnumerable<IDoExecutable> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteDoFunc(ctx);
                }
            });
        }

        protected internal void ExecuteUserAfterFuncInBackground(
            ActionContext ctx,
            IEnumerable<IAfterExecutable> gestureDef)
        {
            Global.UserActionTaskFactory.StartNew(() => {
                foreach (var gDef in gestureDef)
                {
                    gDef.ExecuteAfterFunc(ctx);
                }
            });
        }

        protected internal void ExecuteInBackground(
            Action action)
        {
            Global.UserActionTaskFactory.StartNew(action);
        }

        // Check whether given event must be ignored or not.
        // Return true if given event is in the ignore list, and remove it from the list.
        // Return false if the pair of given event is in the ignore list, and remove it from the list.
        // Otherwise return false.
        protected internal bool MustBeIgnored(Def.Event.IEvent evnt)
        {
            if (evnt is Def.Event.IDoubleActionRelease)
            {
                var ev = evnt as Def.Event.IDoubleActionRelease;
                if (Global.IgnoreNext.Contains(ev))
                {
                    Global.IgnoreNext.Remove(ev);
                    return true;
                }
            }
            else if (evnt is Def.Event.IDoubleActionSet)
            {
                var ev = evnt as Def.Event.IDoubleActionSet;
                var p = ev.GetPair();
                if (Global.IgnoreNext.Contains(p))
                {
                    Global.IgnoreNext.Remove(p);
                    return false;
                }
            }
            return false;
        }

        protected internal void IgnoreNext(Def.Event.IDoubleActionRelease evnt)
        {
            Verbose.Print("IgnoreNext flag is set for {0}. This event will be ignored next time.", evnt.GetType().Name);
            Global.IgnoreNext.Add(evnt);
        }
    }
}
