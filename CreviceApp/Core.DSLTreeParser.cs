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
            List<GestureDefinition> gestureDef = new List<GestureDefinition>();
            Debug.Print("Parsing tree of GestureConfig.DSL");
            foreach (var whenElement in root.whenElements)
            {
                if (whenElement.onElements.Count == 0 && 
                    whenElement.ifSingleTriggerButtonElements.Count == 0 &&
                    whenElement.ifDoubleTriggerButtonElements.Count == 0)
                {
                    gestureDef.Add(new GestureDefinition(whenElement.func));
                    continue;
                }

                foreach (var ifButtonElement in whenElement.ifSingleTriggerButtonElements)
                {
                    if (ifButtonElement.doElements.Count == 0)
                    {
                        gestureDef.Add(new IfButtonGestureDefinition(whenElement.func, ifButtonElement.button, null, null, null));
                        continue;
                    }
                    foreach (var doElement in ifButtonElement.doElements)
                    {
                        gestureDef.Add(new IfButtonGestureDefinition(whenElement.func, ifButtonElement.button, null, doElement.func, null));
                    }
                }

                foreach (var onElement in whenElement.onElements)
                {
                    if (onElement.ifSingleTriggerButtonElements.Count == 0 &&
                        onElement.ifDoubleTriggerButtonElements.Count == 0 &&
                        onElement.ifStrokeElements.Count == 0)
                    {
                        gestureDef.Add(new OnButtonGestureDefinition(whenElement.func, onElement.button));
                        continue;
                    }
                    foreach (var ifButtonElement in onElement.ifSingleTriggerButtonElements)
                    {
                        if (ifButtonElement.doElements.Count == 0)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null, null, null));
                            continue;
                        }
                        foreach (var doElement in ifButtonElement.doElements)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null, doElement.func, null));
                        }
                    }
                    foreach (var ifButtonElement in onElement.ifDoubleTriggerButtonElements)
                    {
                        if (ifButtonElement.beforeElements.Count == 0 &&
                            ifButtonElement.doElements.Count == 0 &&
                            ifButtonElement.afterElements.Count == 0)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null, null, null));
                            continue;
                        }
                        foreach (var prepareElement in ifButtonElement.beforeElements)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, prepareElement.func, null, null));
                        }
                        foreach (var doElement in ifButtonElement.doElements)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null, doElement.func, null));
                        }
                        foreach (var afterElement in ifButtonElement.afterElements)
                        {
                            gestureDef.Add(new OnButtonWithIfButtonGestureDefinition(whenElement.func, onElement.button, ifButtonElement.button, null, null, afterElement.func));
                        }
                    }
                    foreach (var ifStrokeElement in onElement.ifStrokeElements)
                    {
                        var stroke = Helper.Convert(ifStrokeElement.moves);
                        if (ifStrokeElement.doElements.Count == 0)
                        {
                            gestureDef.Add(new OnButtonWithIfStrokeGestureDefinition(whenElement.func, onElement.button, stroke, null));
                            continue;
                        }
                        foreach (var doElement in ifStrokeElement.doElements)
                        {
                            gestureDef.Add(new OnButtonWithIfStrokeGestureDefinition(whenElement.func, onElement.button, stroke, doElement.func));
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
}
