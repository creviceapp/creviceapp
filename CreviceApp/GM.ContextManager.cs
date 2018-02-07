using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.GestureMachine
{
    using System.Drawing;
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
            WhenElement<EvaluationContext, ExecutionContext> whenElement)
        {
            var task = Task.Factory.StartNew(() =>
            {
                return whenElement.WhenEvaluator(evalContext);
            });
            try
            {
                // todo: config
                return task.Wait(20);
            }
            catch (Exception)
            {
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
