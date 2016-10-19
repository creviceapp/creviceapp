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
    public class DoubleTriggerDoElementTests
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
        public void afterTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.RightButton());
            ifElement.
                @do(_ => { }).
                @after(_ => { });
        }
    }
}