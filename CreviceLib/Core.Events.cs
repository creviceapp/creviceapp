using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Events
{
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
    /// All `Event` have `EventId`, greater than 0, for 0 is reserved for special NullEvent.
    /// If an instance of `Event` have the same `EventId` to the other instance of `Event`, both 
    /// are treated as the same.
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
    
    /// <summary>
    /// A special class handles `NullEvent`, which does not match any other instances of 
    /// `Event` include itself. The instances of this class have special value 0 as `EventId`.
    /// </summary>
    public sealed class NullEvent : Event, ILogicalEvent, IPhysicalEvent
    {
        public override bool Equals(Event that) => false;
        public override bool Equals(object obj) => false;

        // Need to avoid a IDE warning.
        public override int GetHashCode() => 0;

        public NullEvent() : base(0) { }
    }

    public abstract class FireEvent : Event
    {
        public FireEvent(int eventId) : base(eventId) { }
    }
    
    public abstract class LogicalFireEvent : FireEvent, ILogicalEvent
    {
        public LogicalFireEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalFireEvent : FireEvent, IPhysicalEvent
    {
        public abstract LogicalFireEvent LogicalNormalized { get; }

        public PhysicalFireEvent(int eventId) : base(eventId) { }
    }
    
    public abstract class PressEvent : Event
    {
        public PressEvent(int eventId) : base(eventId) { }
    }

    public abstract class LogicalPressEvent : PressEvent, ILogicalEvent
    {
        public abstract LogicalReleaseEvent Opposition { get; }
        
        public LogicalPressEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalPressEvent : PressEvent, IPhysicalEvent
    {
        public abstract LogicalPressEvent LogicalNormalized { get; }

        public abstract PhysicalReleaseEvent Opposition { get; }
        
        public PhysicalPressEvent(int eventId) : base(eventId) { }
    }

    public abstract class ReleaseEvent : Event
    {
        public ReleaseEvent(int eventId) : base(eventId) { }
    }

    public abstract class LogicalReleaseEvent : ReleaseEvent, ILogicalEvent
    {
        public abstract LogicalPressEvent Opposition { get; }
        
        public LogicalReleaseEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalReleaseEvent : ReleaseEvent, IPhysicalEvent
    {
        public abstract LogicalReleaseEvent LogicalNormalized { get; }

        public abstract PhysicalPressEvent Opposition { get; }
        
        public PhysicalReleaseEvent(int eventId) : base(eventId) { }
    }
}
