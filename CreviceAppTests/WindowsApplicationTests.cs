using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp.Tests
{
    [TestClass()]
    public class WindowsApplicationTests
    {
        [TestMethod()]
        public void GetForegroundTest()
        {
            var app = new WindowsApplication();
            var result = app.GetForeground();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetOnCursorTest()
        {
            var app = new WindowsApplication();
            var result = app.GetOnCursor(0, 0);
            Assert.IsNotNull(result);
        }
    }
}