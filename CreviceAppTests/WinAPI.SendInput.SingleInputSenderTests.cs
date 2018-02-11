using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceTests
{
    using Crevice.WinAPI.WindowsHookEx;
    using Crevice.WinAPI.SendInput;
    using Crevice.WinAPI.Device;
    using Crevice.WinAPI.Window;
    using Crevice.WinAPI.Constants;

    [TestClass()]
    public class SingleInputSenderTests
    {
        static readonly SingleInputSender sender = new SingleInputSender();

        static CountdownEvent globalCDE;

        static readonly List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>> mouseEvents = new List<Tuple<LowLevelMouseHook.Event, LowLevelMouseHook.MSLLHOOKSTRUCT>>();
        static readonly LowLevelMouseHook mouseHook = new LowLevelMouseHook((evnt, data) => {
            if (data.FromCreviceApp)
            {
                mouseEvents.Add(Tuple.Create(evnt, data));
                globalCDE.Signal();
                return LowLevelMouseHook.Result.Cancel;
            }
            return LowLevelMouseHook.Result.Transfer;
        });

        static readonly List<Tuple<LowLevelKeyboardHook.Event, LowLevelKeyboardHook.KBDLLHOOKSTRUCT>> keyboardEvents = new List<Tuple<LowLevelKeyboardHook.Event, LowLevelKeyboardHook.KBDLLHOOKSTRUCT>>();
        static readonly LowLevelKeyboardHook keyboardHook = new LowLevelKeyboardHook((evnt, data) => {
            //
            // Note: Keyboard events with dwFlags = KeyboardEventType.KEYEVENTF_UNICODE are does not support 
            // signature embedding; so there is no way to discriminate self-published keyboard events against the others. 
            // 
            //if (data.FromCreviceApp)
            //{
            //    keyboardEvents.Add(Tuple.Create(evnt, data));
            //    globalCDE.Signal();
            //    return LowLevelKeyboardHook.Result.Cancel;
            //}
            //return LowLevelMouseHook.Result.Transfer;

            keyboardEvents.Add(Tuple.Create(evnt, data));
            globalCDE.Signal();
            return LowLevelKeyboardHook.Result.Cancel;
        });

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context)
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

        static readonly Mutex mutex = new Mutex(true, "SingleInputSenderTestsMutex");

        [TestInitialize()]
        public void TestInitialize()
        {
            mutex.WaitOne();
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            mouseEvents.Clear();
            keyboardEvents.Clear();
            mutex.ReleaseMutex();
        }

        [TestMethod()]
        public void LeftDownTest()
        {
            using(var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() => {
                    sender.LeftDown();
                    sender.LeftUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
        }

        [TestMethod()]
        public void LeftUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.LeftDown();
                    sender.LeftUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
        }

        [TestMethod()]
        public void LeftClickTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.LeftClick();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_LBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_LBUTTONUP);
        }

        [TestMethod()]
        public void RightDownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.RightDown();
                    sender.RightUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
        }

        [TestMethod()]
        public void RightUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.RightDown();
                    sender.RightUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        [TestMethod()]
        public void RightClickTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.RightClick();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_RBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_RBUTTONUP);
        }

        // https://stackoverflow.com/questions/5977445/how-to-get-windows-display-settings
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        private float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            return ScreenScalingFactor; 
        }

        [TestMethod()]
        public void MoveTest()
        {
            var rand = new Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            var start = Window.GetCursorPos();
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var dx = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var dy = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                using (var cde = new CountdownEvent(1))
                {
                    globalCDE = cde;
                    Task.Run(() =>
                    {
                        sender.Move(dx, dy);
                    });
                    cde.Wait(1000);
                }
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(pos.x - (start.X + dx) * Device.GetScreenScalingFactor() <= 1);
                Assert.IsTrue(pos.y - (start.Y + dy) * Device.GetScreenScalingFactor() <= 1);
            }
        } 

        [TestMethod()]
        public void MoveToTest()
        {
            var rand = new Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var x = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var y = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                using (var cde = new CountdownEvent(1))
                {
                    globalCDE = cde;
                    Task.Run(() =>
                    {
                        sender.MoveTo(x, y);
                    });
                    cde.Wait(1000);
                }
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(pos.x - x * GetScalingFactor() <= 1);
                Assert.IsTrue(pos.y - y * GetScalingFactor() <= 1);
            }
        }

        [TestMethod()]
        public void LogicalMoveTest()
        {
            var rand = new Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            var start = Window.GetPhysicalCursorPos();
            var scalingFactor = Device.GetScreenScalingFactor();
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var dx = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var dy = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                using (var cde = new CountdownEvent(1))
                {
                    globalCDE = cde;
                    Task.Run(() =>
                    {
                        sender.Move(dx, dy, logical: true);
                    });
                    cde.Wait(1000);
                }
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(pos.x - (start.X + dx * scalingFactor) <= 1);
                Assert.IsTrue(pos.y - (start.Y + dy * scalingFactor) <= 1);
            }
        }

        [TestMethod()]
        public void LogicalMoveToTest()
        {
            var rand = new Random();
            var max_cursor_range = 0xFFF;
            var min_cursor_range = -0xFFF;
            var scalingFactor = Device.GetScreenScalingFactor();
            foreach (int i in Enumerable.Range(0, 100))
            {
                mouseEvents.Clear();
                var x = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                var y = rand.Next(max_cursor_range - min_cursor_range) - min_cursor_range;
                using (var cde = new CountdownEvent(1))
                {
                    globalCDE = cde;
                    Task.Run(() =>
                    {
                        sender.MoveTo(x, y, logical: true);
                    });
                    cde.Wait(1000);
                }
                var evnt = mouseEvents[0].Item1;
                var pos = mouseEvents[0].Item2.pt;
                Assert.AreEqual(evnt, LowLevelMouseHook.Event.WM_MOUSEMOVE);
                Assert.IsTrue(pos.x - (x * scalingFactor) <= 1);
                Assert.IsTrue(pos.y - (y * scalingFactor) <= 1);
            }
        }

        [TestMethod()]
        public void MiddleDownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.MiddleDown();
                    sender.MiddleUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
        }

        [TestMethod()]
        public void MiddleUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.MiddleDown();
                    sender.MiddleUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
        }

        [TestMethod()]
        public void MiddleClickTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.MiddleClick();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MBUTTONDOWN);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_MBUTTONUP);
        }

        [TestMethod()]
        public void VerticalWheelTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.VerticalWheel(120);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void WheelDownTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.WheelDown();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents.Count, 1);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, -120);
        }

        [TestMethod()]
        public void WheelUpTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.WheelUp();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void HorizontalWheelTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.HorizontalWheel(120);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEHWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void WheelLeftTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.WheelLeft();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents.Count, 1);
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEHWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, -120);
        }

        [TestMethod()]
        public void WheelRightTest()
        {
            using (var cde = new CountdownEvent(1))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.WheelRight();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_MOUSEHWHEEL);
            Assert.AreEqual(mouseEvents[0].Item2.mouseData.asWheelDelta.delta, 120);
        }

        [TestMethod()]
        public void X1DownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X1Down();
                    sender.X1Up();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.IsXButton1);
        }

        [TestMethod()]
        public void X1UpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X1Down();
                    sender.X1Up();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.IsXButton1);
        }

        [TestMethod()]
        public void X1ClickTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X1Click();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.IsXButton1);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.IsXButton1);
        }

        [TestMethod()]
        public void X2DownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X2Down();
                    sender.X2Up();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.IsXButton2);
        }

        [TestMethod()]
        public void X2UpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X2Down();
                    sender.X2Up();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.IsXButton2);
        }

        [TestMethod()]
        public void X2ClickTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.X2Click();
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(mouseEvents[0].Item1, LowLevelMouseHook.Event.WM_XBUTTONDOWN);
            Assert.IsTrue(mouseEvents[0].Item2.mouseData.asXButton.IsXButton2);
            Assert.AreEqual(mouseEvents[1].Item1, LowLevelMouseHook.Event.WM_XBUTTONUP);
            Assert.IsTrue(mouseEvents[1].Item2.mouseData.asXButton.IsXButton2);
        }

        [TestMethod()]
        public void KeyDownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.KeyDown(VirtualKeys.VK_A);
                    sender.KeyUp(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x00);
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.KeyDown(VirtualKeys.VK_A);
                    sender.KeyUp(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x00);
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyDownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.ExtendedKeyDown(VirtualKeys.VK_A);
                    sender.ExtendedKeyUp(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x00);
            Assert.IsTrue(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.ExtendedKeyDown(VirtualKeys.VK_A);
                    sender.ExtendedKeyUp(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x00);
            Assert.IsTrue(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyDownWithScanCodeTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.KeyDownWithScanCode(VirtualKeys.VK_A);
                    sender.KeyUpWithScanCode(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 0x1E);
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void KeyUpWithScanCodeTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.KeyDownWithScanCode(VirtualKeys.VK_A);
                    sender.KeyUpWithScanCode(VirtualKeys.VK_A);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, VirtualKeys.VK_A);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 0x1E);
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyDownWithScanCodeTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.ExtendedKeyDownWithScanCode(VirtualKeys.VK_LWIN);
                    sender.ExtendedKeyUpWithScanCode(VirtualKeys.VK_LWIN);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, VirtualKeys.VK_LWIN);
            Assert.IsTrue(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void ExtendedKeyUpWithScanCodeTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.ExtendedKeyDownWithScanCode(VirtualKeys.VK_LWIN);
                    sender.ExtendedKeyUpWithScanCode(VirtualKeys.VK_LWIN);
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, VirtualKeys.VK_LWIN);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, VirtualKeys.VK_LWIN);
            Assert.IsTrue(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyDownTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.UnicodeKeyDown('A');
                    sender.UnicodeKeyUp('A');
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, 0xE7);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyUpTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.UnicodeKeyDown('A');
                    sender.UnicodeKeyUp('A');
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, 0xE7);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }

        [TestMethod()]
        public void UnicodeKeyStrokeTest()
        {
            using (var cde = new CountdownEvent(2))
            {
                globalCDE = cde;
                Task.Run(() =>
                {
                    sender.UnicodeKeyStroke("A");
                });
                cde.Wait(1000);
            }
            Assert.AreEqual(keyboardEvents[0].Item1, LowLevelKeyboardHook.Event.WM_KEYDOWN);
            Assert.AreEqual(keyboardEvents[0].Item2.vkCode, 0xE7);
            Assert.AreEqual(keyboardEvents[0].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[0].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));

            Assert.AreEqual(keyboardEvents[1].Item1, LowLevelKeyboardHook.Event.WM_KEYUP);
            Assert.AreEqual(keyboardEvents[1].Item2.vkCode, 0xE7);
            Assert.AreEqual(keyboardEvents[1].Item2.scanCode, 'A');
            Assert.IsFalse(keyboardEvents[1].Item2.flags.HasFlag(LowLevelKeyboardHook.FLAGS.LLKHF_EXTENDED));
        }
    }
}