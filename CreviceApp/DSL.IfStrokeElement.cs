using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class IfStrokeElement
    {
        public class Value : IfElement.Value
        {
            public readonly IEnumerable<Def.AcceptableInIfStrokeClause> moves;

            public Value(IEnumerable<Def.AcceptableInIfStrokeClause> moves)
            {
                this.moves = moves;
            }
        }

        private readonly OnElement.Value parent;
        private readonly Value value;

        public IfStrokeElement(OnElement.Value parent, params Def.AcceptableInIfStrokeClause[] moves)
        {
            this.parent = parent;
            this.value = new Value(moves);
            this.parent.ifStrokeElements.Add(this.value);
        }

        public DoElement @do(Def.DoFunc func)
        {
            return new DoElement(value, func);
        }
    }
}
