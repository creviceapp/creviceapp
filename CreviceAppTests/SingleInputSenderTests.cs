using Microsoft.VisualStudio.TestTools.UnitTesting;
using CreviceApp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp.Tests
{
    [TestClass()]
    public class SingleInputSenderTests
    {
        static SingleInputSender sender = new SingleInputSender();

        static List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>> mouseEvents = new List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>>();
        static LowLevelMouseHook mouseHook = new LowLevelMouseHook((evnt, data) => {
            if (data.fromCreviceApp)
            {
                mouseEvents.Add(Tuple.Create(evnt, data));
            }
            return LowLevelMouseHook.Result.Cancel;
        });

        static List<Tuple<LowLevelKeyboardHook.Event, LowLevelKeyboardHook.KBDLLHOOKSTRUCT>> keyboardEvents = new List<Tuple<LowLevelKeyboardHook.Event, LowLevelKeyboardHook.KBDLLHOOKSTRUCT>>();
        static LowLevelKeyboardHook keyboardHook = new LowLevelKeyboardHook((evnt, data) => {
            keyboardEvents.Add(Tuple.Create(evnt, data));
            return LowLevelKeyboardHook.Result.Cancel;
        });

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            mouseHook.SetHook();
            keyboardHook.SetHook();
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            mouseHook.Unhook();
            keyboardHook.Unhook();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            mouseEvents.Clear();
            keyboardEvents.Clear();
        }

        [TestMethod()]
        public void LeftDownTest()
        {
            sender.LeftDown();
            sender.LeftUp();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
        }

        [TestMethod()]
        public void LeftUpTest()
        {
            sender.LeftDown();
            sender.LeftUp();
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
        }

        [TestMethod()]
        public void LeftClickTest()
        {
            sender.LeftClick();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
        }

        [TestMethod()]
        public void RightDownTest()
        {
            sender.RightDown();
            sender.RightUp();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
        }

        [TestMethod()]
        public void RightUpTest()
        {
            sender.RightDown();
            sender.RightUp();
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        [TestMethod()]
        public void RightClickTest()
        {
            sender.RightClick();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        [TestMethod()]
        public void MoveTest()
        {
            var rand = new System.Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            var error_margin = 2;
            var start = Cursor.Position;
            var min_error = new Point(start.X - error_margin, start.Y - error_margin);
            var max_error = new Point(start.X + error_margin, start.Y + error_margin);
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var dx = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var dy = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                sender.Move(dx, dy);
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(min_error.X <= pos.x || pos.x <= max_error.X);
                Assert.IsTrue(min_error.Y <= pos.y || pos.y <= max_error.Y);
            }
        } 

        [TestMethod()]
        public void MoveToTest()
        {
            var rand = new System.Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            var error_margin = 2;
            var start = Cursor.Position;
            var min_error = new Point(start.X - error_margin, start.Y - error_margin);
            var max_error = new Point(start.X + error_margin, start.Y + error_margin);
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var x = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var y = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                sender.MoveTo(x, y);
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(min_error.X <= pos.x || pos.x <= max_error.X);
                Assert.IsTrue(min_error.Y <= pos.y || pos.y <= max_error.Y);
            }
        }

        [TestMethod()]
        public void MiddleDownTest()
        {
            sender.MiddleDown();
            sender.MiddleUp();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
        }

        [TestMethod()]
        public void MiddleUpTest()
        {
            sender.MiddleDown();
            sender.MiddleUp();
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
        }

        [TestMethod()]
        public void MiddleClickTest()
        {
            sender.MiddleClick();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
        }

        [TestMethod()]
        public void WheelTest()
        {
            sender.Wheel(120);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void WheelDownTest()
        {
            sender.WheelDown();
            Assert.AreEqual(keyboardEvents.Count, 0);
            Assert.AreEqual(mouseEvents.Count, 1);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void WheelUpTest()
        {
            sender.WheelUp();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, -120);
        }

        [TestMethod()]
        public void X1DownTest()
        {
            sender.X1Down();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
            sender.X1Up();
        }

        [TestMethod()]
        public void X1UpTest()
        {
            sender.X1Down();
            sender.X1Up();
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton1);
        }

        [TestMethod()]
        public void X1ClickTest()
        {
            sender.X1Click();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton1);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton1);
        }

        [TestMethod()]
        public void X2DownTest()
        {
            sender.X2Down();
            sender.X2Up();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
        }

        [TestMethod()]
        public void X2UpTest()
        {
            sender.X2Down();
            sender.X2Up();
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
        }

        [TestMethod()]
        public void X2ClickTest()
        {
            sender.X2Click();
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.isXButton2);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.isXButton2);
        }

        [TestMethod()]
        public void KeyDownTest()
        {
            sender.KeyDown(SingleInputSender.VirtualKeys.VK_A);
            sender.KeyUp(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x00U);
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyUpTest()
        {
            sender.KeyDown(SingleInputSender.VirtualKeys.VK_A);
            sender.KeyUp(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x00U);
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyDownTest()
        {
            sender.ExtendedKeyDown(SingleInputSender.VirtualKeys.VK_A);
            sender.ExtendedKeyUp(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x00U);
            Assert.IsTrue(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyUpTest()
        {
            sender.ExtendedKeyDown(SingleInputSender.VirtualKeys.VK_A);
            sender.ExtendedKeyUp(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x00U);
            Assert.IsTrue(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyDownWithScanCodeTest()
        {
            sender.KeyDownWithScanCode(SingleInputSender.VirtualKeys.VK_A);
            sender.KeyUpWithScanCode(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x1EU);
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyUpWithScanCodeTest()
        {
            sender.KeyDownWithScanCode(SingleInputSender.VirtualKeys.VK_A);
            sender.KeyUpWithScanCode(SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, SingleInputSender.VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x1EU);
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyDownWithScanCodeTest()
        {
            sender.ExtendedKeyDownWithScanCode(SingleInputSender.VirtualKeys.VK_LWIN);
            sender.ExtendedKeyUpWithScanCode(SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, (uint)SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.IsTrue(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyUpWithScanCodeTest()
        {
            sender.ExtendedKeyDownWithScanCode(SingleInputSender.VirtualKeys.VK_LWIN);
            sender.ExtendedKeyUpWithScanCode(SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, (uint)SingleInputSender.VirtualKeys.VK_LWIN);
            Assert.IsTrue(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyDownTest()
        {
            sender.UnicodeKeyDown('A');
            sender.UnicodeKeyUp('A');
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, 0xE7U);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyUpTest()
        {
            sender.UnicodeKeyDown('A');
            sender.UnicodeKeyUp('A');
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, 0xE7U);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyStrokeTest()
        {
            sender.UnicodeKeyStroke("A");
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, 0xE7U);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));

            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, 0xE7U);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }
    }
}