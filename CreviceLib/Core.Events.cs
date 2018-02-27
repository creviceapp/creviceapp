using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Events
{
    using Crevice.Core.Keys;

    // Event types; Logical or Physical.

    /// <summary>
    /// A marker interface for representing that an `Event` class is the logical event.
    /// </summary>
    public interface ILogicalEvent { }

    /// <summary>
    /// A marker interface for representing that an `Event` class is the physical event.
    /// </summary>
    public interface IPhysicalEvent { }
    
    /// <summary>
    /// The root class of event type system.
    /// 
    /// All `Event` have `EventId`. If an instance of `Event` have the same `EventId` to the other 
    /// instance of `Event`, both are treated as the same.
    /// </summary>
    public abstract class Event : IEquatable<Event>
    {
        public int EventId { get; }

        public virtual bool Equals(Event that) => this.EventId == that.EventId;

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Event evnt)
            {
                return Equals(evnt);
            }
            return false;
        }

        public override int GetHashCode() => EventId;

        public Event(int eventId)
        {
            EventId = eventId;
        }
    }
    
    // Special class.

    /// <summary>
    /// A special class handles `NullEvent`, which does not match any other instances of 
    /// `Event` include itself. The instances of this class have special value -1 as `EventId`.
    /// </summary>
    public sealed class NullEvent : Event, ILogicalEvent, IPhysicalEvent
    {
        public override bool Equals(Event that) => false;
        public override bool Equals(object obj) => false;

        // Need to avoid a IDE warning.
        public override int GetHashCode() => -1;

        public NullEvent() : base(-1) { }
    }

    // Abstract classes.

    public abstract class FireEvent : Event
    {
        public FireEvent(int eventId) : base(eventId) { }

        public virtual LogicalSingleThrowKey LogicalKey
        {
            get
            {
                if (this is LogicalFireEvent logicalFireEvent)
                {
                    return logicalFireEvent.LogicalKey;
                }
                else if (this is PhysicalFireEvent physicalFireEvent)
                {
                    return physicalFireEvent.LogicalKey;
                }
                throw new InvalidCastException();
            }
        }
    }

    public abstract class PressEvent : Event
    {
        public PressEvent(int eventId) : base(eventId) { }

        public virtual LogicalDoubleThrowKey LogicalKey
        {
            get
            {
                if (this is LogicalPressEvent logicalPressEvent)
                {
                    return logicalPressEvent.LogicalKey;
                }
                else if (this is PhysicalPressEvent physicalPressEvent)
                {
                    return physicalPressEvent.LogicalKey;
                }
                throw new InvalidCastException();
            }
        }
    }

    public abstract class ReleaseEvent : Event
    {
        public ReleaseEvent(int eventId) : base(eventId) { }

        public virtual LogicalDoubleThrowKey LogicalKey
        {
            get
            {
                if (this is LogicalReleaseEvent logicalReleaseEvent)
                {
                    return logicalReleaseEvent.LogicalKey;
                }
                else if (this is PhysicalReleaseEvent physicalReleaseEvent)
                {
                    return physicalReleaseEvent.LogicalKey;
                }
                throw new InvalidCastException();
            }
        }
    }
    
    public sealed class EventIdGenerator
    {
        private static EventIdGenerator Instance = new EventIdGenerator();

        private EventIdGenerator() { }

        private readonly object lockObject = new object();

        private int eventId = 0;

        private int GetAndIncrementEventId()
        {
            lock (lockObject)
            {
                try
                {
                    return eventId;
                }
                finally
                {
                    eventId++;
                }
            }
        }

        public static int Generate() => Instance.GetAndIncrementEventId();
    }
    
    // Concreate classes.

    public class LogicalFireEvent : FireEvent, ILogicalEvent
    {
        public override LogicalSingleThrowKey LogicalKey { get; }

        public LogicalFireEvent(LogicalSingleThrowKey logicalKey) 
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class PhysicalFireEvent : FireEvent, IPhysicalEvent
    {
        public override LogicalSingleThrowKey LogicalKey { get; }

        public PhysicalSingleThrowKey PhysicalKey { get; }

        public LogicalFireEvent LogicalNormalized
            => LogicalKey.FireEvent;

        public PhysicalFireEvent(LogicalSingleThrowKey logicalKey, PhysicalSingleThrowKey physicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
            PhysicalKey = physicalKey;
        }
    }

    public class LogicalPressEvent : PressEvent, ILogicalEvent
    {
        public override LogicalDoubleThrowKey LogicalKey { get; }

        public LogicalReleaseEvent Opposition
            => LogicalKey.ReleaseEvent;

        public LogicalPressEvent(LogicalDoubleThrowKey logicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class LogicalReleaseEvent : ReleaseEvent, ILogicalEvent
    {
        public override LogicalDoubleThrowKey LogicalKey { get; }

        public LogicalPressEvent Opposition
            => LogicalKey.PressEvent;

        public LogicalReleaseEvent(LogicalDoubleThrowKey logicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
        }
    }

    public class PhysicalPressEvent : PressEvent, IPhysicalEvent
    {
        public override LogicalDoubleThrowKey LogicalKey { get; }

        public PhysicalDoubleThrowKey PhysicalKey { get; }

        public LogicalPressEvent LogicalNormalized
            => LogicalKey.PressEvent;

        public PhysicalReleaseEvent Opposition
            => PhysicalKey.ReleaseEvent;

        public PhysicalPressEvent(LogicalDoubleThrowKey logicalKey, PhysicalDoubleThrowKey physicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
            PhysicalKey = physicalKey;
        }
    }

    public class PhysicalReleaseEvent : ReleaseEvent, IPhysicalEvent
    {
        public override LogicalDoubleThrowKey LogicalKey { get; }

        public PhysicalDoubleThrowKey PhysicalKey { get; }

        public LogicalReleaseEvent LogicalNormalized
            => LogicalKey.ReleaseEvent;

        public PhysicalPressEvent Opposition
            => PhysicalKey.PressEvent;

        public PhysicalReleaseEvent(LogicalDoubleThrowKey logicalKey, PhysicalDoubleThrowKey physicalKey)
            : base(EventIdGenerator.Generate())
        {
            LogicalKey = logicalKey;
            PhysicalKey = physicalKey;
        }
    }
}
