using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.DSL
{
    using System.Linq;
    using Crevice.Core.Events;
    using Crevice.Core.Context;
    using Crevice.Core.Keys;
    using Crevice.Core.Stroke;

    public abstract class Element
    {
        public abstract bool IsFull { get; }
    }

    // Todo: Clone interface

    /* RootElement
     * 
     * .When() -> new WhenElement
     */
    public class RootElement<TEvalContext, TExecContext> : Element
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => WhenElements.Any(e => e.IsFull);

        private readonly List<WhenElement<TEvalContext, TExecContext>> whenElements = new List<WhenElement<TEvalContext, TExecContext>>();
        public IReadOnlyList<WhenElement<TEvalContext, TExecContext>> WhenElements => whenElements.ToList();

        public WhenElement<TEvalContext, TExecContext> When(EvaluateAction<TEvalContext> evaluator)
        {
            var elm = new WhenElement<TEvalContext, TExecContext>(evaluator);
            whenElements.Add(elm);
            return elm;
        }
    }

    /* WhenElement
     * 
     * .On(FireEvent) -> new SingleThrowElement
     * 
     * .On(PressEvent) -> new DoubleThrowElement
     */
    public class WhenElement<TEvalContext, TExecContext> : Element
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull
            => WhenEvaluator != null &&
                (SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull));

        public readonly EvaluateAction<TEvalContext> WhenEvaluator;

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<SingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements.ToList();

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements.ToList();

        public WhenElement(EvaluateAction<TEvalContext> evaluator)
        {
            WhenEvaluator = evaluator;
        }

        public SingleThrowElement<TExecContext> On(LogicalSingleThrowKey singleThrowKey)
        {
            var elm = new SingleThrowElement<TExecContext>(singleThrowKey.FireEvent as FireEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public SingleThrowElement<TExecContext> On(PhysicalSingleThrowKey singleThrowKey)
        {
            var elm = new SingleThrowElement<TExecContext>(singleThrowKey.FireEvent as FireEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(LogicalDoubleThrowKey doubleThrowKey)
        {
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(PhysicalDoubleThrowKey doubleThrowKey)
        {
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }
    }

    /* SingleThrowElement
     * 
     * .Do() -> this
     */
    public class SingleThrowElement<TExecContext> : Element
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Trigger != null && DoExecutors.Any(e => e != null);
        
        public readonly FireEvent Trigger;

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors.ToList();

        public SingleThrowElement(FireEvent fireEvent)
        {
            Trigger = fireEvent;
        }

        public SingleThrowElement<TExecContext> Do(ExecuteAction<TExecContext> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }

    /* 
     * .Press() -> this 
     * 
     * .Do() -> this 
     * 
     * .Release() -> this 
     * 
     * .On(FireEvent) -> new SingleThrowElement
     * 
     * .On(PressEvent) -> new DoubleThrowElement
     * 
     * .On(StrokeEvent) -> new StrokeEelement
     */
    public class DoubleThrowElement<TExecContext> : Element
        where TExecContext : ExecutionContext
    {
        public override bool IsFull
            => Trigger != null &&
                (PressExecutors.Any(e => e != null) ||
                 DoExecutors.Any(e => e != null) ||
                 ReleaseExecutors.Any(e => e != null) ||
                 SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull) ||
                 StrokeElements.Any(e => e.IsFull));

        public readonly KeyType Key;

        public readonly PressEvent Trigger;

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<SingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements.ToList();

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<DoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements.ToList();

        private readonly List<StrokeElement<TExecContext>> strokeElements = new List<StrokeElement<TExecContext>>();
        public IReadOnlyList<StrokeElement<TExecContext>> StrokeElements => strokeElements.ToList();

        private readonly List<ExecuteAction<TExecContext>> pressExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> PressExecutors => pressExecutors.ToList();

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors.ToList();

        private readonly List<ExecuteAction<TExecContext>> releaseExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> ReleaseExecutors => releaseExecutors.ToList();

        public DoubleThrowElement(PressEvent pressEvent)
        {
            Trigger = pressEvent;
        }

        public SingleThrowElement<TExecContext> On(LogicalSingleThrowKey singleThrowKey)
        {
            var elm = new SingleThrowElement<TExecContext>(singleThrowKey.FireEvent as FireEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public SingleThrowElement<TExecContext> On(PhysicalSingleThrowKey singleThrowKey)
        {
            var elm = new SingleThrowElement<TExecContext>(singleThrowKey.FireEvent as FireEvent);
            singleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(LogicalDoubleThrowKey doubleThrowKey)
        {
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(PhysicalDoubleThrowKey doubleThrowKey)
        {
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public StrokeElement<TExecContext> On(params StrokeDirection[] strokeDirections)
        {
            var elm = new StrokeElement<TExecContext>(strokeDirections);
            strokeElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> Press(ExecuteAction<TExecContext> executor)
        {
            pressExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<TExecContext> Do(ExecuteAction<TExecContext> executor)
        {
            doExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<TExecContext> Release(ExecuteAction<TExecContext> executor)
        {
            releaseExecutors.Add(executor);
            return this;
        }
    }

    /* 
     * .Do() -> this 
     */
    public class StrokeElement<TExecContext> : Element
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Strokes.Any() && DoExecutors.Any(e => e != null);

        public readonly IReadOnlyList<StrokeDirection> Strokes;

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors.ToList();

        public StrokeElement(params StrokeDirection[] strokes)
        {
            Strokes = strokes;
        }

        public StrokeElement<TExecContext> Do(ExecuteAction<TExecContext> executor)
        {
            doExecutors.Add(executor);
            return this;
        }
    }
}
