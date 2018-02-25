using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;

    public class State0<TConfig, TContextManager, TEvalContext, TExecContext> 
        : State<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> Machine;
        public readonly IReadOnlyRootElement<TEvalContext, TExecContext> RootElement;

        public readonly IDictionary<FireEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            InversedSingleThrowTrigger;
        public readonly IDictionary<PressEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            InversedDoubleThrowTrigger;
        public readonly IDictionary<PressEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            InversedDecomposedTrigger;

        public State0(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            : base(depth: 0)
        {
            Machine = machine;
            RootElement = rootElement;

            // Caches.
            InversedSingleThrowTrigger = GetInversedSingleThrowTrigger(RootElement);
            InversedDoubleThrowTrigger = GetInversedDoubleThrowTrigger(RootElement);
            InversedDecomposedTrigger = GetInversedDecomposedTrigger(RootElement);
        }

        public override Result<TConfig, TContextManager, TEvalContext, TExecContext> Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent && IsSingleThrowTrigger(fireEvent))
            {
                var whenElements = GetWhenElementsBySingleThrowTrigger(fireEvent);
                var evaluationContext = Machine.ContextManager.CreateEvaluateContext();
                var singleThrowElements = GetActiveSingleThrowElements(whenElements, evaluationContext, fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(evaluationContext, singleThrowElements);
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
            }

            else if (evnt is PhysicalPressEvent pressEvent)
            {
                if (IsDoubleThrowTrigger(pressEvent))
                {
                    var whenElements = GetWhenElementsByDoubleThrowTrigger(pressEvent);
                    var evaluationContext = Machine.ContextManager.CreateEvaluateContext();
                    var doubleThrowElements = GetActiveDoubleThrowElements(whenElements, evaluationContext, pressEvent);
                    if (doubleThrowElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(evaluationContext, doubleThrowElements);
                        var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                            Machine,
                            evaluationContext,
                            History.Create(pressEvent.Opposition, this),
                            doubleThrowElements,
                            depth: Depth + 1,
                            canCancel: true);
                        return Result.Create(eventIsConsumed: true, nextState: nextState);
                    }
                } 
                else if (IsDecomposedTrigger(pressEvent))
                {
                    var whenElements = GetWhenElementsByDecomposedTrigger(pressEvent);
                    var evaluationContext = Machine.ContextManager.CreateEvaluateContext();
                    var decomposedElements = GetActiveDecomposedElements(whenElements, evaluationContext, pressEvent);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(evaluationContext, decomposedElements);
                        return Result.Create(eventIsConsumed: true, nextState: this);
                    }
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent)
            {
                if (IsDoubleThrowTrigger(releaseEvent.Opposition))
                {
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
                else if (IsDecomposedTrigger(releaseEvent.Opposition))
                {
                    var whenElements = GetWhenElementsByDecomposedTrigger(releaseEvent.Opposition);
                    var evaluationContext = Machine.ContextManager.CreateEvaluateContext();
                    var decomposedElements = GetActiveDecomposedElements(whenElements, evaluationContext, releaseEvent.Opposition);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecuteReleaseExecutors(evaluationContext, decomposedElements);
                        return Result.Create(eventIsConsumed: true, nextState: this);
                    }
                }
            }
            return base.Input(evnt);
        }

        public bool IsSingleThrowTrigger(PhysicalFireEvent fireEvent)
            => InversedSingleThrowTrigger.Keys.Contains(fireEvent) ||
               InversedSingleThrowTrigger.Keys.Contains(fireEvent.LogicalNormalized);

        public bool IsDoubleThrowTrigger(PhysicalPressEvent pressEvent)
            => InversedDoubleThrowTrigger.Keys.Contains(pressEvent) ||
               InversedDoubleThrowTrigger.Keys.Contains(pressEvent.LogicalNormalized);

        public bool IsDecomposedTrigger(PhysicalPressEvent pressEvent)
            => InversedDecomposedTrigger.Keys.Contains(pressEvent) ||
               InversedDecomposedTrigger.Keys.Contains(pressEvent.LogicalNormalized);

        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsBySingleThrowTrigger(PhysicalFireEvent fireEvent)
        {
            if (InversedSingleThrowTrigger.TryGetValue(fireEvent, out var whenElements))
            {
                return whenElements;
            }
            return InversedSingleThrowTrigger[fireEvent.LogicalNormalized];
        }

        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsByDoubleThrowTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDoubleThrowTrigger.TryGetValue(pressEvent, out var whenElements))
            {
                return whenElements;
            }
            return InversedDoubleThrowTrigger[pressEvent.LogicalNormalized];
        }

        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsByDecomposedTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDecomposedTrigger.TryGetValue(pressEvent, out var whenElements))
            {
                return whenElements;
            }
            return InversedDecomposedTrigger[pressEvent.LogicalNormalized];
        }

        protected internal IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> 
            GetActiveDoubleThrowElements(
                IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements,
                TEvalContext ctx, 
                PhysicalPressEvent triggerEvent)
            => (from w in whenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent)  ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
                .Aggregate(new List<IReadOnlyDoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>
            GetActiveDecomposedElements(
                IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements,
                TEvalContext ctx, 
                PhysicalPressEvent triggerEvent)
            => (from w in whenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from d in w.DecomposedElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent) ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
                .Aggregate(new List<IReadOnlyDecomposedElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> 
            GetActiveSingleThrowElements(
                IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements, 
                TEvalContext ctx, 
                PhysicalFireEvent triggerEvent)
            => (from w in whenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger.Equals(triggerEvent) ||
                                           s.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select s))
                .Aggregate(new List<IReadOnlySingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        internal static IDictionary<FireEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            GetInversedSingleThrowTrigger(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull
                    select Tuple.Create(w, s.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlyWhenElement<TEvalContext, TExecContext>, FireEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>);

        internal static IDictionary<PressEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            GetInversedDoubleThrowTrigger(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull
                    select Tuple.Create(w, d.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlyWhenElement<TEvalContext, TExecContext>, PressEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>);

        internal static IDictionary<PressEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            GetInversedDecomposedTrigger(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DecomposedElements
                    where d.IsFull
                    select Tuple.Create(w, d.Trigger)))
                .Aggregate(new List<Tuple<IReadOnlyWhenElement<TEvalContext, TExecContext>, PressEvent>>(), (a, b) =>
                {
                    a.AddRange(b); return a;
                })
                .ToLookup(t => t.Item2, t => t.Item1)
                .ToDictionary(x => x.Key, x => x.Distinct().ToList() as IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>);
    }
}
