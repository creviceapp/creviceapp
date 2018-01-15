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
    public class SingleTriggerDoElementTests
    {
        [TestMethod()]
        public void funcTest()
        {
            var ctx = new UserActionExecutionContext(new Point());
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.WheelDown());
            var called = false;
            var doElement = ifElement.@do(_ => { called = true; });
            Assert.IsFalse(called);
            root.whenElements[0].ifSingleTriggerButtonElements[0].doElements[0].func(ctx);
            Assert.IsTrue(called);
        }
    }
}