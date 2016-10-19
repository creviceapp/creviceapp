using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.User;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL.Tests
{
    [TestClass()]
    public class DoubleTriggerBeforeElementTests
    {
        [TestMethod()]
        public void funcTest()
        {
            var ctx = new Core.UserActionExecutionContext(new Point());
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            var called = false;
            var doElement = ifElement.@do(_ => { called = true; });
            Assert.IsFalse(called);
            root.whenElements[0].ifDoubleTriggerButtonElements[0].doElements[0].func(ctx);
            Assert.IsTrue(called);
        }

        [TestMethod()]
        public void doTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            ifElement.
                @before(_ => { }).
                @do(_ => { });
        }
        
        [TestMethod()]
        public void afterTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            ifElement.
                @before(_ => { }).
                @after(_ => { });
        }
    }
}