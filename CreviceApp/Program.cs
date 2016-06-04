using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
    
    namespace Core
    {
        public static class Def
        {
            public static class Trigger
            {
                public interface ITrigger { }
                public interface IDoubleActionSet
                {
                    IDoubleActionRelease GetPair();
                }
                public interface IDoubleActionRelease
                {
                    IDoubleActionSet GetPair();
                }
                public interface ISingleAction { }
                public class LeftButtonDown   : ITrigger, IDoubleActionSet
                {
                    public IDoubleActionRelease GetPair() { return Constant.LeftButtonUp; }
                }
                public class LeftButtonUp     : ITrigger, IDoubleActionRelease
                {
                    public IDoubleActionSet GetPair() { return Constant.LeftButtonDown; }
                }
                public class MiddleButtonDown : ITrigger, IDoubleActionSet
                {
                    public IDoubleActionRelease GetPair() { return Constant.MiddleButtonUp; }
                }
                public class MiddleButtonUp   : ITrigger, IDoubleActionRelease
                {
                    public IDoubleActionSet GetPair() { return Constant.MiddleButtonDown; }
                }
                public class RightButtonDown  : ITrigger, IDoubleActionSet
                {
                    public IDoubleActionRelease GetPair() { return Constant.RightButtonUp; }
                }
                public class RightButtonUp    : ITrigger, IDoubleActionRelease
                {
                    public IDoubleActionSet GetPair() { return Constant.RightButtonDown; }
                }
                public class WheelDown        : ITrigger, ISingleAction { }
                public class WheelUp          : ITrigger, ISingleAction { }
                public class WheelLeft        : ITrigger, ISingleAction { }
                public class WheelRight       : ITrigger, ISingleAction { }
                public class X1ButtonDown     : ITrigger, IDoubleActionSet
                {
                    public IDoubleActionRelease GetPair() { return Constant.X1ButtonUp; }
                }
                public class X1ButtonUp       : ITrigger, IDoubleActionRelease
                {
                    public IDoubleActionSet GetPair() { return Constant.X1ButtonDown; }
                }
                public class X2ButtonDown     : ITrigger, IDoubleActionSet
                {
                    public IDoubleActionRelease GetPair() { return Constant.X2ButtonUp; }
                }
                public class X2ButtonUp       : ITrigger, IDoubleActionRelease
                {
                    public IDoubleActionSet GetPair() { return Constant.X2ButtonDown; }
                }
            }

            public enum Move
            {
                Up,
                Down,
                Left,
                Right
            }

            public class Stroke : List<Move>, IEquatable<Stroke>
            {
                public Stroke() : base() { }
                public Stroke(int capacity) : base(capacity) { }
                public Stroke(IEnumerable<Move> moves) : base(moves) { }

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
                            case Move.Up:
                                hash = hash | 0x00;
                                break;
                            case Move.Down:
                                hash = hash | 0x01;
                                break;
                            case Move.Left:
                                hash = hash | 0x02;
                                break;
                            case Move.Right:
                                hash = hash | 0x03;
                                break;
                            default:
                                throw new ArgumentException();
                        }
                    }
                    return hash;
                }
            }

            private static Move FromDSL(Config.Def.AcceptableInIfStrokeClause move)
            {
                if (move is Config.Def.MoveUp)
                {
                    return Move.Up;
                }
                else if (move is Config.Def.MoveDown)
                {
                    return Move.Down;
                }
                else if (move is Config.Def.MoveLeft)
                {
                    return Move.Left;
                }
                else if (move is Config.Def.MoveRight)
                {
                    return Move.Right;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public static Stroke Convert(IEnumerable<Config.Def.AcceptableInIfStrokeClause> moves)
            {
                return new Stroke(moves.Select(m => FromDSL(m)));
            }

            public static Trigger.IDoubleActionSet Convert(Config.Def.AcceptableInOnClause onButton)
            {
                if (onButton is Config.Def.LeftButton)
                {
                    return Constant.LeftButtonDown;
                }
                else if (onButton is Config.Def.MiddleButton)
                {
                    return Constant.MiddleButtonDown;
                }
                else if (onButton is Config.Def.RightButton)
                {
                    return Constant.RightButtonDown;
                }
                else if (onButton is Config.Def.X1Button)
                {
                    return Constant.X1ButtonDown;
                }
                else if (onButton is Config.Def.X2Button)
                {
                    return Constant.X2ButtonDown;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public static Trigger.ITrigger Convert(Config.Def.AcceptableInIfButtonClause ifButton)
            {
                if (ifButton is Config.Def.LeftButton)
                {
                    return Constant.LeftButtonDown;
                }
                else if (ifButton is Config.Def.MiddleButton)
                {
                    return Constant.MiddleButtonDown;
                }
                else if (ifButton is Config.Def.RightButton)
                {
                    return Constant.RightButtonDown;
                }
                else if (ifButton is Config.Def.WheelUp)
                {
                    return Constant.WheelUp;
                }
                else if (ifButton is Config.Def.WheelDown)
                {
                    return Constant.WheelDown;
                }
                else if (ifButton is Config.Def.WheelLeft)
                {
                    return Constant.WheelLeft;
                }
                else if (ifButton is Config.Def.WheelRight)
                {
                    return Constant.WheelRight;
                }
                else if (ifButton is Config.Def.X1Button)
                {
                    return Constant.X1ButtonDown;
                }
                else if (ifButton is Config.Def.X2Button)
                {
                    return Constant.X2ButtonDown;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public static Action Convert(Config.Def.DoFunc doFunc)
            {
                return Delegate.CreateDelegate(typeof(Action), doFunc.Method) as Action;
            }

            public class ConstantSingleton
            {
                private static ConstantSingleton singleton = new ConstantSingleton();

                public readonly Trigger.LeftButtonDown   LeftButtonDown   = new Trigger.LeftButtonDown();
                public readonly Trigger.LeftButtonUp     LeftButtonUp     = new Trigger.LeftButtonUp();
                public readonly Trigger.MiddleButtonDown MiddleButtonDown = new Trigger.MiddleButtonDown();
                public readonly Trigger.MiddleButtonUp   MiddleButtonUp   = new Trigger.MiddleButtonUp();
                public readonly Trigger.RightButtonDown  RightButtonDown  = new Trigger.RightButtonDown();
                public readonly Trigger.RightButtonUp    RightButtonUp    = new Trigger.RightButtonUp();
                public readonly Trigger.WheelDown        WheelDown        = new Trigger.WheelDown();
                public readonly Trigger.WheelUp          WheelUp          = new Trigger.WheelUp();
                public readonly Trigger.WheelLeft        WheelLeft        = new Trigger.WheelLeft();
                public readonly Trigger.WheelRight       WheelRight       = new Trigger.WheelRight();
                public readonly Trigger.X1ButtonDown     X1ButtonDown     = new Trigger.X1ButtonDown();
                public readonly Trigger.X1ButtonUp       X1ButtonUp       = new Trigger.X1ButtonUp();
                public readonly Trigger.X2ButtonDown     X2ButtonDown     = new Trigger.X2ButtonDown();
                public readonly Trigger.X2ButtonUp       X2ButtonUp       = new Trigger.X2ButtonUp();

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

        namespace FSM
        {
            public static class Transition
            {
                #region Transition Definition
                // Transition 0
                // State0 -> State1
                // Transition from initilal state(S0) to the state(S1) holding primary double action mouse button.
                public static IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>>
                    Gen0(IEnumerable<GestureDefinition> gestureDef)
                {
                    return gestureDef
                        .ToLookup(x => Def.Convert(x.onButton))
                        .ToDictionary(x => x.Key, x => x.Select(y => y));
                }

                // Transition 1
                // State1 -> State2
                // Transition from the state(S1) to the state(S2) holding primary and secondary double action mouse buttons.
                public static IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>>
                    Gen1(IEnumerable<GestureDefinition> gestureDef)
                {
                    return gestureDef
                        .Select(x => x as ButtonGestureDefinition)
                        .Where(x => x != null)
                        .ToLookup(x => Def.Convert(x.ifButton))
                        .Where(x => x.Key is Def.Trigger.IDoubleActionSet)
                        .ToDictionary(x => x.Key as Def.Trigger.IDoubleActionSet, x => x.Select(y => y));
                }

                // Transition 2
                // State1 -> State1
                // Transition from the state(S1) to the state(S1), in other words, no tansition. However, functions given as
                // the parameter of `@do` clause of ButtonGestureDefinition are executed. This event happens when a single 
                // action mouse button is pressed.
                public static IDictionary<Def.Trigger.ISingleAction, IEnumerable<ButtonGestureDefinition>>
                    Gen2(IEnumerable<GestureDefinition> gestureDef)
                {
                    return gestureDef
                        .Select(x => x as ButtonGestureDefinition)
                        .Where(x => x != null)
                        .ToLookup(x => Def.Convert(x.ifButton))
                        .Where(x => x.Key is Def.Trigger.ISingleAction)
                        .ToDictionary(x => x.Key as Def.Trigger.ISingleAction, x => x.Select(y => y));
                }

                // Transition 3
                // State1 -> State0
                // Transition from the state(S1) to the state(S0) holding no double action mouse button, and functions given as
                // the parameter of `@do` clause of StrokeGestureDefinition are executed. This event happens when primary double 
                // action mouse button is released.
                public static IDictionary<Def.Stroke, IEnumerable<StrokeGestureDefinition>>
                    Gen3(IEnumerable<GestureDefinition> gestureDef)
                {
                    return gestureDef
                        .Select(x => x as StrokeGestureDefinition)
                        .Where(x => x != null)
                        .ToLookup(x => x.stroke)
                        .ToDictionary(x => x.Key, x => x.Select(y => y));
                }

                // Transition 4
                // State1 -> State0
                // Transition from the state(S1) to the state(S0) holding no double action mouse button with only one side effect
                // that primary double action mouse button will be restored. This event happens whenprimary double action mouse 
                // button is released.


                // Transition 5
                // State1 -> State0
                // Transition from the state(S1) to the state(S0) holding no double action mouse buttion with no side effect.
                // This event happens when primary double action mouse button is released.

                // Transition 6
                // State2 -> State1
                // Transition from the state(S2) to the state(S1) holding primary double action mouse button, and functions 
                // given as the parameter of `@do` clause of ButtonGestureDefinition are executed. This event happens when
                // secondary double action mouse button is released.

                // Transition 7
                // State2 -> State0
                // Transition from the state(S2) to the state(S0) holding no double action mouse button. This event happends when 
                // primary double action is released in irregular order as a trigger. After this transition, previous secondary 
                // double action mouse button left holding is marked as irreggularly holding by the user, and the next release 
                // event of it will be ignored.
                #endregion


            }

            public class GestureMachine
            {
                private IState state;

                public GestureMachine(IEnumerable<GestureDefinition> gestureDef)
                {
                    this.state = new State0(Transition.Gen0(gestureDef));
                }

                public bool Input(Def.Trigger.ITrigger trigger)
                {
                    var result = this.state.Input(trigger);
                    this.state = result.NextState;
                    return result.Trigger.IsConsumed;
                }
            }

            public class Result
            {
                public class TriggerResult
                {
                    public bool IsConsumed { get; private set; }
                    public TriggerResult(bool consumed)
                    {
                        this.IsConsumed = consumed;
                    }
                }
                public TriggerResult Trigger;
                public IState NextState { get; private set; }

                private Result(bool consumed, IState nextState)
                {
                    this.Trigger = new TriggerResult(consumed);
                    this.NextState = nextState;
                }

                public static Result TriggerIsConsumed(IState nextState)
                {
                    return new Result(true, nextState);
                }

                public static Result TriggerIsRemaining(IState nextState)
                {
                    return new Result(false, nextState);
                }
            }

            public interface IState
            {
                Result Input(Def.Trigger.ITrigger trigger);
            }

            public abstract class State : IState
            {
                public class GlobalValues
                {
                    public readonly HashSet<Def.Trigger.IDoubleActionRelease> ignoreNext = new HashSet<Def.Trigger.IDoubleActionRelease>();
                }

                internal readonly GlobalValues Global;

                public State(GlobalValues Global)
                {
                    this.Global = Global;
                }

                // Check whether given trigger must be ignored or not.
                // Return true if given trigger is in the ignore list, and remove it from ignore list.
                // Return false if the pair of given trigger is in the ignore list, and remove it from ignore list.
                // Otherwise return false.
                public bool MustBeIgnored(Def.Trigger.ITrigger trigger)
                {
                    if (trigger is Def.Trigger.IDoubleActionRelease)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionRelease;
                        if (Global.ignoreNext.Contains(t))
                        {
                            Global.ignoreNext.Remove(t);
                            return true;
                        }
                    }
                    else if (trigger is Def.Trigger.IDoubleActionSet)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionSet;
                        var p = t.GetPair();
                        if (Global.ignoreNext.Contains(p))
                        {
                            Global.ignoreNext.Remove(p);
                            return false;
                        }
                    }
                    return false;
                }

                public virtual Result Input(Def.Trigger.ITrigger trigger)
                {
                    return Result.TriggerIsRemaining(nextState: this);
                }
            }
            
            public class State0 : State
            {
                private readonly IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>> T0;
                
                public State0(IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<GestureDefinition>> T0) : base(new GlobalValues())
                {
                    this.T0 = T0;
                }

                public override Result Input(Def.Trigger.ITrigger trigger)
                {
                    if (MustBeIgnored(trigger))
                    {
                        return Result.TriggerIsConsumed(nextState: this);
                    }

                    if (trigger is Def.Trigger.IDoubleActionSet)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionSet;
                        if (T0.Keys.Contains(t))
                        {
                            var gestureDef = FilterByWhenClause(T0[t]);
                            if (gestureDef.Count() > 0)
                            {
                                // Transition 0
                                return Result.TriggerIsConsumed(nextState: new State1(Global, this, t, gestureDef));
                            }
                        }
                    }
                    return base.Input(trigger);
                }

                public static IEnumerable<GestureDefinition> FilterByWhenClause(IEnumerable<GestureDefinition> gestureDef)
                {
                    // This evaluation of functions given as the parameter of `@when` clause can be executed in parallel, 
                    // but executing it in sequential order here for simplicity.
                    
                    var cache = gestureDef
                        .Select(x => x.whenFunc)
                        .Distinct()
                        .ToDictionary(x => x, x => x());

                    return gestureDef
                        .Where(x => cache[x.whenFunc] == true)
                        .ToList();
                }
            }

            public class State1 : State
            {
                private readonly State0 S0;
                private readonly Def.Trigger.IDoubleActionSet primaryTrigger;
                private readonly IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1;
                private readonly IDictionary<Def.Trigger.ISingleAction, IEnumerable<ButtonGestureDefinition>> T2;
                private readonly IDictionary<Def.Stroke, IEnumerable<StrokeGestureDefinition>> T3;

                private readonly SingleInputSender inputSender = new SingleInputSender();

                private bool PrimaryTriggerIsRestorable { get; set; } = true;

                public State1(
                    GlobalValues Global,
                    State0 S0,
                    Def.Trigger.IDoubleActionSet primaryTrigger,
                    IEnumerable<GestureDefinition> gestureDef
                    ) : base(Global)
                {
                    this.S0 = S0;
                    this.primaryTrigger = primaryTrigger;
                    this.T1 = Transition.Gen1(gestureDef);
                    this.T2 = Transition.Gen2(gestureDef);
                    this.T3 = Transition.Gen3(gestureDef);
                }

                public override Result Input(Def.Trigger.ITrigger trigger)
                {
                    if (MustBeIgnored(trigger))
                    {
                        return Result.TriggerIsConsumed(nextState: this);
                    }

                    if (trigger is Def.Trigger.IDoubleActionSet)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionSet;
                        if (T1.Keys.Contains(t))
                        {
                            // Transition 1
                            PrimaryTriggerIsRestorable = false;
                            return Result.TriggerIsConsumed(nextState: new State2(Global, S0, this, primaryTrigger, t, T1));
                        }
                    }
                    else if (trigger is Def.Trigger.ISingleAction)
                    {
                        var t = trigger as Def.Trigger.ISingleAction;
                        if (T2.Keys.Contains(t))
                        {
                            // Transition 2
                            PrimaryTriggerIsRestorable = false;
                            Task.Run(() => {
                                foreach (var gDef in T2[t])
                                {
                                    gDef.doFunc();
                                }
                            });
                            return Result.TriggerIsConsumed(nextState: this);
                        }
                    }
                    else if (trigger is Def.Trigger.IDoubleActionRelease)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionRelease;
                        if (t == primaryTrigger.GetPair())
                        {
                            // if (T3[t] == strokeMachine.GetStroke())
                            //{
                            // Transition 3
                            //}

                            // Transition 4 and 5
                            if (PrimaryTriggerIsRestorable)
                            {
                                // Side effect of Transition 4
                                RestorePrimaryTrigger();
                            }
                            return Result.TriggerIsConsumed(nextState: S0);
                        }
                    }
                    return base.Input(trigger);
                }
                
                private void RestorePrimaryTrigger()
                {
                    if (primaryTrigger == Def.Constant.LeftButtonDown)
                    {
                        inputSender.LeftClick();
                    }
                    else if (primaryTrigger == Def.Constant.MiddleButtonDown)
                    {
                        inputSender.MiddleClick();
                    }
                    else if (primaryTrigger == Def.Constant.RightButtonDown)
                    {
                        inputSender.RightClick();
                    }
                    else if (primaryTrigger == Def.Constant.X1ButtonDown)
                    {
                        inputSender.X1Click();
                    }
                    else if (primaryTrigger == Def.Constant.X2ButtonDown)
                    {
                        inputSender.X2Click();
                    }
                }
            }

            public class State2 : State
            {
                private readonly State0 S0;
                private readonly State1 S1;
                private readonly Def.Trigger.IDoubleActionSet primaryTrigger;
                private readonly Def.Trigger.IDoubleActionSet secondaryTrigger;
                private readonly IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1;

                public State2(
                    GlobalValues Global,
                    State0 S0,
                    State1 S1,
                    Def.Trigger.IDoubleActionSet primaryTrigger,
                    Def.Trigger.IDoubleActionSet secondaryTrigger,
                    IDictionary<Def.Trigger.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>> T1
                    ) : base(Global)
                {
                    this.S0 = S0;
                    this.S1 = S1;
                    this.primaryTrigger = primaryTrigger;
                    this.secondaryTrigger = secondaryTrigger;
                    this.T1 = T1;
                }

                public override Result Input(Def.Trigger.ITrigger trigger)
                {
                    if (MustBeIgnored(trigger))
                    {
                        return Result.TriggerIsConsumed(nextState: this);
                    }
                    
                    if (trigger is Def.Trigger.IDoubleActionRelease)
                    {
                        var t = trigger as Def.Trigger.IDoubleActionRelease;
                        if (t == secondaryTrigger.GetPair())
                        {
                            // Transtion 6
                            Task.Run(() => {
                                foreach (var gDef in T1[secondaryTrigger])
                                {
                                    gDef.doFunc();
                                }
                            });
                            return Result.TriggerIsConsumed(nextState: S1);
                        }
                        else if (t == primaryTrigger.GetPair())
                        {
                            // Transition 7
                            // Add the release event of secondary trigger to ignore list.
                            Global.ignoreNext.Add(secondaryTrigger.GetPair());
                            return Result.TriggerIsConsumed(nextState: S0);
                        }
                    }
                    return base.Input(trigger);
                }
            }

            public static class ConfigDSLTreeParser
            {
                public static IEnumerable<GestureDefinition> TreeToGestureDefinition(Config.DSL.Root root)
                {
                    List<GestureDefinition> gestureDef = new List<GestureDefinition>();
                    Debug.Print("Parsing tree of GestureConfig.DSL");
                    foreach (var whenElement in root.whenElements)
                    {
                        if (whenElement.onElements.Count == 0)
                        {
                            gestureDef.Add(new GestureDefinition(whenElement.func, null));
                            continue;
                        }
                        foreach (var onElement in whenElement.onElements)
                        {
                            if (onElement.ifButtonElements.Count == 0 && onElement.ifStrokeElements.Count == 0)
                            {
                                gestureDef.Add(new GestureDefinition(whenElement.func, onElement.button));
                                continue;
                            }
                            foreach (var ifButtonElement in onElement.ifButtonElements)
                            {
                                if (ifButtonElement.doElements.Count == 0)
                                {
                                    gestureDef.Add(new ButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null));
                                    continue;
                                }
                                foreach (var doElement in ifButtonElement.doElements)
                                {
                                    gestureDef.Add(new ButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, Def.Convert(doElement.func)));
                                }
                            }
                            foreach (var ifStrokeElement in onElement.ifStrokeElements)
                            {
                                var stroke = Def.Convert(ifStrokeElement.moves);
                                if (ifStrokeElement.doElements.Count == 0)
                                {
                                    gestureDef.Add(new StrokeGestureDefinition(whenElement.func, onElement.button, stroke, null));
                                    continue;
                                }
                                foreach (var doElement in ifStrokeElement.doElements)
                                {
                                    gestureDef.Add(new StrokeGestureDefinition(whenElement.func, onElement.button, stroke, Def.Convert(doElement.func)));
                                }
                            }
                        }
                    }
                    Debug.Print("Parse end.");
                    return gestureDef; 
                }
                
                public static IEnumerable<GestureDefinition> FilterComplete(IEnumerable<GestureDefinition> gestureDef)
                {
                    return gestureDef
                        .Where(x => x.IsComplete)
                        .ToList();
                }
            }

            public class GestureDefinition
            {
                public readonly Config.Def.WhenFunc whenFunc;
                public readonly Config.Def.AcceptableInOnClause onButton;
                public GestureDefinition(
                    Config.Def.WhenFunc whenFunc,
                    Config.Def.AcceptableInOnClause onButton
                    )
                {
                    this.whenFunc = whenFunc;
                    this.onButton = onButton;
                }
                virtual public bool IsComplete
                {
                    get { return false; }
                }
            }

            public class ButtonGestureDefinition : GestureDefinition
            {
                public readonly Config.Def.AcceptableInIfButtonClause ifButton;
                public readonly Action doFunc;
                public ButtonGestureDefinition(
                    Config.Def.WhenFunc whenFunc,
                    Config.Def.AcceptableInOnClause onButton,
                    Config.Def.AcceptableInIfButtonClause ifButton,
                    Action doFunc
                    ) : base(whenFunc, onButton)
                {
                    this.ifButton = ifButton;
                    this.doFunc = doFunc;
                }
                override public bool IsComplete
                {
                    get
                    {
                        return whenFunc != null &&
                               onButton != null &&
                               ifButton != null &&
                               doFunc != null;
                    }
                }
            }

            public class StrokeGestureDefinition : GestureDefinition
            {
                public readonly Def.Stroke stroke;
                public readonly Action doFunc;
                public StrokeGestureDefinition(
                    Config.Def.WhenFunc whenFunc,
                    Config.Def.AcceptableInOnClause onButton,
                    Def.Stroke stroke,
                    Action doFunc
                    ) : base(whenFunc, onButton)
                {
                    this.stroke = stroke;
                    this.doFunc = doFunc;
                }
                override public bool IsComplete
                {
                    get
                    {
                        return whenFunc != null &&
                               onButton != null &&
                               stroke != null &&
                               doFunc != null;
                    }
                }
            }
        }
    }

    namespace Config
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
                public readonly RightButton  RightButtonDown = new RightButton();
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

        public static class DSL
        {
            public class Root
            {
                public readonly List<WhenElement.Value> whenElements = new List<WhenElement.Value>();

                public WhenElement @when(Def.WhenFunc func)
                {
                    return new WhenElement(this, func);
                }
            }

            public class WhenElement
            {
                public class Value
                {
                    public readonly List<OnElement.Value> onElements = new List<OnElement.Value>();
                    public readonly Def.WhenFunc func;

                    public Value(Def.WhenFunc func)
                    {
                        this.func = func;
                    }
                }

                private readonly Root parent;
                private readonly Value value;

                public WhenElement(Root parent, Def.WhenFunc func)
                {
                    this.parent = parent;
                    this.value = new Value(func);
                    this.parent.whenElements.Add(this.value);
                }

                public OnElement @on(Def.AcceptableInOnClause button)
                {
                    return new OnElement(value, button);
                }
            }

            public class OnElement
            {
                public class Value
                {
                    public readonly List<IfButtonElement.Value> ifButtonElements = new List<IfButtonElement.Value>();
                    public readonly List<IfStrokeElement.Value> ifStrokeElements = new List<IfStrokeElement.Value>();
                    public readonly Def.AcceptableInOnClause button;

                    public Value(Def.AcceptableInOnClause button)
                    {
                        this.button = button;
                    }
                }

                private readonly WhenElement.Value parent;
                private readonly Value value;

                public OnElement(WhenElement.Value parent, Def.AcceptableInOnClause button)
                {
                    this.parent = parent;
                    this.value = new Value(button);
                    this.parent.onElements.Add(this.value);
                }

                public IfButtonElement @if(Def.AcceptableInIfButtonClause button)
                {
                    return new IfButtonElement(value, button);
                }

                public IfStrokeElement @if(params Def.AcceptableInIfStrokeClause[] moves)
                {
                    return new IfStrokeElement(value, moves);
                }
            }

            public abstract class IfElement
            {
                public abstract class Value
                {
                    public readonly List<DoElement.Value> doElements = new List<DoElement.Value>();
                }
            }

            public class IfButtonElement
            {
                public class Value : IfElement.Value
                {
                    public readonly Def.AcceptableInIfButtonClause button;

                    public Value(Def.AcceptableInIfButtonClause button)
                    {
                        this.button = button;
                    }
                }

                private readonly OnElement.Value parent;
                private readonly Value value;

                public IfButtonElement(OnElement.Value parent, Def.AcceptableInIfButtonClause button)
                {
                    this.parent = parent;
                    this.value = new Value(button);
                    this.parent.ifButtonElements.Add(this.value);
                }

                public DoElement @do(Def.DoFunc func)
                {
                    return new DoElement(value, func);
                }
            }

            public class IfStrokeElement
            {
                public class Value : IfElement.Value
                {
                    public readonly IEnumerable<Def.AcceptableInIfStrokeClause> moves;

                    public Value(IEnumerable<Def.AcceptableInIfStrokeClause> moves)
                    {
                        this.moves = moves;
                    }
                }

                private readonly OnElement.Value parent;
                private readonly Value value;

                public IfStrokeElement(OnElement.Value parent, params Def.AcceptableInIfStrokeClause[] moves)
                {
                    this.parent = parent;
                    this.value = new Value(moves);
                    this.parent.ifStrokeElements.Add(this.value);
                }

                public DoElement @do(Def.DoFunc func)
                {
                    return new DoElement(value, func);
                }
            }

            public class DoElement
            {
                public class Value
                {
                    public readonly Def.DoFunc func;

                    public Value(Def.DoFunc func)
                    {
                        this.func = func;
                    }
                }

                private readonly IfElement.Value parent;
                private readonly Value value;

                public DoElement(IfElement.Value parent, Def.DoFunc func)
                {
                    this.parent = parent;
                    this.value = new Value(func);
                    this.parent.doElements.Add(this.value);
                }
            }
        }
    }

    public class InputSender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        protected struct INPUT
        {
            public int type;
            public INPUTDATA data;
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct INPUTDATA
        {
            [FieldOffset(0)]
            public MOUSEINPUT asMouseInput;
            [FieldOffset(0)]
            public KEYBDINPUT asKeyboardInput;
            [FieldOffset(0)]
            public HARDWAREINPUT asHardwareInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct WHEELDELTA
        {
            public int delta;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct XBUTTON
        {
            public int type;
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct MOUSEDATA
        {
            [FieldOffset(0)]
            public WHEELDELTA asWheelDelta;
            [FieldOffset(0)]
            public XBUTTON asXButton;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public MOUSEDATA mouseData;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        // https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx
        public static class VirtualKeys
        {
            public const ushort VK_LBUTTON              = 0x01; // Left mouse button
            public const ushort VK_RBUTTON              = 0x02; // Right mouse button
            public const ushort VK_CANCEL               = 0x03; // Control-break processing
            public const ushort VK_MBUTTON              = 0x04; // Middle mouse button (three-button mouse)
            public const ushort VK_XBUTTON1             = 0x05; // X1 mouse button
            public const ushort VK_XBUTTON2             = 0x06; // X2 mouse button
            //                  -                         0x07  // Undefined
            public const ushort VK_BACK                 = 0x08; // BACKSPACE key
            public const ushort VK_TAB                  = 0x09; // TAB key
            //                  -                         0x0A-0B // Reserved
            public const ushort VK_CLEAR                = 0x0C; // CLEAR key
            public const ushort VK_RETURN               = 0x0D; // ENTER key
            //                  -                         0x0E-0F // Undefined
            public const ushort VK_SHIFT                = 0x10; // SHIFT key
            public const ushort VK_CONTROL              = 0x11; // CTRL key
            public const ushort VK_MENU                 = 0x12; // ALT key
            public const ushort VK_PAUSE                = 0x13; // PAUSE key
            public const ushort VK_CAPITAL              = 0x14; // CAPS LOCK key
            public const ushort VK_KANA                 = 0x15; // IME Kana mode
            public const ushort VK_HANGUEL              = 0x15; // IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
            public const ushort VK_HANGUL               = 0x15; // IME Hangul mode
            //                  -                         0x16  // Undefined
            public const ushort VK_JUNJA                = 0x17; // IME Junja mode
            public const ushort VK_FINAL                = 0x18; // IME final mode
            public const ushort VK_HANJA                = 0x19; // IME Hanja mode
            public const ushort VK_KANJI                = 0x19; // IME Kanji mode
            //                  -                         0x1A  // Undefined
            public const ushort VK_ESCAPE               = 0x1B; // ESC key
            public const ushort VK_CONVERT              = 0x1C; // IME convert
            public const ushort VK_NONCONVERT           = 0x1D; // IME nonconvert
            public const ushort VK_ACCEPT               = 0x1E; // IME accept
            public const ushort VK_MODECHANGE           = 0x1F; // IME mode change request
            public const ushort VK_SPACE                = 0x20; // SPACEBAR
            public const ushort VK_PRIOR                = 0x21; // PAGE UP key
            public const ushort VK_NEXT                 = 0x22; // PAGE DOWN key
            public const ushort VK_END                  = 0x23; // END key
            public const ushort VK_HOME                 = 0x24; // HOME key
            public const ushort VK_LEFT                 = 0x25; // LEFT ARROW key
            public const ushort VK_UP                   = 0x26; // UP ARROW key
            public const ushort VK_RIGHT                = 0x27; // RIGHT ARROW key
            public const ushort VK_DOWN                 = 0x28; // DOWN ARROW key
            public const ushort VK_SELECT               = 0x29; // SELECT key
            public const ushort VK_PRINT                = 0x2A; // PRINT key
            public const ushort VK_EXECUTE              = 0x2B; // EXECUTE key
            public const ushort VK_SNAPSHOT             = 0x2C; // PRINT SCREEN key
            public const ushort VK_INSERT               = 0x2D; // INS key
            public const ushort VK_DELETE               = 0x2E; // DEL key
            public const ushort VK_HELP                 = 0x2F; // HELP key
            public const ushort VK_0                    = 0x30; // 0 key
            public const ushort VK_1                    = 0x31; // 1 key
            public const ushort VK_2                    = 0x32; // 2 key
            public const ushort VK_3                    = 0x33; // 3 key
            public const ushort VK_4                    = 0x34; // 4 key
            public const ushort VK_5                    = 0x35; // 5 key
            public const ushort VK_6                    = 0x36; // 6 key
            public const ushort VK_7                    = 0x37; // 7 key
            public const ushort VK_8                    = 0x38; // 8 key
            public const ushort VK_9                    = 0x39; // 9 key
            //                  -                         0x3A-40 // Undefined
            public const ushort VK_A                    = 0x41; // A key
            public const ushort VK_B                    = 0x42; // B key
            public const ushort VK_C                    = 0x43; // C key
            public const ushort VK_D                    = 0x44; // D key
            public const ushort VK_E                    = 0x45; // E key
            public const ushort VK_F                    = 0x46; // F key
            public const ushort VK_G                    = 0x47; // G key
            public const ushort VK_H                    = 0x48; // H key
            public const ushort VK_I                    = 0x49; // I key
            public const ushort VK_J                    = 0x4A; // J key
            public const ushort VK_K                    = 0x4B; // K key
            public const ushort VK_L                    = 0x4C; // L key
            public const ushort VK_M                    = 0x4D; // M key
            public const ushort VK_N                    = 0x4E; // N key
            public const ushort VK_O                    = 0x4F; // O key
            public const ushort VK_P                    = 0x50; // P key
            public const ushort VK_Q                    = 0x51; // Q key
            public const ushort VK_R                    = 0x52; // R key
            public const ushort VK_S                    = 0x53; // S key
            public const ushort VK_T                    = 0x54; // T key
            public const ushort VK_U                    = 0x55; // U key
            public const ushort VK_V                    = 0x56; // V key
            public const ushort VK_W                    = 0x57; // W key
            public const ushort VK_X                    = 0x58; // X key
            public const ushort VK_Y                    = 0x59; // Y key
            public const ushort VK_Z                    = 0x5A; // Z key
            public const ushort VK_LWIN                 = 0x5B; // Left Windows key (Natural keyboard)
            public const ushort VK_RWIN                 = 0x5C; // Right Windows key (Natural keyboard)
            public const ushort VK_APPS                 = 0x5D; // Applications key (Natural keyboard)
            //                  -                         0x5E  // Reserved
            public const ushort VK_SLEEP                = 0x5F; // Computer Sleep key
            public const ushort VK_NUMPAD0              = 0x60; // Numeric keypad 0 key
            public const ushort VK_NUMPAD1              = 0x61; // Numeric keypad 1 key
            public const ushort VK_NUMPAD2              = 0x62; // Numeric keypad 2 key
            public const ushort VK_NUMPAD3              = 0x63; // Numeric keypad 3 key
            public const ushort VK_NUMPAD4              = 0x64; // Numeric keypad 4 key
            public const ushort VK_NUMPAD5              = 0x65; // Numeric keypad 5 key
            public const ushort VK_NUMPAD6              = 0x66; // Numeric keypad 6 key
            public const ushort VK_NUMPAD7              = 0x67; // Numeric keypad 7 key
            public const ushort VK_NUMPAD8              = 0x68; // Numeric keypad 8 key
            public const ushort VK_NUMPAD9              = 0x69; // Numeric keypad 9 key
            public const ushort VK_MULTIPLY             = 0x6A; // Multiply key
            public const ushort VK_ADD                  = 0x6B; // Add key
            public const ushort VK_SEPARATOR            = 0x6C; // Separator key
            public const ushort VK_SUBTRACT             = 0x6D; // Subtract key
            public const ushort VK_DECIMAL              = 0x6E; // Decimal key
            public const ushort VK_DIVIDE               = 0x6F; // Divide key
            public const ushort VK_F1                   = 0x70; // F1 key
            public const ushort VK_F2                   = 0x71; // F2 key
            public const ushort VK_F3                   = 0x72; // F3 key
            public const ushort VK_F4                   = 0x73; // F4 key
            public const ushort VK_F5                   = 0x74; // F5 key
            public const ushort VK_F6                   = 0x75; // F6 key
            public const ushort VK_F7                   = 0x76; // F7 key
            public const ushort VK_F8                   = 0x77; // F8 key
            public const ushort VK_F9                   = 0x78; // F9 key
            public const ushort VK_F10                  = 0x79; // F10 key
            public const ushort VK_F11                  = 0x7A; // F11 key
            public const ushort VK_F12                  = 0x7B; // F12 key
            public const ushort VK_F13                  = 0x7C; // F13 key
            public const ushort VK_F14                  = 0x7D; // F14 key
            public const ushort VK_F15                  = 0x7E; // F15 key
            public const ushort VK_F16                  = 0x7F; // F16 key
            public const ushort VK_F17                  = 0x80; // F17 key
            public const ushort VK_F18                  = 0x81; // F18 key
            public const ushort VK_F19                  = 0x82; // F19 key
            public const ushort VK_F20                  = 0x83; // F20 key
            public const ushort VK_F21                  = 0x84; // F21 key
            public const ushort VK_F22                  = 0x85; // F22 key
            public const ushort VK_F23                  = 0x86; // F23 key
            public const ushort VK_F24                  = 0x87; // F24 key
            //                  -                         0x88-8F // Unassigned
            public const ushort VK_NUMLOCK              = 0x90; // NUM LOCK key
            public const ushort VK_SCROLL               = 0x91; // SCROLL LOCK key
            //                                            0x92-96 // OEM specific
            //                  -                         0x97-9F // Unassigned
            public const ushort VK_LSHIFT               = 0xA0; // Left SHIFT key
            public const ushort VK_RSHIFT               = 0xA1; // Right SHIFT key
            public const ushort VK_LCONTROL             = 0xA2; // Left CONTROL key
            public const ushort VK_RCONTROL             = 0xA3; // Right CONTROL key
            public const ushort VK_LMENU                = 0xA4; // Left MENU key
            public const ushort VK_RMENU                = 0xA5; // Right MENU key
            public const ushort VK_BROWSER_BACK         = 0xA6; // Browser Back key
            public const ushort VK_BROWSER_FORWARD      = 0xA7; // Browser Forward key
            public const ushort VK_BROWSER_REFRESH      = 0xA8; // Browser Refresh key
            public const ushort VK_BROWSER_STOP         = 0xA9; // Browser Stop key
            public const ushort VK_BROWSER_SEARCH       = 0xAA; // Browser Search key
            public const ushort VK_BROWSER_FAVORITES    = 0xAB; // Browser Favorites key
            public const ushort VK_BROWSER_HOME         = 0xAC; // Browser Start and Home key
            public const ushort VK_VOLUME_MUTE          = 0xAD; // Volume Mute key
            public const ushort VK_VOLUME_DOWN          = 0xAE; // Volume Down key
            public const ushort VK_VOLUME_UP            = 0xAF; // Volume Up key
            public const ushort VK_MEDIA_NEXT_TRACK     = 0xB0; // Next Track key
            public const ushort VK_MEDIA_PREV_TRACK     = 0xB1; // Previous Track key
            public const ushort VK_MEDIA_STOP           = 0xB2; // Stop Media key
            public const ushort VK_MEDIA_PLAY_PAUSE     = 0xB3; // Play/Pause Media key
            public const ushort VK_LAUNCH_MAIL          = 0xB4; // Start Mail key
            public const ushort VK_LAUNCH_MEDIA_SELECT  = 0xB5; // Select Media key
            public const ushort VK_LAUNCH_APP1          = 0xB6; // Start Application 1 key
            public const ushort VK_LAUNCH_APP2          = 0xB7; // Start Application 2 key
            //                  -                         0xB8-B9 // Reserved
            public const ushort VK_OEM_1                = 0xBA; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the ';:' key = VK_OEM_PLUS, // 0xBB
                                                                // For any country/region, the '+' key
            public const ushort VK_OEM_COMMA            = 0xBC; // For any country/region, the ',' key
            public const ushort VK_OEM_MINUS            = 0xBD; // For any country/region, the '-' key
            public const ushort VK_OEM_PERIOD           = 0xBE; // For any country/region, the '.' key
            public const ushort VK_OEM_2                = 0xBF; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the '/?' key
            public const ushort VK_OEM_3                = 0xC0; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the '`~' key
            //                  -                         0xC1-D7 // Reserved
            //                  -                         0xD8-DA // Unassigned
            public const ushort VK_OEM_4                = 0xDB; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the '[{' key
            public const ushort VK_OEM_5                = 0xDC; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the '\|' key
            public const ushort VK_OEM_6                = 0xDD; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the ']}' key
            public const ushort VK_OEM_7                = 0xDE; // Used for miscellaneous characters; it can vary by keyboard.
                                                                // For the US standard keyboard, the 'single-quote/double-quote' key
            public const ushort VK_OEM_8                = 0xDF; // Used for miscellaneous characters; it can vary by keyboard.
            //                  -                         0xE0 // Reserved
            //                                            0xE1 // OEM specific
            public const ushort VK_OEM_102              = 0xE2; // Either the angle bracket key or the backslash key on the RT 102-key keyboard
            //                                            0xE3-E4 // OEM specific
            public const ushort VK_PROCESSKEY           = 0xE5; // IME PROCESS key
            //                                            0xE6 // OEM specific
            public const ushort VK_PACKET               = 0xE7; // Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
            //                  -                         0xE8 // Unassigned
            //                                            0xE9-F5 // OEM specific
            public const ushort VK_ATTN                 = 0xF6; // Attn key
            public const ushort VK_CRSEL                = 0xF7; // CrSel key
            public const ushort VK_EXSEL                = 0xF8; // ExSel key
            public const ushort VK_EREOF                = 0xF9; // Erase EOF key
            public const ushort VK_PLAY                 = 0xFA; // Play key
            public const ushort VK_ZOOM                 = 0xFB; // Zoom key
            public const ushort VK_NONAME               = 0xFC; // Reserved
            public const ushort VK_PA1                  = 0xFD; // PA1 key
            public const ushort VK_OEM_CLEAR            = 0xFE; // 
        }

        private const int WHEEL_DELTA = 120;

        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;

        private const int INPUT_MOUSE    = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        // https://msdn.microsoft.com/ja-jp/library/windows/desktop/ms646260(v=vs.85).aspx
        private const int MOUSEEVENTF_MOVE       = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN   = 0x0002;
        private const int MOUSEEVENTF_LEFTUP     = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN  = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP    = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP   = 0x0040;
        private const int MOUSEEVENTF_XDOWN      = 0x0080;
        private const int MOUSEEVENTF_XUP        = 0x0100;
        private const int MOUSEEVENTF_WHEEL      = 0x0800;
        private const int MOUSEEVENTF_HWHEEL     = 0x1000;
        private const int MOUSEEVENTF_ABSOLUTE   = 0x8000;
        
        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP       = 0x0002;
        private const int KEYEVENTF_UNICODE     = 0x0004;
        private const int KEYEVENTF_SCANCODE    = 0x0008;
        


        private readonly UIntPtr MOUSEEVENTF_CREVICE_APP = new UIntPtr(0xFFFFFF00);

        protected void Send(INPUT[] input)
        {
            Debug.Print("calling a native method SendInput");
            foreach (var x in input)
            {
                switch (x.type)
                {
                    case INPUT_MOUSE:
                        Debug.Print("dx: {0}", x.data.asMouseInput.dx);
                        Debug.Print("dy: {0}", x.data.asMouseInput.dy);
                        Debug.Print("asWheelDelta.delta: {0}", x.data.asMouseInput.mouseData.asWheelDelta.delta);
                        Debug.Print("asXButton.type: {0}", x.data.asMouseInput.mouseData.asXButton.type);
                        Debug.Print("dwFlags: {0}",BitConverter.ToString(BitConverter.GetBytes(x.data.asMouseInput.dwFlags)));
                        Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asMouseInput.dwExtraInfo.ToUInt64())));
                        break;
                    case INPUT_KEYBOARD:
                        Debug.Print("wVk: {0}", x.data.asKeyboardInput.wVk);
                        Debug.Print("wScan: {0}", x.data.asKeyboardInput.wScan);
                        Debug.Print("dwFlags: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asKeyboardInput.dwFlags)));
                        Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asKeyboardInput.dwExtraInfo.ToUInt64())));
                        break;
                    case INPUT_HARDWARE:
                        break;
                }
            }

            if (SendInput((uint)input.Length, input, Marshal.SizeOf(input[0])) > 0)
            {
                Debug.Print("success");
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.Print("SendInput failed; ErrorCode: {0}", errorCode);
            }
        }

        protected INPUT ToInput(MOUSEINPUT mouseInput)
        {
            var input = new INPUT();
            input.type = INPUT_MOUSE;
            input.data.asMouseInput = mouseInput;
            return input;
        }

        protected INPUT ToInput(KEYBDINPUT keyboardInput)
        {
            var input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.data.asKeyboardInput = keyboardInput;
            return input;
        }

        private MOUSEINPUT GetCreviceMouseInput()
        {
            var mouseInput = new MOUSEINPUT();
            // Set the CreviceApp signature to the mouse event
            mouseInput.dwExtraInfo = MOUSEEVENTF_CREVICE_APP;
            mouseInput.time = 0;
            return mouseInput;
        }
        
        protected MOUSEINPUT MouseLeftDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_LEFTDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseLeftUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_LEFTUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseRightDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_RIGHTDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseRightUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_RIGHTUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMoveEvent(int dx, int dy)
        {
            var x = Cursor.Position.X + dx;
            var y = Cursor.Position.Y + dy;
            return MouseMoveToEvent(x, y);
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms646273(v=vs.85).aspx
        protected MOUSEINPUT MouseMoveToEvent(int x, int y)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dx = (x + 1) * 0xFFFF / Screen.PrimaryScreen.Bounds.Width;
            mouseInput.dy = (y + 1) * 0xFFFF / Screen.PrimaryScreen.Bounds.Height;
            mouseInput.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMiddleDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMiddleUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_MIDDLEUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseWheelEvent(int delta)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_WHEEL;
            mouseInput.mouseData.asWheelDelta.delta = delta;
            return mouseInput;
        }

        protected MOUSEINPUT MouseWheelDownEvent()
        {
            return MouseWheelEvent(-120);
        }

        protected MOUSEINPUT MouseWheelUpEvent()
        {
            return MouseWheelEvent(120);
        }

        private MOUSEINPUT MouseXUpEvent(int type)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_XUP;
            mouseInput.mouseData.asXButton.type = type;
            return mouseInput;
        }

        protected MOUSEINPUT MouseX1UpEvent()
        {
            return MouseXUpEvent(XBUTTON1);
        }

        protected MOUSEINPUT MouseX2UpEvent()
        {
            return MouseXUpEvent(XBUTTON2);
        }

        private MOUSEINPUT MouseXDownEvent(int type)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_XDOWN;
            mouseInput.mouseData.asXButton.type = type;
            return mouseInput;
        }

        protected MOUSEINPUT MouseX1DownEvent()
        {
            return MouseXDownEvent(XBUTTON1);
        }

        protected MOUSEINPUT MouseX2DownEvent()
        {
            return MouseXDownEvent(XBUTTON2);
        }

        private KEYBDINPUT KeyEvent(ushort keyCode)
        {
            var keyboardInput = new KEYBDINPUT();
            keyboardInput.wVk = keyCode;
            return keyboardInput;
        }

        private KEYBDINPUT ExtendedKeyEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.dwFlags = KEYEVENTF_EXTENDEDKEY;
            return keyboardInput;
        }

        private KEYBDINPUT KeyWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.wScan = (ushort)MapVirtualKey(keyCode, 0);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_SCANCODE;
            return keyboardInput;
        }

        private KEYBDINPUT ExtendedKeyWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyEvent(keyCode);
            keyboardInput.wScan = (ushort)MapVirtualKey(keyCode, 0);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_SCANCODE;
            return keyboardInput;
        }

        private KEYBDINPUT UnicodeKeyEvent(char c)
        {
            var keyboardInput = new KEYBDINPUT();
            keyboardInput.wScan = c;
            keyboardInput.dwFlags = KEYEVENTF_UNICODE;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyUpEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.dwFlags = KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT ExtendedKeyUpEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyUpWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = KeyWithScanCodeEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;

        }

        protected KEYBDINPUT ExtendedKeyUpWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyWithScanCodeEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;

        }

        protected KEYBDINPUT UnicodeKeyUpEvent(char c)
        {
            var keyboardInput = UnicodeKeyEvent(c);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyDownEvent(ushort keyCode)
        {
            return KeyEvent(keyCode);
        }

        protected KEYBDINPUT ExtendedKeyDownEvent(ushort keyCode)
        {
            return ExtendedKeyEvent(keyCode);
        }

        protected KEYBDINPUT KeyDownWithScanCodeEvent(ushort keyCode)
        {
            return KeyWithScanCodeEvent(keyCode);
        }

        protected KEYBDINPUT ExtendedKeyDownWithScanCodeEvent(ushort keyCode)
        {
            return ExtendedKeyWithScanCodeEvent(keyCode);
        }

        protected KEYBDINPUT UnicodeKeyDownEvent(char c)
        {
            return UnicodeKeyEvent(c);
        }
    }

    public class SingleInputSender : InputSender
    {
        protected void Send(params MOUSEINPUT[] mouseInput)
        {
            var input = new INPUT[mouseInput.Length];
            for (var i = 0; i < mouseInput.Length; i++)
            {
                input[i] = ToInput(mouseInput[i]);
            }
            Send(input);
        }

        protected void Send(params KEYBDINPUT[] keyboardInput)
        {
            var input = new INPUT[keyboardInput.Length];
            for (var i = 0; i < keyboardInput.Length; i++)
            {
                input[i] = ToInput(keyboardInput[i]);
            }
            Send(input);
        }

        public void LeftDown()
        {
            Send(MouseLeftDownEvent());
        }

        public void LeftUp()
        {
            Send(MouseLeftUpEvent());
        }

        public void LeftClick()
        {
            Send(MouseLeftDownEvent(), MouseLeftUpEvent());
        }

        public void RightDown()
        {
            Send(MouseRightDownEvent());
        }

        public void RightUp()
        {
            Send(MouseRightUpEvent());
        }

        public void RightClick()
        {
            Send(MouseRightDownEvent(), MouseRightUpEvent());
        }

        public void Move(int dx, int dy)
        {
            Send(MouseMoveEvent(dx, dy));
        }

        public void MoveTo(int x, int y)
        {
            Send(MouseMoveToEvent(x, y));
        }

        public void MiddleDown()
        {
            Send(MouseMiddleDownEvent());
        }

        public void MiddleUp()
        {
            Send(MouseMiddleUpEvent());
        }

        public void MiddleClick()
        {
            Send(MouseMiddleDownEvent(), MouseMiddleUpEvent());
        }

        public void Wheel(short delta)
        {
            Send(MouseWheelEvent(delta));
        }

        public void WheelDown()
        {
            Send(MouseWheelEvent(120));
        }

        public void WheelUp()
        {
            Send(MouseWheelEvent(-120));
        }
        
        public void X1Down()
        {
            Send(MouseX1DownEvent());
        }

        public void X1Up()
        {
            Send(MouseX1UpEvent());
        }
        
        public void X1Click()
        {
            Send(MouseX1DownEvent(), MouseX1UpEvent());
        }

        public void X2Down()
        {
            Send(MouseX2DownEvent());
        }

        public void X2Up()
        {
            Send(MouseX2UpEvent());
        }
        
        public void X2Click()
        {
            Send(MouseX2DownEvent(), MouseX2UpEvent());
        }

        public void KeyDown(ushort keyCode)
        {
            Send(KeyDownEvent(keyCode));
        }

        public void KeyUp(ushort keyCode)
        {
            Send(KeyUpEvent(keyCode));
        }

        public void ExtendedKeyDown(ushort keyCode)
        {
            Send(ExtendedKeyDownEvent(keyCode));
        }

        public void ExtendedKeyUp(ushort keyCode)
        {
            Send(ExtendedKeyUpEvent(keyCode));
        }

        public void KeyDownWithScanCode(ushort keyCode)
        {
            Send(KeyDownWithScanCodeEvent(keyCode));
        }

        public void KeyUpWithScanCode(ushort keyCode)
        {
            Send(KeyUpWithScanCodeEvent(keyCode));
        }

        public void ExtendedKeyDownWithScanCode(ushort keyCode)
        {
            Send(ExtendedKeyDownWithScanCodeEvent(keyCode));
        }

        public void ExtendedKeyUpWithScanCode(ushort keyCode)
        {
            Send(ExtendedKeyUpWithScanCodeEvent(keyCode));
        }

        public void UnicodeKeyDown(char c)
        {
            Send(UnicodeKeyDownEvent(c));
        }

        public void UnicodeKeyUp(char c)
        {
            Send(UnicodeKeyUpEvent(c));
        }
        
        public void UnicodeKeyStroke(string str)
        {
            var list = str
                .Select(c => new List<KEYBDINPUT>() { UnicodeKeyDownEvent(c), UnicodeKeyUpEvent(c) })
                .SelectMany(x => x);
            Send(list.ToArray());
        }
    }

    public class InputSequenceBuilder : InputSender
    {
        private readonly IEnumerable<INPUT> buffer;

        public InputSequenceBuilder() : this(new List<INPUT>())
        {

        }

        private InputSequenceBuilder(IEnumerable<INPUT> xs)
        {
            this.buffer = xs;
        }
        
        private InputSequenceBuilder NewInstance(IEnumerable<INPUT> ys)
        {
            var xs = this.buffer.ToList();
            xs.AddRange(ys);
            return new InputSequenceBuilder(xs);
        }

        private InputSequenceBuilder NewInstance(params MOUSEINPUT[] mouseEvent)
        {
            return NewInstance(mouseEvent.Select(x => ToInput(x)));
        }

        private InputSequenceBuilder NewInstance(params KEYBDINPUT[] keyboardEvent)
        {
            return NewInstance(keyboardEvent.Select(x => ToInput(x)));
        }

        public InputSequenceBuilder LeftDown()
        {
            return NewInstance(MouseLeftDownEvent());
        }

        public InputSequenceBuilder LeftUp()
        {
            return NewInstance(MouseLeftUpEvent());
        }

        public InputSequenceBuilder LeftClick()
        {
            return NewInstance(MouseLeftDownEvent(), MouseLeftUpEvent());
        }
    
        public InputSequenceBuilder RightDown()
        {
            return NewInstance(MouseRightDownEvent());
        }

        public InputSequenceBuilder RightUp()
        {
            return NewInstance(MouseRightUpEvent());
        }

        public InputSequenceBuilder RightClick()
        {
            return NewInstance(MouseRightDownEvent(), MouseRightUpEvent());
        }

        public InputSequenceBuilder Move(int dx, int dy)
        {
            return NewInstance(MouseMoveEvent(dx, dy));
        }

        public InputSequenceBuilder MoveTo(int x, int y)
        {
            return NewInstance(MouseMoveToEvent(x, y));
        }

        public InputSequenceBuilder MiddleDown()
        {
            return NewInstance(MouseMiddleDownEvent());
        }

        public InputSequenceBuilder MiddleUp()
        {
            return NewInstance(MouseMiddleUpEvent());
        }

        public InputSequenceBuilder MiddleClick()
        {
            return NewInstance(MouseMiddleDownEvent(), MouseMiddleUpEvent());
        }

        public InputSequenceBuilder Wheel(short delta)
        {
            return NewInstance(MouseWheelEvent(delta));
        }

        public InputSequenceBuilder WheelDown()
        {
            return NewInstance(MouseWheelEvent(120));
        }

        public InputSequenceBuilder WheelUp()
        {
            return NewInstance(MouseWheelEvent(-120));
        }

        public InputSequenceBuilder X1Down()
        {
            return NewInstance(MouseX1DownEvent());
        }

        public InputSequenceBuilder X1Up()
        {
            return NewInstance(MouseX1UpEvent());
        }

        public InputSequenceBuilder X1Click()
        {
            return NewInstance(MouseX1DownEvent(), MouseX1UpEvent());
        }

        public InputSequenceBuilder X2Down()
        {
            return NewInstance(MouseX2UpEvent());
        }

        public InputSequenceBuilder X2Up()
        {
            return NewInstance(MouseX2UpEvent());
        }

        public InputSequenceBuilder X2Click()
        {
            return NewInstance(MouseX2DownEvent(), MouseX2UpEvent());
        }

        public InputSequenceBuilder KeyDown(ushort keyCode)
        {
            return NewInstance(KeyDownEvent(keyCode));
        }

        public InputSequenceBuilder KeyUp(ushort keyCode)
        {
            return NewInstance(KeyUpEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyDown(ushort keyCode)
        {
            return NewInstance(ExtendedKeyDownEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyUp(ushort keyCode)
        {
            return NewInstance(ExtendedKeyUpEvent(keyCode));
        }

        public InputSequenceBuilder KeyDownWithScanCode(ushort keyCode)
        {
            return NewInstance(KeyDownWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder KeyUpWithScanCode(ushort keyCode)
        {
            return NewInstance(KeyUpWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyDownWithScanCode(ushort keyCode)
        {
            return NewInstance(ExtendedKeyDownWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyUpWithScanCode(ushort keyCode)
        {
            return NewInstance(ExtendedKeyUpWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder UnicodeKeyDown(char c)
        {
            return NewInstance(UnicodeKeyDownEvent(c));
        }

        public InputSequenceBuilder UnicodeKeyUp(char c)
        {
            return NewInstance(UnicodeKeyUpEvent(c));
        }

        public InputSequenceBuilder UnicodeKeyStroke(string str)
        {
            return NewInstance(str
                .Select(c => new List<KEYBDINPUT>() { UnicodeKeyDownEvent(c), UnicodeKeyUpEvent(c) })
                .SelectMany(x => x)
                .ToArray());
        }

        public void Send()
        {
            Send(buffer.ToArray());
        }
    }

    public class WindowsApplication
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

        private const int PROCESS_VM_READ           = 0x0010;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;

        private const int maxPathSize = 1024;
        private readonly StringBuilder buffer = new StringBuilder(maxPathSize);

        public WindowsApplicationInfo GetForeground()
        {
            IntPtr hWindow = GetForegroundWindow();
            return FindFromWindowHandle(hWindow);
        }

        public WindowsApplicationInfo GetOnCursor(int x, int y)
        {
            IntPtr hWindow = WindowFromPoint(x, y);
            return FindFromWindowHandle(hWindow);
        }

        private WindowsApplicationInfo FindFromWindowHandle(IntPtr hWindow)
        {
            int pid = 0;
            int tid = GetWindowThreadProcessId(hWindow, out pid);
            Debug.Print("tid: 0x{0:X}", tid);
            Debug.Print("pid: 0x{0:X}", pid);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            Debug.Print("hProcess: 0x{0:X}", hProcess.ToInt64());
            int lpdwSize = maxPathSize;
            try
            {
                QueryFullProcessImageName(hProcess, 0, buffer, out lpdwSize);
            }
            finally
            {
                CloseHandle(hProcess);
            }
            String path = buffer.ToString();
            String name = path.Substring(path.LastIndexOf("\\")+1);
            return new WindowsApplicationInfo(path, name);
        }
    }

    public class WindowsApplicationInfo
    {
        public readonly String path;
        public readonly String name;
        public WindowsApplicationInfo(String path, String name)
        {
            this.path = path;
            this.name = name;
        }
    }

    public class WindowsHook : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, SystemCallback callback, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        private delegate IntPtr SystemCallback(int nCode, IntPtr wParam, IntPtr lParam);
        protected delegate Result UserCallback(IntPtr wParam, IntPtr lParam);
        
        private const int HC_ACTION = 0;
        
        public enum HookType
        {
            WH_MSGFILTER      = -1,
            WH_JOURNALRECORD   = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD        = 2,
            WH_GETMESSAGE      = 3,
            WH_CALLWNDPROC     = 4,
            WH_CBT             = 5,
            WH_SYSMSGFILTER    = 6,
            WH_MOUSE           = 7,
            WH_DEBUG           = 9,
            WH_SHELL          = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL    = 13,
            WH_MOUSE_LL       = 14
        }
        
        public enum Result
        {
            Transfer,
            Determine,
            Cancel
        };
        
        private static readonly IntPtr LRESULTCancel = new IntPtr(1);
        
        private readonly Object lockObject = new Object();
        private readonly UserCallback userCallback;
        // There is need to bind a callback function to a local variable to protect it from GC.
        private readonly SystemCallback systemCallback;
        private readonly HookType hookType;

        private IntPtr hHook = IntPtr.Zero;
        
        protected WindowsHook(HookType hookType, UserCallback userCallback)
        {
            this.hookType = hookType;
            this.userCallback = userCallback;
            this.systemCallback = Callback;
        }

        public bool IsActivated
        {
            get { return hHook != IntPtr.Zero; }
        }

        public void SetHook()
        {
            lock (lockObject)
            {
                if (IsActivated)
                {
                    throw new InvalidOperationException();
                }

                var hInstance = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                Debug.Print("hInstance: 0x{0:X}", hInstance.ToInt64());
                Debug.Print("calling a native method SetWindowsHookEx(HookType: {0})", Enum.GetName(typeof(HookType), hookType));
                hHook = SetWindowsHookEx((int)hookType, this.systemCallback, hInstance, 0);
                Debug.Print("hHook: 0x{0:X}", hHook.ToInt64());
                if (!IsActivated)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("SetWindowsHookEx(HookType: {0}) failed; ErrorCode: {1}", Enum.GetName(typeof(HookType), hookType), errorCode);
                    throw new Win32Exception(errorCode);
                }
                Debug.Print("success");
            }
        }

        public void Unhook()
        {
            lock (lockObject)
            {
                if (!IsActivated)
                {
                    throw new InvalidOperationException();
                }
                Debug.Print("calling a native method UnhookWindowsHookEx(HookType: {0})", Enum.GetName(typeof(HookType), hookType));
                Debug.Print("hHook: 0x{0:X}", hHook);
                if (!UnhookWindowsHookEx(hHook))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("UnhookWindowsHookEx(HookType: {0}) failed; ErrorCode: {1}", Enum.GetName(typeof(HookType), hookType), errorCode);
                    throw new Win32Exception(errorCode);
                }
                hHook = IntPtr.Zero;
                Debug.Print("success");
            }
        }

        public IntPtr Callback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            lock (lockObject)
            {
                if (nCode >= 0)
                {
                    switch (userCallback(wParam, lParam))
                    {
                        case Result.Transfer:
                            return CallNextHookEx(hHook, nCode, wParam, lParam);
                        case Result.Cancel:
                            return LRESULTCancel;
                        case Result.Determine:
                            return IntPtr.Zero;
                    }
                }
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            lock (lockObject)
            {
                if (IsActivated)
                {
                    Unhook();
                }
            }
        }

        ~WindowsHook()
        {
            Dispose();
        }
    }

    public class LowLevelMouseHook : WindowsHook
    {
        public enum Event
        {
            WM_NCMOUSEMOVE     = 0x00A0,
            WM_NCLBUTTONDOWN   = 0x00A1,
            WM_NCLBUTTONUP     = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN   = 0x00A4,
            WM_NCRBUTTONUP     = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN   = 0x00A7,
            WM_NCMBUTTONUP     = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN   = 0x00AB,
            WM_NCXBUTTONUP     = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_MOUSEMOVE       = 0x0200,
            WM_LBUTTONDOWN     = 0x0201,
            WM_LBUTTONUP       = 0x0202,
            WM_LBUTTONDBLCLK   = 0x0203,
            WM_RBUTTONDOWN     = 0x0204,
            WM_RBUTTONUP       = 0x0205,
            WM_RBUTTONDBLCLK   = 0x0206,
            WM_MBUTTONDOWN     = 0x0207,
            WM_MBUTTONUP       = 0x0208,
            WM_MBUTTONDBLCLK   = 0x0209,
            WM_MOUSEWHEEL      = 0x020A,
            WM_XBUTTONDOWN     = 0x020B,
            WM_XBUTTONUP       = 0x020C,
            WM_XBUTTONDBLCLK   = 0x020D,
            WM_MOUSEHWHEEL     = 0x020E
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WHEELDELTA
        {
            private short lower;
            public short delta;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XBUTTON
        {
            private short lower;
            public short type;

            public bool isXButton1
            {
                get { return type == 0x0001; }
            }

            public bool isXButton2
            {
                get { return type == 0x0002; }
            }
        }
        
        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEDATA
        {
            [FieldOffset(0)]
            public WHEELDELTA asWheelDelta;
            [FieldOffset(0)]
            public XBUTTON asXButton;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public MOUSEDATA mouseData;
            public uint flags;
            public uint time;
            public UIntPtr dwExtraInfo;

            public bool fromCreviceApp
            {
                get { return (uint)dwExtraInfo == MOUSEEVENTF_CREVICE_APP; }
            }

            public bool fromTablet
            {
                get { return ((uint)dwExtraInfo & MOUSEEVENTF_TMASK) == MOUSEEVENTF_FROMTABLET; }
            }
        }
        
        private const uint MOUSEEVENTF_CREVICE_APP = 0xFFFFFF00;
        private const uint MOUSEEVENTF_TMASK       = 0xFFFFFF00;
        private const uint MOUSEEVENTF_FROMTABLET  = 0xFF515700;
        
        public LowLevelMouseHook(Func<Event, MSLLHOOKSTRUCT, Result> userCallback) : 
            base
            (
                HookType.WH_MOUSE_LL,
                (wParam, lParam) =>
                {
                    var a = (Event)wParam;
                    var b = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    return userCallback(a, b);
                }
            )
        {

        }
    }

    public class LowLevelKeyboardHook : WindowsHook
    {
        public enum Event
        {
            WM_KEYDOWN    = 0x0100,
            WM_KEYUP      = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP   = 0x0105
        }

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public FLAGS flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum FLAGS : uint
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN  = 0x20,
            LLKHF_UP       = 0x80,
        }
        
        public LowLevelKeyboardHook(Func<Event, KBDLLHOOKSTRUCT, Result> userCallback) :
            base
            (
                HookType.WH_KEYBOARD_LL,
                (wParam, lParam) =>
                {
                    var a = (Event)wParam;
                    var b = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                    return userCallback(a, b);
                }
            )
        {

        }
    }
}
