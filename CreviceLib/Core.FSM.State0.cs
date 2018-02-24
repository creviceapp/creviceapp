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

        public readonly IReadOnlyCollection<FireEvent> SingleThrowTriggers;
        public bool IsSingleThrowTrigger(PhysicalFireEvent fireEvent)
            => SingleThrowTriggers.Contains(fireEvent) ||
               SingleThrowTriggers.Contains(fireEvent.LogicalNormalized);

        public readonly IReadOnlyCollection<PressEvent> DoubleThrowTriggers;
        public bool IsDoubleThrowTrigger(PhysicalPressEvent pressEvent)
            => DoubleThrowTriggers.Contains(pressEvent) ||
               DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized);

        public readonly IReadOnlyCollection<PressEvent> DecomposedTriggers;
        public bool IsDecomposedTrigger(PhysicalPressEvent pressEvent)
            => DecomposedTriggers.Contains(pressEvent) ||
               DecomposedTriggers.Contains(pressEvent.LogicalNormalized);

        public State0(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            : base(depth: 0)
        {
            Machine = machine;
            RootElement = rootElement;

            // Caches.
            SingleThrowTriggers = GetSingleThrowTriggers(RootElement);
            DoubleThrowTriggers = GetDoubleThrowTriggers(RootElement);
            DecomposedTriggers = GetDecomposedTriggers(RootElement);
        }

        public override Result<TConfig, TContextManager, TEvalContext, TExecContext> Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent && IsSingleThrowTrigger(fireEvent))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var singleThrowElements = GetActiveSingleThrowElements(evalContext, fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(evalContext, singleThrowElements);
                    return Result.Create(eventIsConsumed: true, nextState: this);
                }
            }

            else if (evnt is PhysicalPressEvent pressEvent)
            {
                if (IsDoubleThrowTrigger(pressEvent))
                {
                    var evalContext = Machine.ContextManager.CreateEvaluateContext();
                    var doubleThrowElements = GetActiveDoubleThrowElements(evalContext, pressEvent);
                    if (doubleThrowElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(evalContext, doubleThrowElements);
                        var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                            Machine,
                            evalContext,
                            History.Create(pressEvent.Opposition, this),
                            doubleThrowElements,
                            depth: Depth + 1,
                            canCancel: true);
                        return Result.Create(eventIsConsumed: true, nextState: nextState);
                    }
                } 
                else if (IsDecomposedTrigger(pressEvent))
                {
                    var evalContext = Machine.ContextManager.CreateEvaluateContext();
                    var decomposedElements = GetActiveDecomposedElements(evalContext, pressEvent);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecutePressExecutors(evalContext, decomposedElements);
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
                    var evalContext = Machine.ContextManager.CreateEvaluateContext();
                    var decomposedElements = GetActiveDecomposedElements(evalContext, releaseEvent.Opposition);
                    if (decomposedElements.Any())
                    {
                        Machine.ContextManager.ExecuteReleaseExecutors(evalContext, decomposedElements);
                        return Result.Create(eventIsConsumed: true, nextState: this);
                    }
                }
                
            }
            return base.Input(evnt);
        }

        /*
        private static IDictionary<FireEvent, WhenElement<TEvalContext, TExecContext>> 
            GetInverseFireToWhenMap(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => 
        */

        // todo cache the resutl fo when
        protected internal IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> 
            GetActiveDoubleThrowElements(TEvalContext ctx, PhysicalPressEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent)  ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
            .Aggregate(new List<IReadOnlyDoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>>
            GetActiveDecomposedElements(TEvalContext ctx, PhysicalPressEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from d in w.DecomposedElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent) ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
            .Aggregate(new List<IReadOnlyDecomposedElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        protected internal IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> 
            GetActiveSingleThrowElements(TEvalContext ctx, PhysicalFireEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger.Equals(triggerEvent) ||
                                           s.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select s))
                .Aggregate(new List<IReadOnlySingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        private static IReadOnlyCollection<FireEvent> 
            GetSingleThrowTriggers(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull
                    select s.Trigger))
                .Aggregate(new HashSet<FireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        private static IReadOnlyCollection<PressEvent> 
            GetDoubleThrowTriggers(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull
                    select d.Trigger))
                .Aggregate(new HashSet<PressEvent>(), (a, b) => { a.UnionWith(b); return a; });

        private static IReadOnlyCollection<PressEvent>
            GetDecomposedTriggers(IReadOnlyRootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DecomposedElements
                    where d.IsFull
                    select d.Trigger))
                .Aggregate(new HashSet<PressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }
}
