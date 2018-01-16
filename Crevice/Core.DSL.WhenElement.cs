using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.DSL
{
    public class WhenElement
    {
        public class Value
        {
            public readonly List<IfSingleTriggerButtonElement.Value> ifSingleTriggerButtonElements = new List<IfSingleTriggerButtonElement.Value>();
            public readonly List<IfDoubleTriggerButtonElement.Value> ifDoubleTriggerButtonElements = new List<IfDoubleTriggerButtonElement.Value>();
            public readonly List<OnElement.Value> onElements = new List<OnElement.Value>();
            public readonly Def.WhenFunc func;

            public Value(Def.WhenFunc func)
            {
                this.func = func;
            }
        }
        
        private readonly Value value;

        public WhenElement(List<Value> parent, Def.WhenFunc func)
        {
            this.value = new Value(func);
            parent.Add(this.value);
        }

        public OnElement @on(Def.AcceptableInOnClause button)
        {
            return new OnElement(value.onElements, button);
        }

        public IfSingleTriggerButtonElement @if(Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            return new IfSingleTriggerButtonElement(value.ifSingleTriggerButtonElements, button);
        }

        public IfDoubleTriggerButtonElement @if(Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            return new IfDoubleTriggerButtonElement(value.ifDoubleTriggerButtonElements, button);
        }
    }
}
