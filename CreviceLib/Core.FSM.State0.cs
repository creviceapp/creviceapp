using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;

    public class State0<TConfig, TContextManager, TEvalContext, TExecContext> : State
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> Machine;
        public readonly RootElement<TEvalContext, TExecContext> RootElement;

        public readonly IReadOnlyCollection<FireEvent> SingleThrowTriggers;
        public bool IsSingleThrowTrigger(PhysicalFireEvent fireEvent)
            => SingleThrowTriggers.Contains(fireEvent) ||
               SingleThrowTriggers.Contains(fireEvent.LogicalNormalized);

        public readonly IReadOnlyCollection<PressEvent> DoubleThrowTriggers;
        public bool IsDoubleThrowTrigger(PhysicalPressEvent pressEvent)
            => DoubleThrowTriggers.Contains(pressEvent) ||
               DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized);

        public State0(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            RootElement<TEvalContext, TExecContext> rootElement)
            : base(depth: 0)
        {
            Machine = machine;
            RootElement = rootElement;

            // Caches.
            SingleThrowTriggers = GetSingleThrowTriggers(RootElement);
            DoubleThrowTriggers = GetDoubleThrowTriggers(RootElement);
        }

        public override (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            if (evnt is PhysicalFireEvent fireEvent && IsSingleThrowTrigger(fireEvent))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var singleThrowElements = GetActiveSingleThrowElements(evalContext, fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(evalContext, singleThrowElements);
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            else if (evnt is PhysicalPressEvent pressEvent && IsDoubleThrowTrigger(pressEvent))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var doubleThrowElements = GetActiveDoubleThrowElements(evalContext, pressEvent);
                if (doubleThrowElements.Any())
                {
                    Machine.ContextManager.ExecutePressExecutors(evalContext, doubleThrowElements);
                    if (CanTransition(doubleThrowElements))
                    {
                        var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                            Machine,
                            evalContext,
                            CreateHistory(pressEvent),
                            doubleThrowElements,
                            depth: Depth + 1,
                            canCancel: true);
                        return (EventIsConsumed: true, NextState: nextState);
                    }
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            else if (evnt is PhysicalReleaseEvent releaseEvent && IsDoubleThrowTrigger(releaseEvent.Opposition))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var doubleThrowElements = GetActiveDoubleThrowElements(evalContext, releaseEvent.Opposition);
                
                if (HasPressExecutors(doubleThrowElements) || HasReleaseExecutors(doubleThrowElements))
                {
                    Machine.ContextManager.ExecuteReleaseExecutors(evalContext, doubleThrowElements);
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            return base.Input(evnt);
        }

        public IReadOnlyList<(PhysicalReleaseEvent, IState)> 
            CreateHistory(PhysicalPressEvent pressEvent)
            => new List<(PhysicalReleaseEvent, IState)>() { (pressEvent.Opposition, this) };

        public IReadOnlyList<DoubleThrowElement<TExecContext>> 
            GetActiveDoubleThrowElements(TEvalContext ctx, PhysicalPressEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger.Equals(triggerEvent)  ||
                                           d.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select d))
            .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<SingleThrowElement<TExecContext>> 
            GetActiveSingleThrowElements(TEvalContext ctx, PhysicalFireEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.Evaluate(ctx, w)
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger.Equals(triggerEvent) ||
                                           s.Trigger.Equals(triggerEvent.LogicalNormalized))
                        select s))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        private static IReadOnlyCollection<FireEvent> GetSingleThrowTriggers(RootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull
                    select s.Trigger))
                .Aggregate(new HashSet<FireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        private static IReadOnlyCollection<PressEvent> GetDoubleThrowTriggers(RootElement<TEvalContext, TExecContext> rootElement)
            => (from w in rootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull
                    select d.Trigger))
                .Aggregate(new HashSet<PressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }
}
