using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class DoubleTriggerDoElement
    {
        public class Value
        {
            public readonly Def.DoFunc func;

            public Value(Def.DoFunc func)
            {
                this.func = func;
            }
        }

        private readonly List<DoubleTriggerAfterElement.Value> afterParent;

        public DoubleTriggerDoElement(
            List<Value> parentA, 
            List<DoubleTriggerAfterElement.Value> parentB, 
            Def.DoFunc func)
        {
            parentA.Add(new Value(func));
            afterParent = parentB;
        }

        public DoubleTriggerAfterElement @after(Def.AfterFunc func)
        {
            return new DoubleTriggerAfterElement(afterParent, func);
        }
    }
}
