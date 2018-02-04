using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Stroke;

    public class StateN<TConfig, TContextManager, TEvalContext, TExecContext> : State
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> Machine;
        public readonly TEvalContext Ctx;
        public readonly IReadOnlyList<(PhysicalReleaseEvent, IState)> History;
        public readonly IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements;
        public readonly bool CanCancel;

        public StateN(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            TEvalContext ctx,
            IReadOnlyList<(PhysicalReleaseEvent, IState)> history,
            IReadOnlyList<DoubleThrowElement<TExecContext>> doubleThrowElements,
            bool canCancel = true)
        {
            Machine = machine;
            Ctx = ctx;
            History = history;
            DoubleThrowElements = doubleThrowElements;
            CanCancel = canCancel;
        }

        public override (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent &&
                    (SingleThrowTriggers.Contains(fireEvent) ||
                     SingleThrowTriggers.Contains(fireEvent.LogicalNormalized)))
            {
                var singleThrowElements = GetSingleThrowElements(fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(Ctx, singleThrowElements);
                    var notCancellableCopyState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                        Machine,
                        Ctx,
                        History,
                        DoubleThrowElements,
                        canCancel: false);
                    return (EventIsConsumed: true, NextState: notCancellableCopyState);
                }
            }
            else if (evnt is PhysicalPressEvent pressEvent &&
                        (DoubleThrowTriggers.Contains(pressEvent) ||
                         DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized)))
            {
                var doubleThrowElements = GetDoubleThrowElements(pressEvent);
                if (doubleThrowElements.Any())
                {
                    Machine.ContextManager.ExecutePressExecutors(Ctx, doubleThrowElements);
                    if (CanTransition(doubleThrowElements))
                    {
                        var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                            Machine,
                            Ctx,
                            CreateHistory(History, pressEvent, this),
                            doubleThrowElements,
                            canCancel: CanCancel);
                        return (EventIsConsumed: true, NextState: nextState);
                    }
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent)
            {
                if (releaseEvent == NormalEndTrigger)
                {
                    var strokes = Machine.StrokeWatcher.GetStorkes();
                    if (strokes.Any())
                    {
                        var strokeElements = GetStrokeElements(strokes);
                        Machine.ContextManager.ExecuteDoExecutors(Ctx, strokeElements);
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
                    }
                    else if (HasPressExecutors || HasDoExecutors || HasReleaseExecutors)
                    {
                        Machine.ContextManager.ExecuteDoExecutors(Ctx, DoubleThrowElements);
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
                    }
                    else if (/* !HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && */
                             CanCancel)
                    {
                        Machine.OnGestureCancelled(new GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext>.GestureCancelledEventArgs(this));
                    }
                    return (EventIsConsumed: true, NextState: LastState);
                }
                else if (AbnormalEndTriggers.Contains(releaseEvent))
                {
                    var (pastState, skippedReleaseEvents) = FindStateFromHistory(releaseEvent);
                    Machine.invalidReleaseEvents.IgnoreNext(skippedReleaseEvents);
                    return (EventIsConsumed: true, NextState: pastState);
                }
                else if (DoubleThrowTriggers.Contains(releaseEvent.Opposition) ||
                         DoubleThrowTriggers.Contains(releaseEvent.Opposition.LogicalNormalized))
                {
                    var doubleThrowElements = GetDoubleThrowElements(releaseEvent.Opposition);
                    if (HasPressExecutors(doubleThrowElements) ||
                        HasReleaseExecutors(doubleThrowElements))
                    {
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, doubleThrowElements);
                        return (EventIsConsumed: true, NextState: this);
                    }
                }
            }
            return base.Input(evnt);
        }

        public override IState Timeout()
        {
            if (!HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && CanCancel)
            {
                return LastState;
            }
            return this;
        }

        public override IState Reset()
        {
            Machine.invalidReleaseEvents.IgnoreNext(NormalEndTrigger);
            Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
            return LastState;
        }

        public PhysicalReleaseEvent NormalEndTrigger => History.Last().Item1;

        public IState LastState => History.Last().Item2;

        public bool HasPressExecutors => HasPressExecutors(DoubleThrowElements);

        public bool HasDoExecutors => HasDoExecutors(DoubleThrowElements);

        public bool HasReleaseExecutors => HasReleaseExecutors(DoubleThrowElements);

        public IReadOnlyList<(PhysicalReleaseEvent, IState)> CreateHistory(
            IReadOnlyList<(PhysicalReleaseEvent, IState)> history,
            PhysicalPressEvent pressEvent,
            IState state)
        {
            var newHistory = history.ToList();
            newHistory.Add((pressEvent.Opposition, state));
            return newHistory;
        }

        public IReadOnlyCollection<PhysicalReleaseEvent> AbnormalEndTriggers
            => new HashSet<PhysicalReleaseEvent>(from h in History.Reverse().Skip(1) select h.Item1);

        public (IState, IReadOnlyList<PhysicalReleaseEvent>) FindStateFromHistory(PhysicalReleaseEvent releaseEvent)
        {
            var nextHistory = History.TakeWhile(t => t.Item1 != releaseEvent);
            var foundState = History[nextHistory.Count()].Item2;
            var skippedReleaseEvents = History.Skip(nextHistory.Count()).Select(t => t.Item1).ToList();
            return (foundState, skippedReleaseEvents);
        }

        public IReadOnlyList<DoubleThrowElement<TExecContext>> GetDoubleThrowElements(PhysicalPressEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && (dd.Trigger.Equals(triggerEvent) ||
                                        dd.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select dd))
                .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<StrokeElement<TExecContext>> GetStrokeElements(IReadOnlyList<StrokeDirection> strokes)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from ds in d.StrokeElements
                    where ds.IsFull && ds.Strokes.SequenceEqual(strokes)
                    select ds))
                .Aggregate(new List<StrokeElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<SingleThrowElement<TExecContext>> GetSingleThrowElements(PhysicalFireEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from ds in d.SingleThrowElements
                    where ds.IsFull && (ds.Trigger.Equals(triggerEvent) ||
                                       ds.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select ds))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        // Todo: add Get as the prefix and cache it
        public IReadOnlyCollection<FireEvent> SingleThrowTriggers
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from ds in d.SingleThrowElements
                    where ds.IsFull
                    select ds.Trigger))
                .Aggregate(new HashSet<FireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        public IReadOnlyCollection<PressEvent> DoubleThrowTriggers
            => (from ds in DoubleThrowElements
                where ds.IsFull
                select (
                    from dd in ds.DoubleThrowElements
                    where dd.IsFull
                    select dd.Trigger))
                .Aggregate(new HashSet<PressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }
}
