using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core
{
    public static class Helper
    {
        private static Def.Direction Convert(DSL.Def.AcceptableInIfStrokeClause move)
        {
            if (move is DSL.Def.MoveUp)
            {
                return Def.Direction.Up;
            }
            else if (move is DSL.Def.MoveDown)
            {
                return Def.Direction.Down;
            }
            else if (move is DSL.Def.MoveLeft)
            {
                return Def.Direction.Left;
            }
            else if (move is DSL.Def.MoveRight)
            {
                return Def.Direction.Right;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Def.Stroke Convert(IEnumerable<DSL.Def.AcceptableInIfStrokeClause> moves)
        {
            return new Def.Stroke(moves.Select(m => Convert(m)));
        }

        public static Def.Event.IDoubleActionSet Convert(DSL.Def.AcceptableInOnClause onButton)
        {
            if (onButton is DSL.Def.LeftButton)
            {
                return Def.Constant.LeftButtonDown;
            }
            else if (onButton is DSL.Def.MiddleButton)
            {
                return Def.Constant.MiddleButtonDown;
            }
            else if (onButton is DSL.Def.RightButton)
            {
                return Def.Constant.RightButtonDown;
            }
            else if (onButton is DSL.Def.X1Button)
            {
                return Def.Constant.X1ButtonDown;
            }
            else if (onButton is DSL.Def.X2Button)
            {
                return Def.Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static Def.Event.IEvent Convert(DSL.Def.AcceptableInIfButtonClause ifButton)
        {
            if (ifButton is DSL.Def.LeftButton)
            {
                return Def.Constant.LeftButtonDown;
            }
            else if (ifButton is DSL.Def.MiddleButton)
            {
                return Def.Constant.MiddleButtonDown;
            }
            else if (ifButton is DSL.Def.RightButton)
            {
                return Def.Constant.RightButtonDown;
            }
            else if (ifButton is DSL.Def.WheelUp)
            {
                return Def.Constant.WheelUp;
            }
            else if (ifButton is DSL.Def.WheelDown)
            {
                return Def.Constant.WheelDown;
            }
            else if (ifButton is DSL.Def.WheelLeft)
            {
                return Def.Constant.WheelLeft;
            }
            else if (ifButton is DSL.Def.WheelRight)
            {
                return Def.Constant.WheelRight;
            }
            else if (ifButton is DSL.Def.X1Button)
            {
                return Def.Constant.X1ButtonDown;
            }
            else if (ifButton is DSL.Def.X2Button)
            {
                return Def.Constant.X2ButtonDown;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
