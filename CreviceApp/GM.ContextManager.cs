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
                ThreadPriority.Highest, 
                Math.Max(2, Environment.ProcessorCount / 2));

        private readonly TaskFactory _executionTaskFactory
            = LowLatencyScheduler.CreateTaskFactory(
                "ExecutionTaskScheduler", 
                ThreadPriority.Highest, 
                Math.Max(2, Environment.ProcessorCount / 2));

        public override bool Evaluate(
            EvaluationContext evalContext,
            IReadOnlyWhenElement<EvaluationContext, ExecutionContext> whenElement)
        {
            try
            {
                var task = _evaluationTaskFactory.StartNew(() =>
                {
                    return whenElement.WhenEvaluator(evalContext);
                });
                if (!task.Wait(EvaluationLimitTime))
                {
                    Verbose.Print("Evaluation of WhenEvaluator was aborted because it was not finished within limit time ({0}ms).", EvaluationLimitTime);
                    return false;
                }
                return task.Result;
            }
            catch (AggregateException ex)
            {
                Verbose.Error("An exception was thrown while evaluating an evaluator: {0}", ex.InnerException.ToString());
                return false;
            }
            catch (Exception ex)
            {
                Verbose.Error("An unexpected exception was thrown while evaluating an evaluator: {0}", ex.ToString());
                return false;
            }
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
