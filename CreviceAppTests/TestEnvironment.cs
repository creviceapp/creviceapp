using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceAppTests
{
    using System.Threading;

    public static class TestVar
    {
        public static readonly Mutex ConsoleMutex = new Mutex(true, "ConsoleMutex");
    }
}
