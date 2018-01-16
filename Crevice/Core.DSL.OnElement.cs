using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.DSL
{
    public class OnElement
    {
        public class Value
        {
            public readonly List<IfSingleTriggerButtonElement.Value> ifSingleTriggerButtonElements = new List<IfSingleTriggerButtonElement.Value>();
            public readonly List<IfDoubleTriggerButtonElement.Value> ifDoubleTriggerButtonElements = new List<IfDoubleTriggerButtonElement.Value>();
            public readonly List<IfStrokeElement.Value> ifStrokeElements = new List<IfStrokeElement.Value>();
            public readonly Def.AcceptableInOnClause button;

            public Value(Def.AcceptableInOnClause button)
            {
                this.button = button;
            }
        }
        
        private readonly Value value;

        public OnElement(List<Value> parent, Def.AcceptableInOnClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public IfSingleTriggerButtonElement @if(Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            return new IfSingleTriggerButtonElement(value.ifSingleTriggerButtonElements, button);
        }

        public IfDoubleTriggerButtonElement @if(Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            return new IfDoubleTriggerButtonElement(value.ifDoubleTriggerButtonElements, button);
        }

        public IfStrokeElement @if(params Def.AcceptableInIfStrokeClause[] moves)
        {
            return new IfStrokeElement(value.ifStrokeElements, moves);
        }
    }
}
