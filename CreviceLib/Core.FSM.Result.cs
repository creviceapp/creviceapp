using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    public struct Result
    {
        public readonly bool EventIsConsumed;
        public readonly State NextState;
        public Result(bool eventIsConsumed, State nextState)
        {
            EventIsConsumed = eventIsConsumed;
            NextState = nextState;
        }
    }
}
