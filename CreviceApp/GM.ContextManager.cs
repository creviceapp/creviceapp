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

    public class ContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public Point CursorPosition { get; set; }
        
        public int EvaluationLimitTime { get; } = 1000; // ms

        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext(CursorPosition);

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext(evaluationContext, CursorPosition);

        private readonly TaskFactory _evaluationTaskFactory
            = LowLatencyScheduler.CreateTaskFactory(
                "EvaluationTaskScheduler", 
                ThreadPriority.AboveNormal, 
                Math.Max(2, Environment.ProcessorCount / 2));

        private readonly TaskFactory _executionTaskFactory
            = LowLatencyScheduler.CreateTaskFactory(
                "ExecutionTaskScheduler", 
                ThreadPriority.AboveNormal, 
                Math.Max(2, Environment.ProcessorCount / 2));
        
        private async Task<bool> EvaluateAsync(Task<bool> task)
        {
            try
            {
                if (await Task.WhenAny(task, Task.Delay(EvaluationLimitTime)) == task)
                {
                    return task.Result;
                }
                else
                {
                    Verbose.Error("Evaluation of WhenEvaluator was timeout; (EvaluationLimitTime: {0}ms)", EvaluationLimitTime);
                }
            }
            catch (AggregateException ex)
            {
                Verbose.Error("An exception was thrown while evaluating an evaluator: {0}", ex.InnerException.ToString());
            }
            catch (Exception ex)
            {
                Verbose.Error("An unexpected exception was thrown while evaluating an evaluator: {0}", ex.ToString());
            }
            return false;
        }

        public override bool Evaluate(
            EvaluationContext evalContext,
            IReadOnlyWhenElement<EvaluationContext, ExecutionContext> whenElement)
        {
            var task = _evaluationTaskFactory.StartNew(() =>
            {
                return whenElement.WhenEvaluator(evalContext);
            });
            return EvaluateAsync(task).Result;
        }

        public override void Execute(
            ExecutionContext execContext,
            ExecuteAction<ExecutionContext> executeAction)
        {
            _executionTaskFactory.StartNew(() =>
            {
                executeAction(execContext);
            });
        }
    }
}
