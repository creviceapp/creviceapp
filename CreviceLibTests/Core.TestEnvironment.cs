using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Types;
    using Crevice.Core.Context;
    using Crevice.Core.FSM;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

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
            : base(new TestGestureMachineConfig(), new TestContextManager(),  rootElement) { }

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
            : base(logicalGroup, physicalGroup, eventId) { }

        public override LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchA> LogicalEquivalentFireEvent
            => LogicalGroup.TestFireEventA;
    }

    public class TestPhysicalPressEventA : PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchA>
    {
        public TestPhysicalPressEventA(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId) { }

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
            : base(logicalGroup, physicalGroup, eventId) { }

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
            : base(logicalGroup, physicalGroup, eventId) { }

        public override LogicalFireEvent<TestLogicalGroup, TestSingleThrowSwitchB> LogicalEquivalentFireEvent
            => LogicalGroup.TestFireEventB;
    }

    public class TestPhysicalPressEventB : PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB>
    {
        public TestPhysicalPressEventB(
            TestLogicalGroup logicalGroup,
            TestPhysicalGroup physicalGroup,
            int eventId)
            : base(logicalGroup, physicalGroup, eventId) { }

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
            : base(logicalGroup, physicalGroup, eventId) { }

        public override LogicalReleaseEvent<TestLogicalGroup, TestDoubleThrowSwitchB> LogicalEquivalentReleaseEvent
            => LogicalGroup.TestReleaseEventB;

        public override PhysicalPressEvent<TestLogicalGroup, TestPhysicalGroup, TestDoubleThrowSwitchB> OppositePhysicalPressEvent
            => PhysicalGroup.TestPhysicalPressEventB;
    }
}
