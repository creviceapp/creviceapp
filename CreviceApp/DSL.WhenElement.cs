using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class WhenElement
    {
        public class Value
        {
            public readonly List<OnElement.Value> onElements = new List<OnElement.Value>();
            public readonly Def.WhenFunc func;

            public Value(Def.WhenFunc func)
            {
                this.func = func;
            }
        }

        private readonly Root parent;
        private readonly Value value;

        public WhenElement(Root parent, Def.WhenFunc func)
        {
            this.parent = parent;
            this.value = new Value(func);
            this.parent.whenElements.Add(this.value);
        }

        public OnElement @on(Def.AcceptableInOnClause button)
        {
            return new OnElement(value, button);
        }
    }
}
