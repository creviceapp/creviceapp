using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
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

        public class StrokeWatcherResult
        {
            public readonly bool IsResetRequested;
            public StrokeWatcherResult(bool resetRequested)
            {
                IsResetRequested = resetRequested;
            }
        }

        public EventResult Event;
        public StrokeWatcherResult StrokeWatcher;
        public IState NextState { get; private set; }

        private Result(bool consumed, IState nextState, bool resetStrokeWatcher)
        {
            this.Event = new EventResult(consumed);
            this.StrokeWatcher = new StrokeWatcherResult(resetStrokeWatcher);
            this.NextState = nextState;
        }

        public static Result EventIsConsumed(IState nextState, bool resetStrokeWatcher = false)
        {
            return new Result(true, nextState, resetStrokeWatcher);
        }

        public static Result EventIsRemaining(IState nextState, bool resetStrokeWatcher = false)
        {
            return new Result(false, nextState, resetStrokeWatcher);
        }
    }
}
