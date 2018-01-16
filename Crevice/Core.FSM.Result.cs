using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.FSM
{
    public class Result
    {
        public class EventResult
        {
            public readonly bool IsConsumed;
            public EventResult(bool consumed)
            {
                IsConsumed = consumed;
            }
        }

        public EventResult Event;
        public IState NextState { get; private set; }

        private Result(bool consumed, IState nextState)
        {
            this.Event = new EventResult(consumed);
            this.NextState = nextState;
        }

        public static Result EventIsConsumed(IState nextState)
        {
            return new Result(true, nextState);
        }

        public static Result EventIsRemained(IState nextState)
        {
            return new Result(false, nextState);
        }
    }
}
