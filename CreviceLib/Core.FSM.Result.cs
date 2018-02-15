using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using Crevice.Core.Context;

    public struct Result<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly bool EventIsConsumed;
        public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> NextState;
        public Result(bool eventIsConsumed, State<TConfig, TContextManager, TEvalContext, TExecContext> nextState)
        {
            EventIsConsumed = eventIsConsumed;
            NextState = nextState;
        }
    }

    public static class Result
    {
        public static Result<TConfig, TContextManager, TEvalContext, TExecContext> Create<TConfig, TContextManager, TEvalContext, TExecContext>(
            bool eventIsConsumed,
            State<TConfig, TContextManager, TEvalContext, TExecContext> nextState) 
            where TConfig : GestureMachineConfig
            where TContextManager : ContextManager<TEvalContext, TExecContext>
            where TEvalContext : EvaluationContext
            where TExecContext : ExecutionContext
        {
            return new Result<TConfig, TContextManager, TEvalContext, TExecContext>(eventIsConsumed, nextState);
        }
    }
}
