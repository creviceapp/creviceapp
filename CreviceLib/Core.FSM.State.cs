using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;

    public abstract class State
    {
        public int Depth { get; }

        public State(int depth)
        {
            Depth = depth;
        }

        public virtual Result Input(IPhysicalEvent evnt)
        {
            return new Result(eventIsConsumed: false, nextState: this);
        }

        public virtual State Timeout()
        {
            return this;
        }

        public virtual State Reset()
        {
            return this;
        }

        protected static bool CanTransition<TExecContext>(
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d =>
                    d.DoExecutors.Any() ||
                    d.StrokeElements.Any(ds => ds.IsFull) ||
                    d.SingleThrowElements.Any(ds => ds.IsFull) ||
                    d.DoubleThrowElements.Any(dd => dd.IsFull));

        protected static bool HasPressExecutors<TExecContext>(
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.PressExecutors.Any());

        protected static bool HasDoExecutors<TExecContext>(
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.DoExecutors.Any());

        protected static bool HasReleaseExecutors<TExecContext>(
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.ReleaseExecutors.Any());
    }
}
