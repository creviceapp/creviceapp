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
            NCX_BUTTONDOWN   = 0x00AB,
            NCX_BUTTONUP     = 0x00AC,
            NCX_BUTTONDBLCLK = 0x00AD,
            MOUSEMOVE        = 0x0200,
            L_BUTTONDOWN     = 0x0201,
            L_BUTTONUP       = 0x0202,
            L_BUTTONDBLCLK   = 0x0203,
            R_BUTTONDOWN     = 0x0204,
            R_BUTTONUP       = 0x0205,
            R_BUTTONDBLCLK   = 0x0206,
            M_BUTTONDOWN     = 0x0207,
            M_BUTTONUP       = 0x0208,
            M_BUTTONDBLCLK   = 0x0209,
            V_MOUSEWHEEL     = 0x020A,
            X_BUTTONDOWN     = 0x020B,
            X_BUTTONUP       = 0x020C,
            X_BUTTONDBLCLK   = 0x020D,
            H_MOUSEWHEEL     = 0x020E
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
            public IntPtr dwExtraInfo;
        }

        public enum Result
        {
            Transfer,
            Determine,
            Cancel
        };

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
                Debug.Print("trying to set a hook(WH_MOUSE_LL)");
                hHook = SetWindowsHookEx(WH_MOUSE_LL, this.bindedProcedure, hInstance, 0);
                Debug.Print("hHook: 0x{0:X}", hHook.ToInt64());
                if (!Activated())
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("failed to set a hook(WH_MOUSE_LL), ErrorCode: {0}", errorCode);
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
                Debug.Print("trying to unhook a hook(WH_MOUSE_LL)");
                Debug.Print("hHook: 0x{0:X}", hHook);
                if (!UnhookWindowsHookEx(hHook))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("failed to set a hook(WH_MOUSE_LL), ErrorCode: {0}", errorCode);
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
