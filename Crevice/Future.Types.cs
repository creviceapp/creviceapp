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

namespace Crevice.Future
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


    public abstract class Event
    {
        public int EventId { get; }

        public Event(int eventId)
        {
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
    
    public interface IFireEvent
    {

    }

    public interface IPressEvent
    {
        IReleaseEvent Opposition { get; }
    }

    public interface IReleaseEvent
    {
        IPressEvent Opposition { get; }
    }

    public abstract class FireEvent<T> : Event, IFireEvent
        where T : SingleThrowSwitch
    {
        public FireEvent(int eventId) : base(eventId) { }
    }

    public abstract class PressEvent<T> : Event, IPressEvent
        where T : DoubleThrowSwitch
    {
        public IReleaseEvent Opposition { get { return OppositePressEvent; } }

        public abstract ReleaseEvent<T> OppositePressEvent { get; }
        
        public PressEvent(int eventId) : base(eventId) { }
    }

    public abstract class ReleaseEvent<T> : Event, IReleaseEvent
        where T : DoubleThrowSwitch
    {
        public IPressEvent Opposition { get { return OppositeReleaseEvent; } }

        public abstract PressEvent<T> OppositeReleaseEvent { get; }
        
        public ReleaseEvent(int eventId) : base(eventId) { }
    }

    /*
     * 関係ないが、IEQuatable<>というのは、～のアップデート処理を行えますよ、というのにつかえそう
     */


    // Strokeをどういう扱いにするか、
    // システム組み込みなのでライブラリユーザーが触る必要はなさげ
    public class StrokeEvent : FireEvent<StrokeSwitch>, IEquatable<StrokeEvent>
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
            // Reserved EventId for StrokeEvent is 0.
            : base(0)
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

    // スタックマシン？

    // When以降の定義は各Eventごとにまとめることができる


}
