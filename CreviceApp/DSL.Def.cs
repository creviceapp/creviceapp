using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    /**
    * BNF of Gesture Definition DSL 
    * 
    * WHEN      ::= @when(WHEN_FUNC) ON
    * 
    * ON        ::= @on(ON_BUTTON)   IF
    * 
    * IF        ::= @if(IF_BUTTON)   DO
    *             | @if(MOVE *)      DO
    * 
    * DO        ::= @do(DO_FUNC) 
    * 
    * ON_BUTTON ::= L | M | R | X1 | X2
    * 
    * IF_BUTTON ::= L | M | R | X1 | X2 | W_UP | W_DOWN | W_LEFT | W_RIGHT
    * 
    * MOVE      ::= MOVE_UP | MOVE_DOWN | MOVE_LEFT | MOVE_RIGHT
    * 
    * WHEN_FUNC ::= Func<bool>
    * 
    * DO_FUNC   ::= Action
    * 
    */

    public static class Def
    {
        public delegate bool WhenFunc();
        public delegate void DoFunc();
            
        public interface AcceptableInOnClause { }
        public interface AcceptableInIfButtonClause { }
        public class LeftButton   : AcceptableInOnClause, AcceptableInIfButtonClause { }
        public class MiddleButton : AcceptableInOnClause, AcceptableInIfButtonClause { }
        public class RightButton  : AcceptableInOnClause, AcceptableInIfButtonClause { }
        public class WheelUp      :                       AcceptableInIfButtonClause { }
        public class WheelDown    :                       AcceptableInIfButtonClause { }
        public class WheelLeft    :                       AcceptableInIfButtonClause { }
        public class WheelRight   :                       AcceptableInIfButtonClause { }
        public class X1Button     : AcceptableInOnClause, AcceptableInIfButtonClause { }
        public class X2Button     : AcceptableInOnClause, AcceptableInIfButtonClause { }
            
        public interface AcceptableInIfStrokeClause { }
        public class MoveUp    : AcceptableInIfStrokeClause { }
        public class MoveDown  : AcceptableInIfStrokeClause { }
        public class MoveLeft  : AcceptableInIfStrokeClause { }
        public class MoveRight : AcceptableInIfStrokeClause { }
            
        public class ConstantSingleton
        {
            private static ConstantSingleton singleton = new ConstantSingleton();

            public readonly LeftButton   LeftButton      = new LeftButton();
            public readonly MiddleButton MiddleButton    = new MiddleButton();
            public readonly RightButton  RightButton     = new RightButton();
            public readonly WheelDown    WheelDown       = new WheelDown();
            public readonly WheelUp      WheelUp         = new WheelUp();
            public readonly WheelLeft    WheelLeft       = new WheelLeft();
            public readonly WheelRight   WheelRight      = new WheelRight();
            public readonly X1Button     X1ButtonDown    = new X1Button();
            public readonly X2Button     X2ButtonDown    = new X2Button();
                
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
