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

    public class StateN<TConfig, TContextManager, TEvalContext, TExecContext>
        : State<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> Machine;

        public readonly TEvalContext Ctx;
        public readonly History<TConfig, TContextManager, TEvalContext, TExecContext> History;
        public readonly IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements;
        public readonly bool CanCancel;
        
        public readonly IReadOnlyCollection<FireEvent> SingleThrowTriggers;
        public bool IsSingleThrowTrigger(PhysicalFireEvent fireEvent)
            => SingleThrowTriggers.Contains(fireEvent) ||
               SingleThrowTriggers.Contains(fireEvent.LogicalNormalized);

        public readonly IReadOnlyCollection<PressEvent> DoubleThrowTriggers;
        public bool IsDoubleThrowTrigger(PhysicalPressEvent pressEvent)
            => DoubleThrowTriggers.Contains(pressEvent) ||
               DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized);

        public readonly IReadOnlyCollection<PhysicalReleaseEvent> EndTriggers;
        public bool IsEndTrigger(PhysicalReleaseEvent releaseEvent)
            => EndTriggers.Contains(releaseEvent);

        public readonly IReadOnlyCollection<PhysicalReleaseEvent> AbnormalEndTriggers;
        public bool IsAbnormalEndTrigger(PhysicalReleaseEvent releaseEvent)
            => AbnormalEndTriggers.Contains(releaseEvent);

        public StateN(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            TEvalContext ctx,
            History<TConfig, TContextManager, TEvalContext, TExecContext> history,
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements,
            int depth,
            bool canCancel = true)
            : base(depth)
        {
            Machine = machine;
            Ctx = ctx;
            History = history;
            DoubleThrowElements = doubleThrowElements;
            CanCancel = canCancel;

            // Caches.
            SingleThrowTriggers = GetSingleThrowTriggers(DoubleThrowElements);
            DoubleThrowTriggers = GetDoubleThrowTriggers(DoubleThrowElements);
            EndTriggers = GetEndTriggers(History.Records);
            AbnormalEndTriggers = GetAbnormalEndTriggers(History.Records);
        }

        public override Result<TConfig, TContextManager, TEvalContext, TExecContext> Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent && IsSingleThrowTrigger(fireEvent))
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
                        depth: Depth,
                        canCancel: false);
                    return Result.Create(eventIsConsumed: true, nextState: notCancellableCopyState);
                }
            }
            else if (evnt is PhysicalPressEvent pressEvent && IsDoubleThrowTrigger(pressEvent))
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
                            History.CreateNext(pressEvent.Opposition, this),
                            doubleThrowElements,
                            depth: Depth + 1,
                            canCancel: CanCancel);
                        return Result.Create(eventIsConsumed: true, nextState: nextState);
                    }
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent)
            {
                if (IsNormalEndTrigger(releaseEvent))
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
                    else if (CanCancel)
                    {
                        Machine.CallbackManager.OnGestureCancelled(this);
                    }
                    return Result.Create(eventIsConsumed: true, nextState: LastState);
                }
                else if (IsAbnormalEndTrigger(releaseEvent))
                {
                    var queryResult = History.Query(releaseEvent);
                    Machine.invalidEvents.IgnoreNext(queryResult.SkippedReleaseEvents);
                    return Result.Create(eventIsConsumed: true, nextState: queryResult.FoundState);
                }
                else if (IsDoubleThrowTrigger(releaseEvent.Opposition))
                {
                    var doubleThrowElements = GetDoubleThrowElements(releaseEvent.Opposition);

                    // The following condition, will be true when the opposition of the DoubleThrowTrigger 
                    // of next `StateN` is given as a input, and when next `StateN` has press or release executors.
                    if (HasPressExecutors(doubleThrowElements) ||
                        HasReleaseExecutors(doubleThrowElements))
                    {
                        // If the next provisional `StateN` does not have any do executor, this `StateN` does not transit 
                        // to the next `StateN`, then release executers of it should be executed here.
                        // And if the release event comes firstly inverse to the expectation, in this pattern, 
                        // it should also be executed.
                        Machine.ContextManager.ExecuteReleaseExecutors(Ctx, doubleThrowElements);
                        return Result.Create(eventIsConsumed: true, nextState: this);
                    }
                }
            }
            return base.Input(evnt);
        }

        public override State<TConfig, TContextManager, TEvalContext, TExecContext> Timeout()
        {
            if (!HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && CanCancel)
            {
                return LastState;
            }
            return this;
        }

        public override State<TConfig, TContextManager, TEvalContext, TExecContext> Reset()
        {
            Machine.invalidEvents.IgnoreNext(NormalEndTrigger);
            Machine.ContextManager.ExecuteReleaseExecutors(Ctx, DoubleThrowElements);
            return LastState;
        }

        public PhysicalReleaseEvent NormalEndTrigger => History.Records.Last().ReleaseEvent;

        public bool IsNormalEndTrigger(PhysicalReleaseEvent releaseEvent)
            => NormalEndTrigger == releaseEvent;

        public State<TConfig, TContextManager, TEvalContext, TExecContext> LastState => History.Records.Last().State;

        public new bool HasPressExecutors => HasPressExecutors(DoubleThrowElements);

        public new bool HasDoExecutors => HasDoExecutors(DoubleThrowElements);

        public new bool HasReleaseExecutors => HasReleaseExecutors(DoubleThrowElements);

        public IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> 
            CreateHistory(
                IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> history,
                PhysicalPressEvent pressEvent,
                State<TConfig, TContextManager, TEvalContext, TExecContext> state)
        {
            var newHistory = history.ToList();
            newHistory.Add(HistoryRecord.Create(pressEvent.Opposition, state));
            return newHistory;
        }

        private static IReadOnlyCollection<PhysicalReleaseEvent> 
            GetEndTriggers(IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> history)
            => new HashSet<PhysicalReleaseEvent>(from h in history select h.ReleaseEvent);

        private static IReadOnlyCollection<PhysicalReleaseEvent> 
            GetAbnormalEndTriggers(IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> history)
            => new HashSet<PhysicalReleaseEvent>(from h in history.Reverse().Skip(1) select h.ReleaseEvent);

        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> 
            GetDoubleThrowElements(PhysicalPressEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull && (dd.Trigger.Equals(triggerEvent) ||
                                        dd.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select dd))
                .Aggregate(new List<IReadOnlyDoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> 
            GetStrokeElements(IReadOnlyList<StrokeDirection> strokes)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from ds in d.StrokeElements
                    where ds.IsFull && ds.Strokes.SequenceEqual(strokes)
                    select ds))
                .Aggregate(new List<IReadOnlyStrokeElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> 
            GetSingleThrowElements(PhysicalFireEvent triggerEvent)
            => (from d in DoubleThrowElements
                where d.IsFull
                select (
                    from ds in d.SingleThrowElements
                    where ds.IsFull && (ds.Trigger.Equals(triggerEvent) ||
                                       ds.Trigger.Equals(triggerEvent.LogicalNormalized))
                    select ds))
                .Aggregate(new List<IReadOnlySingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        private static IReadOnlyCollection<FireEvent> 
            GetSingleThrowTriggers(IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from d in doubleThrowElements
                where d.IsFull
                select (
                    from ds in d.SingleThrowElements
                    where ds.IsFull
                    select ds.Trigger))
                .Aggregate(new HashSet<FireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        private static IReadOnlyCollection<PressEvent> 
            GetDoubleThrowTriggers(IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from ds in doubleThrowElements
                where ds.IsFull
                select (
                    from dd in ds.DoubleThrowElements
                    where dd.IsFull
                    select dd.Trigger))
                .Aggregate(new HashSet<PressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }
}
