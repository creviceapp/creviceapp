using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceTests
{
    using Crevice.Future;
    using System.Linq;

    class TestGestureMachine : GestureMachine<EvaluationContext, ExecutionContext>
    {
        public override EvaluationContext CreateEvaluateContext()
            => new EvaluationContext();
        
        public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            => new ExecutionContext();
    }

    public class TestEvents
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
        public TestEvents()
        {
            var id = 0;
            TestFireEventA = new TestFireEventA(++id);
            TestPressEventA = new TestPressEventA(++id);
            TestReleaseEventA = new TestReleaseEventA(++id);
            TestPhysicalFireEventA = new TestPhysicalFireEventA(++id);
            TestPhysicalPressEventA = new TestPhysicalPressEventA(++id);
            TestPhysicalReleaseEventA = new TestPhysicalReleaseEventA(++id);
            TestFireEventB = new TestFireEventB(++id);
            TestPressEventB = new TestPressEventB(++id);
            TestReleaseEventB = new TestReleaseEventB(++id);
            TestPhysicalFireEventB = new TestPhysicalFireEventB(++id);
            TestPhysicalPressEventB = new TestPhysicalPressEventB(++id);
            TestPhysicalReleaseEventB = new TestPhysicalReleaseEventB(++id);
        }

        private static TestEvents singleton = new TestEvents();
        public static TestEvents Constants => singleton;
    }

    public class TestSingleThrowSwitchA : SingleThrowSwitch { }

    public class TestDoubleThrowSwitchA : DoubleThrowSwitch { }

    public class TestFireEventA : FireEvent<TestSingleThrowSwitchA>
    {
        public TestFireEventA(int eventId) : base(eventId) { }
    }

    public class TestPressEventA : PressEvent<TestDoubleThrowSwitchA>
    {
        public TestPressEventA(int eventId) : base(eventId) { }

        public override ReleaseEvent<TestDoubleThrowSwitchA> OppositeReleaseEvent
            => TestEvents.Constants.TestReleaseEventA;
    }

    public class TestReleaseEventA : ReleaseEvent<TestDoubleThrowSwitchA>
    {
        public TestReleaseEventA(int eventId) : base(eventId) { }

        public override PressEvent<TestDoubleThrowSwitchA> OppositePressEvent
            => TestEvents.Constants.TestPressEventA;
    }

    public class TestPhysicalFireEventA : PhysicalFireEvent<TestSingleThrowSwitchA>
    {
        public TestPhysicalFireEventA(int eventId) : base(eventId) { }

        public override FireEvent<TestSingleThrowSwitchA> LogicalEquivalentFireEvent
            => TestEvents.Constants.TestFireEventA;
    }

    public class TestPhysicalPressEventA : PhysicalPressEvent<TestDoubleThrowSwitchA>
    {
        public TestPhysicalPressEventA(int eventId) : base(eventId) { }

        public override PressEvent<TestDoubleThrowSwitchA> LogicalEquivalentPressEvent
            => TestEvents.Constants.TestPressEventA;

        public override PhysicalReleaseEvent<TestDoubleThrowSwitchA> OppositePhysicalReleaseEvent
            => TestEvents.Constants.TestPhysicalReleaseEventA;
    }

    public class TestPhysicalReleaseEventA : PhysicalReleaseEvent<TestDoubleThrowSwitchA>
    {
        public TestPhysicalReleaseEventA(int eventId) : base(eventId) { }

        public override ReleaseEvent<TestDoubleThrowSwitchA> LogicalEquivalentReleaseEvent
            => TestEvents.Constants.TestReleaseEventA;

        public override PhysicalPressEvent<TestDoubleThrowSwitchA> OppositePhysicalPressEvent
            => TestEvents.Constants.TestPhysicalPressEventA;
    }

    public class TestSingleThrowSwitchB : SingleThrowSwitch { }

    public class TestDoubleThrowSwitchB : DoubleThrowSwitch { }

    public class TestFireEventB : FireEvent<TestSingleThrowSwitchB>
    {
        public TestFireEventB(int eventId) : base(eventId) { }
    }

    public class TestPressEventB : PressEvent<TestDoubleThrowSwitchB>
    {
        public TestPressEventB(int eventId) : base(eventId) { }

        public override ReleaseEvent<TestDoubleThrowSwitchB> OppositeReleaseEvent
            => TestEvents.Constants.TestReleaseEventB;
    }

    public class TestReleaseEventB : ReleaseEvent<TestDoubleThrowSwitchB>
    {
        public TestReleaseEventB(int eventId) : base(eventId) { }

        public override PressEvent<TestDoubleThrowSwitchB> OppositePressEvent
            => TestEvents.Constants.TestPressEventB;
    }

    public class TestPhysicalFireEventB : PhysicalFireEvent<TestSingleThrowSwitchB>
    {
        public TestPhysicalFireEventB(int eventId) : base(eventId) { }

        public override FireEvent<TestSingleThrowSwitchB> LogicalEquivalentFireEvent
            => TestEvents.Constants.TestFireEventB;
    }

    public class TestPhysicalPressEventB : PhysicalPressEvent<TestDoubleThrowSwitchB>
    {
        public TestPhysicalPressEventB(int eventId) : base(eventId) { }

        public override PressEvent<TestDoubleThrowSwitchB> LogicalEquivalentPressEvent
            => TestEvents.Constants.TestPressEventB;

        public override PhysicalReleaseEvent<TestDoubleThrowSwitchB> OppositePhysicalReleaseEvent
            => TestEvents.Constants.TestPhysicalReleaseEventB;
    }

    public class TestPhysicalReleaseEventB : PhysicalReleaseEvent<TestDoubleThrowSwitchB>
    {
        public TestPhysicalReleaseEventB(int eventId) : base(eventId) { }

        public override ReleaseEvent<TestDoubleThrowSwitchB> LogicalEquivalentReleaseEvent
            => TestEvents.Constants.TestReleaseEventB;

        public override PhysicalPressEvent<TestDoubleThrowSwitchB> OppositePhysicalPressEvent
            => TestEvents.Constants.TestPhysicalPressEventB;
    }

    [TestClass]
    public class TypeSystemTest
    {
        [TestMethod]
        public void FireEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestFireEventA is FireEvent<TestSingleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestFireEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Constants.TestFireEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Constants.TestFireEventA.EventId, 1);
        }

        [TestMethod]
        public void PressEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestPressEventA is PressEvent<TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestPressEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Constants.TestPressEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Constants.TestPressEventA.OppositeReleaseEvent, TestEvents.Constants.TestReleaseEventA);
            Assert.AreEqual(TestEvents.Constants.TestPressEventA.EventId, 2);
        }

        [TestMethod]
        public void ReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestReleaseEventA is ReleaseEvent<TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestReleaseEventA is ILogicalEvent);
            Assert.IsTrue(TestEvents.Constants.TestReleaseEventA is IPhysicalEvent == false);
            Assert.AreEqual(TestEvents.Constants.TestReleaseEventA.OppositePressEvent, TestEvents.Constants.TestPressEventA);
            Assert.AreEqual(TestEvents.Constants.TestReleaseEventA.EventId, 3);
        }

        [TestMethod]
        public void PhysicalFireEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestPhysicalFireEventA is PhysicalFireEvent<TestSingleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalFireEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalFireEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Constants.TestPhysicalFireEventA.EventId, 4);
        }

        [TestMethod]
        public void PhysicalPressEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestPhysicalPressEventA is PhysicalPressEvent<TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalPressEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalPressEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Constants.TestPhysicalPressEventA.OppositePhysicalReleaseEvent, TestEvents.Constants.TestPhysicalReleaseEventA);
            Assert.AreEqual(TestEvents.Constants.TestPhysicalPressEventA.EventId, 5);
        }

        [TestMethod]
        public void PhysicalReleaseEventTest()
        {
            Assert.IsTrue(TestEvents.Constants.TestPhysicalReleaseEventA is PhysicalReleaseEvent<TestDoubleThrowSwitchA>);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalReleaseEventA is ILogicalEvent == false);
            Assert.IsTrue(TestEvents.Constants.TestPhysicalReleaseEventA is IPhysicalEvent);
            Assert.AreEqual(TestEvents.Constants.TestPhysicalReleaseEventA.OppositePhysicalPressEvent, TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(TestEvents.Constants.TestPhysicalReleaseEventA.EventId, 6);
        }
    }

    [TestClass]
    public class DSLSyntexTest
    {
        [TestMethod]
        public void RootWhenTest()
        {
            var root = new RootElement<EvaluationContext, ExecutionContext>();

            Assert.AreEqual(root.WhenElements.Count, 0);
            var w = root.When(ctx => { return true; });
            Assert.AreEqual(root.WhenElements.Count, 1);
        }

        [TestMethod]
        public void WhenOnSingleThrowTest()
        {
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.SingleThrowElements.Count, 0);
            var fire = when.On(TestEvents.Constants.TestFireEventA);
            Assert.AreEqual(when.SingleThrowElements.Count, 1);

            Assert.AreEqual(fire.DoExecutors.Count, 0);
            fire.Do(ctx => { });
            Assert.AreEqual(fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void WhenOnDoubleThrowTest()
        {
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DoubleThrowElements.Count, 0);
            var press = when.On(TestEvents.Constants.TestPressEventA);
            Assert.AreEqual(when.DoubleThrowElements.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowTest()
        {
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Constants.TestPressEventA);

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
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Constants.TestPressEventA);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.Constants.TestFireEventA);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowDoubleThrowTest()
        {
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Constants.TestPressEventA);
            var press_press = press.On(TestEvents.Constants.TestPressEventA);

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
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.Constants.TestPressEventA);

            Assert.AreEqual(press.StrokeElements.Count, 0);
            var press_stroke = press.On(StrokeEvent.Direction.Up);
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
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            Assert.AreEqual(s0.Machine, gm);
            Assert.AreEqual(s0.RootElement, root);
        }

        [TestMethod]
        public void CreateHistoryTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s1.History.Count, 1);
            Assert.AreEqual(s1.History[0].Item1, TestEvents.Constants.TestPhysicalReleaseEventA);
            Assert.AreEqual(s1.History[0].Item2, s0);
        }

        [TestMethod]
        public void DoesNotConsumeGivenInputWhenNoDefinition()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenPhysicalSingleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestFireEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalFireEventWhenLogicalSingleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPhysicalFireEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenPhysicalDoubleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPhysicalPressEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            { 
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.IsTrue(result.NextState is StateN<EvaluationContext, ExecutionContext>);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }

        [TestMethod]
        public void ConsumeGivenPhysicalPressEventWhenLogicalDoubleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.IsTrue(result.NextState is StateN<EvaluationContext, ExecutionContext>);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }
        [TestMethod]
        public void CreateHistory()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.CreateHistory(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0].Item1, TestEvents.Constants.TestPhysicalReleaseEventA);
            Assert.AreEqual(result[0].Item2, s0);
        }


        [TestMethod]
        public void GetActiveDoubleThrowElementsTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var evalContext = gm.CreateEvaluateContext();
            var result = s0.GetActiveDoubleThrowElements(evalContext, TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], when.DoubleThrowElements[0]);
        }

        [TestMethod]
        public void GetActiveSingleThrowElementsTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestFireEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var evalContext = gm.CreateEvaluateContext();
            var result = s0.GetActiveSingleThrowElements(evalContext, TestEvents.Constants.TestPhysicalFireEventA);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], when.SingleThrowElements[0]);
        }

        [TestMethod]
        public void SingleThrowTriggersTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestFireEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.SingleThrowTriggers;
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Contains(when.SingleThrowElements[0].Trigger), true);
        }

        [TestMethod]
        public void DoubleThrowTriggersTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.DoubleThrowTriggers;
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result.Contains(when.DoubleThrowElements[0].Trigger), true);
        }
        
        // todo move to GestureMachine
        [TestMethod]
        public void ConsumeGivenPhysicalReleaseEventWhenItInIgnoreList()
        {
            var gm = new TestGestureMachine();
            gm.IgnoreNext(TestEvents.Constants.TestPhysicalReleaseEventA);
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
        }
    }

    [TestClass]
    public class StateNTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var gm = new TestGestureMachine();
            var evalContext = gm.CreateEvaluateContext();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var history = s0.CreateHistory(TestEvents.Constants.TestPhysicalPressEventA);
            var dt = new List<DoubleThrowElement<ExecutionContext>>();
            {
                var s1 = new StateN<EvaluationContext, ExecutionContext>(gm, evalContext, history, dt);
                Assert.AreEqual(s1.Machine, gm);
                Assert.AreEqual(s1.EvaluationContext, evalContext);
                Assert.AreEqual(s1.History, history);
                Assert.AreEqual(s1.DoubleThrowElements, dt);
                Assert.AreEqual(s1.CancelAllowed, true);
            }
            {
                var s1 = new StateN<EvaluationContext, ExecutionContext>(gm, evalContext, history, dt, allowCancel: false);
                Assert.AreEqual(s1.Machine, gm);
                Assert.AreEqual(s1.EvaluationContext, evalContext);
                Assert.AreEqual(s1.History, history);
                Assert.AreEqual(s1.DoubleThrowElements, dt);
                Assert.AreEqual(s1.CancelAllowed, false);
            }
        }

        [TestMethod]
        public void CreateHistoryTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s1.History.Count, 1);
            Assert.AreEqual(s1.History[0].Item1, TestEvents.Constants.TestPhysicalReleaseEventA);
            Assert.AreEqual(s1.History[0].Item2, s0);
            
            var res1 = s1.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res1.Event.IsConsumed, true);
            Assert.AreEqual(res1.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            var s2 = res1.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s2.History.Count, 2);
            Assert.AreEqual(s2.History[1].Item1, TestEvents.Constants.TestPhysicalReleaseEventB);
            Assert.AreEqual(s2.History[1].Item2, s1);

            var res2 = s2.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res2.Event.IsConsumed, true);
            Assert.AreEqual(res2.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            var s3 = res2.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s3.History.Count, 3);
            Assert.AreEqual(s3.History[2].Item1, TestEvents.Constants.TestPhysicalReleaseEventA);
            Assert.AreEqual(s3.History[2].Item2, s2);

            var res3 = s3.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res3.Event.IsConsumed, true);
            Assert.AreEqual(res3.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            var s4 = res3.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s4.History.Count, 4);
            Assert.AreEqual(s4.History[3].Item1, TestEvents.Constants.TestPhysicalReleaseEventB);
            Assert.AreEqual(s4.History[3].Item2, s3);
        }

        [TestMethod]
        public void AbnormalEndTriggersTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res1 = s1.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res1.Event.IsConsumed, true);
            Assert.AreEqual(res1.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s1.AbnormalEndTriggers.Count, 0);

            var s2 = res1.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res2 = s2.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res2.Event.IsConsumed, true);
            Assert.AreEqual(res2.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s2.AbnormalEndTriggers.Count, 1);
            Assert.AreEqual(s2.AbnormalEndTriggers.Contains(TestEvents.Constants.TestPhysicalReleaseEventA), true);

            var s3 = res2.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res3 = s3.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res3.Event.IsConsumed, true);
            Assert.AreEqual(res3.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s3.AbnormalEndTriggers.Count, 2);
            Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Constants.TestPhysicalReleaseEventA), true);
            Assert.AreEqual(s3.AbnormalEndTriggers.Contains(TestEvents.Constants.TestPhysicalReleaseEventB), true);

            var s4 = res3.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res4 = s4.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res4.Event.IsConsumed, true);
            Assert.AreEqual(res4.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s4.AbnormalEndTriggers.Count, 2);
            Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Constants.TestPhysicalReleaseEventA), true);
            Assert.AreEqual(s4.AbnormalEndTriggers.Contains(TestEvents.Constants.TestPhysicalReleaseEventB), true);
        }

        [TestMethod]
        public void FindStateFromHistoryTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .On(TestEvents.Constants.TestPressEventA)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res1 = s1.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res1.Event.IsConsumed, true);
            Assert.AreEqual(res1.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s1.History.Count, 1);

            var s2 = res1.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res2 = s2.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res2.Event.IsConsumed, true);
            Assert.AreEqual(res2.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s2.History.Count, 2);

            var s3 = res2.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res3 = s3.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res3.Event.IsConsumed, true);
            Assert.AreEqual(res3.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s3.History.Count, 3);

            var (foundState, skippedReleaseEvents) = s3.FindStateFromHistory(TestEvents.Constants.TestPhysicalReleaseEventB);
            Assert.AreEqual(foundState, s1);
            Assert.AreEqual(skippedReleaseEvents.Count, 2);
            Assert.AreEqual(skippedReleaseEvents[0], TestEvents.Constants.TestPhysicalReleaseEventB);
            Assert.AreEqual(skippedReleaseEvents[1], TestEvents.Constants.TestPhysicalReleaseEventA);
        }

        [TestMethod]
        public void NormalEndTriggerTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res1 = s1.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res1.Event.IsConsumed, true);
            Assert.AreEqual(res1.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s1.History.Count, 1);
            Assert.AreEqual(s1.NormalEndTrigger, TestEvents.Constants.TestPhysicalReleaseEventA);
        }

        [TestMethod]
        public void IsNormalEndTriggerTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .On(TestEvents.Constants.TestPressEventB)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var res1 = s1.Input(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(res1.Event.IsConsumed, true);
            Assert.AreEqual(res1.NextState is StateN<EvaluationContext, ExecutionContext>, true);
            Assert.AreEqual(s1.History.Count, 1);
            Assert.AreEqual(s1.IsNormalEndTrigger(TestEvents.Constants.TestPhysicalReleaseEventA), true);
        }

        [TestMethod]
        public void LastStateTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
             .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            Assert.AreEqual(res0.Event.IsConsumed, true);
            Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            Assert.AreEqual(s1.History.Count, 1);
            Assert.AreEqual(s1.LastState, s0);
        }

        [TestMethod]
        public void HasPressExecutorsTest()
        {
            var gm = new TestGestureMachine();
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Do((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasPressExecutors, false);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Press((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasPressExecutors, true);
            }
        }

        [TestMethod]
        public void HasDoExecutorsTest()
        {
            var gm = new TestGestureMachine();
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Press((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasDoExecutors, false);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Do((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasDoExecutors, true);
            }
        }

        [TestMethod]
        public void HasReleaseExecutorsTest()
        {
            var gm = new TestGestureMachine();
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Do((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasReleaseExecutors, false);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Release((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.HasReleaseExecutors, true);
            }
        }

        [TestMethod]
        public void ShouldFinalizeTest()
        {
            var gm = new TestGestureMachine();
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Do((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.ShouldFinalize, false);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Release((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.ShouldFinalize, true);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Press((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.ShouldFinalize, true);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Release((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.ShouldFinalize, true);
            }
            {
                var root = new RootElement<EvaluationContext, ExecutionContext>();
                var when = root.When((ctx) => { return true; });
                when.On(TestEvents.Constants.TestPressEventA)
                 .Press((ctx) => { })
                 .Release((ctx) => { });
                var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
                var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(res0.Event.IsConsumed, true);
                Assert.AreEqual(res0.NextState is StateN<EvaluationContext, ExecutionContext>, true);

                var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
                Assert.AreEqual(s1.ShouldFinalize, true);
            }
        }

        [TestMethod]
        public void GetDoubleThrowElementsTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .On(TestEvents.Constants.TestPressEventB)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var evalContext = gm.CreateEvaluateContext();
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var result = s1.GetDoubleThrowElements(TestEvents.Constants.TestPhysicalPressEventB);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], when.DoubleThrowElements[0].DoubleThrowElements[0]);
        }

        [TestMethod]
        public void GetSingleThrowElementsTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .On(TestEvents.Constants.TestFireEventA)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var evalContext = gm.CreateEvaluateContext();
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var result = s1.GetSingleThrowElements(TestEvents.Constants.TestPhysicalFireEventA);
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], when.DoubleThrowElements[0].SingleThrowElements[0]);
        }

        [TestMethod]
        public void GetStrokeElementsTest()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEventA)
                .On(StrokeEvent.Direction.Up)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var evalContext = gm.CreateEvaluateContext();
            var res0 = s0.Input(TestEvents.Constants.TestPhysicalPressEventA);
            var s1 = res0.NextState as StateN<EvaluationContext, ExecutionContext>;
            var result = s1.GetStrokeElements(new List<StrokeEvent.Direction>() { StrokeEvent.Direction.Up });
            Assert.AreEqual(result.Count, 1);
            Assert.AreEqual(result[0], when.DoubleThrowElements[0].StrokeElements[0]);
        }

        // todo move to GestureMachine
        [TestMethod]
        public void ConsumeGivenPhysicalReleaseEventWhenItInIgnoreList()
        {
            var gm = new TestGestureMachine();
            gm.IgnoreNext(TestEvents.Constants.TestPhysicalReleaseEventA);
            var evalContext = gm.CreateEvaluateContext();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var history = s0.CreateHistory(TestEvents.Constants.TestPhysicalPressEventA);
            var dt = new List<DoubleThrowElement<ExecutionContext>>();
            var s1 = new StateN<EvaluationContext, ExecutionContext>(gm, evalContext, history, dt);

            {
                var result = s1.Input(TestEvents.Constants.TestPhysicalFireEventA);
                Assert.AreEqual(result.NextState, s1);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s1.Input(TestEvents.Constants.TestPhysicalPressEventA);
                Assert.AreEqual(result.NextState, s1);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s1.Input(TestEvents.Constants.TestPhysicalReleaseEventA);
                Assert.AreEqual(result.NextState, s1);
                Assert.AreEqual(result.Event.IsConsumed, true);
            }
        }
    }
}

