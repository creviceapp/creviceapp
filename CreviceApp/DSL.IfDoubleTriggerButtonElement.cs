using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class IfDoubleTriggerButtonElement
    {
        public class Value
        {
            public readonly List<DoubleTriggerBeforeElement.Value> beforeElements = new List<DoubleTriggerBeforeElement.Value>();
            public readonly List<DoubleTriggerDoElement.Value> doElements = new List<DoubleTriggerDoElement.Value>();
            public readonly List<DoubleTriggerAfterElement.Value> afterElements = new List<DoubleTriggerAfterElement.Value>();

            public readonly Def.AcceptableInIfDoubleTriggerButtonClause button;

            public Value(Def.AcceptableInIfDoubleTriggerButtonClause button)
            {
                this.button = button;
            }
        }
        
        private readonly Value value;

        public IfDoubleTriggerButtonElement(List<Value> parent, Def.AcceptableInIfDoubleTriggerButtonClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }
        
        public DoubleTriggerBeforeElement @prepare(Def.BeforeFunc func)
        {
            return new DoubleTriggerBeforeElement(value.beforeElements, value.doElements, value.afterElements, func);
        }

        public DoubleTriggerDoElement @do(Def.DoFunc func)
        {
            return new DoubleTriggerDoElement(value.doElements, value.afterElements, func);
        }
    }
}
