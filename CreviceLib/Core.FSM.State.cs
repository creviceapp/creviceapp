using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using Crevice.Core.Types;

    public interface IState
    {
        (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt);
        IState Timeout();
        IState Reset();
    }

    public abstract class State : IState
    {
        public virtual (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            return (EventIsConsumed: false, NextState: this);
        }

        public virtual IState Timeout()
        {
            return this;
        }

        public virtual IState Reset()
        {
            return this;
        }
    }
}
