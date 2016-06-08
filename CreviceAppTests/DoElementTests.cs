using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.DSL.Tests
{
    [TestClass()]
    public class DoElementTests
    {
        [TestMethod()]
        public void funcTest()
        {
            var root = new DSL.Root();
            var appElement = root.@when(() => true);
            var onElement = appElement.@on(new Def.RightButton());
            var ifElement = onElement.@if(new Def.MoveDown(), new Def.MoveRight());
            var called = false;
            var doEmenent = ifElement.@do(() => { called = true; });
            Assert.IsFalse(called);
            root.whenElements[0].onElements[0].ifStrokeElements[0].doElements[0].func();
            Assert.IsTrue(called);
        }
    }
}