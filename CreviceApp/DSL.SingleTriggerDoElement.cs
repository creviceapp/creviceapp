using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class SingleTriggerDoElement
    {
        public class Value
        {
            public readonly Def.DoFunc func;

            public Value(Def.DoFunc func)
            {
                this.func = func;
            }
        }

        public SingleTriggerDoElement(List<Value> parent, Def.DoFunc func)
        {
            parent.Add(new Value(func));
        }
    }
}
