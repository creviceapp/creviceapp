using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class OnElement
    {
        public class Value
        {
            public readonly List<IfButtonElement.Value> ifButtonElements = new List<IfButtonElement.Value>();
            public readonly List<IfStrokeElement.Value> ifStrokeElements = new List<IfStrokeElement.Value>();
            public readonly Def.AcceptableInOnClause button;

            public Value(Def.AcceptableInOnClause button)
            {
                this.button = button;
            }
        }
        
        private readonly Value value;

        public OnElement(List<Value> parent, Def.AcceptableInOnClause button)
        {
            this.value = new Value(button);
            parent.Add(this.value);
        }

        public IfButtonElement @if(Def.AcceptableInIfButtonClause button)
        {
            return new IfButtonElement(value.ifButtonElements, button);
        }

        public IfStrokeElement @if(params Def.AcceptableInIfStrokeClause[] moves)
        {
            return new IfStrokeElement(value.ifStrokeElements, moves);
        }
    }
}
