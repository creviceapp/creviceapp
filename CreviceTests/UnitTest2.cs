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
            Assert.IsTrue(Events.Constants.Move is FireEvent<MoveSwich>);
            Assert.IsTrue(Events.Constants.Move is ILogicalEvent);
            Assert.IsTrue(Events.Constants.Move is IPhysicalEvent == false);
            Assert.AreEqual(Events.Constants.Move.EventId, 1);

            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is PressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is ILogicalEvent);
            Assert.IsTrue(Events.Constants.LeftButtonDownEvent is IPhysicalEvent == false);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.OppositeReleaseEvent, Events.Constants.LeftButtonUpEvent);
            Assert.AreEqual(Events.Constants.LeftButtonDownEvent.EventId, 2);

            Assert.IsTrue(Events.Constants.PhysicalLeftButtonDownEvent is PhysicalPressEvent<LeftButtonSwitch>);
            Assert.IsTrue(Events.Constants.PhysicalLeftButtonDownEvent is ILogicalEvent == false);
            Assert.IsTrue(Events.Constants.PhysicalLeftButtonDownEvent is IPhysicalEvent);
            Assert.AreEqual(Events.Constants.PhysicalLeftButtonDownEvent.OppositePhysicalReleaseEvent, Events.Constants.PhysicalLeftButtonUpEvent);
            Assert.AreEqual(Events.Constants.PhysicalLeftButtonDownEvent.EventId, 100);
        }

        [TestMethod]
        public void TestDSLSyntex()
        {
            var r = new RootElement<EvaluationContext, ExecutionContext>();

            Assert.AreEqual(r.WhenElements.Count, 0);
            var w = r.When(ctx => { return true; });
            Assert.AreEqual(r.WhenElements.Count, 1);
            
            {
                Assert.AreEqual(w.SingleThrowElements.Count, 0);
                var f = w.On(Events.Constants.WheelDownEvent);
                Assert.AreEqual(w.SingleThrowElements.Count, 1);

                {
                    Assert.AreEqual(f.DoExecutors.Count, 0);
                    f.Do(ctx => { });
                    Assert.AreEqual(f.DoExecutors.Count, 1);
                }
            }

            {
                Assert.AreEqual(w.DoubleThrowElements.Count, 0);
                var p = w.On(Events.Constants.LeftButtonDownEvent);
                Assert.AreEqual(w.DoubleThrowElements.Count, 1);

                {
                    Assert.AreEqual(p.SingleThrowElements.Count, 0);
                    var f = p.On(Events.Constants.WheelDownEvent);
                    Assert.AreEqual(p.SingleThrowElements.Count, 1);

                    {
                        Assert.AreEqual(f.DoExecutors.Count, 0);
                        f.Do(ctx => { });
                        Assert.AreEqual(f.DoExecutors.Count, 1);
                    }
                }

                { 
                    Assert.AreEqual(p.PressExecutors.Count, 0);
                    p.Press(ctx => { });
                    Assert.AreEqual(p.PressExecutors.Count, 1);

                    Assert.AreEqual(p.DoExecutors.Count, 0);
                    p.Do(ctx => { });
                    Assert.AreEqual(p.DoExecutors.Count, 1);

                    Assert.AreEqual(p.ReleaseExecutors.Count, 0);
                    p.Release(ctx => { });
                    Assert.AreEqual(p.ReleaseExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.StrokeElements.Count, 0);
                    var s = p.On(StrokeEvent.Direction.Up);
                    Assert.AreEqual(p.StrokeElements.Count, 1);

                    Assert.AreEqual(s.DoExecutors.Count, 0);
                    s.Do(ctx => { });
                    Assert.AreEqual(s.DoExecutors.Count, 1);
                }

                {
                    Assert.AreEqual(p.DoubleThrowElements.Count, 0);
                    var pp = p.On(Events.Constants.LeftButtonDownEvent);
                    Assert.AreEqual(p.DoubleThrowElements.Count, 1);

                    {
                        Assert.AreEqual(pp.SingleThrowElements.Count, 0);
                        var f = pp.On(Events.Constants.WheelDownEvent);
                        Assert.AreEqual(pp.SingleThrowElements.Count, 1);

                        {
                            Assert.AreEqual(f.DoExecutors.Count, 0);
                            f.Do(ctx => { });
                            Assert.AreEqual(f.DoExecutors.Count, 1);
                        }
                    }

                    {
                        Assert.AreEqual(pp.PressExecutors.Count, 0);
                        pp.Press(ctx => { });
                        Assert.AreEqual(pp.PressExecutors.Count, 1);

                        Assert.AreEqual(pp.DoExecutors.Count, 0);
                        pp.Do(ctx => { });
                        Assert.AreEqual(pp.DoExecutors.Count, 1);

                        Assert.AreEqual(pp.ReleaseExecutors.Count, 0);
                        pp.Release(ctx => { });
                        Assert.AreEqual(pp.ReleaseExecutors.Count, 1);
                    }

                    {
                        Assert.AreEqual(pp.StrokeElements.Count, 0);
                        var s = pp.On(StrokeEvent.Direction.Up);
                        Assert.AreEqual(pp.StrokeElements.Count, 1);

                        Assert.AreEqual(s.DoExecutors.Count, 0);
                        s.Do(ctx => { });
                        Assert.AreEqual(s.DoExecutors.Count, 1);
                    }
                }

                
            }
        }
        
        class TestGestureMachine : GestureMachine<EvaluationContext, ExecutionContext>
        {
            public override EvaluationContext CreateEvaluateContext()
            {
                return new EvaluationContext();
            }

            public override ExecutionContext CreateExecutionContext(EvaluationContext evaluationContext)
            {
                return new ExecutionContext();
            }
        }

        public class TestEvents
        {
            public readonly TestFireEvent TestFireEvent;
            public readonly TestPressEvent TestPressEvent;
            public readonly TestReleaseEvent TestReleaseEvent;
            public readonly TestPhysicalFireEvent TestPhysicalFireEvent;
            public readonly TestPhysicalPressEvent TestPhysicalPressEvent;
            public readonly TestPhysicalReleaseEvent TestPhysicalReleaseEvent;
            public TestEvents()
            {
                var id = 0;
                TestFireEvent = new TestFireEvent(++id);
                TestPressEvent = new TestPressEvent(++id);
                TestReleaseEvent = new TestReleaseEvent(++id);
                TestPhysicalFireEvent = new TestPhysicalFireEvent(++id);
                TestPhysicalPressEvent = new TestPhysicalPressEvent(++id);
                TestPhysicalReleaseEvent = new TestPhysicalReleaseEvent(++id);
            }

            private static TestEvents singleton = new TestEvents();
            public static TestEvents Constants
            {
                get { return singleton; }
            }
        }

        public class TestSwitchA : SingleThrowSwitch { }

        public class TestSwitchB : DoubleThrowSwitch { }

        public class TestFireEvent : FireEvent<TestSwitchA>
        {
            public TestFireEvent(int eventId) : base(eventId) { }
        }

        public class TestPressEvent : PressEvent<TestSwitchB>
        {
            public TestPressEvent(int eventId) : base(eventId) { }

            public override ReleaseEvent<TestSwitchB> OppositeReleaseEvent 
                => TestEvents.Constants.TestReleaseEvent;
        }

        public class TestReleaseEvent : ReleaseEvent<TestSwitchB>
        {
            public TestReleaseEvent(int eventId) : base(eventId) { }

            public override PressEvent<TestSwitchB> OppositePressEvent
                => TestEvents.Constants.TestPressEvent;
        }

        public class TestPhysicalFireEvent : PhysicalFireEvent<TestSwitchA>
        {
            public TestPhysicalFireEvent(int eventId) : base(eventId) { }

            public override FireEvent<TestSwitchA> LogicalEquivalentFireEvent
                => TestEvents.Constants.TestFireEvent;
        }

        public class TestPhysicalPressEvent : PhysicalPressEvent<TestSwitchB>
        {
            public TestPhysicalPressEvent(int eventId) : base(eventId) { }

            public override PressEvent<TestSwitchB> LogicalEquivalentPressEvent
                => TestEvents.Constants.TestPressEvent;

            public override PhysicalReleaseEvent<TestSwitchB> OppositePhysicalReleaseEvent
                => TestEvents.Constants.TestPhysicalReleaseEvent;
        }

        public class TestPhysicalReleaseEvent : PhysicalReleaseEvent<TestSwitchB>
        {
            public TestPhysicalReleaseEvent(int eventId) : base(eventId) { }
            
            public override ReleaseEvent<TestSwitchB> LogicalEquivalentReleaseEvent
                => TestEvents.Constants.TestReleaseEvent;

            public override PhysicalPressEvent<TestSwitchB> OppositePhysicalPressEvent
                => TestEvents.Constants.TestPhysicalPressEvent;
        }

        [TestMethod]
        public void TestState0_DoesNotConsumeGivenInputWhenNoDefinition()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalFireEvent);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalPressEvent);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
            {
                var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEvent);
                Assert.AreEqual(result.NextState, s0);
                Assert.AreEqual(result.Event.IsConsumed, false);
            }
        }

        [TestMethod]
        public void TestState0_ConsumeGivenInputWhenPhysicalSingleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestFireEvent)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.Input(TestEvents.Constants.TestPhysicalFireEvent);
            Assert.AreEqual(result.NextState, s0);
            Assert.AreEqual(result.Event.IsConsumed, true);
        }

        [TestMethod]
        public void TestState0_ConsumeGivenInputWhenLogicalSingleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPhysicalFireEvent)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.Input(TestEvents.Constants.TestPhysicalFireEvent);
            Assert.AreEqual(result.NextState, s0);
            Assert.AreEqual(result.Event.IsConsumed, true);
        }

        [TestMethod]
        public void TestState0_ConsumeGivenInputWhenPhysicalDoubleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPhysicalPressEvent)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.Input(TestEvents.Constants.TestPhysicalPressEvent);
            Assert.IsTrue(result.NextState is StateN<EvaluationContext, ExecutionContext>);
            Assert.AreEqual(result.Event.IsConsumed, true);
        }

        [TestMethod]
        public void TestState0_ConsumeGivenInputWhenLogicalDoubleThrowDefinisitonExists()
        {
            var gm = new TestGestureMachine();
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPressEvent)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.Input(TestEvents.Constants.TestPhysicalPressEvent);
            Assert.IsTrue(result.NextState is StateN<EvaluationContext, ExecutionContext>);
            Assert.AreEqual(result.Event.IsConsumed, true);
        }

        [TestMethod]
        public void TestState0_ConsumeGivenInputWhenItInIgnoreList()
        {
            var gm = new TestGestureMachine();
            gm.IgnoreNext(TestEvents.Constants.TestPhysicalReleaseEvent);
            var root = new RootElement<EvaluationContext, ExecutionContext>();
            var when = root.When((ctx) => { return true; });
            when.On(TestEvents.Constants.TestPhysicalPressEvent)
                .Do((ctx) => { });
            var s0 = new State0<EvaluationContext, ExecutionContext>(gm, root);
            var result = s0.Input(TestEvents.Constants.TestPhysicalReleaseEvent);
            Assert.AreEqual(result.NextState, s0);
            Assert.AreEqual(result.Event.IsConsumed, true);
        }


    }
}
