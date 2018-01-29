using System;
using System.Collections.Generic;
using System.Text;

namespace Crevice.Core.Types
{
    public abstract class Switch { }
    public abstract class SingleThrowSwitch : Switch { }
    public abstract class DoubleThrowSwitch : Switch { }

    public class Group { }
    public class LogicalGroup : Group { }
    public class PhysicalGroup : Group { }

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

        public Event(int eventId)
        {
            EventId = eventId;
        }

        public bool Equals(Event that) => EventId == that.EventId;

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Event evnt)
            {
                return Equals(evnt);
            }
            return false;
        }

        public override int GetHashCode() => EventId;
    }

    public abstract class LogicalFireEvent<TLoG, TSw> : Event, IFireEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : SingleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public IFireEvent LogicalNormalized => this;
        
        public LogicalFireEvent(TLoG logicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
        }
    }

    public abstract class LogicalPressEvent<TLoG, TSw> : Event, IPressEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public IReleaseEvent Opposition => OppositeReleaseEvent;

        public abstract LogicalReleaseEvent<TLoG, TSw> OppositeReleaseEvent { get; }

        public IPressEvent LogicalNormalized => this;

        public LogicalPressEvent(TLoG logicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
        }
    }

    public abstract class LogicalReleaseEvent<TLoG, TSw> : Event, IReleaseEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public IPressEvent Opposition => OppositePressEvent;

        public abstract LogicalPressEvent<TLoG, TSw> OppositePressEvent { get; }

        public IReleaseEvent LogicalNormalized => this;

        public LogicalReleaseEvent(TLoG logicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
        }
    }

    public abstract class PhysicalFireEvent<TLoG, TPhyG, TSw> : Event, IFireEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : SingleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public TPhyG PhysicalGroup { get; }

        public IFireEvent LogicalNormalized => LogicalEquivalentFireEvent;

        public abstract LogicalFireEvent<TLoG, TSw> LogicalEquivalentFireEvent { get; }

        public PhysicalFireEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalPressEvent<TLoG, TPhyG, TSw> : Event, IPressEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public TPhyG PhysicalGroup { get; }

        public IReleaseEvent Opposition => OppositePhysicalReleaseEvent;

        public abstract PhysicalReleaseEvent<TLoG, TPhyG, TSw> OppositePhysicalReleaseEvent { get; }

        public IPressEvent LogicalNormalized => LogicalEquivalentPressEvent;

        public abstract LogicalPressEvent<TLoG, TSw> LogicalEquivalentPressEvent { get; }

        public PhysicalPressEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalReleaseEvent<TLoG, TPhyG, TSw> : Event, IReleaseEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TLoG LogicalGroup { get; }

        public TPhyG PhysicalGroup { get; }

        public IPressEvent Opposition => OppositePhysicalPressEvent;

        public abstract PhysicalPressEvent<TLoG, TPhyG, TSw> OppositePhysicalPressEvent { get; }

        public IReleaseEvent LogicalNormalized => LogicalEquivalentReleaseEvent;

        public abstract LogicalReleaseEvent<TLoG, TSw> LogicalEquivalentReleaseEvent { get; }

        public PhysicalReleaseEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(eventId)
        {
            LogicalGroup = logicalGroup;
            PhysicalGroup = physicalGroup;
        }
    }
}
