using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crevice4Tests
{
    using Crevice.WinAPI.Constants;
    using Crevice.WinAPI.SendInput;

    [TestClass()]
    public class SingleInputSenderTests
    {
        private const int InputMouse = 0x0;
        private const int InputKeyboard = 0x1;

        private const uint MouseMove = 0x0001;
        private const uint MouseLeftDown = 0x0002;
        private const uint MouseLeftUp = 0x0004;
        private const uint MouseRightDown = 0x0008;
        private const uint MouseRightUp = 0x0010;
        private const uint MouseMiddleDown = 0x0020;
        private const uint MouseMiddleUp = 0x0040;
        private const uint MouseXDown = 0x0080;
        private const uint MouseXUp = 0x0100;
        private const uint MouseWheel = 0x0800;
        private const uint MouseHWheel = 0x1000;
        private const uint MouseAbsolute = 0x8000;

        private const uint KeyboardExtended = 0x01;
        private const uint KeyboardKeyUp = 0x02;
        private const uint KeyboardUnicode = 0x04;
        private const uint KeyboardScanCode = 0x08;

        private static readonly UIntPtr CreviceSignature = new UIntPtr(0xFF190700);

        private RecordingSingleInputSender sender;

        [TestInitialize()]
        public void TestInitialize()
        {
            sender = new RecordingSingleInputSender();
        }

        [TestMethod()]
        public void LeftDownTest()
        {
            sender.LeftDown();
            AssertSingleMouse(MouseLeftDown);
        }

        [TestMethod()]
        public void LeftUpTest()
        {
            sender.LeftUp();
            AssertSingleMouse(MouseLeftUp);
        }

        [TestMethod()]
        public void LeftClickTest()
        {
            sender.LeftClick();
            AssertMouseSequence(MouseLeftDown, MouseLeftUp);
        }

        [TestMethod()]
        public void RightDownTest()
        {
            sender.RightDown();
            AssertSingleMouse(MouseRightDown);
        }

        [TestMethod()]
        public void RightUpTest()
        {
            sender.RightUp();
            AssertSingleMouse(MouseRightUp);
        }

        [TestMethod()]
        public void RightClickTest()
        {
            sender.RightClick();
            AssertMouseSequence(MouseRightDown, MouseRightUp);
        }

        [TestMethod()]
        public void MoveTest()
        {
            sender.Move(10, 20);
            AssertSingleMouse(MouseMove | MouseAbsolute);
        }

        [TestMethod()]
        public void MoveToTest()
        {
            sender.MoveTo(10, 20);
            AssertSingleMouse(MouseMove | MouseAbsolute);
        }

        [TestMethod()]
        public void LogicalMoveTest()
        {
            sender.Move(10, 20, logical: true);
            AssertSingleMouse(MouseMove | MouseAbsolute);
        }

        [TestMethod()]
        public void LogicalMoveToTest()
        {
            sender.MoveTo(10, 20, logical: true);
            AssertSingleMouse(MouseMove | MouseAbsolute);
        }

        [TestMethod()]
        public void MiddleDownTest()
        {
            sender.MiddleDown();
            AssertSingleMouse(MouseMiddleDown);
        }

        [TestMethod()]
        public void MiddleUpTest()
        {
            sender.MiddleUp();
            AssertSingleMouse(MouseMiddleUp);
        }

        [TestMethod()]
        public void MiddleClickTest()
        {
            sender.MiddleClick();
            AssertMouseSequence(MouseMiddleDown, MouseMiddleUp);
        }

        [TestMethod()]
        public void VerticalWheelTest()
        {
            sender.VerticalWheel(120);
            AssertSingleMouse(MouseWheel, mouseData: 120);
        }

        [TestMethod()]
        public void WheelDownTest()
        {
            sender.WheelDown();
            AssertSingleMouse(MouseWheel, mouseData: -120);
        }

        [TestMethod()]
        public void WheelUpTest()
        {
            sender.WheelUp();
            AssertSingleMouse(MouseWheel, mouseData: 120);
        }

        [TestMethod()]
        public void HorizontalWheelTest()
        {
            sender.HorizontalWheel(120);
            AssertSingleMouse(MouseHWheel, mouseData: 120);
        }

        [TestMethod()]
        public void WheelLeftTest()
        {
            sender.WheelLeft();
            AssertSingleMouse(MouseHWheel, mouseData: -120);
        }

        [TestMethod()]
        public void WheelRightTest()
        {
            sender.WheelRight();
            AssertSingleMouse(MouseHWheel, mouseData: 120);
        }

        [TestMethod()]
        public void X1DownTest()
        {
            sender.X1Down();
            AssertSingleMouse(MouseXDown, mouseData: 1);
        }

        [TestMethod()]
        public void X1UpTest()
        {
            sender.X1Up();
            AssertSingleMouse(MouseXUp, mouseData: 1);
        }

        [TestMethod()]
        public void X1ClickTest()
        {
            sender.X1Click();
            AssertMouseSequence(
                Tuple.Create(MouseXDown, 1),
                Tuple.Create(MouseXUp, 1));
        }

        [TestMethod()]
        public void X2DownTest()
        {
            sender.X2Down();
            AssertSingleMouse(MouseXDown, mouseData: 2);
        }

        [TestMethod()]
        public void X2UpTest()
        {
            sender.X2Up();
            AssertSingleMouse(MouseXUp, mouseData: 2);
        }

        [TestMethod()]
        public void X2ClickTest()
        {
            sender.X2Click();
            AssertMouseSequence(
                Tuple.Create(MouseXDown, 2),
                Tuple.Create(MouseXUp, 2));
        }

        [TestMethod()]
        public void KeyDownTest()
        {
            sender.KeyDown(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0, 0);
        }

        [TestMethod()]
        public void KeyUpTest()
        {
            sender.KeyUp(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0, KeyboardKeyUp);
        }

        [TestMethod()]
        public void ExtendedKeyDownTest()
        {
            sender.ExtendedKeyDown(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0, KeyboardExtended);
        }

        [TestMethod()]
        public void ExtendedKeyUpTest()
        {
            sender.ExtendedKeyUp(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0, KeyboardExtended | KeyboardKeyUp);
        }

        [TestMethod()]
        public void KeyDownWithScanCodeTest()
        {
            sender.KeyDownWithScanCode(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0x1E, KeyboardScanCode);
        }

        [TestMethod()]
        public void KeyUpWithScanCodeTest()
        {
            sender.KeyUpWithScanCode(VirtualKeys.VK_A);
            AssertSingleKeyboard(VirtualKeys.VK_A, 0x1E, KeyboardScanCode | KeyboardKeyUp);
        }

        [TestMethod()]
        public void ExtendedKeyDownWithScanCodeTest()
        {
            sender.ExtendedKeyDownWithScanCode(VirtualKeys.VK_LWIN);
            AssertSingleKeyboard(VirtualKeys.VK_LWIN, VirtualKeys.VK_LWIN, KeyboardExtended | KeyboardScanCode);
        }

        [TestMethod()]
        public void ExtendedKeyUpWithScanCodeTest()
        {
            sender.ExtendedKeyUpWithScanCode(VirtualKeys.VK_LWIN);
            AssertSingleKeyboard(VirtualKeys.VK_LWIN, VirtualKeys.VK_LWIN, KeyboardExtended | KeyboardScanCode | KeyboardKeyUp);
        }

        [TestMethod()]
        public void UnicodeKeyDownTest()
        {
            sender.UnicodeKeyDown('A');
            AssertSingleKeyboard(0, 'A', KeyboardUnicode);
        }

        [TestMethod()]
        public void UnicodeKeyUpTest()
        {
            sender.UnicodeKeyUp('A');
            AssertSingleKeyboard(0, 'A', KeyboardUnicode | KeyboardKeyUp);
        }

        [TestMethod()]
        public void UnicodeKeyStrokeTest()
        {
            sender.UnicodeKeyStroke("A");
            AssertKeyboardSequence(
                Tuple.Create(0, (int)'A', KeyboardUnicode, CreviceSignature),
                Tuple.Create(0, (int)'A', KeyboardUnicode | KeyboardKeyUp, CreviceSignature));
        }

        private void AssertSingleMouse(uint flags, int mouseData = 0)
        {
            AssertMouseSequence(Tuple.Create(flags, mouseData));
        }

        private void AssertMouseSequence(params uint[] flags)
        {
            AssertMouseSequence(flags.Select(f => Tuple.Create(f, 0)).ToArray());
        }

        private void AssertMouseSequence(params Tuple<uint, int>[] expected)
        {
            var inputs = sender.LastCall;
            Assert.AreEqual(expected.Length, inputs.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(InputMouse, inputs[i].Type);
                Assert.AreEqual(expected[i].Item1, inputs[i].Flags);
                Assert.AreEqual(expected[i].Item2, inputs[i].MouseData);
                Assert.AreEqual(CreviceSignature, inputs[i].ExtraInfo);
            }
        }

        private void AssertSingleKeyboard(int keyCode, int scanCode, uint flags)
        {
            AssertSingleKeyboard(keyCode, scanCode, flags, CreviceSignature);
        }

        private void AssertSingleKeyboard(int keyCode, int scanCode, uint flags, UIntPtr extraInfo)
        {
            AssertKeyboardSequence(Tuple.Create(keyCode, scanCode, flags, extraInfo));
        }

        private void AssertKeyboardSequence(params Tuple<int, int, uint, UIntPtr>[] expected)
        {
            var inputs = sender.LastCall;
            Assert.AreEqual(expected.Length, inputs.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(InputKeyboard, inputs[i].Type);
                Assert.AreEqual(expected[i].Item1, inputs[i].VirtualKey);
                Assert.AreEqual(expected[i].Item2, inputs[i].ScanCode);
                Assert.AreEqual(expected[i].Item3, inputs[i].Flags);
                Assert.AreEqual(expected[i].Item4, inputs[i].ExtraInfo);
            }
        }

        private sealed class RecordingSingleInputSender : SingleInputSender
        {
            private readonly List<RecordedInput[]> calls = new List<RecordedInput[]>();

            public RecordedInput[] LastCall
                => calls.Last();

            protected override void Send(INPUT[] input)
            {
                calls.Add(input.Select(Record).ToArray());
            }

            private static RecordedInput Record(INPUT input)
            {
                var recorded = new RecordedInput
                {
                    Type = input.type,
                };

                if (input.type == InputMouse)
                {
                    var mouse = input.data.asMouseInput;
                    recorded.Dx = mouse.dx;
                    recorded.Dy = mouse.dy;
                    recorded.Flags = mouse.dwFlags;
                    recorded.MouseData = mouse.mouseData.asWheelDelta.delta;
                    recorded.ExtraInfo = mouse.dwExtraInfo;
                }
                else if (input.type == InputKeyboard)
                {
                    var keyboard = input.data.asKeyboardInput;
                    recorded.VirtualKey = keyboard.wVk;
                    recorded.ScanCode = keyboard.wScan;
                    recorded.Flags = keyboard.dwFlags;
                    recorded.ExtraInfo = keyboard.dwExtraInfo;
                }

                return recorded;
            }
        }

        private sealed class RecordedInput
        {
            public int Type { get; set; }

            public int Dx { get; set; }

            public int Dy { get; set; }

            public uint Flags { get; set; }

            public int MouseData { get; set; }

            public int VirtualKey { get; set; }

            public int ScanCode { get; set; }

            public UIntPtr ExtraInfo { get; set; }
        }
    }
}
