using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.DSL
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
