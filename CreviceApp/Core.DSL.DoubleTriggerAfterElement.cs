using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.DSL
{
    public class DoubleTriggerAfterElement
    {
        public class Value
        {
            public readonly Def.AfterFunc func;

            public Value(Def.AfterFunc func)
            {
                this.func = func;
            }
        }

        public DoubleTriggerAfterElement(List<Value> parent, Def.AfterFunc func)
        {
            parent.Add(new Value(func));
        }
    }
}
