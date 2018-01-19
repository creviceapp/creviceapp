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
            Assert.IsTrue(Events.Constants.Move is FireEvent<MoveSwich>);
            Assert.IsTrue(Events.Constants.Move is ILogicalEvent);
            Assert.IsTrue(Events.Constants.Move is IPysicalEvent == false);
            Assert.IsTrue(Events.Constants.Move.EventId == 1000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositeEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositeEvent, Events.Constants.LeftButtonUpEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent.EventId == 2000);

            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is PhysicalPressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is ILogicalEvent == false);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event is IPysicalEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDownP0Event.OppositeEvent, Events.Constants.LeftButtonUpP0Event);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.EventId == 2001);
            Assert.IsTrue(Events.Constants.LeftButtonDownP0Event.LogicallyEquals(Events.Constants.LeftButtonDownEvent));
        }
    }
}
