using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp.WinAPI.Window
{
    namespace Impl
    {
        public class WindowInfo
        {
            public static class NativeMethods
            {
                [DllImport("user32.dll")]
                public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowLong(IntPtr hWnd, WindowLongParam nIndex);

                [DllImport("user32.dll")]
                public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

                [DllImport("user32.dll")]
                public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

                [DllImport("user32.dll")]
                public static extern IntPtr GetParent(IntPtr hWnd);

                [DllImport("kernel32.dll")]
                public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

                [DllImport("kernel32.dll")]
                public static extern bool CloseHandle(IntPtr hHandle);

                [DllImport("kernel32.dll")]
                public static extern bool QueryFullProcessImageName(IntPtr hProcess, int dwFlags, StringBuilder lpExeName, out int lpdwSize);

                [DllImport("user32.dll", SetLastError = true)]
                public static extern bool BringWindowToTop(IntPtr hWnd);

                [DllImport("user32.dll")]
                public static extern long SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

                [DllImport("user32.dll", SetLastError = true)]
                public extern static bool PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

                [DllImport("user32.dll", SetLastError = true)]
                public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public extern static bool SetForegroundWindow(IntPtr hWnd);

                [DllImport("user32.dll")]
                public extern static bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

                [DllImport("user32.dll", ExactSpelling = true)]
                public extern static IntPtr GetAncestor(IntPtr hwnd, uint flags);
            }

            public const int MaxPathSize = 1024;

            // http://www.pinvoke.net/default.aspx/kernel32/OpenProcess.html
            [Flags]
            public enum ProcessAccessFlags : uint
            {
                All = 0x001F0FFF,
                Terminate = 0x00000001,
                CreateThread = 0x00000002,
                VirtualMemoryOperation = 0x00000008,
                VirtualMemoryRead = 0x00000010,
                VirtualMemoryWrite = 0x00000020,
                DuplicateHandle = 0x00000040,
                CreateProcess = 0x000000080,
                SetQuota = 0x00000100,
                SetInformation = 0x00000200,
                QueryInformation = 0x00000400,
                QueryLimitedInformation = 0x00001000,
                Synchronize = 0x00100000
            }

            // http://pinvoke.net/default.aspx/Constants/GWL%20-%20GetWindowLong.html
            public enum WindowLongParam: int
            {
                /// <summary>Sets a new address for the window procedure.</summary>
                /// <remarks>You cannot change this attribute if the window does not belong to the same process as the calling thread.</remarks>
                GWL_WNDPROC = -4,

                /// <summary>Sets a new application instance handle.</summary>
                GWLP_HINSTANCE = -6,

                GWLP_HWNDPARENT = -8,

                /// <summary>Sets a new identifier of the child window.</summary>
                /// <remarks>The window cannot be a top-level window.</remarks>
                GWL_ID = -12,

                /// <summary>Sets a new window style.</summary>
                GWL_STYLE = -16,

                /// <summary>Sets a new extended window style.</summary>
                /// <remarks>See <see cref="ExWindowStyles"/>.</remarks>
                GWL_EXSTYLE = -20,

                /// <summary>Sets the user data associated with the window.</summary>
                /// <remarks>This data is intended for use by the application that created the window. Its value is initially zero.</remarks>
                GWL_USERDATA = -21,

                /// <summary>Sets the return value of a message processed in the dialog box procedure.</summary>
                /// <remarks>Only applies to dialog boxes.</remarks>
                DWLP_MSGRESULT = 0,

                /// <summary>Sets new extra information that is private to the application, such as handles or pointers.</summary>
                /// <remarks>Only applies to dialog boxes.</remarks>
                DWLP_USER = 8,

                /// <summary>Sets the new address of the dialog box procedure.</summary>
                /// <remarks>Only applies to dialog boxes.</remarks>
                DWLP_DLGPROC = 4
            }

            public readonly IntPtr WindowHandle;

            private readonly Lazy<Tuple<int, int>> threadProcessId;
            public int ThreadId { get { return threadProcessId.Value.Item1; } }
            public int ProcessId { get { return threadProcessId.Value.Item2; } }

            private readonly Lazy<IntPtr> windowId;
            public IntPtr WindowId { get { return windowId.Value; } }

            public string Text { get { return GetWindowText(WindowHandle); } }

            private readonly Lazy<string> className;
            public string ClassName { get { return className.Value; } }

            private readonly Lazy<WindowInfo> parent;
            public WindowInfo Parent { get { return parent.Value; } }

            private Lazy<string> modulePath;
            public string ModulePath { get { return modulePath.Value; } }

            private Lazy<string> moduleName;
            public string ModuleName { get { return moduleName.Value; } }


            public WindowInfo(IntPtr hWnd)
            {
                this.WindowHandle = hWnd;
                this.threadProcessId = new Lazy<Tuple<int, int>>(() =>
                {
                    return GetThreadProcessId(WindowHandle);
                });
                this.windowId = new Lazy<IntPtr>(() =>
                {
                    return GetWindowId(WindowHandle);
                });
                this.className = new Lazy<string>(() =>
                {
                    return GetClassName(WindowHandle);
                });
                this.parent = new Lazy<WindowInfo>(() =>
                {
                    var res = NativeMethods.GetParent(WindowHandle);
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
                    return GetPath(ProcessId);
                });
                this.moduleName = new Lazy<string>(() =>
                {
                    return GetName(ModulePath);
                });
            }

            private static Tuple<int, int> GetThreadProcessId(IntPtr hWnd)
            {
                int pid = 0;
                int tid = NativeMethods.GetWindowThreadProcessId(hWnd, out pid);
                return Tuple.Create(tid, pid);
            }

            private static IntPtr GetWindowId(IntPtr hWnd)
            {
                return NativeMethods.GetWindowLong(hWnd, WindowLongParam.GWL_ID);
            }

            private static string GetWindowText(IntPtr hWnd)
            {
                StringBuilder buffer = new StringBuilder(MaxPathSize);
                NativeMethods.GetWindowText(hWnd, buffer, MaxPathSize);
                return buffer.ToString();
            }

            private static string GetClassName(IntPtr hWnd)
            {
                StringBuilder buffer = new StringBuilder(MaxPathSize);
                NativeMethods.GetClassName(hWnd, buffer, MaxPathSize);
                return buffer.ToString();
            }

            private static string GetPath(int pid)
            {
                StringBuilder buffer = new StringBuilder(MaxPathSize);
                IntPtr hProcess = NativeMethods.OpenProcess(ProcessAccessFlags.QueryInformation | ProcessAccessFlags.VirtualMemoryRead, false, pid);
                int lpdwSize = MaxPathSize;
                try
                {
                    NativeMethods.QueryFullProcessImageName(hProcess, 0, buffer, out lpdwSize);
                }
                finally
                {
                    NativeMethods.CloseHandle(hProcess);
                }
                return buffer.ToString();
            }

            private static string GetName(string path)
            {
                return path.Substring(path.LastIndexOf("\\") + 1);
            }

            public bool Activate()
            {
                var hwndTarget = NativeMethods.GetAncestor(WindowHandle, 2);
                var hwndActive = ForegroundWindowInfo.NativeMethods.GetForegroundWindow();
                if (hwndTarget == hwndActive)
                    return true;

                var outTmp = 0;
                var tidTarget = NativeMethods.GetWindowThreadProcessId(WindowHandle, out outTmp);
                var tidActive = NativeMethods.GetWindowThreadProcessId(hwndActive, out outTmp);

                if (NativeMethods.SetForegroundWindow(WindowHandle))
                    return true;
                if (tidTarget == tidActive)
                    return BringWindowToTop();
                else
                {
                    NativeMethods.AttachThreadInput(tidTarget, tidActive, true);
                    try { return BringWindowToTop(); }
                    finally { NativeMethods.AttachThreadInput(tidTarget, tidActive, false); }
                }
            }

            public bool BringWindowToTop()
            {
                return NativeMethods.BringWindowToTop(WindowHandle);
            }

            public long SendMessage(uint Msg, uint wParam, uint lParam)
            {
                return NativeMethods.SendMessage(WindowHandle, Msg, wParam, lParam);
            }

            public bool PostMessage(uint Msg, uint wParam, uint lParam)
            {
                return NativeMethods.PostMessage(WindowHandle, Msg, wParam, lParam);
            }
            
            public WindowInfo FindWindowEx(IntPtr hwndChildAfter, string lpszClass, string lpszWindow)
            {
                return new WindowInfo(NativeMethods.FindWindowEx(WindowHandle, hwndChildAfter, lpszClass, lpszWindow));
            }

            public WindowInfo FindWindowEx(string lpszClass, string lpszWindow)
            {
                return new WindowInfo(NativeMethods.FindWindowEx(WindowHandle, IntPtr.Zero, lpszClass, lpszWindow));
            }
            
            public IEnumerable<WindowInfo> GetChildWindows()
            {
                return new Enumerables.ChildWindows(WindowHandle);
            }

            public IEnumerable<WindowInfo> GetPointedDescendantWindows(Point point, Window.WindowFromPointFlags flags)
            {
                return new Enumerables.PointedDescendantWindows(WindowHandle, point, flags);
            }

            public IEnumerable<WindowInfo> GetPointedDescendantWindows(Point point)
            {
                return new Enumerables.PointedDescendantWindows(WindowHandle, point, Window.WindowFromPointFlags.CWP_ALL);
            }
        }

        public class ForegroundWindowInfo : WindowInfo
        {
            public static new class NativeMethods
            {
                [DllImport("user32.dll")]
                public static extern IntPtr GetForegroundWindow();
            }

            public ForegroundWindowInfo() : base(NativeMethods.GetForegroundWindow()) { }
        }

        public class PointedWindowInfo : WindowInfo
        {
            public static new class NativeMethods
            {
                [DllImport("user32.dll")]
                public static extern IntPtr WindowFromPhysicalPoint(Point point);
            }

            public PointedWindowInfo(Point point) : base(NativeMethods.WindowFromPhysicalPoint(point)) { }
        }

        namespace Enumerables
        {
            // http://qiita.com/katabamisan/items/081547f42512e93a31ab

            public abstract class WindowEnumerable : IEnumerable<WindowInfo>
            {
                internal delegate bool EnumWindowsProcDelegate(IntPtr hWnd, IntPtr lParam);

                internal List<IntPtr> handles;

                public WindowEnumerable()
                {
                    handles = new List<IntPtr>();
                }

                public IEnumerator<WindowInfo> GetEnumerator()
                {
                    return handles.Select(x => new WindowInfo(x)).GetEnumerator();
                }

                System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
                {
                    return handles.Select(x => new WindowInfo(x)).GetEnumerator();
                }

                internal bool EnumWindowProc(IntPtr handle, IntPtr lParam)
                {
                    handles.Add(handle);
                    return true;
                }
            }

            public sealed class TopLevelWindows : WindowEnumerable
            {
                private static class NativeMethods
                {
                    [DllImport("user32.dll", SetLastError = true)]
                    public static extern bool EnumWindows(EnumWindowsProcDelegate lpEnumFunc, IntPtr lParam);
                }

                public TopLevelWindows()
                    : base()
                {
                    handles = new List<IntPtr>();
                    NativeMethods.EnumWindows(EnumWindowProc, IntPtr.Zero);
                }
            }

            public sealed class ChildWindows : WindowEnumerable
            {
                private static class NativeMethods
                {
                    [DllImport("user32.dll", SetLastError = true)]
                    public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProcDelegate lpEnumFunc, IntPtr lParam);
                }

                public readonly IntPtr WindowHandle;

                public ChildWindows(IntPtr hWnd)
                    : base()
                {
                    this.WindowHandle = hWnd;
                    NativeMethods.EnumChildWindows(hWnd, EnumWindowProc, IntPtr.Zero);
                }

            }

            public sealed class PointedDescendantWindows : WindowEnumerable
            {
                private static class NativeMethods
                {
                    [DllImport("user32.dll")]
                    public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

                    [DllImport("user32.dll")]
                    public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Point pt, Window.WindowFromPointFlags uFlags);
                }

                public readonly IntPtr WindowHandle;
                public readonly Point Point;

                public PointedDescendantWindows(IntPtr hWnd, Point point, Window.WindowFromPointFlags flags)
                    : base()
                {
                    this.WindowHandle = hWnd;
                    this.Point = point;
                    if (hWnd != IntPtr.Zero)
                    {
                        var res = ChildWindowFromPointEx(hWnd, point, flags);
                        while (hWnd != res && res != IntPtr.Zero)
                        {
                            hWnd = res;
                            handles.Add(hWnd);
                            res = ChildWindowFromPointEx(hWnd, point, flags);
                        }
                    }
                }

                private IntPtr ChildWindowFromPointEx(IntPtr hWnd, Point point, Window.WindowFromPointFlags flags)
                {
                    Point clientPoint = new Point(point.X, point.Y);
                    NativeMethods.ScreenToClient(hWnd, ref clientPoint);
                    return NativeMethods.ChildWindowFromPointEx(hWnd, clientPoint, flags);
                }
            }

            public sealed class ThreadWindows : WindowEnumerable
            {
                private static class NativeMethods
                {
                    [DllImport("user32.dll", SetLastError = true)]
                    public static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProcDelegate lpfn, IntPtr lParam);
                }

                public readonly uint ThreadId;

                public ThreadWindows(uint threadId)
                    : base()
                {
                    this.ThreadId = threadId;
                    NativeMethods.EnumThreadWindows(threadId, EnumWindowProc, IntPtr.Zero);
                }
            }
        }
    }

    public static class Window
    {
        public static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool GetCursorPos(out Point lpPoint);

            [DllImport("user32.dll")]
            public static extern bool GetPhysicalCursorPos(out Point lpPoint);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        // http://pinvoke.net/default.aspx/Enums/ChildWindowFromPointFlags.html
        /// <summary>
        /// For use with ChildWindowFromPointEx 
        /// </summary>
        [Flags]
        public enum WindowFromPointFlags : uint
        {
            /// <summary>
            /// Does not skip any child windows
            /// </summary>
            CWP_ALL = 0x0000,
            /// <summary>
            /// Skips invisible child windows
            /// </summary>
            CWP_SKIPINVISIBLE = 0x0001,
            /// <summary>
            /// Skips disabled child windows
            /// </summary>
            CWP_SKIPDISABLED = 0x0002,
            /// <summary>
            /// Skips transparent child windows
            /// </summary>
            CWP_SKIPTRANSPARENT = 0x0004
        }

        public static Impl.WindowInfo From(IntPtr hWnd)
        {
            return new Impl.WindowInfo(hWnd);
        }

        public static Point GetCursorPos()
        {
            var point = new Point();
            NativeMethods.GetCursorPos(out point);
            return point;
        }

        public static Point GetPhysicalCursorPos()
        {
            var point = new Point();
            NativeMethods.GetPhysicalCursorPos(out point);
            return point;
        }

        public static Impl.WindowInfo GetForegroundWindow()
        {
            return new Impl.ForegroundWindowInfo();
        }

        public static Impl.WindowInfo GetPointedWindow()
        {
            return WindowFromPoint(GetPhysicalCursorPos());
        }

        public static Impl.WindowInfo WindowFromPoint(Point point)
        {
            return new Impl.PointedWindowInfo(point);
        }
        
        public static Impl.WindowInfo FindWindow(string lpClassName, string lpWindowName)
        {
            return From(NativeMethods.FindWindow(lpClassName, lpWindowName));
        }
        
        public static IEnumerable<Impl.WindowInfo> GetTopLevelWindows()
        {
            return new Impl.Enumerables.TopLevelWindows();
        }

        public static IEnumerable<Impl.WindowInfo> GetThreadWindows(uint threadId)
        {
            return new Impl.Enumerables.ThreadWindows(threadId);
        }
    }
}
