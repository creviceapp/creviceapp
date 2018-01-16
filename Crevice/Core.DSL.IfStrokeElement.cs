using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crevice.Core.DSL
{
    public class IfStrokeElement
    {
        public class Value
        {
            public readonly List<SingleTriggerDoElement.Value> doElements = new List<SingleTriggerDoElement.Value>();

            public readonly IEnumerable<Def.AcceptableInIfStrokeClause> moves;

            public Value(IEnumerable<Def.AcceptableInIfStrokeClause> moves)
            {
                this.moves = moves;
            }
        }
        
        private readonly Value value;

        public IfStrokeElement(List<Value> parent, params Def.AcceptableInIfStrokeClause[] moves)
        {
            this.value = new Value(moves);
            parent.Add(this.value);
        }

        public SingleTriggerDoElement @do(Def.DoFunc func)
        {
            return new SingleTriggerDoElement(value.doElements, func);
        }
    }
}
