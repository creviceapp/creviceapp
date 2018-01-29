using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLib.Tests
{
    using System.Linq;
    using Crevice;
    using Crevice.Core;
    using Crevice.Core.Types;
    using Crevice.Core.Context;
    using Crevice.Core.FSM;
    using Crevice.Core.DSL;
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestState0 = Crevice.Core.FSM.State0<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;
    using TestStateN = Crevice.Core.FSM.StateN<TestGestureMachineConfig, TestContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    class TestGestureMachineConfig : GestureMachineConfig { }

    class TestContextManager : ContextManager<EvaluationContext, ExecutionContext>
    {
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext();

        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext();
    }

    class TestGestureMachine : GestureMachine<TestGestureMachineConfig, TestContextManager, EvaluationContext, ExecutionContext>
    {
        public TestGestureMachine(TestRootElement rootElement) 
            : base(new TestGestureMachineConfig(), new TestContextManager(),  rootElement)
        {
            
        }

        public System.Threading.CountdownEvent OnGestureTimeoutCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureTimeoutCallCount { get; private set; } = 0;

        internal override void OnGestureTimeout()
        {
            OnGestureTimeoutCallCount += 1;
            OnGestureTimeoutCDE.Signal();
            base.OnMachineReset();
        }

        public System.Threading.CountdownEvent OnGestureCancelledCDE = new System.Threading.CountdownEvent(1);
        public int OnGestureCancelledCallCount { get; private set; } = 0;

        internal override void OnGestureCancelled()
        {
            OnGestureCancelledCallCount += 1;
            OnGestureCancelledCDE.Signal();
            base.OnMachineReset();
        }

        public System.Threading.CountdownEvent OnMachineResetCDE = new System.Threading.CountdownEvent(1);
        public int OnMachineResetCallCount { get; private set; } = 0;

        internal override void OnMachineReset()
        {
            OnMachineResetCallCount += 1;
            OnMachineResetCDE.Signal();
            base.OnMachineReset();
        }
    }

    public class TestEvents
    {
        private static TestLogicalGroup logical = new TestLogicalGroup(1000);
        public static TestLogicalGroup Logical => logical;
        
        private static TestPhysicalGroup physical = new TestPhysicalGroup(Logical, 2000);
        public static TestPhysicalGroup Physical => physical;
    }

    public class TestLogicalGroup : LogicalGroup
    {
        public readonly TestFireEventA TestFireEventA;
        public readonly TestPressEventA TestPressEventA;
        public readonly TestReleaseEventA TestReleaseEventA;
        public readonly TestPhysicalFireEventA TestPhysicalFireEventA;
        public readonly TestPhysicalPressEventA TestPhysicalPressEventA;
        public readonly TestPhysicalReleaseEventA TestPhysicalReleaseEventA;
        public readonly TestFireEventB TestFireEventB;
        public readonly TestPressEventB TestPressEventB;
        public readonly TestReleaseEventB TestReleaseEventB;
        public readonly TestPhysicalFireEventB TestPhysicalFireEventB;
        public readonly TestPhysicalPressEventB TestPhysicalPressEventB;
        public readonly TestPhysicalReleaseEventB TestPhysicalReleaseEventB;
        public TestLogicalGroup(int offset)
        {
            var id = offset;
            TestFireEventA = new TestFireEventA(this, id++);
            TestPressEventA = new TestPressEventA(this, id++);
            TestReleaseEventA = new TestReleaseEventA(this, id++);
            TestFireEventB = new TestFireEventB(this, id++);
            TestPressEventB = new TestPressEventB(this, id++);
            TestReleaseEventB = new TestReleaseEventB(this, id++);
        }
    }

    public class TestPhysicalGroup : PhysicalGroup
    {
        public readonly TestPhysicalFireEventA TestPhysicalFireEventA;
        public readonly TestPhysicalPressEventA TestPhysicalPressEventA;
        public readonly TestPhysicalReleaseEventA TestPhysicalReleaseEventA;
        public readonly TestPhysicalFireEventB TestPhysicalFireEventB;
        public readonly TestPhysicalPressEventB TestPhysicalPressEventB;
        public readonly TestPhysicalReleaseEventB TestPhysicalReleaseEventB;
        public TestPhysicalGroup(TestLogicalGroup logicalGroup, int offset)
        {
            var id = offset;
            TestPhysicalFireEventA = new TestPhysicalFireEventA(logicalGroup, this, id++);
            TestPhysicalPressEventA = new TestPhysicalPressEventA(logicalGroup, this, id++);
            TestPhysicalReleaseEventA = new TestPhysicalReleaseEventA(logicalGroup, this, id++);
            TestPhysicalFireEventB = new TestPhysicalFireEventB(logicalGroup, this, id++);
            TestPhysicalPressEventB = new TestPhysicalPressEventB(logicalGroup, this, id++);
            TestPhysicalReleaseEventB = new TestPhysicalReleaseEventB(logicalGroup, this, id++);
        }
    }

    public class TestSingleThrowSwitchA : SingleThrowSwitch { }

    public class TestDoubleThrowSwitchA : DoubleThrowSwitch { }

    public class TestFireEventA : LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchA>
    {
        public TestFireEventA(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public class TestPressEventA : LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchA>
    {
        public TestPressEventA(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }

        public override LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchA> OppositeReleaseEvent
            => LogicalGroup.TestReleaseEventA;
    }

    public class TestReleaseEventA : LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchA>
    {
        public TestReleaseEventA(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }

        public override LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchA> OppositePressEvent
            => LogicalGroup.TestPressEventA;
    }

    public class TestPhysicalFireEventA : PhysicalFireEvent<TestLogicalGroup, TestPhysicalGroup, TestSingleThrowSwitchA>
    {
        public TestPhysicalFireEventA(
            TestLogicalGroup logicalGroup, 
            TestPhysicalGroup physicalGroup, 
            int eventId) 
            : base(logicalGroup, physicalGroup, eventId)
        { }

        public override LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchA> LogicalEquivalentFireEvent
            => LogicalGroup.TestFireEventA;
    }

    public class TestPhysicalPressEventA : PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA>
    {
        public TestPhysicalPressEventA(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId)
        { }

        public override LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchA> LogicalEquivalentPressEvent
            => LogicalGroup.TestPressEventA;

        public override PhysicalReleaseEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA> OppositePhysicalReleaseEvent
            => PhysicalGroup.TestPhysicalReleaseEventA;
    }

    public class TestPhysicalReleaseEventA : PhysicalReleaseEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA>
    {
        public TestPhysicalReleaseEventA(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId)
        { }

        public override LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchA> LogicalEquivalentReleaseEvent
            => LogicalGroup.TestReleaseEventA;

        public override PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA> OppositePhysicalPressEvent
            => PhysicalGroup.TestPhysicalPressEventA;
    }

    public class TestSingleThrowSwitchB : SingleThrowSwitch { }

    public class TestDoubleThrowSwitchB : DoubleThrowSwitch { }

    public class TestFireEventB : LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchB>
    {
        public TestFireEventB(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }
    }

    public class TestPressEventB : LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchB>
    {
        public TestPressEventB(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }

        public override LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchB> OppositeReleaseEvent
            => LogicalGroup.TestReleaseEventB;
    }

    public class TestReleaseEventB : LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchB>
    {
        public TestReleaseEventB(TestLogicalGroup logicalGroup, int eventId) : base(logicalGroup, eventId) { }

        public override LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchB> OppositePressEvent
            => LogicalGroup.TestPressEventB;
    }

    public class TestPhysicalFireEventB : PhysicalFireEvent<TestLogicalGroup, TestPhysicalGroup, TestSingleThrowSwitchB>
    {
        public TestPhysicalFireEventB(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId)
        { }


        public override LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchB> LogicalEquivalentFireEvent
            => LogicalGroup.TestFireEventB;
    }

    public class TestPhysicalPressEventB : PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB>
    {
        public TestPhysicalPressEventB(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId)
        { }


        public override LogicalPressEvent<TestLogicalGroup, TestDoubleThrowSwitchB> LogicalEquivalentPressEvent
            => LogicalGroup.TestPressEventB;

        public override PhysicalReleaseEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB> OppositePhysicalReleaseEvent
            => PhysicalGroup.TestPhysicalReleaseEventB;
    }

    public class TestPhysicalReleaseEventB : PhysicalReleaseEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB>
    {
        public TestPhysicalReleaseEventB(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId)
        { }


        public override LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchB> LogicalEquivalentReleaseEvent
            => LogicalGroup.TestReleaseEventB;

        public override PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB> OppositePhysicalPressEvent
            => PhysicalGroup.TestPhysicalPressEventB;
    }

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

    [TestClass]
    public class DSLSyntexTest
    {
        [TestMethod]
        public void RootWhenTest()
        {
            var root = new TestRootElement();

            Assert.AreEqual(root.WhenElements.Count, 0);
            var w = root.When(ctx => { return true; });
            Assert.AreEqual(root.WhenElements.Count, 1);
        }

        [TestMethod]
        public void WhenOnSingleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.SingleThrowElements.Count, 0);
            var fire = when.On(TestEvents.Logical.TestFireEventA);
            Assert.AreEqual(when.SingleThrowElements.Count, 1);

            Assert.AreEqual(fire.DoExecutors.Count, 0);
            fire.Do(ctx => { });
            Assert.AreEqual(fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void WhenOnDoubleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DoubleThrowElements.Count, 0);
            var press = when.On(TestEvents.Logical.TestPressEventA);
            Assert.AreEqual(when.DoubleThrowElements.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Logical.TestPressEventA);

            Assert.AreEqual(press.PressExecutors.Count, 0);
            press.Press(ctx => { });
            Assert.AreEqual(press.PressExecutors.Count, 1);

            Assert.AreEqual(press.DoExecutors.Count, 0);
            press.Do(ctx => { });
            Assert.AreEqual(press.DoExecutors.Count, 1);

            Assert.AreEqual(press.ReleaseExecutors.Count, 0);
            press.Release(ctx => { });
            Assert.AreEqual(press.ReleaseExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowSingleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Logical.TestPressEventA);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.Logical.TestFireEventA);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowDoubleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Logical.TestPressEventA);
            var press_press = press.On(TestEvents.Logical.TestPressEventA);

            Assert.AreEqual(press_press.PressExecutors.Count, 0);
            press_press.Press(ctx => { });
            Assert.AreEqual(press_press.PressExecutors.Count, 1);

            Assert.AreEqual(press_press.DoExecutors.Count, 0);
            press_press.Do(ctx => { });
            Assert.AreEqual(press_press.DoExecutors.Count, 1);

            Assert.AreEqual(press_press.ReleaseExecutors.Count, 0);
            press_press.Release(ctx => { });
            Assert.AreEqual(press_press.ReleaseExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowStrokeTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Logical.TestPressEventA);

            Assert.AreEqual(press.StrokeElements.Count, 0);
            var press_stroke = press.On(StrokeDirection.Up);
            Assert.AreEqual(press.StrokeElements.Count, 1);

            Assert.AreEqual(press_stroke.DoExecutors.Count, 0);
            press_stroke.Do(ctx => { });
            Assert.AreEqual(press_stroke.DoExecutors.Count, 1);
        }
    }

    [TestClass]
    public class State0Test
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                Assert.AreEqual(s0.Machine, gm);
                Assert.AreEqual(s0.RootElement, root);
            }
        }

        [TestMethod]
        public void CreateHistoryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.History[0].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(s1.History[0].Item2, s0);
            }
        }

        [TestMethod]
        public void DoesNotConsumeGivenInputWhenNoDefinition()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenPhysicalSingleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestFireEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenLogicalSingleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Physical.TestPhysicalFireEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Physical.TestPhysicalPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(result.NextState is TestStateN);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowDefinisitonExists()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(result.NextState is TestStateN);
                    Assert.AreEqual(result.EventIsConsumed, true);
                }
                {
                    var result = s0.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result.NextState, s0);
                    Assert.AreEqual(result.EventIsConsumed, false);
                }
            }
        }
        [TestMethod]
        public void CreateHistory()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var s0 = new TestState0(gm, root);
                var result = s0.CreateHistory(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(result[0].Item2, s0);
            }
        }


        [TestMethod]
        public void GetActiveDoubleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Logical.TestPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var result = s0.GetActiveDoubleThrowElements(evalContext, TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetActiveSingleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestFireEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var result = s0.GetActiveSingleThrowElements(evalContext, TestEvents.Physical.TestPhysicalFireEventA);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.SingleThrowElements[0]);
            }
        }

        [TestMethod]
        public void SingleThrowTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestFireEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var result = s0.SingleThrowTriggers;
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result.Contains(when.SingleThrowElements[0].Trigger), true);
            }
        }

        [TestMethod]
        public void DoubleThrowTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var result = s0.DoubleThrowTriggers;
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result.Contains(when.DoubleThrowElements[0].Trigger), true);
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var result = s0.Reset();
                    Assert.AreEqual(s0, result);
                }
            }
        }
    }

    [TestClass]
    public class StateNTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var s0 = new TestState0(gm, root);
                var history = s0.CreateHistory(TestEvents.Physical.TestPhysicalPressEventA);
                var dt = new List<DoubleThrowElement<ExecutionContext>>();
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, true);
                }
                {
                    var s1 = new TestStateN(gm, evalContext, history, dt, canCancel: false);
                    Assert.AreEqual(s1.Machine, gm);
                    Assert.AreEqual(s1.Ctx, evalContext);
                    Assert.AreEqual(s1.History, history);
                    Assert.AreEqual(s1.DoubleThrowElements, dt);
                    Assert.AreEqual(s1.CanCancel, false);
                }
            }
        }

        [TestMethod]
        public void CreateHistoryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.History[0].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(s1.History[0].Item2, s0);

                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                var s2 = res1.NextState as TestStateN;
                Assert.AreEqual(s2.History.Count, 2);
                Assert.AreEqual(s2.History[1].Item1, TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(s2.History[1].Item2, s1);

                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                var s3 = res2.NextState as TestStateN;
                Assert.AreEqual(s3.History.Count, 3);
                Assert.AreEqual(s3.History[2].Item1, TestEvents.Physical.TestPhysicalReleaseEventA);
                Assert.AreEqual(s3.History[2].Item2, s2);

                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                var s4 = res3.NextState as TestStateN;
                Assert.AreEqual(s4.History.Count, 4);
                Assert.AreEqual(s4.History[3].Item1, TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(s4.History[3].Item2, s3);
            }
        }

        [TestMethod]
        public void AbnormalEndTriggersTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                        .On(TestEvents.Logical.TestPressEventB)
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.AbnormalEndTriggers.Count, 0);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.AbnormalEndTriggers.Count, 1);
                Assert.AreEqual(s2.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);
                Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventB), true);

                var s4 = res3.NextState as TestStateN;
                var res4 = s4.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res4.EventIsConsumed, true);
                Assert.AreEqual(res4.NextState is TestStateN, true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Count, 2);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventA), true);
                Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Physical.TestPhysicalReleaseEventB), true);
            }
        }

        [TestMethod]
        public void FindStateFromHistoryTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                            .On(TestEvents.Logical.TestPressEventA)
                                .On(TestEvents.Logical.TestPressEventB)
                                    .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);

                var s2 = res1.NextState as TestStateN;
                var res2 = s2.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res2.EventIsConsumed, true);
                Assert.AreEqual(res2.NextState is TestStateN, true);
                Assert.AreEqual(s2.History.Count, 2);

                var s3 = res2.NextState as TestStateN;
                var res3 = s3.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res3.EventIsConsumed, true);
                Assert.AreEqual(res3.NextState is TestStateN, true);
                Assert.AreEqual(s3.History.Count, 3);

                var (foundState, skippedReleaseEvents) = s3.FindStateFromHistory(TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(foundState, s1);
                Assert.AreEqual(skippedReleaseEvents.Count, 2);
                Assert.AreEqual(skippedReleaseEvents[0], TestEvents.Physical.TestPhysicalReleaseEventB);
                Assert.AreEqual(skippedReleaseEvents[1], TestEvents.Physical.TestPhysicalReleaseEventA);
            }
        }

        [TestMethod]
        public void NormalEndTriggerTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.NormalEndTrigger, TestEvents.Physical.TestPhysicalReleaseEventA);
            }
        }

        [TestMethod]
        public void IsNormalEndTriggerTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(res1.EventIsConsumed, true);
                Assert.AreEqual(res1.NextState is TestStateN, true);
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.IsNormalEndTrigger(TestEvents.Physical.TestPhysicalReleaseEventA), true);
            }
        }

        [TestMethod]
        public void LastStateTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                    .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                Assert.AreEqual(res0.EventIsConsumed, true);
                Assert.AreEqual(res0.NextState is TestStateN, true);

                var s1 = res0.NextState as TestStateN;
                Assert.AreEqual(s1.History.Count, 1);
                Assert.AreEqual(s1.LastState, s0);
            }
        }

        [TestMethod]
        public void HasPressExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasPressExecutors, true);
                }
            }
        }

        [TestMethod]
        public void HasDoExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasDoExecutors, true);
                }
            }
        }

        [TestMethod]
        public void HasReleaseExecutorsTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, false);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Release((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.EventIsConsumed, true);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    Assert.AreEqual(s1.HasReleaseExecutors, true);
                }
            }
        }

        [TestMethod]
        public void GetDoubleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestPressEventB)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetDoubleThrowElements(TestEvents.Physical.TestPhysicalPressEventB);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].DoubleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetSingleThrowElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(TestEvents.Logical.TestFireEventA)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetSingleThrowElements(TestEvents.Physical.TestPhysicalFireEventA);
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].SingleThrowElements[0]);
            }
        }

        [TestMethod]
        public void GetStrokeElementsTest()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                var when = root.When((ctx) => { return true; });
                when
                    .On(TestEvents.Logical.TestPressEventA)
                        .On(StrokeDirection.Up)
                        .Do((ctx) => { });
                var s0 = new TestState0(gm, root);
                var evalContext = gm.ContextManager.CreateEvaluateContext();
                var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                var s1 = res0.NextState as TestStateN;
                var result = s1.GetStrokeElements(new List<StrokeDirection>() { StrokeDirection.Up });
                Assert.AreEqual(result.Count, 1);
                Assert.AreEqual(result[0], when.DoubleThrowElements[0].StrokeElements[0]);
            }
        }
        
        [TestMethod]
        public void ResetTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                                .On(TestEvents.Logical.TestPressEventA)
                                    .On(TestEvents.Logical.TestPressEventB)
                                        .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    var result = s1.Reset();
                    Assert.AreEqual(s0, result);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                                .On(TestEvents.Logical.TestPressEventA)
                                    .On(TestEvents.Logical.TestPressEventB)
                                        .On(TestEvents.Logical.TestPressEventA)
                                        .Do((ctx) => { });
                    var s0 = new TestState0(gm, root);
                    var res0 = s0.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(res0.NextState is TestStateN, true);

                    var s1 = res0.NextState as TestStateN;
                    var res1 = s1.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.AreEqual(res1.NextState is TestStateN, true);

                    var s2 = res1.NextState as TestStateN;
                    var result = s2.Reset();
                    Assert.AreEqual(s1, result);
                }
            }
        }
    }

    [TestClass]
    public class GestureMachineTest
    {
        [TestMethod]
        public void ConsumeGivenPhysicalReleaseEventWhenItInIgnoreList()
        {
            var root = new TestRootElement();
            using (var gm = new TestGestureMachine(root))
            {
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, false);
                }

                gm.invalidReleaseEvents.IgnoreNext(TestEvents.Physical.TestPhysicalReleaseEventA);

                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, true);
                }

                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(result, false);
                }
                {
                    var result = gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(result, false);
                }
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            var root = new TestRootElement();
            var when = root.When((ctx) => { return true; });
            when
                .On(TestEvents.Logical.TestPressEventA)
                    .On(TestEvents.Logical.TestPressEventB)
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventB], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
            {
                using (var gm = new TestGestureMachine(root))
                {
                    var s0 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    var s1 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventB);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    var s2 = gm.CurrentState;
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.IsTrue(s0 != gm.CurrentState);
                    Assert.IsTrue(s1 != gm.CurrentState);
                    Assert.IsTrue(s2 != gm.CurrentState);
                    var s3 = gm.CurrentState;
                    Assert.AreEqual(gm.OnMachineResetCallCount, 0);
                    gm.Reset();
                    Assert.AreEqual(gm.OnMachineResetCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnMachineResetCallCount, 1);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventA], 2);
                    Assert.AreEqual(gm.invalidReleaseEvents[TestEvents.Physical.TestPhysicalReleaseEventB], 1);
                    Assert.AreEqual(s0, gm.CurrentState);
                }
            }
        }

        [TestMethod]
        public void OnGestureCancelledTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalReleaseEventA);
                    Assert.AreEqual(gm.OnGestureCancelledCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureCancelledCallCount, 1);
                }
            }
        }

        [TestMethod]
        public void OnGestureTimeoutTest()
        {
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(StrokeDirection.Up)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Press((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Do((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                        .Release((ctx) => { })
                            .On(TestEvents.Logical.TestPressEventB)
                            .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    //gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(1000), true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 1);
                }
            }
            {
                var root = new TestRootElement();
                using (var gm = new TestGestureMachine(root))
                {
                    var when = root.When((ctx) => { return true; });
                    when
                        .On(TestEvents.Logical.TestPressEventA)
                            .On(TestEvents.Logical.TestFireEventA)
                                .Do((ctx) => { });
                    gm.Config.GestureTimeout = 5; // ms
                    Assert.AreEqual(gm.CurrentState is TestState0, true);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                    gm.Input(TestEvents.Physical.TestPhysicalPressEventA);
                    Assert.AreEqual(gm.CurrentState is TestStateN, true);
                    gm.Input(TestEvents.Physical.TestPhysicalFireEventA);
                    Assert.AreEqual((gm.CurrentState as TestStateN).CanCancel, false);
                    Assert.AreEqual(gm.OnGestureTimeoutCDE.Wait(100), false);
                    Assert.AreEqual(gm.OnGestureTimeoutCallCount, 0);
                }
            }
        }
    }
}
