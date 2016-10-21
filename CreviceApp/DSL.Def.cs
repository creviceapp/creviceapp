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
    * WHEN                  ::= @when(WHEN_FUNC)           ( ON | IF_A | IF_B )
    * 
    * ON                    ::= @on(DOUBLE_TRIGGER_BUTTON) ( IF_A | IF_B | IF_C )
    * 
    * IF_A                  ::= @if(DOUBLE_TRIGGER_BUTTON) [ BEFORE ] DO [ AFTER ]
    *           
    * IF_B                  ::= @if(SINGLE_TRIGGER_BUTTON)   DO
    * 
    * IF_C                  ::= @if(MOVE *)                  DO
    * 
    * BEFORE                ::= @before(BEFORE_FUNC)
    * 
    * DO                    ::= @do(DO_FUNC) 
    * 
    * AFTER                 ::= @after(AFTER_FUNC)
    * 
    * DOUBLE_TRIGGER_BUTTON ::= L | M | R | X1 | X2
    * 
    * SINGLE_TRIGGER_BUTTON ::= W_UP | W_DOWN | W_LEFT | W_RIGHT
    * 
    * MOVE                  ::= MOVE_UP | MOVE_DOWN | MOVE_LEFT | MOVE_RIGHT
    * 
    * WHEN_FUNC             ::= delegate bool
    * 
    * BEFORE_FUNC           ::= delegate void
    * 
    * DO_FUNC               ::= delegate void
    * 
    * AFTER_FUNC            ::= delegate void
    * 
    */

    public static class Def
    {
        public delegate bool WhenFunc(Core.UserActionExecutionContext ctx);
        public delegate void BeforeFunc(Core.UserActionExecutionContext ctx);
        public delegate void DoFunc(Core.UserActionExecutionContext ctx);
        public delegate void AfterFunc(Core.UserActionExecutionContext ctx);

        public interface Button { }
        public interface AcceptableInOnClause : Button { }
        public interface AcceptableInIfButtonClause : Button { }
        public interface AcceptableInIfSingleTriggerButtonClause : AcceptableInIfButtonClause { }
        public interface AcceptableInIfDoubleTriggerButtonClause : AcceptableInIfButtonClause { }

        public class LeftButton   : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class MiddleButton : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class RightButton  : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class WheelUp      :                       AcceptableInIfSingleTriggerButtonClause { }
        public class WheelDown    :                       AcceptableInIfSingleTriggerButtonClause { }
        public class WheelLeft    :                       AcceptableInIfSingleTriggerButtonClause { }
        public class WheelRight   :                       AcceptableInIfSingleTriggerButtonClause { }
        public class X1Button     : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
        public class X2Button     : AcceptableInOnClause, AcceptableInIfDoubleTriggerButtonClause { }
            
        public interface Move { }
        public interface AcceptableInIfStrokeClause : Move { }
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
            public readonly X1Button     X1Button        = new X1Button();
            public readonly X2Button     X2Button        = new X2Button();

            public readonly MoveUp    MoveUp    = new MoveUp();
            public readonly MoveDown  MoveDown  = new MoveDown();
            public readonly MoveLeft  MoveLeft  = new MoveLeft();
            public readonly MoveRight MoveRight = new MoveRight();

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
