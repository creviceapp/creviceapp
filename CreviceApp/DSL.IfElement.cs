using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public abstract class IfElement
    {
        public abstract class Value
        {
            public readonly List<DoElement.Value> doElements = new List<DoElement.Value>();
        }
    }
}
