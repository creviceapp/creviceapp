using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace CreviceApp.WinAPI.Window
{
    public static class Window
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Point pt, uint uFlags);

        private const int CWP_ALL             = 0x0000;
        private const int CWP_SKIPINVISIBLE   = 0x0001;
        private const int CWP_SKIPDISABLED    = 0x0002;
        private const int CWP_SKIPTRANSPARENT = 0x0004;

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

        private const int PROCESS_VM_READ = 0x0010;
        private const int PROCESS_QUERY_INFORMATION = 0x0400;

        private const int maxPathSize = 1024;

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_ID = -12;
                
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
                
        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern long SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public extern static bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);


        private static IntPtr GetWindowOnCursor(IntPtr hWnd, Point point)
        {
            Point clientPoint = new Point(point.X, point.Y);
            ScreenToClient(hWnd, ref clientPoint);
            IntPtr res = ChildWindowFromPointEx(hWnd, clientPoint, CWP_ALL);
            if (hWnd == res || res == IntPtr.Zero)
            {
                return hWnd;
            }
            return GetWindowOnCursor(res, point);
        }

        public static IntPtr GetWindowOnCursor(Point point)
        {
            return GetWindowOnCursor(WindowFromPoint(point), point);
        }
        
        public static IntPtr GetWindowId(IntPtr hWnd)
        {
            return GetWindowLong(hWnd, GWL_ID);
        }

        public static Tuple<int, int> GetThreadProcessId(IntPtr hWnd)
        {
            int pid = 0;
            int tid = GetWindowThreadProcessId(hWnd, out pid);
            return Tuple.Create(tid, pid);
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder buffer = new StringBuilder(maxPathSize);
            GetWindowText(hWnd, buffer, maxPathSize);
            return buffer.ToString();
        }

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder buffer = new StringBuilder(maxPathSize);
            GetClassName(hWnd, buffer, maxPathSize);
            return buffer.ToString();
        }

        public static string GetPath(int pid)
        {
            StringBuilder buffer = new StringBuilder(maxPathSize);
            IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, pid);
            int lpdwSize = maxPathSize;
            try
            {
                QueryFullProcessImageName(hProcess, 0, buffer, out lpdwSize);
            }
            finally
            {
                CloseHandle(hProcess);
            }
            return buffer.ToString();
        }

        public static string GetName(string path)
        {
            return path.Substring(path.LastIndexOf("\\") + 1);
        }
    }
            
    public class WindowInfo
    {
        public readonly IntPtr Handle;
        
        private readonly Lazy<Tuple<int, int>> threadProcessId;
        public int ThreadId  { get { return threadProcessId.Value.Item1; } }
        public int ProcessId { get { return threadProcessId.Value.Item2; } }

        private readonly Lazy<IntPtr> id;
        public IntPtr Id { get { return id.Value; } }

        private readonly Lazy<string> text;
        public string Text { get { return text.Value; } }

        private readonly Lazy<string> className;
        public string ClassName { get { return className.Value; } }

        private readonly Lazy<WindowInfo> parent;
        public WindowInfo Parent { get { return parent.Value; } }

        private Lazy<string> modulePath;
        public string ModulePath { get { return modulePath.Value; } }

        private Lazy<string> moduleName;
        public string ModuleName { get { return moduleName.Value; } }


        public WindowInfo(IntPtr handle)
        {
            this.Handle = handle;
            this.threadProcessId = new Lazy<Tuple<int, int>>(() =>
            {
                return Window.GetThreadProcessId(Handle);
            });
            this.id = new Lazy<IntPtr>(() =>
            {
                return Window.GetWindowId(Handle);
            });
            this.text = new Lazy<string>(() =>
            {
                return Window.GetWindowText(Handle);
            });
            this.className = new Lazy<string>(() =>
            {
                return Window.GetClassName(Handle);
            });
            this.parent = new Lazy<WindowInfo>(() =>
            {
                var res = Window.GetParent(Handle);
                if (res == null)
                {
                    return null;
                }
                else
                {
                    return new WindowInfo(res);
                }
            });
            this.modulePath = new Lazy<string>(() =>
            {
                return Window.GetPath(ProcessId);
            });
            this.moduleName = new Lazy<string>(() =>
            {
                return Window.GetName(ModulePath);
            });
        }

        public bool BringWindowToTop()
        {
            return Window.BringWindowToTop(Handle);
        }

        public long SendMessage(uint Msg, uint wParam, uint lParam)
        {
            return Window.SendMessage(Handle, Msg, wParam, lParam);
        }

        public bool PostMessage(uint Msg, uint wParam, uint lParam)
        {
            return Window.PostMessage(Handle, Msg, wParam, lParam);
        }
    }

    public class ForegroundWindowInfo : WindowInfo
    {
        public ForegroundWindowInfo() : base(Window.GetForegroundWindow()) { }
    }
            
    public class OnCursorWindowInfo : WindowInfo
    {
        public OnCursorWindowInfo(Point point) : base(Window.GetWindowOnCursor(point))
        {

        }
    }
}