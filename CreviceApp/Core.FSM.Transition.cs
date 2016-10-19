using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.FSM
{
    public static class Transition
    {
        #region Transition Definition
        #region State0
        // Transition 0_0 (single action gesture established)
        //
        // State0 -> State0
        //
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.ISingleAction, IEnumerable<IfButtonGestureDefinition>>
            Gen0_0(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 0_1 (gesture with primary double action mouse button start)
        //
        // State0 -> State1
        //
        // Transition from the state(S0) to the state(S1).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonGestureDefinition>>
            Gen0_1(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.onButton))
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 0_2 (forced reset)
        //
        // State0 -> State0
        // 
        // Transition from the state(S0) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition have no side effect.
        #endregion

        #region State1
        // Transition 1_0 (single action gesture established)
        //
        // State1 -> State2
        //
        // Transition from the state(S1) to the state(S2). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.ISingleAction, IEnumerable<OnButtonWithIfButtonGestureDefinition>>
            Gen1_0(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 1_1 (gesture with primary and secondary double action mouse button start)
        //
        // State1 -> State3
        //
        // Transition from the state(S1) to the state(S3).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of OnButtonWithIfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonWithIfButtonGestureDefinition>>
            Gen1_1(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 1_2 (stroke gesture established)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button
        // and a gesture stroke existing in OnButtonWithIfStrokeGestureDefinition are given.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Stroke, IEnumerable<OnButtonWithIfStrokeGestureDefinition>>
            Gen1_2(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonWithIfStrokeGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => x.stroke)
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 1_3 (default gesture established)
        // 
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happens when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<IfButtonGestureDefinition>>
            Gen1_3(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 1_4 (restoration of the `set` event)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has one side effect.
        // 1. The `set` and `release` events of primary double action mouse button will be restored.

        // Transition 1_5 (forced cancel)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `cancel` command given and functions given as the parameter of
        // `@before` and `@after` clauses of IfButtonGestureDefinition do not exist.
        // This transition has one side effect.
        // 1. The `set` and `release` events of primary double action mouse button will be restored.

        // Transition 1_6 (forced reset)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has two side effects.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        #endregion

        #region State2
        // Transition 2_0 (single action gesture established)
        //
        // State2 -> State2
        //
        // Transition from the state(S2) to the state(S2). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 2_1 (gesture with primary and secondary double action mouse button start)
        //
        // State2 -> State3
        //
        // Transition from the state(S2) to the state(S2).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@before` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 2_2 (stroke gesture established)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given
        // and a gesture stroke existing in OnButtonWithIfStrokeGestureDefinition is established.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.

        // Transition 2_3 (default gesture established)
        // 
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happens when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.

        // Transition 2_4 (forced reset)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has two side effects.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        #endregion

        #region State3
        // Transition 3_0 (double action gesture established)
        //
        // State3 -> State2
        //
        // Transition from the state(S3) to the state(S2).
        // This transition happends when `release` event of secondary double action mouse button is given.
        // This transition has two side effects.
        // 1. Functions given as the parameter of `@do` clause of OnButtonWithIfButtonGestureDefinition are executed. 
        // 2. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 3-1 (irregular end)
        //
        // State3 -> State0
        //
        // Transition from the state(S3) to the state(S0).
        // This transition happends when primary double action mouse button is released in irregular order. 
        // This transition has three side effects.
        // 1. Secondary double action mouse button left holding will be marked as irreggularly holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        // 3. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.

        // Transition 3-2 (forced reset)
        //
        // State3 -> State0
        //
        // Transition from the state(S3) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition has three side effects.
        // 1. Primary and secondly double action mouse buttons left holding is marked as irregularly 
        // holding by the user.
        // 2. Functions given as the parameter of `@after` clause of IfButtonGestureDefinition are executed.
        // 3. Functions given as the parameter of `@after` clause of OnButtonWithIfButtonGestureDefinition are executed.
        #endregion

        #region Special side effects
        // Special side effects
        //
        // 1. Transition any state to the state S1 and S2 will reset the gesture stroke.
        // 2. Input given to the state S1 and S2 is intepreted as a gesture stroke.
        // 3. Each state will remove the mark of irregularly holding from double action mouse button when 
        //    `set` event of it to be given. 
        // 4. Each state will remove the mark of irregularly holding from double action mouse button and ignore it when 
        //    `release` event of it to be given.
        #endregion
        #endregion
    }
}
