using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class IfButtonElement
    {
        public class Value : IfElement.Value
        {
            public readonly Def.AcceptableInIfButtonClause button;

            public Value(Def.AcceptableInIfButtonClause button)
            {
                this.button = button;
            }
        }

        private readonly OnElement.Value parent;
        private readonly Value value;

        public IfButtonElement(OnElement.Value parent, Def.AcceptableInIfButtonClause button)
        {
            this.parent = parent;
            this.value = new Value(button);
            this.parent.ifButtonElements.Add(this.value);
        }

        public DoElement @do(Def.DoFunc func)
        {
            return new DoElement(value, func);
        }
    }
}
