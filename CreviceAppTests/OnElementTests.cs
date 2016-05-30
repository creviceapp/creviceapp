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
    public class OnElementTests
    {
        [TestMethod()]
        public void ifButtonTest()
        {
            var root = new Root();
            var appElement = root.App(x => true);
            var onElement = appElement.@on(Button.RightButton);
            Assert.AreEqual(root.whenElements[0].onElements[0].ifButtonElements.Count, 0);
            onElement.@if(Button.WheelUpButton);
            Assert.AreEqual(root.whenElements[0].onElements[0].ifButtonElements.Count, 1);
        }

        [TestMethod()]
        public void ifStrokeTest()
        {
            var root = new Root();
            var appElement = root.App(x => true);
            var onElement = appElement.@on(Button.RightButton);
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 0);
            onElement.@if(Move.MoveDown, Move.MoveRight);
            Assert.AreEqual(root.whenElements[0].onElements[0].ifStrokeElements.Count, 1);
        }
    }
}