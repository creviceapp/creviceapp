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
                var ctx = Machine.ContextManager.CreateEvaluateContext();
                var whenElements = FilterActiveWhenElements(GetWhenElementsBySingleThrowTrigger(fireEvent), ctx);
                var singleThrowElements = GetSingleThrowElements(whenElements, fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(ctx, singleThrowElements);
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
            }

            else if (evnt is PhysicalPressEvent pressEvent)
            {
                if (IsDoubleThrowTrigger(pressEvent))
                {
                    var ctx = Machine.ContextManager.CreateEvaluateContext();
                    var whenElements = FilterActiveWhenElements(GetWhenElementsByDoubleThrowTrigger(pressEvent), ctx);
                    var doubleThrowElements = GetDoubleThrowElements(whenElements, pressEvent);
                    if (doubleThrowElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(ctx, doubleThrowElements);
                        var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                            Machine,
                            ctx,
                            History.Create(pressEvent.Opposition, this),
                            doubleThrowElements,
                            depth: Depth + 1,
                            canCancel: true);
                        return Result.Create(eventIsConsumed: true, nextState: nextState);
                    }
                } 
                else if (IsDecomposedTrigger(pressEvent))
                {
                    var ctx = Machine.ContextManager.CreateEvaluateContext();
                    var whenElements = FilterActiveWhenElements(GetWhenElementsByDecomposedTrigger(pressEvent), ctx);
                    var decomposedElements = GetDecomposedElements(whenElements, pressEvent);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(ctx, decomposedElements);
                        return Result.Create(eventIsConsumed: true, nextState: this);
                    }
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent)
            {
                var oppositeEvent = releaseEvent.Opposition;

                if (IsDoubleThrowTrigger(oppositeEvent) || IsDecomposedTrigger(oppositeEvent))
                {
                    var ctx = Machine.ContextManager.CreateEvaluateContext();
                    var whenElements = FilterActiveWhenElements(
                        GetWhenElementsByDoubleThrowTrigger(oppositeEvent)
                            .Union(GetWhenElementsByDecomposedTrigger(oppositeEvent)), ctx);
                    
                    var doubleThrowElements = GetDoubleThrowElements(whenElements, oppositeEvent);
                    var decomposedElements = GetDecomposedElements(whenElements, oppositeEvent);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecuteReleaseExecutors(ctx, decomposedElements);
                    }
                    if (doubleThrowElements.Any() || decomposedElements.Any())
                    {
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


        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> FilterActiveWhenElements(
                IEnumerable<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements,
                TEvalContext ctx)
            => whenElements.Where(w => Machine.ContextManager.Evaluate(ctx, w)).ToList();
        
        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsBySingleThrowTrigger(PhysicalFireEvent fireEvent)
        {
            if (InversedSingleThrowTrigger.TryGetValue(fireEvent, out var a))
            {
                return a;
            }
            else if (InversedSingleThrowTrigger.TryGetValue(fireEvent.LogicalNormalized, out var b))
            {
                return b;
            }
            return new List<IReadOnlyWhenElement<TEvalContext, TExecContext>>();
        }

        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsByDoubleThrowTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDoubleThrowTrigger.TryGetValue(pressEvent, out var a))
            {
                return a;
            }
            else if (InversedDoubleThrowTrigger.TryGetValue(pressEvent.LogicalNormalized, out var b))
            {
                return b;
            }
            return new List<IReadOnlyWhenElement<TEvalContext, TExecContext>>();
        }

        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> GetWhenElementsByDecomposedTrigger(PhysicalPressEvent pressEvent)
        {
            if (InversedDecomposedTrigger.TryGetValue(pressEvent, out var a))
            {
                return a;
            }
            else if (InversedDecomposedTrigger.TryGetValue(pressEvent.LogicalNormalized, out var b))
            {
                return b;
            }
            return new List<IReadOnlyWhenElement<TEvalContext, TExecContext>>();
        }

        protected internal IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> 
            GetDoubleThrowElements(
                IEnumerable<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements,
                PhysicalPressEvent triggerEvent)
            => (from w in whenElements
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent)  ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
                .Aggregate(new List<IReadOnlyDoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>
            GetDecomposedElements(
                IEnumerable<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements,
                PhysicalPressEvent triggerEvent)
            => (from w in whenElements
                select (from d in w.DecomposedElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent) ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
                .Aggregate(new List<IReadOnlyDecomposedElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> 
            GetSingleThrowElements(
                IEnumerable<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements, 
                PhysicalFireEvent triggerEvent)
            => (from w in whenElements
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger.Equals(triggerEvent) ||
                                           s.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select s))
                .Aggregate(new List<IReadOnlySingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        internal static IDictionary<FireEvent, IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>>>
            GetInversedSingleThrowTrigger(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                select (from s in w.SingleThrowElements
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
                select (from d in w.DoubleThrowElements
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
                select (from d in w.DecomposedElements
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
