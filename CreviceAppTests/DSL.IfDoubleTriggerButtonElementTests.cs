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
    public class IfDoubleTriggerButtonElementTests
    {
        [TestMethod()]
        public void beforeTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].beforeElements.Count, 0);
            ifElement.@before(_ => { });
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].beforeElements.Count, 1);
        }

        [TestMethod()]
        public void doTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].doElements.Count, 0);
            ifElement.@do(_ => { });
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].doElements.Count, 1);
        }

        [TestMethod()]
        public void afterTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].afterElements.Count, 0);
            ifElement.@after(_ => { });
            Assert.AreEqual(root.whenElements[0].ifDoubleTriggerButtonElements[0].afterElements.Count, 1);
        }        
    }
}