using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class IfStrokeElement
    {
        public class Value
        {
            public readonly List<DoElement.Value> doElements = new List<DoElement.Value>();

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

        public DoElement @do(Def.DoFunc func)
        {
            return new DoElement(value.doElements, func);
        }
    }
}
