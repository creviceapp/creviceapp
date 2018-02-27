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

        public readonly IDictionary<FireEvent, IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>>>
            InversedSingleThrowTrigger;
        public readonly IDictionary<PressEvent, IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>>>
            InversedDoubleThrowTrigger;
        public readonly IDictionary<StrokeSequence, IReadOnlyList<IReadOnlyStrokeElement<TExecContext>>>
            InversedStrokeTrigger;
        public readonly IDictionary<PressEvent, IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>>
            InversedDecomposedTrigger;

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
            InversedSingleThrowTrigger = GetInversedSingleThrowTrigger(DoubleThrowElements);
            InversedDoubleThrowTrigger = GetInversedDoubleThrowTrigger(DoubleThrowElements);
            InversedStrokeTrigger = GetInversedStrokeTrigger(DoubleThrowElements);
            InversedDecomposedTrigger = GetInversedDecomposedTrigger(DoubleThrowElements);
            EndTriggers = GetEndTriggers(History.Records);
            AbnormalEndTriggers = GetAbnormalEndTriggers(History.Records);
        }

        public override Result<TConfig, TContextManager, TEvalContext, TExecContext> Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent && IsSingleThrowTrigger(fireEvent))
            {
                var singleThrowElements = GetSingleThrowElementsByTrigger(fireEvent);
                Machine.ContextManager.ExecuteDoExecutors(Ctx, singleThrowElements);
                return Result.Create(eventIsConsumed: true, nextState: ToNonCancellableClone());
            }
            else if (evnt is PhysicalPressEvent pressEvent)
            {
                if (IsRepeatedStartTrigger(pressEvent))
                {
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
                else if (IsDoubleThrowTrigger(pressEvent))
                {
                    var nextDoubleThrowElements = GetDoubleThrowElementsByTrigger(pressEvent);
                    if (HasPressExecutors(nextDoubleThrowElements) ||
                        HasReleaseExecutors(nextDoubleThrowElements))
                    {
                        Machine.ContextManager.ExecutePressExecutors(Ctx, nextDoubleThrowElements);
                        return Result.Create(eventIsConsumed: true, 
                            nextState: CreateNonCancellableNextState(pressEvent, nextDoubleThrowElements));
                    }
                    else if (!CanCancel)
                    {
                        return Result.Create(eventIsConsumed: true, 
                            nextState: CreateNonCancellableNextState(pressEvent, nextDoubleThrowElements));
                    }
                    return Result.Create(eventIsConsumed: true, 
                        nextState: CreateCancellableNextState(pressEvent, nextDoubleThrowElements));
                }
                else if (IsDecomposedTrigger(pressEvent))
                {
                    var decomposedElements = GetDecomposedElementsByTrigger(pressEvent);
                    Machine.ContextManager.ExecutePressExecutors(Ctx, decomposedElements);
                    return Result.Create(eventIsConsumed: true, nextState: ToNonCancellableClone());
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent)
            {
                var oppositeEvent = releaseEvent.Opposition;

                if (IsNormalEndTrigger(releaseEvent))
                {
                    var strokeSequence = Machine.StrokeWatcher.GetStrokeSequence();
                    if (strokeSequence.Any())
                    {
                        if (IsStrokeTrigger(strokeSequence))
                        {
                            var strokeElements = GetStrokeElementsByTrigger(strokeSequence);
                            Machine.ContextManager.ExecuteDoExecutors(Ctx, strokeElements);
                        }
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

                    if (!CanCancel && LastState is StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN)
                    {
                        return Result.Create(eventIsConsumed: true, nextState: stateN.ToNonCancellableClone());
                    }
                    return Result.Create(eventIsConsumed: true, nextState: LastState);
                }
                else if (IsAbnormalEndTrigger(releaseEvent))
                {
                    var queryResult = History.Query(releaseEvent);
                    Machine.invalidEvents.IgnoreNext(queryResult.SkippedReleaseEvents);

                    if (!CanCancel && queryResult.FoundState is StateN<TConfig, TContextManager, TEvalContext, TExecContext> stateN)
                    {
                        return Result.Create(eventIsConsumed: true, nextState: stateN.ToNonCancellableClone());
                    }
                    return Result.Create(eventIsConsumed: true, nextState: queryResult.FoundState);
                }
                else if (IsDoubleThrowTrigger(oppositeEvent))
                {
                    var doubleThrowElements = GetDoubleThrowElementsByTrigger(oppositeEvent);
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
                else if (IsDecomposedTrigger(oppositeEvent))
                {
                    var decomposedElements = GetDecomposedElementsByTrigger(oppositeEvent);
                    Machine.ContextManager.ExecuteReleaseExecutors(Ctx, decomposedElements);
                    return Result.Create(eventIsConsumed: true, nextState: ToNonCancellableClone());
                }
            }
            return base.Input(evnt);
        }

        public override State<TConfig, TContextManager, TEvalContext, TExecContext> Timeout()
        {
            if (CanCancel && !HasPressExecutors && !HasDoExecutors && !HasReleaseExecutors && 
                !Machine.StrokeWatcher.GetStorkes().Any())
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

        private StateN<TConfig, TContextManager, TEvalContext, TExecContext> ToNonCancellableClone()
            => new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                    Machine, Ctx, History, DoubleThrowElements, depth: Depth, canCancel: false);

        private StateN<TConfig, TContextManager, TEvalContext, TExecContext> CreateCancellableNextState(
            PhysicalPressEvent pressEvent,
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                    Machine, Ctx, History.CreateNext(pressEvent.Opposition, this), 
                    doubleThrowElements, depth: Depth + 1, canCancel: true);

        private StateN<TConfig, TContextManager, TEvalContext, TExecContext> CreateNonCancellableNextState(
            PhysicalPressEvent pressEvent,
            IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                    Machine, Ctx, History.CreateNext(pressEvent.Opposition, ToNonCancellableClone()),
                    doubleThrowElements, depth: Depth + 1, canCancel: false);

        public bool IsRepeatedStartTrigger(PhysicalPressEvent pressEvent)
            => EndTriggers.Contains(pressEvent.Opposition);

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

        public bool IsSingleThrowTrigger(PhysicalFireEvent fireEvent)
            => InversedSingleThrowTrigger.Keys.Contains(fireEvent) ||
               InversedSingleThrowTrigger.Keys.Contains(fireEvent.LogicalNormalized);

        public bool IsDoubleThrowTrigger(PhysicalPressEvent pressEvent)
            => InversedDoubleThrowTrigger.Keys.Contains(pressEvent) ||
               InversedDoubleThrowTrigger.Keys.Contains(pressEvent.LogicalNormalized);

        public bool IsDecomposedTrigger(PhysicalPressEvent pressEvent)
            => InversedDecomposedTrigger.Keys.Contains(pressEvent) ||
               InversedDecomposedTrigger.Keys.Contains(pressEvent.LogicalNormalized);

        public bool IsStrokeTrigger(StrokeSequence strokes)
            => InversedStrokeTrigger.Keys.Contains(strokes);

        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> 
            GetSingleThrowElementsByTrigger(PhysicalFireEvent fireEvent)
        {
            if (InversedSingleThrowTrigger.TryGetValue(fireEvent, out var doubleThrowElements))
            {
                return doubleThrowElements;
            }
            return InversedSingleThrowTrigger[fireEvent.LogicalNormalized];
        }

        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> 
            GetDoubleThrowElementsByTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDoubleThrowTrigger.TryGetValue(pressEvent, out var doubleThrowElements))
            {
                return doubleThrowElements;
            }
            return InversedDoubleThrowTrigger[pressEvent.LogicalNormalized];
        }

        public IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> 
            GetStrokeElementsByTrigger(StrokeSequence strokeSequence)
            => InversedStrokeTrigger[strokeSequence];

        public IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>> 
            GetDecomposedElementsByTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDecomposedTrigger.TryGetValue(pressEvent, out var doubleThrowElements))
            {
                return doubleThrowElements;
            }
            return InversedDecomposedTrigger[pressEvent.LogicalNormalized];
        }

        private static IDictionary<FireEvent, IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>>>
            GetInversedSingleThrowTrigger(
                IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from d in doubleThrowElements
                where d.IsFull
                select (
                    from s in d.SingleThrowElements
                    where s.IsFull
                    select Tuple.Create(s, s.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlySingleThrowElement<TExecContext>, FireEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>>);

        private static IDictionary<PressEvent, IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>>>
            GetInversedDoubleThrowTrigger(
                IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from d in doubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DoubleThrowElements
                    where dd.IsFull
                    select Tuple.Create(dd, dd.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlyDoubleThrowElement<TExecContext>, PressEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>>);

        private static IDictionary<StrokeSequence, IReadOnlyList<IReadOnlyStrokeElement<TExecContext>>>
            GetInversedStrokeTrigger(
                IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from d in doubleThrowElements
                where d.IsFull
                select (
                    from s in d.StrokeElements
                    where s.IsFull
                    select Tuple.Create(s, s.Strokes)))
                .Aggregate(new List<Tuple<IReadOnlyStrokeElement<TExecContext>, StrokeSequence>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyStrokeElement<TExecContext>>);

        private static IDictionary<PressEvent, IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>>
            GetInversedDecomposedTrigger(
                IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
            => (from d in doubleThrowElements
                where d.IsFull
                select (
                    from dd in d.DecomposedElements
                    where dd.IsFull
                    select Tuple.Create(dd, dd.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlyDecomposedElement<TExecContext>, PressEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>);
    }
}
