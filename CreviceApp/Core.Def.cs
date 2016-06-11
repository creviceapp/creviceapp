using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CreviceApp.Core
{
    public static class Def
    {
        public static class Event
        {
            public interface IEvent { }
                
            public class Move             : IEvent { }

            public interface IDoubleActionSet
            {
                IDoubleActionRelease GetPair();
            }
            public interface IDoubleActionRelease
            {
                IDoubleActionSet GetPair();
            }
            public interface ISingleAction { }

            public class LeftButtonDown   : IEvent, IDoubleActionSet
            {
                public IDoubleActionRelease GetPair() { return Constant.LeftButtonUp; }
            }
            public class LeftButtonUp     : IEvent, IDoubleActionRelease
            {
                public IDoubleActionSet GetPair() { return Constant.LeftButtonDown; }
            }

            public class MiddleButtonDown : IEvent, IDoubleActionSet
            {
                public IDoubleActionRelease GetPair() { return Constant.MiddleButtonUp; }
            }
            public class MiddleButtonUp   : IEvent, IDoubleActionRelease
            {
                public IDoubleActionSet GetPair() { return Constant.MiddleButtonDown; }
            }

            public class RightButtonDown  : IEvent, IDoubleActionSet
            {
                public IDoubleActionRelease GetPair() { return Constant.RightButtonUp; }
            }
            public class RightButtonUp    : IEvent, IDoubleActionRelease
            {
                public IDoubleActionSet GetPair() { return Constant.RightButtonDown; }
            }

            public class WheelDown        : IEvent, ISingleAction { }
            public class WheelUp          : IEvent, ISingleAction { }
            public class WheelLeft        : IEvent, ISingleAction { }
            public class WheelRight       : IEvent, ISingleAction { }

            public class X1ButtonDown     : IEvent, IDoubleActionSet
            {
                public IDoubleActionRelease GetPair() { return Constant.X1ButtonUp; }
            }
            public class X1ButtonUp       : IEvent, IDoubleActionRelease
            {
                public IDoubleActionSet GetPair() { return Constant.X1ButtonDown; }
            }

            public class X2ButtonDown     : IEvent, IDoubleActionSet
            {
                public IDoubleActionRelease GetPair() { return Constant.X2ButtonUp; }
            }
            public class X2ButtonUp       : IEvent, IDoubleActionRelease
            {
                public IDoubleActionSet GetPair() { return Constant.X2ButtonDown; }
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public class Stroke : List<Direction>, IEquatable<Stroke>
        {
            public Stroke() : base() { }
            public Stroke(int capacity) : base(capacity) { }
            public Stroke(IEnumerable<Direction> dirs) : base(dirs) { }

            public bool Equals(Stroke that)
            {
                if (that == null)
                {
                    return false;
                }
                return (this.SequenceEqual(that));
            }

            public override bool Equals(object obj)
            {
                if (obj == null || this.GetType() != obj.GetType())
                {
                    return false;
                }
                return Equals(obj as Stroke);
            }

            public override int GetHashCode()
            {
                var hash = 0x00;
                foreach (var move in this)
                {
                    hash = hash << 2;
                    switch(move)
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
                foreach (var move in this)
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

        private static Direction FromDSL(DSL.Def.AcceptableInIfStrokeClause move)
        {
            if (move is DSL.Def.MoveUp)
            {
                return Direction.Up;
            }
            else if (move is DSL.Def.MoveDown)
            {
                return Direction.Down;
            }
            else if (move is DSL.Def.MoveLeft)
            {
                return Direction.Left;
            }
            else if (move is DSL.Def.MoveRight)
            {
                return Direction.Right;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Stroke Convert(IEnumerable<DSL.Def.AcceptableInIfStrokeClause> moves)
        {
            return new Stroke(moves.Select(m => FromDSL(m)));
        }

        public static Event.IDoubleActionSet Convert(DSL.Def.AcceptableInOnClause onButton)
        {
            if (onButton is DSL.Def.LeftButton)
            {
                return Constant.LeftButtonDown;
            }
            else if (onButton is DSL.Def.MiddleButton)
            {
                return Constant.MiddleButtonDown;
            }
            else if (onButton is DSL.Def.RightButton)
            {
                return Constant.RightButtonDown;
            }
            else if (onButton is DSL.Def.X1Button)
            {
                return Constant.X1ButtonDown;
            }
            else if (onButton is DSL.Def.X2Button)
            {
                return Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Event.IEvent Convert(DSL.Def.AcceptableInIfButtonClause ifButton)
        {
            if (ifButton is DSL.Def.LeftButton)
            {
                return Constant.LeftButtonDown;
            }
            else if (ifButton is DSL.Def.MiddleButton)
            {
                return Constant.MiddleButtonDown;
            }
            else if (ifButton is DSL.Def.RightButton)
            {
                return Constant.RightButtonDown;
            }
            else if (ifButton is DSL.Def.WheelUp)
            {
                return Constant.WheelUp;
            }
            else if (ifButton is DSL.Def.WheelDown)
            {
                return Constant.WheelDown;
            }
            else if (ifButton is DSL.Def.WheelLeft)
            {
                return Constant.WheelLeft;
            }
            else if (ifButton is DSL.Def.WheelRight)
            {
                return Constant.WheelRight;
            }
            else if (ifButton is DSL.Def.X1Button)
            {
                return Constant.X1ButtonDown;
            }
            else if (ifButton is DSL.Def.X2Button)
            {
                return Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Action Convert(DSL.Def.DoFunc doFunc)
        {
            return Delegate.CreateDelegate(typeof(Action), doFunc.Method) as Action;
        }

        public class ConstantSingleton
        {
            private static ConstantSingleton singleton = new ConstantSingleton();

            public readonly Event.Move             Move             = new Event.Move();
            public readonly Event.LeftButtonDown   LeftButtonDown   = new Event.LeftButtonDown();
            public readonly Event.LeftButtonUp     LeftButtonUp     = new Event.LeftButtonUp();
            public readonly Event.MiddleButtonDown MiddleButtonDown = new Event.MiddleButtonDown();
            public readonly Event.MiddleButtonUp   MiddleButtonUp   = new Event.MiddleButtonUp();
            public readonly Event.RightButtonDown  RightButtonDown  = new Event.RightButtonDown();
            public readonly Event.RightButtonUp    RightButtonUp    = new Event.RightButtonUp();
            public readonly Event.WheelDown        WheelDown        = new Event.WheelDown();
            public readonly Event.WheelUp          WheelUp          = new Event.WheelUp();
            public readonly Event.WheelLeft        WheelLeft        = new Event.WheelLeft();
            public readonly Event.WheelRight       WheelRight       = new Event.WheelRight();
            public readonly Event.X1ButtonDown     X1ButtonDown     = new Event.X1ButtonDown();
            public readonly Event.X1ButtonUp       X1ButtonUp       = new Event.X1ButtonUp();
            public readonly Event.X2ButtonDown     X2ButtonDown     = new Event.X2ButtonDown();
            public readonly Event.X2ButtonUp       X2ButtonUp       = new Event.X2ButtonUp();

            public static ConstantSingleton GetInstance()
            {
                return singleton;
            }
        }

        public static ConstantSingleton Constant
        {
            get { return ConstantSingleton.GetInstance(); }
        }
    }
}
