using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace CreviceApp.WinAPI.Window
{
    public static class Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

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
            
    public class ModuleFileInfo
    {
        private WindowInfo info;

        private Lazy<string> path;
        public string Path { get { return path.Value; } }

        private Lazy<string> name;
        public string Name { get { return name.Value; } }

        public ModuleFileInfo(WindowInfo info)
        {
            this.info = info;
            this.path = new Lazy<string>(() =>
            {
                return Window.GetPath(info.ProcessId);
            });
            this.name = new Lazy<string>(() =>
            {
                return Window.GetName(path.Value);
            });
        }
    }

    public class WindowInfo
    {
        public readonly IntPtr Handle;
        public readonly ModuleFileInfo Module;
        
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
            this.Module = new ModuleFileInfo(this);
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
        public OnCursorWindowInfo(int x, int y) : base(Window.WindowFromPoint(x, y)) { }
    }
}