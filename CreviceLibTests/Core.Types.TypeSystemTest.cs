using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Types;

    [TestClass]
    public class TypeSystemTest
    {
        [TestMethod]
        public void FireEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is LogicalFireEvent<SingleThrowSwitch>);
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalSingleThrowKeys[0].FireEvent is IPhysicalEvent == false);
        }

        [TestMethod]
        public void PressEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is LogicalPressEvent<DoubleThrowSwitch>);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].PressEvent is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.LogicalDoubleThrowKeys[0].PressEvent.OppositeReleaseEvent, TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent);
        }

        [TestMethod]
        public void ReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is LogicalReleaseEvent<DoubleThrowSwitch>);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is ILogicalEvent);
            Assert.IsTrue(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.LogicalDoubleThrowKeys[0].ReleaseEvent.OppositePressEvent, TestEvents.LogicalDoubleThrowKeys[0].PressEvent);
        }

        [TestMethod]
        public void PhysicalFireEventTest()
        {
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is PhysicalFireEvent<SingleThrowSwitch>);
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalSingleThrowKeys[0].FireEvent is IPhysicalEvent);
        }

        [TestMethod]
        public void PhysicalPressEventTest()
        {
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is PhysicalPressEvent<DoubleThrowSwitch>);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent is IPhysicalEvent);
            Assert.AreEqual(TestEvents.PhysicalDoubleThrowKeys[0].PressEvent.OppositePhysicalReleaseEvent, TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent);
        }

        [TestMethod]
        public void PhysicalReleaseEventTest()
        {
            var pressEvent = TestEvents.PhysicalDoubleThrowKeys[0].PressEvent;
            var releaseEvent = TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent;
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is PhysicalReleaseEvent<DoubleThrowSwitch>);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent is IPhysicalEvent);
            Assert.AreEqual(TestEvents.PhysicalDoubleThrowKeys[0].ReleaseEvent.OppositePhysicalPressEvent, TestEvents.PhysicalDoubleThrowKeys[0].PressEvent);
        }
    }
}
