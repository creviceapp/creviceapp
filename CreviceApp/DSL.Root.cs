using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL
{
    public class Root
    {
        public readonly List<WhenElement.Value> whenElements = new List<WhenElement.Value>();

        public WhenElement @when(Def.WhenFunc func)
        {
            return new WhenElement(whenElements, func);
        }
    }
}
