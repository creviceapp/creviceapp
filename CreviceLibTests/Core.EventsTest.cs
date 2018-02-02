using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Events;

    [TestClass]
    public class TypeSystemTest
    {
        [TestMethod]
        public void NullEventTest()
        {
            var nullEvent0 = new NullEvent();
            var nullEvent1 = new NullEvent();
            Assert.IsTrue(nullEvent0.GetHashCode() == 0);
            Assert.IsTrue(nullEvent1.GetHashCode() == 0);
            Assert.IsTrue((nullEvent0 == nullEvent1) == false);
            Assert.IsTrue(nullEvent0.Equals(nullEvent1) == false);
            Assert.IsTrue(nullEvent1.Equals(nullEvent0) == false);
        }

        [TestMethod]
        public void FireEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is LogicalFireEvent);
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is IPhysicalEvent == false);
        }

        [TestMethod]
        public void PressEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is LogicalPressEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.LogicalDoubleThrowKeys[0].LogicalPressEvent.Opposition, TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent);
        }

        [TestMethod]
        public void ReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is LogicalReleaseEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.LogicalDoubleThrowKeys[0].LogicalReleaseEvent.Opposition, TestEvents.LogicalDoubleThrowKeys[0].PressEvent);
        }

        [TestMethod]
        public void PhysicalFireEventTest()
        {
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is PhysicalFireEvent);
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is IPhysicalEvent);
        }

        [TestMethod]
        public void PhysicalPressEventTest()
        {
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is PhysicalPressEvent);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is IPhysicalEvent);
            Assert.AreEqual(TestEvents.PhysicalDoubleThrowKeys[0].PhysicalPressEvent.Opposition, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
        }

        [TestMethod]
        public void PhysicalReleaseEventTest()
        {
            var pressEvent = TestEvents.PhysicalDoubleThrowKeys[0].PressEvent;
            var releaseEvent = TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent;
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is PhysicalReleaseEvent);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is IPhysicalEvent);
            Assert.AreEqual(TestEvents.PhysicalDoubleThrowKeys[0].PhysicalReleaseEvent.Opposition, TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
        }
    }
}
