using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class IfButtonElement
    {
        public class Value
        {
            public readonly List<DoElement.Value> doElements = new List<DoElement.Value>();

            public readonly Def.AcceptableInIfButtonClause button;

            public Value(Def.AcceptableInIfButtonClause button)
            {
                this.button = button;
            }
        }
        
        private readonly Value value;

        public IfButtonElement(List<Value> parent, Def.AcceptableInIfButtonClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public DoElement @do(Def.DoFunc func)
        {
            return new DoElement(value.doElements, func);
        }
    }
}
