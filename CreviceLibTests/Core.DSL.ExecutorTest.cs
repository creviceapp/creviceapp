using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Stroke;

    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    [TestClass]
    public class ExecutorTest
    {
        [TestMethod]
        public void SingleThrowDoTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var st = when.On(TestEvents.LogicalSingleThrowKeys[0]);
            st.Do(ctx => { });
            Assert.AreEqual(st.DoExecutors.Count, 1);
            Assert.AreEqual(st.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(st.DoExecutors[0].Description, "");
        }

        [TestMethod]
        public void SingleThrowDoTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var st = when.On(TestEvents.LogicalSingleThrowKeys[0]);
            st.Do(ctx => { }, description: "foo");
            Assert.AreEqual(st.DoExecutors.Count, 1);
            Assert.AreEqual(st.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(st.DoExecutors[0].Description, "foo");
        }

        [TestMethod]
        public void DoubleThrowPressTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Press(ctx => { });
            Assert.AreEqual(dt.PressExecutors.Count, 1);
            Assert.AreEqual(dt.DoExecutors.Count, 0);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 0);
            Assert.AreEqual(dt.PressExecutors[0].Type, Crevice.Core.Context.ExecutorType.Press);
            Assert.AreEqual(dt.PressExecutors[0].Description, "");
        }

        [TestMethod]
        public void DoubleThrowPressTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Press(ctx => { }, description: "foo");
            Assert.AreEqual(dt.PressExecutors.Count, 1);
            Assert.AreEqual(dt.DoExecutors.Count, 0);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 0);
            Assert.AreEqual(dt.PressExecutors[0].Type, Crevice.Core.Context.ExecutorType.Press);
            Assert.AreEqual(dt.PressExecutors[0].Description, "foo");
        }


        [TestMethod]
        public void DoubleThrowDoTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Do(ctx => { });
            Assert.AreEqual(dt.PressExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors.Count, 1);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(dt.DoExecutors[0].Description, "");
        }

        [TestMethod]
        public void DoubleThrowDoTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Do(ctx => { }, description: "foo");
            Assert.AreEqual(dt.PressExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors.Count, 1);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(dt.DoExecutors[0].Description, "foo");
        }

        [TestMethod]
        public void DoubleThrowReleaseTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Release(ctx => { });
            Assert.AreEqual(dt.PressExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors.Count, 0);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 1);
            Assert.AreEqual(dt.ReleaseExecutors[0].Type, Crevice.Core.Context.ExecutorType.Release);
            Assert.AreEqual(dt.ReleaseExecutors[0].Description, "");
        }

        [TestMethod]
        public void DoubleThrowReleaseTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            dt.Release(ctx => { }, description: "foo");
            Assert.AreEqual(dt.PressExecutors.Count, 0);
            Assert.AreEqual(dt.DoExecutors.Count, 0);
            Assert.AreEqual(dt.ReleaseExecutors.Count, 1);
            Assert.AreEqual(dt.ReleaseExecutors[0].Type, Crevice.Core.Context.ExecutorType.Release);
            Assert.AreEqual(dt.ReleaseExecutors[0].Description, "foo");
        }

        [TestMethod]
        public void DecomposedPressTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var de = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            de.Press(ctx => { });
            Assert.AreEqual(de.PressExecutors.Count, 1);
            Assert.AreEqual(de.ReleaseExecutors.Count, 0);
            Assert.AreEqual(de.PressExecutors[0].Type, Crevice.Core.Context.ExecutorType.Press);
            Assert.AreEqual(de.PressExecutors[0].Description, "");
        }

        [TestMethod]
        public void DecomposedPressTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var de = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            de.Press(ctx => { }, description: "foo");
            Assert.AreEqual(de.PressExecutors.Count, 1);
            Assert.AreEqual(de.ReleaseExecutors.Count, 0);
            Assert.AreEqual(de.PressExecutors[0].Type, Crevice.Core.Context.ExecutorType.Press);
            Assert.AreEqual(de.PressExecutors[0].Description, "foo");
        }

        [TestMethod]
        public void DecomposedReleaseTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var de = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            de.Release(ctx => { });
            Assert.AreEqual(de.PressExecutors.Count, 0);
            Assert.AreEqual(de.ReleaseExecutors.Count, 1);
            Assert.AreEqual(de.ReleaseExecutors[0].Type, Crevice.Core.Context.ExecutorType.Release);
            Assert.AreEqual(de.ReleaseExecutors[0].Description, "");
        }

        [TestMethod]
        public void DecomposedReleaseTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var de = when.OnDecomposed(TestEvents.LogicalDoubleThrowKeys[0]);
            de.Release(ctx => { }, description: "foo");
            Assert.AreEqual(de.PressExecutors.Count, 0);
            Assert.AreEqual(de.ReleaseExecutors.Count, 1);
            Assert.AreEqual(de.ReleaseExecutors[0].Type, Crevice.Core.Context.ExecutorType.Release);
            Assert.AreEqual(de.ReleaseExecutors[0].Description, "foo");
        }

        [TestMethod]
        public void StrokeDoTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            var stroke = dt.On(StrokeDirection.Up);
            stroke.Do(ctx => { });
            Assert.AreEqual(stroke.DoExecutors.Count, 1);
            Assert.AreEqual(stroke.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(stroke.DoExecutors[0].Description, "");
        }

        [TestMethod]
        public void StrokeDoTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            var dt = when.On(TestEvents.LogicalDoubleThrowKeys[0]);
            var stroke = dt.On(StrokeDirection.Up);
            stroke.Do(ctx => { }, description: "foo");
            Assert.AreEqual(stroke.DoExecutors.Count, 1);
            Assert.AreEqual(stroke.DoExecutors[0].Type, Crevice.Core.Context.ExecutorType.Do);
            Assert.AreEqual(stroke.DoExecutors[0].Description, "foo");
        }
    }
}
