using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Context
{
    using System.Linq;
    using Crevice.Core.DSL;
    
    public class EvaluationContext { }
    public class ExecutionContext  { }

    public delegate bool EvaluateAction<in T>(T ctx);
    public delegate void ExecuteAction<in T>(T ctx);

    public abstract class ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public virtual TEvalContext CreateEvaluateContext()
            => throw new NotImplementedException();

        public virtual TExecContext CreateExecutionContext(TEvalContext evaluationContext)
            => throw new NotImplementedException();

        public virtual bool Evaluate(TEvalContext evalContext, IReadOnlyWhenElement<TEvalContext, TExecContext> whenElement)
            => whenElement.WhenEvaluator(evalContext);

        public virtual void Execute(TExecContext execContext, ExecuteAction<TExecContext> executeAction)
            => executeAction(execContext);

        public void ExecutePressExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.PressExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecutePressExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyDecomposedElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.PressExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<IReadOnlySingleThrowElement<TExecContext>> singleThrowElements)
        {
            if (singleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in singleThrowElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyStrokeElement<TExecContext>> strokeElements)
        {
            if (strokeElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in strokeElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteReleaseExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyDoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.ReleaseExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteReleaseExecutors(TEvalContext evalContext, IEnumerable<IReadOnlyDecomposedElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.ReleaseExecutors)
                    {
                        Execute(execContext, executor);
                    }
                }
            }
        }
    }
}
