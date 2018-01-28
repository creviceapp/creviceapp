using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// CreviceLib

/*
 * Strokeの描写などをするにはGestureMachineのイベントが取れればいけそうかな
 * 
 * 
 * Linuxなどのマルチプラットフォーム対応、
 * キーボードやマウスパッドの入力への対応を考えると、InputデータとFSMをもっと抽象化する必要がある。
 * また、ジェスチャの深さは本来的には制限をなくせるはず。
 * 
 * 複数デバイスの対応は、イベントの同一判定と定義を工夫することで可能になる、ような気がする。
 * 
 * var Whenever = @when(ctx => { return true; });
 * Whenever.
 * @if().
 * @if().
 * ...
 * @press(ctx => {
 *     //
 * }).
 * @do(ctx => {
 *     //
 * }).
 * @release(ctx => {
 *     //
 * }).
 * @as("name"); // // @as({"name": "hogehoge", "icon": "path", "url": "path" ... });
 * 
 * でSingleActionかDoubleActionで次の型が変わる
 * 
 * 互換性のために@ifと@onはどっちでもいいとか
 * 
 * 
 * 
 * ジェスチャの候補の列挙
 * →ジェスチャのステート切替時がイベントとして存在して、マシンにデータが有るなら可能
 * 
 */

namespace Crevice.Core
{
    /* ・マルチプラットフォーム対応（設計のみ）
         * ・深さ無段階のジェスチャ定義
         * ・マウス/キーボード/ゲームパッド入力対応
         * ・
         * ・
         */

    public abstract class Switch { }
    public abstract class SingleThrowSwitch : Switch { }
    public abstract class DoubleThrowSwitch : Switch { }

    public class StrokeSwitch : SingleThrowSwitch { }
    public class MoveSwich : SingleThrowSwitch { }

    public class WheelDownSwitch : SingleThrowSwitch { }
    public class WheelUpSwitch : SingleThrowSwitch { }
    public class WheelLeftSwitch : SingleThrowSwitch { }
    public class WHeelRightSwitch : SingleThrowSwitch { }

    public class LeftButtonSwitch : DoubleThrowSwitch { }
    public class MiddleButtonSwitch : DoubleThrowSwitch { }
    public class RightButtonSwitch : DoubleThrowSwitch { }
    public class X1ButtonSwitch : DoubleThrowSwitch { }
    public class X2ButtonSwitch : DoubleThrowSwitch { }

    public class EventLogicalGroup { }
    public class EventPhysicalGroup { }

    public abstract class Event<L>
        where L : EventLogicalGroup
    {
        public L LogicalGroup { get; }

        public int EventId { get; }

        public Event(L group, int eventId)
        {
            this.LogicalGroup = group;
            this.EventId = eventId;
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
    
    public abstract class FireEvent<L, SW> : Event<L>, IFireEvent, ILogicalEvent
        where L : EventLogicalGroup
        where SW : SingleThrowSwitch
    {
        public IFireEvent LogicalNormalized => this;
        
        public FireEvent(L logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class PressEvent<L, SW> : Event<L>, IPressEvent, ILogicalEvent
        where L : EventLogicalGroup
        where SW : DoubleThrowSwitch
    {
        public IReleaseEvent Opposition => OppositeReleaseEvent;

        public abstract ReleaseEvent<L, SW> OppositeReleaseEvent { get; }

        public IPressEvent LogicalNormalized => this;

        public PressEvent(L logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class ReleaseEvent<L, SW> : Event<L>, IReleaseEvent, ILogicalEvent
        where L : EventLogicalGroup
        where SW : DoubleThrowSwitch
    {
        public IPressEvent Opposition => OppositePressEvent;

        public abstract PressEvent<L, SW> OppositePressEvent { get; }

        public IReleaseEvent LogicalNormalized => this;

        public ReleaseEvent(L logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public abstract class PhysicalFireEvent<L, P, SW> : Event<L>, IFireEvent, IPhysicalEvent
        where L : EventLogicalGroup
        where P : EventPhysicalGroup
        where SW : SingleThrowSwitch
    {
        public P PhysicalGroup { get; }

        public IFireEvent LogicalNormalized => LogicalEquivalentFireEvent;

        public abstract FireEvent<L, SW> LogicalEquivalentFireEvent { get; }

        public PhysicalFireEvent(L logicalGroup, P physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalPressEvent<L, P, SW> : Event<L>, IPressEvent, IPhysicalEvent
        where L : EventLogicalGroup
        where P : EventPhysicalGroup
        where SW : DoubleThrowSwitch
    {
        public P PhysicalGroup { get; }

        public IReleaseEvent Opposition => OppositePhysicalReleaseEvent;

        public abstract PhysicalReleaseEvent<L, P, SW> OppositePhysicalReleaseEvent { get; }

        public IPressEvent LogicalNormalized => LogicalEquivalentPressEvent;

        public abstract PressEvent<L, SW> LogicalEquivalentPressEvent { get; }

        public PhysicalPressEvent(L logicalGroup, P physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }

    public abstract class PhysicalReleaseEvent<L, P, SW> : Event<L>, IReleaseEvent, IPhysicalEvent
        where L : EventLogicalGroup
        where P : EventPhysicalGroup
        where SW : DoubleThrowSwitch
    {
        public P PhysicalGroup { get; }

        public IPressEvent Opposition => OppositePhysicalPressEvent;

        public abstract PhysicalPressEvent<L, P, SW> OppositePhysicalPressEvent { get; }

        public IReleaseEvent LogicalNormalized => LogicalEquivalentReleaseEvent;

        public abstract ReleaseEvent<L, SW> LogicalEquivalentReleaseEvent { get; }

        public PhysicalReleaseEvent(L logicalGroup, P physicalGroup, int eventId) : base(logicalGroup, eventId)
        {
            PhysicalGroup = physicalGroup;
        }
    }

    // Strokeをどういう扱いにするか、
    // システム組み込みなのでライブラリユーザーが触る必要はなさげ
    // イベントとして実装する意味はなさげ
    public class StrokeEvent : IEquatable<StrokeEvent>
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public readonly IEnumerable<Direction> Directions;

        public StrokeEvent(IEnumerable<Direction> directions)
        {
            Directions = directions;
        }

        public bool Equals(StrokeEvent that)
        {
            if (that == null)
            {
                return false;
            }
            return (Directions.SequenceEqual(that.Directions));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Equals(obj as StrokeEvent);
        }

        public override int GetHashCode()
        {
            var hash = 0x00;
            foreach (var move in Directions)
            {
                hash = hash << 2;
                switch (move)
                {
                    case Direction.Up:
                        hash = hash | 0x00;
                        break;
                    case Direction.Down:
                        hash = hash | 0x01;
                        break;
                    case Direction.Left:
                        hash = hash | 0x02;
                        break;
                    case Direction.Right:
                        hash = hash | 0x03;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return hash;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var move in Directions)
            {
                switch (move)
                {
                    case Direction.Up:
                        sb.Append("U");
                        break;
                    case Direction.Down:
                        sb.Append("D");
                        break;
                    case Direction.Left:
                        sb.Append("L");
                        break;
                    case Direction.Right:
                        sb.Append("R");
                        break;
                }
            }
            return sb.ToString();
        }
    }
}
