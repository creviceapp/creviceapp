using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Types;
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

        public State0(
            GestureMachine<TConfig, TContextManager, TEvalContext, TExecContext> machine,
            RootElement<TEvalContext, TExecContext> rootElement)
        {
            Machine = machine;
            RootElement = rootElement;
        }

        public override (bool EventIsConsumed, IState NextState) Input(IPhysicalEvent evnt)
        {
            if (evnt is IFireEvent fireEvent &&
                    (SingleThrowTriggers.Contains(fireEvent) ||
                     SingleThrowTriggers.Contains(fireEvent.LogicalNormalized)))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var singleThrowElements = GetActiveSingleThrowElements(evalContext, fireEvent);
                if (singleThrowElements.Any())
                {
                    Machine.ContextManager.ExecuteDoExecutors(evalContext, singleThrowElements);
                    return (EventIsConsumed: true, NextState: this);
                }
            }
            else if (evnt is IPressEvent pressEvent &&
                        (DoubleThrowTriggers.Contains(pressEvent) ||
                         DoubleThrowTriggers.Contains(pressEvent.LogicalNormalized)))
            {
                var evalContext = Machine.ContextManager.CreateEvaluateContext();
                var doubleThrowElements = GetActiveDoubleThrowElements(evalContext, pressEvent);
                if (doubleThrowElements.Any())
                {
                    Machine.ContextManager.ExecutePressExecutors(evalContext, doubleThrowElements);
                    var nextState = new StateN<TConfig, TContextManager, TEvalContext, TExecContext>(
                        Machine,
                        evalContext,
                        CreateHistory(pressEvent),
                        doubleThrowElements,
                        canCancel: true
                        );
                    return (EventIsConsumed: true, NextState: nextState);
                }
            }
            return base.Input(evnt);
        }

        public IReadOnlyList<(IReleaseEvent, IState)> CreateHistory(IPressEvent pressEvent)
            => new List<(IReleaseEvent, IState)>() { (pressEvent.Opposition, this) };

        public IReadOnlyList<DoubleThrowElement<TExecContext>> GetActiveDoubleThrowElements(TEvalContext ctx, IPressEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.EvaluateWhenEvaluator(ctx, w)
                select (from d in w.DoubleThrowElements
                        where d.IsFull && (d.Trigger == triggerEvent ||
                                           d.Trigger == triggerEvent.LogicalNormalized)
                        select d))
            .Aggregate(new List<DoubleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyList<SingleThrowElement<TExecContext>> GetActiveSingleThrowElements(TEvalContext ctx, IFireEvent triggerEvent)
            => (from w in RootElement.WhenElements
                where w.IsFull && Machine.ContextManager.EvaluateWhenEvaluator(ctx, w)
                select (from s in w.SingleThrowElements
                        where s.IsFull && (s.Trigger == triggerEvent ||
                                           s.Trigger == triggerEvent.LogicalNormalized)
                        select s))
                .Aggregate(new List<SingleThrowElement<TExecContext>>(), (a, b) => { a.AddRange(b); return a; });

        public IReadOnlyCollection<IFireEvent> SingleThrowTriggers
            => (from w in RootElement.WhenElements
                where w.IsFull
                select (
                    from s in w.SingleThrowElements
                    where s.IsFull
                    select s.Trigger))
                .Aggregate(new HashSet<IFireEvent>(), (a, b) => { a.UnionWith(b); return a; });

        public IReadOnlyCollection<IPressEvent> DoubleThrowTriggers
            => (from w in RootElement.WhenElements
                where w.IsFull
                select (
                    from d in w.DoubleThrowElements
                    where d.IsFull
                    select d.Trigger))
                .Aggregate(new HashSet<IPressEvent>(), (a, b) => { a.UnionWith(b); return a; });
    }
}
