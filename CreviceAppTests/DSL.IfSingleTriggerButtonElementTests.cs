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
    public class IfSingleTriggerButtonElementTests
    {
        [TestMethod()]
        public void doTest()
        {
            var root = new Root();
            var appElement = root.@when(_ => true);
            var ifElement = appElement.@if(new Def.WheelUp());
            Assert.AreEqual(root.whenElements[0].ifSingleTriggerButtonElements[0].doElements.Count, 0);
            ifElement.@do(_ => { });
            Assert.AreEqual(root.whenElements[0].ifSingleTriggerButtonElements[0].doElements.Count, 1);
        }
    }
}