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
        public void WhenOnSingleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });

            Assert.AreEqual(when.SingleThrowElements.Count, 0);
            var fire = when.On(TestEvents.LogicalSingleThrowKeys[0].FireEvent);
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
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);
            Assert.AreEqual(when.DoubleThrowElements.Count, 1);
        }

        [TestMethod]
        public void DoubleThrowTest()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);

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
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);

            Assert.AreEqual(press.SingleThrowElements.Count, 0);
            var press_fire = press.On(TestEvents.LogicalSingleThrowKeys[0].FireEvent);
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
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);
            var press_press = press.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);

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
            var press = when.On(TestEvents.LogicalDoubleThrowKeys[0].PressEvent);

            Assert.AreEqual(press.StrokeElements.Count, 0);
            var press_stroke = press.On(StrokeDirection.Up);
            Assert.AreEqual(press.StrokeElements.Count, 1);

            Assert.AreEqual(press_stroke.DoExecutors.Count, 0);
            press_stroke.Do(ctx => { });
            Assert.AreEqual(press_stroke.DoExecutors.Count, 1);
        }
    }
}
