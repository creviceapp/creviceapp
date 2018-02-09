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

    public interface IReadOnlyElement
    {
        bool IsFull { get; }
    }

    public abstract class Element : IReadOnlyElement
    {
        public abstract bool IsFull { get; }
    }
    
    public interface IReadOnlyRootElement<TEvalContext, TExecContext> : IReadOnlyElement
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> WhenElements { get; }
    }
    
    public class RootElement<TEvalContext, TExecContext> : Element, IReadOnlyRootElement<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => WhenElements.Any(e => e.IsFull);

        private readonly List<WhenElement<TEvalContext, TExecContext>> whenElements = new List<WhenElement<TEvalContext, TExecContext>>();
        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> WhenElements => whenElements;

        public WhenElement<TEvalContext, TExecContext> When(EvaluateAction<TEvalContext> evaluator)
        {
            var elm = new WhenElement<TEvalContext, TExecContext>(evaluator);
            whenElements.Add(elm);
            return elm;
        }
    }

    public interface IReadOnlyWhenElement<TEvalContext, TExecContext> : IReadOnlyElement
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        EvaluateAction<TEvalContext> WhenEvaluator { get; }
        IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements { get; }
        IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements { get; }
    }

    public class WhenElement<TEvalContext, TExecContext> : Element, IReadOnlyWhenElement<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull
            => WhenEvaluator != null &&
                (SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull));

        public EvaluateAction<TEvalContext> WhenEvaluator { get; }

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements;

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements;

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

    public interface IReadOnlySingleThrowElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        FireEvent Trigger { get; }
        IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors { get; }
    }

    public class SingleThrowElement<TExecContext> : Element, IReadOnlySingleThrowElement<TExecContext>
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Trigger != null && DoExecutors.Any(e => e != null);
        
        public FireEvent Trigger { get; }

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors;

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

    public interface IReadOnlyDoubleThrowElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        PressEvent Trigger { get; }
        IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements { get; }
        IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements { get; }
        IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> StrokeElements { get; }
        IReadOnlyList<ExecuteAction<TExecContext>> PressExecutors { get; }
        IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors { get; }
        IReadOnlyList<ExecuteAction<TExecContext>> ReleaseExecutors { get; }
    }

    public class DoubleThrowElement<TExecContext> : Element, IReadOnlyDoubleThrowElement<TExecContext>
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

        public PressEvent Trigger { get; }

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements;

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements;

        private readonly List<StrokeElement<TExecContext>> strokeElements = new List<StrokeElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> StrokeElements => strokeElements;

        private readonly List<ExecuteAction<TExecContext>> pressExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> PressExecutors => pressExecutors;

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors;

        private readonly List<ExecuteAction<TExecContext>> releaseExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> ReleaseExecutors => releaseExecutors;

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

    public interface IReadOnlyStrokeElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        IReadOnlyList<StrokeDirection> Strokes { get; }
        IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors { get; }
    }

    public class StrokeElement<TExecContext> : Element, IReadOnlyStrokeElement<TExecContext>
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Strokes.Any() && DoExecutors.Any(e => e != null);

        public IReadOnlyList<StrokeDirection> Strokes { get; }

        private readonly List<ExecuteAction<TExecContext>> doExecutors = new List<ExecuteAction<TExecContext>>();
        public IReadOnlyList<ExecuteAction<TExecContext>> DoExecutors => doExecutors;

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
