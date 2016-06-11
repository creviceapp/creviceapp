using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class DoElement
    {
        public class Value
        {
            public readonly Def.DoFunc func;

            public Value(Def.DoFunc func)
            {
                this.func = func;
            }
        }

        private readonly IfElement.Value parent;
        private readonly Value value;

        public DoElement(IfElement.Value parent, Def.DoFunc func)
        {
            this.parent = parent;
            this.value = new Value(func);
            this.parent.doElements.Add(this.value);
        }
    }
}
