using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Context
{
    using System.Linq;
    using Crevice.Core.DSL;
    
    public class EvaluationContext { }
    public class ExecutionContext { }

    public delegate bool EvaluateAction<in T>(T ctx);
    public delegate void ExecuteAction<in T>(T ctx);
    
    public class Evaluator<TEvalContext>
        where TEvalContext : EvaluationContext
    {
        public readonly EvaluateAction<TEvalContext> Action;
        public readonly string Description;

        public Evaluator(EvaluateAction<TEvalContext> action, string description)
        {
            this.Action = action;
            this.Description = description;
        }

        public bool Evaluate(TEvalContext ctx) => Action(ctx);
    }

    public enum ExecutorType
    {
        Press,
        Do,
        Release
    }

    public class Executor<TExecContext>
        where TExecContext : ExecutionContext
    {
        public readonly ExecuteAction<TExecContext> Action;
        public readonly ExecutorType Type;
        public readonly string Description;

        public Executor(ExecuteAction<TExecContext> action, ExecutorType type, string description)
        {
            this.Action = action;
            this.Type = type;
            this.Description = description;
        }

        public void Execute(TExecContext ctx) => Action(ctx);
    }

    public abstract class ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public virtual TEvalContext CreateEvaluateContext()
            => throw new NotImplementedException();

        public virtual TExecContext CreateExecutionContext(TEvalContext evaluationContext)
            => throw new NotImplementedException();

        public virtual IReadOnlyList<IReadOnlyWhenElement<TEvalContext, TExecContext>> Evaluate(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlyWhenElement<TEvalContext, TExecContext>> whenElements)
            => whenElements.Where(w => w.WhenEvaluator.Evaluate(evalContext)).ToList();

        public virtual void Execute(TExecContext execContext, Executor<TExecContext> executor)
            => executor.Execute(execContext);

        private void Execute(
            TEvalContext evalContext,
            IEnumerable<Executor<TExecContext>> executors)
        {
            var execContext = CreateExecutionContext(evalContext);
            foreach (var executor in executors)
            {
                Execute(execContext, executor);
            }
        }

        public void ExecutePressExecutors(
            TEvalContext evalContext,
            IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                foreach (var element in doubleThrowElements)
                {
                    Execute(evalContext, element.PressExecutors);
                }
            }
        }

        public void ExecutePressExecutors(
            TEvalContext evalContext,
            IEnumerable<IReadOnlyDecomposedElement<TExecContext>> decomposedElements)
        {
            if (decomposedElements.Any())
            {
                foreach (var element in decomposedElements)
                {
                    Execute(evalContext, element.PressExecutors);
                }
            }
        }

        public void ExecuteDoExecutors(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                foreach (var element in doubleThrowElements)
                {
                    Execute(evalContext, element.DoExecutors);
                }
            }
        }

        public void ExecuteDoExecutors(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlySingleThrowElement<TExecContext>> singleThrowElements)
        {
            if (singleThrowElements.Any())
            {
                foreach (var element in singleThrowElements)
                {
                    Execute(evalContext, element.DoExecutors);
                }
            }
        }

        public void ExecuteDoExecutors(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlyStrokeElement<TExecContext>> strokeElements)
        {
            if (strokeElements.Any())
            {
                foreach (var element in strokeElements)
                {
                    Execute(evalContext, element.DoExecutors);
                }
            }
        }

        public void ExecuteReleaseExecutors(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                foreach (var element in doubleThrowElements)
                {
                    Execute(evalContext, element.ReleaseExecutors);
                }
            }
        }

        public void ExecuteReleaseExecutors(
            TEvalContext evalContext, 
            IEnumerable<IReadOnlyDecomposedElement<TExecContext>> decomposedElements)
        {
            if (decomposedElements.Any())
            {
                foreach (var element in decomposedElements)
                {
                    Execute(evalContext, element.ReleaseExecutors);
                }
            }
        }
    }
}
