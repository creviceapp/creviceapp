using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Types
{
    public abstract class Switch { }
    public abstract class SingleThrowSwitch : Switch { }
    public abstract class DoubleThrowSwitch : Switch { }

    public interface ILogicalEvent { }
    public interface IPhysicalEvent { }

    public interface IFireEvent
    {
        IFireEvent LogicalNormalized { get; }
    }

    public interface IPressEvent
    {
        IReleaseEvent Opposition { get; }
        IPressEvent LogicalNormalized { get; }
    }

    public interface IReleaseEvent
    {
        IPressEvent Opposition { get; }
        IReleaseEvent LogicalNormalized { get; }
    }

    public abstract class Event : IEquatable<Event>
    {
        public int EventId { get; }

        public bool Equals(Event that) => this.EventId == that.EventId;

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

    public abstract class LogicalFireEvent<TSw> : Event, IFireEvent, ILogicalEvent
        where TSw : SingleThrowSwitch
    {
        public IFireEvent LogicalNormalized => this;

        public LogicalFireEvent(int eventId) : base(eventId) { }
    }

    public abstract class LogicalPressEvent<TSw> : Event, IPressEvent, ILogicalEvent
        where TSw : DoubleThrowSwitch
    {
        public IReleaseEvent Opposition => OppositeReleaseEvent;

        public abstract LogicalReleaseEvent<TSw> OppositeReleaseEvent { get; }

        public IPressEvent LogicalNormalized => this;

        public LogicalPressEvent(int eventId) : base(eventId) { }
    }

    public abstract class LogicalReleaseEvent<TSw> : Event, IReleaseEvent, ILogicalEvent
        where TSw : DoubleThrowSwitch
    {
        public IPressEvent Opposition => OppositePressEvent;

        public abstract LogicalPressEvent<TSw> OppositePressEvent { get; }

        public IReleaseEvent LogicalNormalized => this;

        public LogicalReleaseEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalFireEvent<TSw> : Event, IFireEvent, IPhysicalEvent
        where TSw : SingleThrowSwitch
    {
        public IFireEvent LogicalNormalized => LogicalEquivalentFireEvent;

        public abstract LogicalFireEvent<TSw> LogicalEquivalentFireEvent { get; }

        public PhysicalFireEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalPressEvent<TSw> : Event, IPressEvent, IPhysicalEvent
        where TSw : DoubleThrowSwitch
    {
        public IReleaseEvent Opposition => OppositePhysicalReleaseEvent;

        public abstract PhysicalReleaseEvent<TSw> OppositePhysicalReleaseEvent { get; }

        public IPressEvent LogicalNormalized => LogicalEquivalentPressEvent;

        public abstract LogicalPressEvent<TSw> LogicalEquivalentPressEvent { get; }

        public PhysicalPressEvent(int eventId) : base(eventId) { }
    }

    public abstract class PhysicalReleaseEvent<TSw> : Event, IReleaseEvent, IPhysicalEvent
        where TSw : DoubleThrowSwitch
    {
        public IPressEvent Opposition => OppositePhysicalPressEvent;

        public abstract PhysicalPressEvent<TSw> OppositePhysicalPressEvent { get; }

        public IReleaseEvent LogicalNormalized => LogicalEquivalentReleaseEvent;

        public abstract LogicalReleaseEvent<TSw> LogicalEquivalentReleaseEvent { get; }

        public PhysicalReleaseEvent(int eventId) : base(eventId) { }
    }
}
