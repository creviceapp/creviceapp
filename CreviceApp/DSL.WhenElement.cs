using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class WhenElement
    {
        public class Value
        {
            public readonly List<IfButtonElement.Value> ifButtonElements = new List<IfButtonElement.Value>();
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

        public IfButtonElement @if(Def.AcceptableInIfButtonClause button)
        {
            return new IfButtonElement(value.ifButtonElements, button);
        }
    }
}
