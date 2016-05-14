﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    class InputSender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [StructLayout(LayoutKind.Sequential)]
        protected struct INPUT
        {
            public int type;
            public INPUTDATA data;
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct INPUTDATA
        {
            [FieldOffset(0)]
            public MOUSEINPUT asMouseInput;
            [FieldOffset(0)]
            public KEYBDINPUT asKeyboardInput;
            [FieldOffset(0)]
            public HARDWAREINPUT asHardwareInput;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct WHEELDELTA
        {
            private short lower;
            public short delta;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct XBUTTON
        {
            private short lower;
            public short type;
        }

        [StructLayout(LayoutKind.Explicit)]
        protected struct MouseData
        {
            [FieldOffset(0)]
            public WHEELDELTA asWheelDelta;
            [FieldOffset(0)]
            public XBUTTON asXButton;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public MouseData mouseData;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        private const short WHEEL_DELTA = 120;

        private const short XBUTTON1 = 0x0001;
        private const short XBUTTON2 = 0x0002;

        private const int INPUT_MOUSE    = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        private const int MOUSEEVENTF_MOVED      = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN   = 0x0002;
        private const int MOUSEEVENTF_LEFTUP     = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN  = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP    = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP   = 0x0040;
        private const int MOUSEEVENTF_WHEEL      = 0x0080;
        private const int MOUSEEVENTF_XDOWN      = 0x0100;
        private const int MOUSEEVENTF_XUP        = 0x0200;
        private const int MOUSEEVENTF_ABSOLUTE   = 0x8000;


        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP       = 0x0002;
        private const int KEYEVENTF_UNICODE     = 0x0004;
        private const int KEYEVENTF_SCANCODE    = 0x0008;
        


        private readonly UIntPtr MOUSEEVENTF_CREVICE_APP = new UIntPtr(0xFFFFFF00);

        protected void Send(INPUT[] input)
        {
            Debug.Print("calling a native method SendInput");
            foreach (var x in input)
            {
                switch (x.type)
                {
                    case INPUT_MOUSE:
                        Debug.Print("dx: {0}", x.data.asMouseInput.dx);
                        Debug.Print("dy: {0}", x.data.asMouseInput.dy);
                        Debug.Print("asWheelDelta.delta: {0}", x.data.asMouseInput.mouseData.asWheelDelta.delta);
                        Debug.Print("asXButton.type: {0}", x.data.asMouseInput.mouseData.asXButton.type);
                        Debug.Print("dwFlags: {0}",BitConverter.ToString(BitConverter.GetBytes(x.data.asMouseInput.dwFlags)));
                        Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asMouseInput.dwExtraInfo.ToUInt64())));
                        break;
                    case INPUT_KEYBOARD:
                        Debug.Print("wVk: {0}", x.data.asKeyboardInput.wVk);
                        Debug.Print("wScan: {0}", x.data.asKeyboardInput.wScan);
                        Debug.Print("dwFlags: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asKeyboardInput.dwFlags)));
                        Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(x.data.asKeyboardInput.dwExtraInfo.ToUInt64())));
                        break;
                    case INPUT_HARDWARE:
                        break;
                }
            }

            if (SendInput((uint)input.Length, input, Marshal.SizeOf(input[0])) > 0)
            {
                Debug.Print("success");
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.Print("SendInput failed; ErrorCode: {0}", errorCode);
            }
        }

        protected INPUT ToInput(MOUSEINPUT mouseInput)
        {
            var input = new INPUT();
            input.type = INPUT_MOUSE;
            input.data.asMouseInput = mouseInput;
            return input;
        }

        protected INPUT ToInput(KEYBDINPUT keyboardInput)
        {
            var input = new INPUT();
            input.type = INPUT_KEYBOARD;
            input.data.asKeyboardInput = keyboardInput;
            return input;
        }

        private MOUSEINPUT GetCreviceMouseInput()
        {
            var mouseInput = new MOUSEINPUT();
            // Set the CreviceApp signature to the mouse event
            mouseInput.dwExtraInfo = MOUSEEVENTF_CREVICE_APP;
            return mouseInput;
        }
        
        protected MOUSEINPUT MouseLeftDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_LEFTDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseLeftUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_LEFTUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseRightDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_RIGHTDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseRightUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_RIGHTUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMoveEvent(int dx, int dy)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dx = dx;
            mouseInput.dx = dy;
            mouseInput.dwFlags = MOUSEEVENTF_MOVED;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMoveToEvent(int x, int y)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dx = x;
            mouseInput.dx = y;
            mouseInput.dwFlags = MOUSEEVENTF_MOVED | MOUSEEVENTF_ABSOLUTE;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMiddleDownEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_MIDDLEDOWN;
            return mouseInput;
        }

        protected MOUSEINPUT MouseMiddleUpEvent()
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_MIDDLEUP;
            return mouseInput;
        }

        protected MOUSEINPUT MouseWheelEvent(short delta)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_WHEEL;
            mouseInput.mouseData.asWheelDelta.delta = delta;
            return mouseInput;
        }

        protected MOUSEINPUT MouseWheelDownEvent()
        {
            return MouseWheelEvent(-120);
        }

        protected MOUSEINPUT MouseWheelUpEvent()
        {
            return MouseWheelEvent(120);
        }

        private MOUSEINPUT MouseXUpEvent(short type)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_XUP;
            mouseInput.mouseData.asXButton.type = type;
            return mouseInput;
        }

        protected MOUSEINPUT MouseX1UpEvent()
        {
            return MouseXUpEvent(XBUTTON1);
        }

        protected MOUSEINPUT MouseX2UpEvent()
        {
            return MouseXUpEvent(XBUTTON2);
        }

        private MOUSEINPUT MouseXDownEvent(short type)
        {
            var mouseInput = GetCreviceMouseInput();
            mouseInput.dwFlags = MOUSEEVENTF_XDOWN;
            mouseInput.mouseData.asXButton.type = type;
            return mouseInput;
        }

        protected MOUSEINPUT MouseX1DownEvent()
        {
            return MouseXDownEvent(XBUTTON1);
        }

        protected MOUSEINPUT MouseX2DownEvent()
        {
            return MouseXDownEvent(XBUTTON2);
        }

        private KEYBDINPUT KeyEvent(ushort keyCode)
        {
            var keyboardInput = new KEYBDINPUT();
            keyboardInput.wVk = keyCode;
            return keyboardInput;
        }

        private KEYBDINPUT ExtendedKeyEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.dwFlags = KEYEVENTF_EXTENDEDKEY;
            return keyboardInput;
        }

        private KEYBDINPUT KeyWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.wScan = (ushort)MapVirtualKey(keyCode, 0);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_SCANCODE;
            return keyboardInput;
        }

        private KEYBDINPUT ExtendedKeyWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyEvent(keyCode);
            keyboardInput.wScan = (ushort)MapVirtualKey(keyCode, 0);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_SCANCODE;
            return keyboardInput;
        }

        private KEYBDINPUT UnicodeKeyEvent(char c)
        {
            var keyboardInput = new KEYBDINPUT();
            keyboardInput.wScan = c;
            keyboardInput.dwFlags = KEYEVENTF_UNICODE;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyUpEvent(ushort keyCode)
        {
            var keyboardInput = KeyEvent(keyCode);
            keyboardInput.dwFlags = KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT ExtendedKeyUpEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyUpWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = KeyWithScanCodeEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;

        }

        protected KEYBDINPUT ExtendedKeyUpWithScanCodeEvent(ushort keyCode)
        {
            var keyboardInput = ExtendedKeyWithScanCodeEvent(keyCode);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;

        }

        protected KEYBDINPUT UnicodeKeyUpEvent(char c)
        {
            var keyboardInput = UnicodeKeyEvent(c);
            keyboardInput.dwFlags = keyboardInput.dwFlags | KEYEVENTF_KEYUP;
            return keyboardInput;
        }

        protected KEYBDINPUT KeyDownEvent(ushort keyCode)
        {
            return KeyEvent(keyCode);
        }

        protected KEYBDINPUT ExtendedKeyDownEvent(ushort keyCode)
        {
            return ExtendedKeyEvent(keyCode);
        }

        protected KEYBDINPUT KeyDownWithScanCodeEvent(ushort keyCode)
        {
            return KeyWithScanCodeEvent(keyCode);
        }

        protected KEYBDINPUT ExtendedKeyDownWithScanCodeEvent(ushort keyCode)
        {
            return ExtendedKeyWithScanCodeEvent(keyCode);
        }

        protected KEYBDINPUT UnicodeKeyDownEvent(char c)
        {
            return UnicodeKeyEvent(c);
        }
    }

    class SingleInputSender : InputSender
    {
        protected void Send(params MOUSEINPUT[] mouseInput)
        {
            var input = new INPUT[mouseInput.Length];
            for (var i = 0; i < mouseInput.Length; i++)
            {
                input[i] = ToInput(mouseInput[i]);
            }
            Send(input);
        }

        protected void Send(params KEYBDINPUT[] keyboardInput)
        {
            var input = new INPUT[keyboardInput.Length];
            for (var i = 0; i < keyboardInput.Length; i++)
            {
                input[i] = ToInput(keyboardInput[i]);
            }
            Send(input);
        }

        public void LeftDown()
        {
            Send(MouseLeftDownEvent());
        }

        public void LeftUp()
        {
            Send(MouseLeftUpEvent());
        }

        public void LeftClick()
        {
            Send(MouseLeftDownEvent(), MouseLeftUpEvent());
        }

        public void RightDown()
        {
            Send(MouseRightDownEvent());
        }

        public void RightUp()
        {
            Send(MouseRightUpEvent());
        }

        public void RightClick()
        {
            Send(MouseRightDownEvent(), MouseRightUpEvent());
        }

        public void Move(int dx, int dy)
        {
            Send(MouseMoveEvent(dx, dy));
        }

        public void MoveTo(int x, int y)
        {
            Send(MouseMoveToEvent(x, y));
        }

        public void MiddleDown()
        {
            Send(MouseMiddleDownEvent());
        }

        public void MiddleUp()
        {
            Send(MouseMiddleUpEvent());
        }

        public void MiddleClick()
        {
            Send(MouseMiddleDownEvent(), MouseMiddleUpEvent());
        }

        public void Wheel(short delta)
        {
            Send(MouseWheelEvent(delta));
        }

        public void WheelDown()
        {
            Send(MouseWheelEvent(120));
        }

        public void WheelUp()
        {
            Send(MouseWheelEvent(-120));
        }
        
        public void X1Down()
        {
            Send(MouseX2DownEvent());
        }

        public void X1Up()
        {
            Send(MouseX1UpEvent());
        }
        
        public void X1Click()
        {
            Send(MouseX1DownEvent(), MouseX1UpEvent());
        }

        public void X2Down()
        {
            Send(MouseX2DownEvent());
        }

        public void X2Up()
        {
            Send(MouseX2UpEvent());
        }
        
        public void X2Click()
        {
            Send(MouseX2DownEvent(), MouseX2UpEvent());
        }

        public void KeyDown(ushort keyCode)
        {
            Send(KeyDownEvent(keyCode));
        }

        public void KeyUp(ushort keyCode)
        {
            Send(KeyUpEvent(keyCode));
        }

        public void ExtendedKeyDown(ushort keyCode)
        {
            Send(ExtendedKeyDownEvent(keyCode));
        }

        public void ExtendedKeyUp(ushort keyCode)
        {
            Send(ExtendedKeyUpEvent(keyCode));
        }

        public void KeyDownWithScanCode(ushort keyCode)
        {
            Send(KeyDownWithScanCodeEvent(keyCode));
        }

        public void KeyUpWithScanCode(ushort keyCode)
        {
            Send(KeyUpWithScanCodeEvent(keyCode));
        }

        public void ExtendedKeyDownWithScanCode(ushort keyCode)
        {
            Send(ExtendedKeyDownWithScanCodeEvent(keyCode));
        }

        public void ExtendedKeyUpWithScanCode(ushort keyCode)
        {
            Send(ExtendedKeyUpWithScanCodeEvent(keyCode));
        }

        public void UnicodeKeyDown(char c)
        {
            Send(UnicodeKeyDownEvent(c));
        }

        public void UnicodeKeyUp(char c)
        {
            Send(UnicodeKeyUpEvent(c));
        }
        
        public void UnicodeKeyStroke(string str)
        {
            var list = str
                .Select(c => new List<KEYBDINPUT>() { UnicodeKeyDownEvent(c), UnicodeKeyUpEvent(c) })
                .SelectMany(x => x);
            Send(list.ToArray());
        }
    }

    class InputSequenceBuilder : InputSender
    {
        private readonly IEnumerable<INPUT> buffer;

        public InputSequenceBuilder() : this(new List<INPUT>())
        {

        }

        private InputSequenceBuilder(IEnumerable<INPUT> xs)
        {
            this.buffer = xs;
        }
        
        private InputSequenceBuilder NewInstance(IEnumerable<INPUT> ys)
        {
            var xs = this.buffer.ToList();
            xs.AddRange(ys);
            return new InputSequenceBuilder(xs);
        }

        private InputSequenceBuilder NewInstance(params MOUSEINPUT[] mouseEvent)
        {
            return NewInstance(mouseEvent.Select(x => ToInput(x)));
        }

        private InputSequenceBuilder NewInstance(params KEYBDINPUT[] keyboardEvent)
        {
            return NewInstance(keyboardEvent.Select(x => ToInput(x)));
        }

        public InputSequenceBuilder LeftDown()
        {
            return NewInstance(MouseLeftDownEvent());
        }

        public InputSequenceBuilder LeftUp()
        {
            return NewInstance(MouseLeftUpEvent());
        }

        public InputSequenceBuilder LeftClick()
        {
            return NewInstance(MouseLeftDownEvent(), MouseLeftUpEvent());
        }
    
        public InputSequenceBuilder RightDown()
        {
            return NewInstance(MouseRightDownEvent());
        }

        public InputSequenceBuilder RightUp()
        {
            return NewInstance(MouseRightUpEvent());
        }

        public InputSequenceBuilder RightClick()
        {
            return NewInstance(MouseRightDownEvent(), MouseRightUpEvent());
        }

        public InputSequenceBuilder Move(int dx, int dy)
        {
            return NewInstance(MouseMoveEvent(dx, dy));
        }

        public InputSequenceBuilder MoveTo(int x, int y)
        {
            return NewInstance(MouseMoveToEvent(x, y));
        }

        public InputSequenceBuilder MiddleDown()
        {
            return NewInstance(MouseMiddleDownEvent());
        }

        public InputSequenceBuilder MiddleUp()
        {
            return NewInstance(MouseMiddleUpEvent());
        }

        public InputSequenceBuilder MiddleClick()
        {
            return NewInstance(MouseMiddleDownEvent(), MouseMiddleUpEvent());
        }

        public InputSequenceBuilder Wheel(short delta)
        {
            return NewInstance(MouseWheelEvent(delta));
        }

        public InputSequenceBuilder WheelDown()
        {
            return NewInstance(MouseWheelEvent(120));
        }

        public InputSequenceBuilder WheelUp()
        {
            return NewInstance(MouseWheelEvent(-120));
        }

        public InputSequenceBuilder X1Down()
        {
            return NewInstance(MouseX1DownEvent());
        }

        public InputSequenceBuilder X1Up()
        {
            return NewInstance(MouseX1UpEvent());
        }

        public InputSequenceBuilder X1Click()
        {
            return NewInstance(MouseX1DownEvent(), MouseX1UpEvent());
        }

        public InputSequenceBuilder X2Down()
        {
            return NewInstance(MouseX2UpEvent());
        }

        public InputSequenceBuilder X2Up()
        {
            return NewInstance(MouseX2UpEvent());
        }

        public InputSequenceBuilder X2Click()
        {
            return NewInstance(MouseX2DownEvent(), MouseX2UpEvent());
        }

        public InputSequenceBuilder KeyDown(ushort keyCode)
        {
            return NewInstance(KeyDownEvent(keyCode));
        }

        public InputSequenceBuilder KeyUp(ushort keyCode)
        {
            return NewInstance(KeyUpEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyDown(ushort keyCode)
        {
            return NewInstance(ExtendedKeyDownEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyUp(ushort keyCode)
        {
            return NewInstance(ExtendedKeyUpEvent(keyCode));
        }

        public InputSequenceBuilder KeyDownWithScanCode(ushort keyCode)
        {
            return NewInstance(KeyDownWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder KeyUpWithScanCode(ushort keyCode)
        {
            return NewInstance(KeyUpWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyDownWithScanCode(ushort keyCode)
        {
            return NewInstance(ExtendedKeyDownWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder ExtendedKeyUpWithScanCode(ushort keyCode)
        {
            return NewInstance(ExtendedKeyUpWithScanCodeEvent(keyCode));
        }

        public InputSequenceBuilder UnicodeKeyDown(char c)
        {
            return NewInstance(UnicodeKeyDownEvent(c));
        }

        public InputSequenceBuilder UnicodeKeyUp(char c)
        {
            return NewInstance(UnicodeKeyUpEvent(c));
        }

        public InputSequenceBuilder UnicodeKeyStroke(string str)
        {
            return NewInstance(str
                .Select(c => new List<KEYBDINPUT>() { UnicodeKeyDownEvent(c), UnicodeKeyUpEvent(c) })
                .SelectMany(x => x)
                .ToArray());
        }

        public void Send()
        {
            Send(buffer.ToArray());
        }
    }

    class WindowsApplication
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

        private const int PROCESS_VM_READ           = 0x0010;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;

        private const int maxPathSize = 1024;
        private readonly StringBuilder buffer = new StringBuilder(maxPathSize);

        public WindowsApplicationInfo GetForeground()
        {
            IntPtr hWindow = GetForegroundWindow();
            return FindFromWindowHandle(hWindow);
        }
        public WindowsApplicationInfo GetOnCursor(int x, int y)
        {
            IntPtr hWindow = WindowFromPoint(x, y);
            return FindFromWindowHandle(hWindow);
        }

        private WindowsApplicationInfo FindFromWindowHandle(IntPtr hWindow)
        {
            int pid = 0;
            int tid = GetWindowThreadProcessId(hWindow, out pid);
            Debug.Print("tid: 0x{0:X}", tid);
            Debug.Print("pid: 0x{0:X}", pid);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            Debug.Print("hProcess: 0x{0:X}", hProcess.ToInt64());
            int lpdwSize = maxPathSize;
            try
            {
                QueryFullProcessImageName(hProcess, 0, buffer, out lpdwSize);
            }
            finally
            {
                CloseHandle(hProcess);
            }
            String path = buffer.ToString();
            String name = path.Substring(path.LastIndexOf("\\")+1);
            return new WindowsApplicationInfo(path, name);
        }
    }

    class WindowsApplicationInfo
    {
        public readonly String path;
        public readonly String name;
        public WindowsApplicationInfo(String path, String name)
        {
            this.path = path;
            this.name = name;
        }
    }

    public class LowLevelMouseHook : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Procedure callback, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        
        private delegate IntPtr Procedure(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate Result UserProcedure(Event evnt, MSLLHOOKSTRUCT data);

        private const int HC_ACTION = 0;

        //private const int WH_MOUSE    =  7;
        private const int WH_MOUSE_LL = 14;

        public enum Event
        {
            WM_NCMOUSEMOVE     = 0x00A0,
            WM_NCLBUTTONDOWN   = 0x00A1,
            WM_NCLBUTTONUP     = 0x00A2,
            WM_NCLBUTTONDBLCLK = 0x00A3,
            WM_NCRBUTTONDOWN   = 0x00A4,
            WM_NCRBUTTONUP     = 0x00A5,
            WM_NCRBUTTONDBLCLK = 0x00A6,
            WM_NCMBUTTONDOWN   = 0x00A7,
            WM_NCMBUTTONUP     = 0x00A8,
            WM_NCMBUTTONDBLCLK = 0x00A9,
            WM_NCXBUTTONDOWN   = 0x00AB,
            WM_NCXBUTTONUP     = 0x00AC,
            WM_NCXBUTTONDBLCLK = 0x00AD,
            WM_MOUSEMOVE       = 0x0200,
            WM_LBUTTONDOWN     = 0x0201,
            WM_LBUTTONUP       = 0x0202,
            WM_LBUTTONDBLCLK   = 0x0203,
            WM_RBUTTONDOWN     = 0x0204,
            WM_RBUTTONUP       = 0x0205,
            WM_RBUTTONDBLCLK   = 0x0206,
            WM_MBUTTONDOWN     = 0x0207,
            WM_MBUTTONUP       = 0x0208,
            WM_MBUTTONDBLCLK   = 0x0209,
            WM_MOUSEWHEEL      = 0x020A,
            WM_XBUTTONDOWN     = 0x020B,
            WM_XBUTTONUP       = 0x020C,
            WM_XBUTTONDBLCLK   = 0x020D,
            WM_MOUSEHWHEEL     = 0x020E
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseData
        {
            public short lower;
            public short higher;

            public bool isXButton1
            {
                get { return higher == 0x0001; }
            }

            public bool isXButton2
            {
                get { return higher == 0x0002; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public MouseData mouseData;
            public uint flags;
            public uint time;
            public UIntPtr dwExtraInfo;

            public bool fromCreviceApp
            {
                get { return (uint)dwExtraInfo == MOUSEEVENTF_CREVICE_APP; }
            }

            public bool fromTablet
            {
                get { return ((uint)dwExtraInfo & MOUSEEVENTF_TMASK) == MOUSEEVENTF_FROMTABLET; }
            }
        }

        public enum Result
        {
            Transfer,
            Determine,
            Cancel
        };
        
        private const uint MOUSEEVENTF_CREVICE_APP = 0xFFFFFF00;
        private const uint MOUSEEVENTF_TMASK       = 0xFFFFFF00;
        private const uint MOUSEEVENTF_FROMTABLET  = 0xFF515700;

        private static readonly IntPtr LRESULTCancel = new IntPtr(1);
        
        private readonly Object lockObject = new Object();
        private readonly UserProcedure userProcedure;
        private readonly Procedure bindedProcedure;

        private IntPtr hHook = IntPtr.Zero;
        
        public LowLevelMouseHook(UserProcedure userProcedure)
        {
            this.userProcedure = userProcedure;
            this.bindedProcedure = MouseHookProc;
        }

        public bool Activated()
        {
            return hHook != IntPtr.Zero;
        }

        public void SetHook()
        {
            lock (lockObject)
            {
                if (Activated())
                {
                    throw new InvalidOperationException();
                }
                var hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
                Debug.Print("hInstance: 0x{0:X}", hInstance.ToInt64());
                Debug.Print("calling a native method SetWindowsHookEx(WH_MOUSE_LL)");
                hHook = SetWindowsHookEx(WH_MOUSE_LL, this.bindedProcedure, hInstance, 0);
                Debug.Print("hHook: 0x{0:X}", hHook.ToInt64());
                if (!Activated())
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("SetWindowsHookEx(WH_MOUSE_LL) failed; ErrorCode: {0}", errorCode);
                    throw new Win32Exception(errorCode);
                }
                Debug.Print("success");
            }
        }

        public void Unhook()
        {
            lock (lockObject)
            {
                if (!Activated())
                {
                    throw new InvalidOperationException();
                }
                Debug.Print("calling a native method UnhookWindowsHookEx(WH_MOUSE_LL)");
                Debug.Print("hHook: 0x{0:X}", hHook);
                if (!UnhookWindowsHookEx(hHook))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("UnhookWindowsHookEx(WH_MOUSE_LL) failed; ErrorCode: {0}", errorCode);
                    throw new Win32Exception(errorCode);
                }
                hHook = IntPtr.Zero;
                Debug.Print("success");
            }
        }

        public IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            lock (lockObject)
            {
                if (nCode >= 0 && Activated())
                {
                    var a = (Event)wParam;
                    var b = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    switch (userProcedure(a, b))
                    {
                        case Result.Transfer:
                            return CallNextHookEx(hHook, nCode, wParam, lParam);
                        case Result.Cancel:
                            return LRESULTCancel;
                        case Result.Determine:
                            return IntPtr.Zero;
                    }
                }
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            lock (lockObject)
            {
                if (Activated())
                {
                    Unhook();
                }
            }
        }

        ~ LowLevelMouseHook()
        {
            Dispose();
        }
    }
}
