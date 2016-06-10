using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL.Tests
{
    [TestClass()]
    public class AppElementTests
    {
        [TestMethod()]
        public void onTest()
        {
            var root = new Root();
            var appElement = root.@when(() => true);
            Assert.AreEqual(root.whenElements[0].onElements.Count, 0);
            appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements.Count, 1);
        }

        [TestMethod()]
        public void funcTest()
        {
            var root = new Root();
            var appElement = root.when(() => true);
            Assert.IsTrue(root.whenElements[0].func());
        }

    }
}