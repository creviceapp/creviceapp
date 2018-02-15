using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Context;
    using Crevice.Core.Events;

    public class HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly PhysicalReleaseEvent ReleaseEvent;
        public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> State;
        public HistoryRecord(
            PhysicalReleaseEvent releaseEvent, 
            State<TConfig, TContextManager, TEvalContext, TExecContext> state)
        {
            ReleaseEvent = releaseEvent;
            State = state;
        }
    }

    public static class HistoryRecord
    {
        public static HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>
            Create<TConfig, TContextManager, TEvalContext, TExecContext>(
            PhysicalReleaseEvent releaseEvent,
            State<TConfig, TContextManager, TEvalContext, TExecContext> state)
            where TConfig : GestureMachineConfig
            where TContextManager : ContextManager<TEvalContext, TExecContext>
            where TEvalContext : EvaluationContext
            where TExecContext : ExecutionContext
        {
            return new HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>(releaseEvent, state);
        }
    }

    public class HistoryQueryResult<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly State<TConfig, TContextManager, TEvalContext, TExecContext> FoundState;
        public readonly IReadOnlyList<PhysicalReleaseEvent> SkippedReleaseEvents;
        public HistoryQueryResult(
            State<TConfig, TContextManager, TEvalContext, TExecContext> foundState, 
            IReadOnlyList<PhysicalReleaseEvent> skippedReleaseEvents)
        {
            FoundState = foundState;
            SkippedReleaseEvents = skippedReleaseEvents;
        }
    }

    public class History<TConfig, TContextManager, TEvalContext, TExecContext>
        where TConfig : GestureMachineConfig
        where TContextManager : ContextManager<TEvalContext, TExecContext>
        where TEvalContext : EvaluationContext
        where TExecContext : ExecutionContext
    {
        public readonly IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> Records;

        public History(
            PhysicalReleaseEvent releaseEvent, 
            State<TConfig, TContextManager, TEvalContext, TExecContext> state)
            : this(new List<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>>() {
                HistoryRecord.Create(releaseEvent, state) })
        { }

        public History(IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> records)
        {
            Records = records;
        }

        public HistoryQueryResult<TConfig, TContextManager, TEvalContext, TExecContext> 
            Query(PhysicalReleaseEvent releaseEvent)
        {
            var nextHistory = Records.TakeWhile(t => t.ReleaseEvent != releaseEvent);
            var foundState = Records[nextHistory.Count()].State;
            var skippedReleaseEvents = Records.Skip(nextHistory.Count()).Select(t => t.ReleaseEvent).ToList();
            return new HistoryQueryResult<TConfig, TContextManager, TEvalContext, TExecContext>(foundState, skippedReleaseEvents);
        }

        public History<TConfig, TContextManager, TEvalContext, TExecContext> 
            CreateNext(PhysicalReleaseEvent releaseEvent, State<TConfig, TContextManager, TEvalContext, TExecContext> state)
        {
            var newRecords = Records.ToList();
            newRecords.Add(HistoryRecord.Create(releaseEvent, state));
            return new History<TConfig, TContextManager, TEvalContext, TExecContext>(newRecords);
        }
    }

    public static class History
    {
        public static History<TConfig, TContextManager, TEvalContext, TExecContext>
            Create<TConfig, TContextManager, TEvalContext, TExecContext>(
            PhysicalReleaseEvent releaseEvent,
            State<TConfig, TContextManager, TEvalContext, TExecContext> state)
            where TConfig : GestureMachineConfig
            where TContextManager : ContextManager<TEvalContext, TExecContext>
            where TEvalContext : EvaluationContext
            where TExecContext : ExecutionContext
        {
            return new History<TConfig, TContextManager, TEvalContext, TExecContext>(releaseEvent, state);
        }

        public static History<TConfig, TContextManager, TEvalContext, TExecContext>
            Create<TConfig, TContextManager, TEvalContext, TExecContext>(
            IReadOnlyList<HistoryRecord<TConfig, TContextManager, TEvalContext, TExecContext>> records)
            where TConfig : GestureMachineConfig
            where TContextManager : ContextManager<TEvalContext, TExecContext>
            where TEvalContext : EvaluationContext
            where TExecContext : ExecutionContext
        {
            return new History<TConfig, TContextManager, TEvalContext, TExecContext>(records);
        }
    }
}
