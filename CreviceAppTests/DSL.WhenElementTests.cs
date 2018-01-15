using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.DSL.Tests
{
    [TestClass()]
    public class AppElementTests
    {
        [TestMethod()]
        public void onTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            Assert.AreEqual(root.whenElements[0].onElements.Count, 0);
            appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements.Count, 1);
        }

        [TestMethod()]
        public void ifSingleTriggerButtonTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            Assert.AreEqual(root.whenElements[0].ifSingleTriggerButtonElements.Count, 0);
            appElement.@if(new Def.WheelUp());
            Assert.AreEqual(root.whenElements[0].ifSingleTriggerButtonElements.Count, 1);
        }

        [TestMethod()]
        public void ifDoubleTriggerButtonTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements.Count, 0);
            appElement.@if(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements.Count, 1);
        }

        [TestMethod()]
        public void funcTest()
        {
            var ctx = new UserActionExecutionContext(new Point());
            var root = new Root();
            var appElement = root.when(_ => true);
            Assert.IsTrue(root.whenElements[0].func(ctx));
        }
    }
}