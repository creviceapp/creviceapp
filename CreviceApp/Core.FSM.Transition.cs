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
        // Transition 00 (single action gesture established)
        //
        // State0 -> State0
        //
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.ISingleAction, IEnumerable<IfButtonGestureDefinition>>
            Gen0(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Def.Event.ISingleAction, x => x.Select(y => y));
        }


        // Transition 01 (gesture with primary double action mouse button start)
        //
        // State0 -> State1
        //
        // Transition from the state(S0) to the state(S1).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has no side effect.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonGestureDefinition>>
            Gen1(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.onButton))
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 02 (single action gesture established)
        //
        // State1 -> State1
        //
        // Transition from the state(S1) to the state(S1). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonIfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.ISingleAction, IEnumerable<OnButtonIfButtonGestureDefinition>>
            Gen2(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonIfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 03 (gesture with primary and secondary double action mouse button start)
        //
        // State1 -> State2
        //
        // Transition from the state(S1) to the state(S2).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has no side effect.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<OnButtonIfButtonGestureDefinition>>
            Gen3(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonIfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 04 (stroke gesture established)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button
        // and a gesture stroke existing in OnButtonIfStrokeGestureDefinition are given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        public static IDictionary<Def.Stroke, IEnumerable<OnButtonIfStrokeGestureDefinition>>
            Gen4(IEnumerable<OnButtonGestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as OnButtonIfStrokeGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => x.stroke)
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 05 (default gesture established)
        // 
        // State 1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happens when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // 1. Functions given as the parameter of `@do` clause of IfButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<IfButtonGestureDefinition>>
            Gen5(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as IfButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 06 (gesture canceled)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given and 
        // there have not been any actions executed.
        // This transition has one side effect.
        // 1. Primary double action mouse button will be restored.


        // Transition 07 (gesture end)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given.
        // This transition has no side effect.

        // Transition 08 (double action gesture established)
        //
        // State2 -> State1
        //
        // Transition from the state(S2) to the state(S1).
        // This transition happends when `release` event of secondary double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of OnButtonIfButtonGestureDefinition are executed. 

        // Transition 09 (irregular end)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happends when primary double action mouse button is released in irregular order. 
        // This transition has one side effect.
        // 1. Secondary double action mouse button left holding will be marked as irreggularly holding by the user.

        // Transition 10 (forced reset)
        //
        // State0 -> State0
        // 
        // Transition from the state(S0) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition have no side effect.

        // Transition 11 (forced reset)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has one side effect.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user,

        // Transition 12 (forced reset)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition has one side effects.
        // 1. Primary and secondly double action mouse buttons left holding is marked as irregularly 
        // holding by the user.

        // Special side effects
        //
        // 1. Transition any state to the State(S1) will reset the gesture stroke.
        // 2. Input given to the State(S1) is intepreted as a gesture stroke.
        // 3. Each state will remove the mark of irregularly holding from double action mouse button when 
        //    `set` event of it is given.
        // 4. Each state will remove the mark of irregularly holding from double action mouse button and ignore it when 
        //    `release` event of it is given.
        #endregion
    }
}
