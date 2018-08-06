using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Threading;
    using System.Drawing;
    using Crevice.Logging;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Threading;




    public class ContextManager : ContextManager<EvaluationContext, ExecutionContext>, IDisposable
    {
        public Point CursorPosition { get; set; }
        
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext(CursorPosition);

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext(evaluationContext, CursorPosition);

        private readonly LowLatencyScheduler _executionScheduler = 
            new LowLatencyScheduler(
                "ExecutorExecutionTaskScheduler",
                ThreadPriority.AboveNormal,
                Math.Max(2, Environment.ProcessorCount / 2));

        private TaskFactory _executionTaskFactory
            => new TaskFactory(_executionScheduler);
        
        public override IReadOnlyList<IReadOnlyWhenElement<EvaluationContext, ExecutionContext>> Evaluate(
            EvaluationContext evalContext,
            IEnumerable<IReadOnlyWhenElement<EvaluationContext, ExecutionContext>> whenElements)
            => whenElements.AsParallel()
                .WithDegreeOfParallelism(Math.Max(2, Environment.ProcessorCount / 2))
                .Where(w => 
                {
                    try
                    {
                        return w.WhenEvaluator.Evaluate(evalContext);
                    }
                    catch (Exception ex)
                    {
                        Verbose.Error($"An exception was thrown while evaluating a WhenEvaluator(\"{w.WhenEvaluator.Description}\"): {ex.ToString()}");
                    }
                    return false;
                })
                .ToList();

        public override void Execute(
            ExecutionContext execContext,
            Executor<ExecutionContext> executor)
        {
            var task = _executionTaskFactory.StartNew(() =>
            {
                try
                {
                    executor.Execute(execContext);
                }
                catch (Exception ex)
                {
                    Verbose.Error($"An exception was thrown while executing an Executor(\"{executor.Description}\"): {ex.InnerException.ToString()}");
                }
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _executionScheduler.Dispose();
            }
        }

        ~ContextManager() => Dispose(false);
    }
}
