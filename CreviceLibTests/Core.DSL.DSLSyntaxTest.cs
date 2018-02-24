using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

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
        public void WhenSingleThrow0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.SingleThrowElements.Count, 0);
            var fire = when.On(TestEvents.LogicalSingleThrowKeys[0]);
            Assert.AreEqual(when.SingleThrowElements.Count, 1);

            Assert.AreEqual(fire.DoExecutors.Count, 0);
            fire.Do(ctx => { });
            Assert.AreEqual(fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void WhenSingleThrow1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.SingleThrowElements.Count, 0);
            var fire = when.On(TestEvents.PhysicalSingleThrowKeys0[0]);
            Assert.AreEqual(when.SingleThrowElements.Count, 1);

            Assert.AreEqual(fire.DoExecutors.Count, 0);
            fire.Do(ctx => { });
            Assert.AreEqual(fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void WhenDoubleThrow0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DoubleThrowElements.Count, 0);
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            Assert.AreEqual(when.DoubleThrowElements.Count, 1);
        }

        [TestMethod]
        public void WhenDoubleThrow1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DoubleThrowElements.Count, 0);
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
            Assert.AreEqual(when.DoubleThrowElements.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDoubleThrowDecomposedError0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDoubleThrowDecomposedError1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDoubleThrowDecomposedError2Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
            when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDoubleThrowDecomposedError3Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
            when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);
        }

        [TestMethod]
        public void WhenDecomposed0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DecomposedElements.Count, 0);
            var press = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            Assert.AreEqual(when.DecomposedElements.Count, 1);
        }

        [TestMethod]
        public void WhenDecomposed1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.DecomposedElements.Count, 0);
            var press = when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);
            Assert.AreEqual(when.DecomposedElements.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDecomposedDoubleThrowError0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            when.On(TestEvents.LogicalDoubleThrowKeys[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDecomposedDoubleThrowError1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDecomposedDoubleThrowError2Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);
            when.On(TestEvents.LogicalDoubleThrowKeys[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WhenDecomposedDoubleThrowError3Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);
            when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
        }

        [TestMethod]
        public void DoubleThrow0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);

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
        public void DoubleThrow1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

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
        public void Decomposed0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);

            Assert.AreEqual(press.PressExecutors.Count, 0);
            press.Press(ctx => { });
            Assert.AreEqual(press.PressExecutors.Count, 1);

            Assert.AreEqual(press.ReleaseExecutors.Count, 0);
            press.Release(ctx => { });
            Assert.AreEqual(press.ReleaseExecutors.Count, 1);
        }

        [TestMethod]
        public void Decomposed1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.OnDecomposed(TestEvents.PhysicalDoubleThrowKeys0[0]);

            Assert.AreEqual(press.PressExecutors.Count, 0);
            press.Press(ctx => { });
            Assert.AreEqual(press.PressExecutors.Count, 1);

            Assert.AreEqual(press.ReleaseExecutors.Count, 0);
            press.Release(ctx => { });
            Assert.AreEqual(press.ReleaseExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowSingleThrow0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.LogicalSingleThrowKeys[0]);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowSingleThrow1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.PhysicalSingleThrowKeys0[0]);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowSingleThrow2Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.LogicalSingleThrowKeys[0]);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowSingleThrow3Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.PhysicalSingleThrowKeys0[0]);
            Assert.AreEqual(press.SingleThrowElements.Count, 1);

            Assert.AreEqual(press_fire.DoExecutors.Count, 0);
            press_fire.Do(ctx => { });
            Assert.AreEqual(press_fire.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowDoubleThrow0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            var press_press = press.On(TestEvents.LogicalDoubleThrowKeys[0]);

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
        public void DoubleThrowDoubleThrow1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            var press_press = press.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

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
        public void DoubleThrowDoubleThrow2Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
            var press_press = press.On(TestEvents.LogicalDoubleThrowKeys[0]);

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
        public void DoubleThrowDoubleThrow3Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);
            var press_press = press.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

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
        public void DoubleThrowStroke0Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0]);

            Assert.AreEqual(press.StrokeElements.Count, 0);
            var press_stroke = press.On(StrokeDirection.Up);
            Assert.AreEqual(press.StrokeElements.Count, 1);

            Assert.AreEqual(press_stroke.DoExecutors.Count, 0);
            press_stroke.Do(ctx => { });
            Assert.AreEqual(press_stroke.DoExecutors.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowStroke1Test()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.PhysicalDoubleThrowKeys0[0]);

            Assert.AreEqual(press.StrokeElements.Count, 0);
            var press_stroke = press.On(StrokeDirection.Up);
            Assert.AreEqual(press.StrokeElements.Count, 1);

            Assert.AreEqual(press_stroke.DoExecutors.Count, 0);
            press_stroke.Do(ctx => { });
            Assert.AreEqual(press_stroke.DoExecutors.Count, 1);
        }
    }
}
