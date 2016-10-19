using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class DoubleTriggerBeforeElement
    {
        public class Value
        {
            public readonly Def.BeforeFunc func;

            public Value(Def.BeforeFunc func)
            {
                this.func = func;
            }
        }

        private readonly List<DoubleTriggerDoElement.Value> doParent;
        private readonly List<DoubleTriggerAfterElement.Value> afterParent;

        public DoubleTriggerBeforeElement(
            List<Value> parentA, 
            List<DoubleTriggerDoElement.Value> parentB,
            List<DoubleTriggerAfterElement.Value> parentC,
            Def.BeforeFunc func)
        {
            parentA.Add(new Value(func));
            doParent = parentB;
            afterParent = parentC;
        }

        public DoubleTriggerDoElement @do(Def.DoFunc func)
        {
            return new DoubleTriggerDoElement(doParent, afterParent, func);
        }

        public DoubleTriggerAfterElement @after(Def.AfterFunc func)
        {
            return new DoubleTriggerAfterElement(afterParent, func);
        }
    }
}
