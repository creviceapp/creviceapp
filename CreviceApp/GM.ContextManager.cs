using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Drawing;
    using Crevice.Logging;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;

    public class ContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public Point CursorPosition { get; set; }

        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext(CursorPosition);

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext(evaluationContext, CursorPosition);

        public override bool Evaluate(
            EvaluationContext evalContext,
            IReadOnlyWhenElement<EvaluationContext, ExecutionContext> whenElement)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return whenElement.WhenEvaluator(evalContext);
            });

            try
            {
                var finished = task.Wait(1000);
                if (!finished)
                {
                    Verbose.Print("Evaluation of WhenEvaluator aborted because it does not finished within limit time.");
                    return false;
                }
                return task.Result;
            }
            catch (Exception ex)
            {
                Verbose.Error("An exception was thrown while evaluating an evaluator: {0}", ex.ToString());
                return false;
            }
        }

        public override void Execute(
            ExecutionContext execContext,
            ExecuteAction<ExecutionContext> executeAction)
        {
            Task.Factory.StartNew(() =>
            {
                executeAction(execContext);
            });
        }
    }
}
