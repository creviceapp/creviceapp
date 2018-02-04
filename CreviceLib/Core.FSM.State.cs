﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Stroke;

    public interface IState
    {
        (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt);
        IState Timeout();
        IState Reset();
    }

    public abstract class State : IState
    {
        public int Depth { get; }

        public State(int depth)
        {
            Depth = depth;
        }

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

        public static bool CanTransition<TExecContext>(
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d =>
                    d.DoExecutors.Any() ||
                    d.StrokeElements.Any(ds => ds.IsFull) ||
                    d.SingleThrowElements.Any(ds => ds.IsFull) ||
                    d.DoubleThrowElements.Any(dd => dd.IsFull));

        public static bool HasPressExecutors<TExecContext>(
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.PressExecutors.Any());

        public static bool HasDoExecutors<TExecContext>(
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.DoExecutors.Any());

        public static bool HasReleaseExecutors<TExecContext>(
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements)
            where TExecContext : ExecutionContext
            => doubleThrowElements.Any(d => d.ReleaseExecutors.Any());
    }
}
