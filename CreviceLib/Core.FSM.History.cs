using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.FSM
{
    using System.Linq;
    using Crevice.Core.Events;

    public class HistoryRecord
    {
        public readonly PhysicalReleaseEvent ReleaseEvent;
        public readonly State State;
        public HistoryRecord(PhysicalReleaseEvent releaseEvent, State state)
        {
            ReleaseEvent = releaseEvent;
            State = state;
        }
    }

    public class HistoryQueryResult
    {
        public readonly State FoundState;
        public readonly IReadOnlyList<PhysicalReleaseEvent> SkippedReleaseEvents;
        public HistoryQueryResult(State foundState, IReadOnlyList<PhysicalReleaseEvent> skippedReleaseEvents)
        {
            FoundState = foundState;
            SkippedReleaseEvents = skippedReleaseEvents;
        }
    }

    public class History
    {
        public readonly IReadOnlyList<HistoryRecord> Records;

        public History(PhysicalReleaseEvent releaseEvent, State state)
            : this(new List<HistoryRecord>() { new HistoryRecord(releaseEvent, state) })
        { }

        public History(IReadOnlyList<HistoryRecord> records)
        {
            Records = records;
        }

        public HistoryQueryResult Query(PhysicalReleaseEvent releaseEvent)
        {
            var nextHistory = Records.TakeWhile(t => t.ReleaseEvent != releaseEvent);
            var foundState = Records[nextHistory.Count()].State;
            var skippedReleaseEvents = Records.Skip(nextHistory.Count()).Select(t => t.ReleaseEvent).ToList();
            return new HistoryQueryResult(foundState, skippedReleaseEvents);
        }

        public History CreateNext(PhysicalReleaseEvent releaseEvent, State state)
        {
            var newRecords = Records.ToList();
            newRecords.Add(new HistoryRecord(releaseEvent, state));
            return new History(newRecords);
        }
    }
}
