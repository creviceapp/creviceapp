using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceTests
{
    using Crevice.Future2;

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestTypeSystem()
        {
            Assert.IsTrue(Events.Constants.Move is FireEvent<MoveSource>);
            Assert.IsTrue(Events.Constants.Move is ILogicalEvent);
            Assert.IsTrue(Events.Constants.Move is IPysicalEvent == false);
            Assert.IsTrue(Events.Constants.Move.EventId == 100);

            Assert.IsTrue(Events.Constants.LeftButtonDown is PressEvent<LeftButtonSource>);
            Assert.IsTrue(Events.Constants.LeftButtonDown is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDown is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDown.OppositeEvent, Events.Constants.LeftButtonUp);
            Assert.IsTrue(Events.Constants.LeftButtonDown.EventId == 200);

            Assert.IsTrue(Events.Constants.LeftButtonDown is PressEvent<LeftButtonSource>);
            Assert.IsTrue(Events.Constants.LeftButtonDown is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDown is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDown.OppositeEvent, Events.Constants.LeftButtonUp);
            Assert.IsTrue(Events.Constants.LeftButtonDown.EventId == 200);

            Assert.IsTrue(Events.Constants.LeftButtonDownP0 is PhysicalPressEvent<LeftButtonSource>);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0 is ILogicalEvent == false);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0 is IPysicalEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDownP0.OppositeEvent, Events.Constants.LeftButtonUpP0);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0.EventId == 201);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0.LogicallyEquals(Events.Constants.LeftButtonDown));
        }
    }
}
