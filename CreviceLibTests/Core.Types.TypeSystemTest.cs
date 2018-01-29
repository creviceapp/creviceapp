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
            Assert.IsTrue(TestEvents.Logical.TestFireEventA is LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Logical.TestFireEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Logical.TestFireEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Logical.TestFireEventA.EventId, 1000);
        }

        [TestMethod]
        public void PressEventTest()
        {
            Assert.IsTrue(TestEvents.Logical.TestPressEventA is LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Logical.TestPressEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Logical.TestPressEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Logical.TestPressEventA.OppositeReleaseEvent, TestEvents.Logical.TestReleaseEventA);
            Assert.AreEqual(TestEvents.Logical.TestPressEventA.EventId, 1001);
        }

        [TestMethod]
        public void ReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.Logical.TestReleaseEventA is LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Logical.TestReleaseEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Logical.TestReleaseEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Logical.TestReleaseEventA.OppositePressEvent, TestEvents.Logical.TestPressEventA);
            Assert.AreEqual(TestEvents.Logical.TestReleaseEventA.EventId, 1002);
        }

        [TestMethod]
        public void PhysicalFireEventTest()
        {
            Assert.IsTrue(TestEvents.Physical.TestPhysicalFireEventA is PhysicalFireEvent<TestLogicalGroup, TestPhysicalGroup, TestSingleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalFireEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalFireEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Physical.TestPhysicalFireEventA.EventId, 2000);
        }

        [TestMethod]
        public void PhysicalPressEventTest()
        {
            Assert.IsTrue(TestEvents.Physical.TestPhysicalPressEventA is PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalPressEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalPressEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Physical.TestPhysicalPressEventA.OppositePhysicalReleaseEvent, TestEvents.Physical.TestPhysicalReleaseEventA);
            Assert.AreEqual(TestEvents.Physical.TestPhysicalPressEventA.EventId, 2001);
        }

        [TestMethod]
        public void PhysicalReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.Physical.TestPhysicalReleaseEventA is PhysicalReleaseEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalReleaseEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Physical.TestPhysicalReleaseEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Physical.TestPhysicalReleaseEventA.OppositePhysicalPressEvent, TestEvents.Physical.TestPhysicalPressEventA);
            Assert.AreEqual(TestEvents.Physical.TestPhysicalReleaseEventA.EventId, 2002);
        }
    }
}
