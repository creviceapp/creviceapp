using System;
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

    class EventSender
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public INPUTDATA data;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUTDATA
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public MOUSEINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseData
        {
            public short lower;
            public short higher;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public MouseData mouseData;
            public int dwFlags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }
        
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
        
        private readonly UIntPtr MOUSEEVENTF_CREVICE_APP = new UIntPtr(0xFFFFFF00);
        
        private void Send(INPUT[] input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                //input[i].data.mi.dwExtraInfo = MOUSEEVENTF_CREVICE_APP;
            }

            if (SendInput(1, input, Marshal.SizeOf(input[0])) > 0)
            {
                Debug.Print("success");
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                Debug.Print("SendInput failed; ErrorCode: {0}", errorCode);
            }
        }

        public void LeftDown()
        {
            INPUT[] input = new INPUT[1];
            input[0].data.mi.dwFlags = MOUSEEVENTF_LEFTDOWN;
            Send(input);
        }

        public void LeftUp()
        {
            INPUT[] input = new INPUT[1];
            input[0].data.mi.dwFlags = MOUSEEVENTF_LEFTUP;
            Send(input);
        }

        public void RightDown()
        {
            INPUT[] input = new INPUT[1];
            input[0].data.mi.dwFlags = MOUSEEVENTF_RIGHTDOWN;
            Send(input);
        }

        public void RightUp()
        {
            INPUT[] input = new INPUT[1];
            input[0].data.mi.dwFlags = MOUSEEVENTF_RIGHTUP;
            Send(input);
        }

        //KeyDown, KeyUp, Key
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
