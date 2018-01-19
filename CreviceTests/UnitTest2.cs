using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceTests
{
    using Crevice.Future;

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestTypeSystem()
        {
            Assert.IsTrue(Events.Constants.Move is FireEvent);
            Assert.IsTrue(Events.Constants.Move is ILogicalEvent);
            Assert.IsTrue(Events.Constants.Move.EventId == 100);

            Assert.IsTrue(Events.Constants.LeftButtonDown is PressEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDown is ILogicalEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDown.OppositeEvent, Events.Constants.LeftButtonUp);
            Assert.IsTrue(Events.Constants.LeftButtonDown.EventId == 200);



        }
    }
}
