using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.DSL
{
    public class IfSingleTriggerButtonElement
    {
        public class Value
        {
            public readonly List<SingleTriggerDoElement.Value> doElements = new List<SingleTriggerDoElement.Value>();

            public readonly Def.AcceptableInIfSingleTriggerButtonClause button;

            public Value(Def.AcceptableInIfSingleTriggerButtonClause button)
            {
                this.button = button;
            }
        }
        
        private readonly Value value;

        public IfSingleTriggerButtonElement(List<Value> parent, Def.AcceptableInIfSingleTriggerButtonClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public SingleTriggerDoElement @do(Def.DoFunc func)
        {
            return new SingleTriggerDoElement(value.doElements, func);
        }
    }
}
