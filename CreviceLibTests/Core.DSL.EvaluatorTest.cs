using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CreviceLibTests
{
    using TestRootElement = Crevice.Core.DSL.RootElement<Crevice.Core.Context.EvaluationContext, Crevice.Core.Context.ExecutionContext>;

    [TestClass]
    public class DSLEvaluatorTest
    {
        [TestMethod]
        public void WhenDescriptionTest0()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; });
            Assert.AreEqual(when.WhenEvaluator.Description, "");
        }

        [TestMethod]
        public void WhenDescriptionTest1()
        {
            var root = new TestRootElement();
            var when = root.When(ctx => { return true; }, description: "foo");
            Assert.AreEqual(when.WhenEvaluator.Description, "foo");
        }
    }
}
