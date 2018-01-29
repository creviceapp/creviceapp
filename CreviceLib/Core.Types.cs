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

    public abstract class Event<TLoG>
        where TLoG : LogicalGroup
    {
        public TLoG LogicalGroup { get; }

        public int EventId { get; }

        public Event(TLoG logicalGroup, int eventId)
        {
            LogicalGroup = logicalGroup;
            EventId = eventId;
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType();
        }

        public override int GetHashCode()
        {
            return EventId;
        }
    }

    public abstract class LogicalFireEvent<TLoG, TSw> : Event<TLoG>, IFireEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : SingleThrowSwitch
    {
        public IFireEvent LogicalNormalized => this;
        
        public LogicalFireEvent(TLoG logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class LogicalPressEvent<TLoG, TSw> : Event<TLoG>, IPressEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : DoubleThrowSwitch
    {
        public IReleaseEvent Opposition => OppositeReleaseEvent;

        public abstract LogicalReleaseEvent<TLoG, TSw> OppositeReleaseEvent { get; }

        public IPressEvent LogicalNormalized => this;

        public LogicalPressEvent(TLoG logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class LogicalReleaseEvent<TLoG, TSw> : Event<TLoG>, IReleaseEvent, ILogicalEvent
        where TLoG : LogicalGroup
        where TSw : DoubleThrowSwitch
    {
        public IPressEvent Opposition => OppositePressEvent;

        public abstract LogicalPressEvent<TLoG, TSw> OppositePressEvent { get; }

        public IReleaseEvent LogicalNormalized => this;

        public LogicalReleaseEvent(TLoG logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class PhysicalFireEvent<TLoG, TPhyG, TSw> : Event<TLoG>, IFireEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : SingleThrowSwitch
    {
        public TPhyG PhysicalGroup { get; }

        public IFireEvent LogicalNormalized => LogicalEquivalentFireEvent;

        public abstract LogicalFireEvent<TLoG, TSw> LogicalEquivalentFireEvent { get; }

        public PhysicalFireEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalPressEvent<TLoG, TPhyG, TSw> : Event<TLoG>, IPressEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TPhyG PhysicalGroup { get; }

        public IReleaseEvent Opposition => OppositePhysicalReleaseEvent;

        public abstract PhysicalReleaseEvent<TLoG, TPhyG, TSw> OppositePhysicalReleaseEvent { get; }

        public IPressEvent LogicalNormalized => LogicalEquivalentPressEvent;

        public abstract LogicalPressEvent<TLoG, TSw> LogicalEquivalentPressEvent { get; }

        public PhysicalPressEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalReleaseEvent<TLoG, TPhyG, TSw> : Event<TLoG>, IReleaseEvent, IPhysicalEvent
        where TLoG : LogicalGroup
        where TPhyG : PhysicalGroup
        where TSw : DoubleThrowSwitch
    {
        public TPhyG PhysicalGroup { get; }

        public IPressEvent Opposition => OppositePhysicalPressEvent;

        public abstract PhysicalPressEvent<TLoG, TPhyG, TSw> OppositePhysicalPressEvent { get; }

        public IReleaseEvent LogicalNormalized => LogicalEquivalentReleaseEvent;

        public abstract LogicalReleaseEvent<TLoG, TSw> LogicalEquivalentReleaseEvent { get; }

        public PhysicalReleaseEvent(TLoG logicalGroup, TPhyG physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }
}
