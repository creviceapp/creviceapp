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
        // Transition 0 (gesture start)
        //
        // State0 -> State1
        //
        // Transition from the state(S0) to the state(S1).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has no side effect.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<GestureDefinition>>
            Gen0(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .ToLookup(x => Helper.Convert(x.onButton))
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 01 (double action gesture start)
        //
        // State1 -> State2
        //
        // Transition from the state(S1) to the state(S2).
        // This transition happends when `set` event of double action mouse button is given.
        // This transition has no side effect.
        public static IDictionary<Def.Event.IDoubleActionSet, IEnumerable<ButtonGestureDefinition>>
            Gen1(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as ButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.IDoubleActionSet)
                .ToDictionary(x => x.Key as Def.Event.IDoubleActionSet, x => x.Select(y => y));
        }

        // Transition 02 (single action gesture)
        //
        // State1 -> State1
        //
        // Transition from the state(S1) to the state(S1). 
        // This transition happends when `fire` event of single action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of ButtonGestureDefinition are executed.
        public static IDictionary<Def.Event.ISingleAction, IEnumerable<ButtonGestureDefinition>>
            Gen2(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as ButtonGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => Helper.Convert(x.ifButton))
                .Where(x => x.Key is Def.Event.ISingleAction)
                .ToDictionary(x => x.Key as Def.Event.ISingleAction, x => x.Select(y => y));
        }

        // Transition 03 (stroke gesture)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button
        // and a gesture stroke existing in StrokeGestureDefinition are given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of StrokeGestureDefinition are executed.
        public static IDictionary<Def.Stroke, IEnumerable<StrokeGestureDefinition>>
            Gen3(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Select(x => x as StrokeGestureDefinition)
                .Where(x => x != null)
                .ToLookup(x => x.stroke)
                .ToDictionary(x => x.Key, x => x.Select(y => y));
        }

        // Transition 04 (cancel gesture)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given.
        // This transition has one side effect.
        // 1. Primary double action mouse button will be restored.


        // Transition 05 (gesture end)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0).
        // This transition happends when `release` event of primary double action mouse button is given.
        // This transition has no side effect.

        // Transition 06 (double action gesture end)
        //
        // State2 -> State1
        //
        // Transition from the state(S2) to the state(S1).
        // This transition happends when `release` event of secondary double action mouse button is given.
        // This transition has one side effect.
        // 1. Functions given as the parameter of `@do` clause of ButtonGestureDefinition are executed. 

        // Transition 07 (irregular end)
        //
        // State2 -> State0
        //
        // Transition from the state(S2) to the state(S0).
        // This transition happends when primary double action mouse button is released in irregular order. 
        // This transition has one side effect.
        // 1. Secondary double action mouse button left holding will be marked as irreggularly holding by the user.

        // Transition 08 (forced reset)
        //
        // State0 -> State0
        // 
        // Transition from the state(S0) to the state(S0).
        // This event happens when a `reset` command given.
        // This transition have no side effect.

        // Transition 09 (forced reset)
        //
        // State1 -> State0
        //
        // Transition from the state(S1) to the state(S0). 
        // This event happens when a `reset` command given.
        // This transition has one side effect.
        // 1. Primary double action mouse button left holding is marked as irregularly holding by the user,

        // Transition 10 (forced reset)
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
