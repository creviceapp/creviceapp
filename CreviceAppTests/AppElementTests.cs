using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.GestureConfig.DSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.GestureConfig.DSL.Tests
{
    [TestClass()]
    public class AppElementTests
    {
        [TestMethod()]
        public void onTest()
        {
            var root = new Root();
            var appElement = root.App(x => true);
            Assert.AreEqual(root.appElements[0].onElements.Count, 0);
            appElement.@on(Button.RightButton);
            Assert.AreEqual(root.appElements[0].onElements.Count, 1);
        }

        [TestMethod()]
        public void funcTest()
        {
            var root = new Root();
            var appElement = root.App(x => true);
            Assert.IsTrue(root.appElements[0].func(new AppContext()));
        }

    }
}