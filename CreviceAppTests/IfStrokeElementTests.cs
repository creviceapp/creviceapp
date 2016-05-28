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
    public class IfStrokeElementTests
    {
        [TestMethod()]
        public void doTest()
        {
            var root = new Root();
            var appElement = root.App(x => true);
            var onElement = appElement.@on(Button.RightButton);
            var ifElement = onElement.@if(Move.MoveDown, Move.MoveRight);
            Assert.AreEqual(root.appElements[0].onElements[0].ifStrokeElements[0].doElements.Count, 0);
            ifElement.@do(x => { });
            Assert.AreEqual(root.appElements[0].onElements[0].ifStrokeElements[0].doElements.Count, 1);
        }
    }
}