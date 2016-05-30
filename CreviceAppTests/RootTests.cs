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
    public class RootTests
    {
        [TestMethod()]
        public void AppTest()
        {
            var root = new Root();
            Assert.AreEqual(root.whenElements.Count, 0);
            var appElement = root.App(x => true);
            Assert.AreEqual(root.whenElements.Count, 1);
        }
    }
}