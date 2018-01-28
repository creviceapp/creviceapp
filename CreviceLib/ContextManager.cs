using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice
{
    public class EvaluationContext { }
    public class ExecutionContext { }

    public delegate bool EvaluateAction<in T>(T ctx);
    public delegate void ExecuteAction<in T>(T ctx);

    public class ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public virtual TEvalContext CreateEvaluateContext()
            => throw new NotImplementedException();

        public virtual TExecContext CreateExecutionContext(TEvalContext evaluationContext)
            => throw new NotImplementedException();

        public virtual bool EvaluateWhenEvaluator(TEvalContext evalContext, WhenElement<TEvalContext, TExecContext> whenElement)
            => whenElement.WhenEvaluator(evalContext);

        public virtual void ExecuteExcutor(TExecContext execContext, ExecuteAction<TExecContext> executeAction)
            => executeAction(execContext);

        public void ExecutePressExecutors(TEvalContext evalContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.PressExecutors)
                    {
                        ExecuteExcutor(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        ExecuteExcutor(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<SingleThrowElement<TExecContext>> singleThrowElements)
        {
            if (singleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in singleThrowElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        ExecuteExcutor(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteDoExecutors(TEvalContext evalContext, IEnumerable<StrokeElement<TExecContext>> strokeElements)
        {
            if (strokeElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in strokeElements)
                {
                    foreach (var executor in element.DoExecutors)
                    {
                        ExecuteExcutor(execContext, executor);
                    }
                }
            }
        }

        public void ExecuteReleaseExecutors(TEvalContext evalContext, IEnumerable<DoubleThrowElement<TExecContext>> doubleThrowElements)
        {
            if (doubleThrowElements.Any())
            {
                var execContext = CreateExecutionContext(evalContext);
                foreach (var element in doubleThrowElements)
                {
                    foreach (var executor in element.ReleaseExecutors)
                    {
                        ExecuteExcutor(execContext, executor);
                    }
                }
            }
        }
    }
}
