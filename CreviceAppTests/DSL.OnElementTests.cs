using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.DSL.Tests
{
    [TestClass()]
    public class OnElementTests
    {
        [TestMethod()]
        public void ifSingleTriggerButtonTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var onElement = appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifSingleTriggerButtonElements.Count, 0);
            onElement.@if(new Def.WheelUp());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifSingleTriggerButtonElements.Count, 1);
        }

        [TestMethod()]
        public void ifDoubleTriggerButtonTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var onElement = appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifDoubleTriggerButtonElements.Count, 0);
            onElement.@if(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifDoubleTriggerButtonElements.Count, 1);
        }

        [TestMethod()]
        public void ifStrokeTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var onElement = appElement.@on(new Def.RightButton());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 0);
            onElement.@if(new Def.MoveDown(), new Def.MoveRight());
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 1);
        }
    }
}