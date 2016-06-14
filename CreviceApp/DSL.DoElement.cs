using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private readonly Value value;

        public DoElement(List<Value> parent, Def.DoFunc func)
        {
            this.value = new Value(func);
            parent.Add(this.value);
        }
    }
}
