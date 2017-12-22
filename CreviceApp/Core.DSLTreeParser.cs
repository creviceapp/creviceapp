using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core
{
    public static class DSLTreeParser
    {
        public static IEnumerable<GestureDefinition> TreeToGestureDefinition(DSL.Root root)
        {
            var stopwatch = new Stopwatch();
            Verbose.Print("Parsing GestureConfigTree...");
            stopwatch.Start();
            var gestureDef = Parse(root).ToList();
            stopwatch.Stop();
            Verbose.Print("GestureConfigTree parsing finished. ({0})", stopwatch.Elapsed);
            return gestureDef; 
        }

        internal static IEnumerable<GestureDefinition> Parse(
            DSL.Root root)
        {
            foreach (var elm in root.whenElements)
            {
                foreach (var def in Parse(elm)) { yield return def; }
            }
        }

        internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement)
        {
            if (whenElement.onElements.Count == 0 &&
                whenElement.ifSingleTriggerButtonElements.Count == 0 &&
                whenElement.ifDoubleTriggerButtonElements.Count == 0)
            {
                yield return new GestureDefinition(whenElement.func);
            }
            else
            {
                foreach (var elm in whenElement.onElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
                foreach (var elm in whenElement.ifSingleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
                foreach (var elm in whenElement.ifDoubleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, elm)) { yield return def; }
                }
            }
        }

        internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement,
            DSL.IfSingleTriggerButtonElement.Value ifSingleTriggerButtonElement)
        {
            if (ifSingleTriggerButtonElement.doElements.Count == 0)
            {
                yield return new IfButtonGestureDefinition(whenElement.func, ifSingleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var doElement in ifSingleTriggerButtonElement.doElements)
                {
                    yield return new IfButtonGestureDefinition(whenElement.func, ifSingleTriggerButtonElement.button, null, doElement.func, null);
                }
            }
        }

       internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement,
            DSL.IfDoubleTriggerButtonElement.Value ifDoubleTriggerButtonElement)
        {
            if (ifDoubleTriggerButtonElement.beforeElements.Count == 0 &&
                ifDoubleTriggerButtonElement.doElements.Count == 0 &&
                ifDoubleTriggerButtonElement.afterElements.Count == 0
                )
            {
                yield return new IfButtonGestureDefinition(whenElement.func, ifDoubleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var elm in ifDoubleTriggerButtonElement.beforeElements)
                {
                    yield return new IfButtonGestureDefinition(whenElement.func, ifDoubleTriggerButtonElement.button, elm.func, null, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.doElements)
                {
                    yield return new IfButtonGestureDefinition(whenElement.func, ifDoubleTriggerButtonElement.button, null, elm.func, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.afterElements)
                {
                    yield return new IfButtonGestureDefinition(whenElement.func, ifDoubleTriggerButtonElement.button, null, null, elm.func);
                }
            }
        }

        internal static IEnumerable<GestureDefinition> Parse(
             DSL.WhenElement.Value whenElement,
             DSL.OnElement.Value onElement)
        {
            if (onElement.ifSingleTriggerButtonElements.Count == 0 &&
                onElement.ifDoubleTriggerButtonElements.Count == 0 &&
                onElement.ifStrokeElements.Count == 0)
            {
                yield return new OnButtonGestureDefinition(whenElement.func, onElement.button);
            }
            else
            {
                foreach (var elm in onElement.ifSingleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
                foreach (var elm in onElement.ifDoubleTriggerButtonElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
                foreach (var elm in onElement.ifStrokeElements)
                {
                    foreach (var def in Parse(whenElement, onElement, elm)) { yield return def; }
                }
            }
        }

        internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement,
            DSL.OnElement.Value onElement,
            DSL.IfSingleTriggerButtonElement.Value ifSingleTriggerButtonElement)
        {
            if (ifSingleTriggerButtonElement.doElements.Count == 0)
            {
                yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifSingleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var doElement in ifSingleTriggerButtonElement.doElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifSingleTriggerButtonElement.button, null, doElement.func, null);
                }
            }
        }

       internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement,
            DSL.OnElement.Value onElement,
            DSL.IfDoubleTriggerButtonElement.Value ifDoubleTriggerButtonElement)
        {
            if (ifDoubleTriggerButtonElement.beforeElements.Count == 0 &&
                ifDoubleTriggerButtonElement.doElements.Count == 0 &&
                ifDoubleTriggerButtonElement.afterElements.Count == 0
                )
            {
                yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, null, null);
            }
            else
            {
                foreach (var elm in ifDoubleTriggerButtonElement.beforeElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, elm.func, null, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.doElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, elm.func, null);
                }
                foreach (var elm in ifDoubleTriggerButtonElement.afterElements)
                {
                    yield return new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifDoubleTriggerButtonElement.button, null, null, elm.func);
                }
            }
        }

        internal static IEnumerable<GestureDefinition> Parse(
            DSL.WhenElement.Value whenElement,
            DSL.OnElement.Value onElement,
            DSL.IfStrokeElement.Value ifStrokeElement)
        {
            var stroke = Helper.Convert(ifStrokeElement.moves);
            if (ifStrokeElement.doElements.Count == 0)
            {
                yield return new OnButtonWithIfStrokeGestureDefinition(whenElement.func, onElement.button, stroke, null);
            }
            else
            {
                foreach (var doElement in ifStrokeElement.doElements)
                {
                    yield return new OnButtonWithIfStrokeGestureDefinition(whenElement.func, onElement.button, stroke, doElement.func);
                }
            }
        }

        public static IEnumerable<GestureDefinition> FilterComplete(IEnumerable<GestureDefinition> gestureDef)
        {
            return gestureDef
                .Where(x => x.IsComplete)
                .ToList();
        }
    }
}
