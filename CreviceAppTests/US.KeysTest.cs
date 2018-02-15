using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceAppTests
{
    using Crevice.Core.Keys;
    using Crevice.Core.Stroke;
    using Crevice.UserScript.Keys;

    [TestClass()]
    public class KeysTests
    {
        SupportedKeys.LogicalKeyDeclaration Keys => SupportedKeys.Keys;
        SupportedKeys.PhysicalKeyDeclaration PhysicalKeys => SupportedKeys.PhysicalKeys;

        [TestMethod()]
        public void WheelTest()
        {
            Assert.AreEqual(Keys.WheelUp is LogicalSingleThrowKey, true);
            Assert.AreEqual(Keys.WheelDown is LogicalSingleThrowKey, true);
            Assert.AreEqual(Keys.WheelLeft is LogicalSingleThrowKey, true);
            Assert.AreEqual(Keys.WheelRight is LogicalSingleThrowKey, true);
        }

        [TestMethod()]
        public void MoveTest()
        {
            Assert.AreEqual(Keys.MoveUp, StrokeDirection.Up);
            Assert.AreEqual(Keys.MoveDown, StrokeDirection.Down);
            Assert.AreEqual(Keys.MoveLeft, StrokeDirection.Left);
            Assert.AreEqual(Keys.MoveRight, StrokeDirection.Right);
        }

        [TestMethod()]
        public void KeyCodeTest()
        {
            Assert.AreEqual(Keys.KeyCode & Keys.None, Keys.None);
            Assert.AreEqual(Keys.KeyCode & Keys.LButton, Keys.LButton);
            Assert.AreEqual(Keys.KeyCode & Keys.OemClear, Keys.OemClear);
        }

        [TestMethod()]
        public void ModifiersTest()
        {
            Assert.AreEqual(Keys.Modifiers & Keys.None, 0);
            Assert.AreEqual(Keys.Modifiers & Keys.LButton, 0);
            Assert.AreEqual(Keys.Modifiers & Keys.OemClear, 0);

            Assert.AreEqual(Keys.Modifiers & Keys.Shift, Keys.Shift);
            Assert.AreEqual(Keys.Modifiers & Keys.Control, Keys.Control);
            Assert.AreEqual(Keys.Modifiers & Keys.Alt, Keys.Alt);
        }

    }
}
