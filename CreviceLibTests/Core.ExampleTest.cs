using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using Crevice.Core.Example;
    using Crevice.Core.Events;
    using Crevice.Core.Keys;

    [TestClass]
    public class SimpleKeySetATest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var ks = new SimpleKeySetA(1);
            Assert.AreEqual(ks is PhysicalDoubleThrowKeySet, true);
            var key0 = ks[0];
            Assert.AreEqual(ks[0] is PhysicalDoubleThrowKey, true);
            Assert.AreEqual(ks[0].PressEvent is PressEvent, true);
            Assert.AreEqual(ks[0].ReleaseEvent is ReleaseEvent, true);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void InvalidIndexTest()
        {
            var ks = new SimpleKeySetB(1);
            var key1 = ks[1];
        }
    }

    [TestClass]
    public class SimpleKeySetBTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var ks = new SimpleKeySetB(1);
            Assert.AreEqual(ks is PhysicalSingleThrowKeySet, true);
            var key0 = ks[0];
            Assert.AreEqual(ks[0] is PhysicalSingleThrowKey, true);
            Assert.AreEqual(ks[0].FireEvent is FireEvent, true);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void InvalidIndexTest()
        {
            var ks = new SimpleKeySetA(1);
            var key1 = ks[1];
        }
    }

    [TestClass]
    public class SimpleGestureMachineConfigTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var config = new SimpleGestureMachineConfig();
            Assert.AreEqual(config is Crevice.Core.FSM.GestureMachineConfig, true);
        }
    }

    [TestClass]
    public class SimpleRootElementTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var elm = new SimpleRootElement();
            Assert.AreEqual(elm is Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>, true);
        }
    }

    [TestClass]
    public class SimpleContextManagerTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var cm = new SimpleContextManager();
            Assert.AreEqual(cm is Crevice.Core.Context.ContextManager<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>, true);
        }

        [TestMethod]
        public void CreateEvaluateContextTest()
        {
            var cm = new SimpleContextManager();
            var res0 = cm.CreateEvaluateContext();
            Assert.AreEqual(res0 is Crevice.Core.Context.EvaluationContext, true);
        }

        [TestMethod]
        public void CreateExecutionContextTest()
        {
            var cm = new SimpleContextManager();
            var res0 = cm.CreateEvaluateContext();
            var res1 = cm.CreateExecutionContext(res0);
            Assert.AreEqual(res1 is Crevice.Core.Context.ExecutionContext, true);
        }
    }

    [TestClass]
    public class SimpleCallbackManagerTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var cm = new SimpleCallbackManager();

            Assert.AreEqual(cm is Crevice.Core.Callback.CallbackManager<SimpleGestureMachineConfig, SimpleContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>, true);
        }
    }

    [TestClass]
    public class SimpleGestureMachineTest
    {
        [TestMethod]
        public void ConstractorTest()
        {
            var gm = new SimpleGestureMachine();
            Assert.AreEqual(gm is Crevice.Core.FSM.GestureMachine<SimpleGestureMachineConfig, SimpleContextManager, Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>, true);
        }
    }
}
