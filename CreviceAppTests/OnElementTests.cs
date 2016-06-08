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
    public class OnElementTests
    {
        [TestMethod()]
        public void ifButtonTest()
        {
            var root = new Root();
            var appElement = root.@when(() => true);
            var onElement = appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifButtonElements.Count, 0);
            onElement.@if(new Def.WheelUp());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifButtonElements.Count, 1);
        }

        [TestMethod()]
        public void ifStrokeTest()
        {
            var root = new Root();
            var appElement = root.@when(() => true);
            var onElement = appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 0);
            onElement.@if(new Def.MoveDown(), new Def.MoveRight());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 1);
        }
    }
}