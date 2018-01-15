using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Core.User.Tests
{
    [TestClass()]
    public class RootTests
    {
        [TestMethod()]
        public void AppTest()
        {
            var root = new DSL.Root();
            Assert.AreEqual(root.whenElements.Count, 0);
            var appElement = root.@when(_ => true);
            Assert.AreEqual(root.whenElements.Count, 1);
        }
    }
}