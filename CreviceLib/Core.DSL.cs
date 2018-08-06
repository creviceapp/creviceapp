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
        int GestureCount { get; }
    }

    public abstract class Element : IReadOnlyElement
    {
        public abstract bool IsFull { get; }
        public abstract int GestureCount { get; }
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

        public override int GestureCount => WhenElements.Sum(e => e.GestureCount);

        private readonly List<WhenElement<TEvalContext, TExecContext>> whenElements = new List<WhenElement<TEvalContext, TExecContext>>();
        public IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> WhenElements => whenElements;

        public WhenElement<TEvalContext, TExecContext> When(EvaluateAction<TEvalContext> evaluateAction, string description = "")
        {
            var evaluator = new Evaluator<TEvalContext>(evaluateAction, description);
            var elm = new WhenElement<TEvalContext, TExecContext>(evaluator);
            whenElements.Add(elm);
            return elm;
        }
    }

    public interface IReadOnlyWhenElement<TEvalContext, TExecContext> : IReadOnlyElement
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        Evaluator<TEvalContext> WhenEvaluator { get; }
        IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements { get; }
        IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements { get; }
        IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>> DecomposedElements { get; }
    }

    public class WhenElement<TEvalContext, TExecContext> : Element, IReadOnlyWhenElement<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public override bool IsFull
            => WhenEvaluator != null &&
                (SingleThrowElements.Any(e => e.IsFull) ||
                 DoubleThrowElements.Any(e => e.IsFull) ||
                 DecomposedElements.Any(e => e.IsFull));

        public override int GestureCount
            => SingleThrowElements.Sum(e => e.GestureCount) +
               DoubleThrowElements.Sum(e => e.GestureCount) +
               DecomposedElements.Sum(e => e.GestureCount);

        public Evaluator<TEvalContext> WhenEvaluator { get; }

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements;

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements;

        private readonly List<DecomposedElement<TExecContext>> decomposedElements = new List<DecomposedElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>> DecomposedElements => decomposedElements;

        public WhenElement(Evaluator<TEvalContext> evaluator)
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
            if (DecomposedElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(PhysicalDoubleThrowKey doubleThrowKey)
        {
            if (DecomposedElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DecomposedElement<TExecContext> OnDecomposed(LogicalDoubleThrowKey doubleThrowKey)
        {
            if (DoubleThrowElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DecomposedElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            decomposedElements.Add(elm);
            return elm;
        }

        public DecomposedElement<TExecContext> OnDecomposed(PhysicalDoubleThrowKey doubleThrowKey)
        {
            if (DoubleThrowElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DecomposedElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            decomposedElements.Add(elm);
            return elm;
        }
    }

    public interface IReadOnlySingleThrowElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        FireEvent Trigger { get; }
        IReadOnlyList<Executor<TExecContext>> DoExecutors { get; }
    }

    public class SingleThrowElement<TExecContext> : Element, IReadOnlySingleThrowElement<TExecContext>
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Trigger != null && DoExecutors.Any(e => e != null);

        public override int GestureCount => DoExecutors.Count;

        public FireEvent Trigger { get; }

        private readonly List<Executor<TExecContext>> doExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> DoExecutors => doExecutors;

        public SingleThrowElement(FireEvent fireEvent)
        {
            Trigger = fireEvent;
        }

        public SingleThrowElement<TExecContext> Do(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Do, description);
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
        IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>> DecomposedElements { get; }
        IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> StrokeElements { get; }
        IReadOnlyList<Executor<TExecContext>> PressExecutors { get; }
        IReadOnlyList<Executor<TExecContext>> DoExecutors { get; }
        IReadOnlyList<Executor<TExecContext>> ReleaseExecutors { get; }
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
                 DecomposedElements.Any(e => e.IsFull) ||
                 StrokeElements.Any(e => e.IsFull));
        
        public override int GestureCount
            => SingleThrowElements.Sum(e => e.GestureCount) +
               DoubleThrowElements.Sum(e => e.GestureCount) +
               DecomposedElements.Sum(e => e.GestureCount) +
               StrokeElements.Sum(e => e.GestureCount) +
               PressExecutors.Count +
               DoExecutors.Count +
               ReleaseExecutors.Count;

        public PressEvent Trigger { get; }

        private readonly List<SingleThrowElement<TExecContext>> singleThrowElements = new List<SingleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlySingleThrowElement<TExecContext>> SingleThrowElements => singleThrowElements;

        private readonly List<DoubleThrowElement<TExecContext>> doubleThrowElements = new List<DoubleThrowElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDoubleThrowElement<TExecContext>> DoubleThrowElements => doubleThrowElements;

        private readonly List<DecomposedElement<TExecContext>> decomposedElements = new List<DecomposedElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyDecomposedElement<TExecContext>> DecomposedElements => decomposedElements;

        private readonly List<StrokeElement<TExecContext>> strokeElements = new List<StrokeElement<TExecContext>>();
        public IReadOnlyList<IReadOnlyStrokeElement<TExecContext>> StrokeElements => strokeElements;

        private readonly List<Executor<TExecContext>> pressExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> PressExecutors => pressExecutors;

        private readonly List<Executor<TExecContext>> doExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> DoExecutors => doExecutors;

        private readonly List<Executor<TExecContext>> releaseExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> ReleaseExecutors => releaseExecutors;

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
            if (DecomposedElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> On(PhysicalDoubleThrowKey doubleThrowKey)
        {
            if (DecomposedElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DoubleThrowElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            doubleThrowElements.Add(elm);
            return elm;
        }

        public DecomposedElement<TExecContext> OnDecomposed(LogicalDoubleThrowKey doubleThrowKey)
        {
            if (DoubleThrowElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DecomposedElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            decomposedElements.Add(elm);
            return elm;
        }

        public DecomposedElement<TExecContext> OnDecomposed(PhysicalDoubleThrowKey doubleThrowKey)
        {
            if (DoubleThrowElements.Any())
            {
                throw new InvalidOperationException("Declaration is ambiguous; " +
                    "DoubleThrowKey can not be used with `On()` and `OnDecomposed()` on the same context.");
            }
            var elm = new DecomposedElement<TExecContext>(doubleThrowKey.PressEvent as PressEvent);
            decomposedElements.Add(elm);
            return elm;
        }

        public StrokeElement<TExecContext> On(params StrokeDirection[] strokeDirections)
        {
            var elm = new StrokeElement<TExecContext>(strokeDirections);
            strokeElements.Add(elm);
            return elm;
        }

        public DoubleThrowElement<TExecContext> Press(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Press, description);
            pressExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<TExecContext> Do(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Do, description);
            doExecutors.Add(executor);
            return this;
        }

        public DoubleThrowElement<TExecContext> Release(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Release, description);
            releaseExecutors.Add(executor);
            return this;
        }
    }

    public interface IReadOnlyDecomposedElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        PressEvent Trigger { get; }
        IReadOnlyList<Executor<TExecContext>> PressExecutors { get; }
        IReadOnlyList<Executor<TExecContext>> ReleaseExecutors { get; }
    }

    public class DecomposedElement<TExecContext> : Element, IReadOnlyDecomposedElement<TExecContext>
        where TExecContext : ExecutionContext
    {
        public override bool IsFull
            => Trigger != null &&
                (PressExecutors.Any(e => e != null) ||
                 ReleaseExecutors.Any(e => e != null));

        public override int GestureCount 
            => PressExecutors.Count + ReleaseExecutors.Count;

        public PressEvent Trigger { get; }

        private readonly List<Executor<TExecContext>> pressExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> PressExecutors => pressExecutors;

        private readonly List<Executor<TExecContext>> releaseExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> ReleaseExecutors => releaseExecutors;

        public DecomposedElement(PressEvent pressEvent)
        {
            Trigger = pressEvent;
        }

        public DecomposedElement<TExecContext> Press(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Press, description);
            pressExecutors.Add(executor);
            return this;
        }

        public DecomposedElement<TExecContext> Release(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Release, description);
            releaseExecutors.Add(executor);
            return this;
        }
    }

    public interface IReadOnlyStrokeElement<TExecContext> : IReadOnlyElement
        where TExecContext : ExecutionContext
    {
        StrokeSequence Strokes { get; }
        IReadOnlyList<Executor<TExecContext>> DoExecutors { get; }
    }

    public class StrokeElement<TExecContext> : Element, IReadOnlyStrokeElement<TExecContext>
        where TExecContext : ExecutionContext
    {
        public override bool IsFull => Strokes.Any() && DoExecutors.Any(e => e != null);

        public override int GestureCount => DoExecutors.Count;

        public StrokeSequence Strokes { get; }

        private readonly List<Executor<TExecContext>> doExecutors = new List<Executor<TExecContext>>();
        public IReadOnlyList<Executor<TExecContext>> DoExecutors => doExecutors;

        public StrokeElement(params StrokeDirection[] strokes)
        {
            Strokes = new StrokeSequence(strokes);
        }

        public StrokeElement<TExecContext> Do(ExecuteAction<TExecContext> executeAction, string description = "")
        {
            var executor = new Executor<TExecContext>(executeAction, ExecutorType.Do, description);
            doExecutors.Add(executor);
            return this;
        }
    }
}
