using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Events;
    using Crevice.Core.Keys;

    using BenchmarkDotNet.Running;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;

    [OrderProvider(SummaryOrderPolicy.FastestToSlowest)]
    public class EventsBenchmark
    {
        public LogicalDoubleThrowKeys LogicalDoubleThrowKeys = new LogicalDoubleThrowKeys(100);

        [Benchmark]
        public LogicalDoubleThrowKeys Create_LogicalDoubleThrowKeys() => new LogicalDoubleThrowKeys(100);
        
        [Benchmark]
        public PressEvent LogicalDoubleThrowKeys_PressEvent() => LogicalDoubleThrowKeys[0].PressEvent;

    }

    [TestClass]
    public class EventsBenchmarkTest
    {
        [TestMethod]
        public void BenchmarkTest()
        {
            BenchmarkRunner.Run<EventsBenchmark>();
        }
    }
}
