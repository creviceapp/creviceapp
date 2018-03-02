using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Crevice.WinAPI.WindowsHookEx
{
    using System.Threading;
    using Crevice.WinAPI.Helper;

    public class WindowsHook : IDisposable
    {
        static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, SystemCallback callback, IntPtr hInstance, int threadId);
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool UnhookWindowsHookEx(IntPtr hook);
            [DllImport("user32.dll")]
            public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr GetModuleHandle(string name);
        }

        private delegate IntPtr SystemCallback(int nCode, IntPtr wParam, IntPtr lParam);
        protected delegate Result UserCallback(IntPtr wParam, IntPtr lParam);

        public const int HC_ACTION = 0;
        
        public enum HookType
        {
            WH_MSGFILTER      = -1,
            WH_JOURNALRECORD   = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD        = 2,
            WH_GETMESSAGE      = 3,
            WH_CALLWNDPROC     = 4,
            WH_CBT             = 5,
            WH_SYSMSGFILTER    = 6,
            WH_MOUSE           = 7,
            WH_DEBUG           = 9,
            WH_SHELL          = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL    = 13,
            WH_MOUSE_LL       = 14
        }
        
        public enum Result
        {
            Transfer,
            Determine,
            Cancel
        };
        
        private static readonly IntPtr LRESULTCancel = new IntPtr(1);

        // These callback functions should be hold as a local variable to prevent it from GC.
        private readonly UserCallback _userCallback;
        private readonly SystemCallback _systemCallback;

        private readonly HookType _hookType;

        private IntPtr _hHook = IntPtr.Zero;
        
        protected WindowsHook(HookType hookType, UserCallback userCallback)
        {
            _hookType = hookType;
            _userCallback = userCallback;
            _systemCallback = Callback;
        }

        public bool IsActivated
            => _hHook != IntPtr.Zero;

        public void SetHook()
        {
            if (IsActivated)
            {
                throw new InvalidOperationException();
            }
            var log = new WinAPILogger("SetWindowsHookEx");
            log.Add("Hook type: {0}", Enum.GetName(typeof(HookType), _hookType));
            var hInstance = NativeMethods.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            log.Add("hInstance: 0x{0:X}", hInstance.ToInt64());
            _hHook = NativeMethods.SetWindowsHookEx((int)_hookType, _systemCallback, hInstance, 0);
            if (IsActivated)
            {
                log.Add("_hHook: 0x{0:X}", _hHook.ToInt64());
                log.Success();
            }
            else
            {
                log.FailWithErrorCode();
            }
        }

        public void Unhook()
        {
            if (!IsActivated)
            {
                throw new InvalidOperationException();
            }
            var log = new WinAPILogger("UnhookWindowsHookEx");
            log.Add("Hook type: {0}", Enum.GetName(typeof(HookType), _hookType));
            log.Add("_hHook: 0x{0:X}", _hHook);
            if (NativeMethods.UnhookWindowsHookEx(_hHook))
            {
                log.Success();
            }
            else
            {
                log.FailWithErrorCode();
            }
            _hHook = IntPtr.Zero;
        }

        public IntPtr Callback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                switch (_userCallback(wParam, lParam))
                {
                    case Result.Transfer:
                        return NativeMethods.CallNextHookEx(_hHook, nCode, wParam, lParam);
                    case Result.Cancel:
                        return LRESULTCancel;
                    case Result.Determine:
                        return IntPtr.Zero;
                }
            }
            return NativeMethods.CallNextHookEx(_hHook, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (IsActivated)
                {
                    Unhook();
                }
            }
        }

        ~WindowsHook() => Dispose(false);
    }

    public class LowLevelMouseHook : WindowsHook
    {
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
        public struct WHEELDELTA
        {
            private short lower;
            public short delta;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XBUTTON
        {
            private short lower;
            public short type;

            public bool IsXButton1 => type == 0x0001;
            public bool IsXButton2 => type == 0x0002;
        }
        
        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSEDATA
        {
            [FieldOffset(0)]
            public WHEELDELTA asWheelDelta;
            [FieldOffset(0)]
            public XBUTTON asXButton;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public MOUSEDATA mouseData;
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;

            public bool FromCreviceApp
                => ((uint)dwExtraInfo & MOUSEEVENTF_TMASK) == MOUSEEVENTF_CREVICE_APP;

            public bool FromTablet
                => ((uint)dwExtraInfo & MOUSEEVENTF_TMASK) == MOUSEEVENTF_FROMTABLET;
        }

        public const uint MOUSEEVENTF_CREVICE_APP = 0xFFFFFF00;
        public const uint MOUSEEVENTF_TMASK       = 0xFFFFFF00;
        public const uint MOUSEEVENTF_FROMTABLET  = 0xFF515700;
        
        public LowLevelMouseHook(Func<Event, MSLLHOOKSTRUCT, Result> userCallback) : 
            base
            (
                HookType.WH_MOUSE_LL,
                (wParam, lParam) =>
                {
                    var a = (Event)wParam;
                    var b = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    return userCallback(a, b);
                }
            )
        {

        }
    }

    public class LowLevelKeyboardHook : WindowsHook
    {
        public enum Event
        {
            WM_KEYDOWN    = 0x0100,
            WM_KEYUP      = 0x0101,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP   = 0x0105
        }

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public FLAGS flags;
            public int time;
            public UIntPtr dwExtraInfo;

            public bool FromCreviceApp
                => ((uint)dwExtraInfo & KEYBOARDEVENTF_TMASK) == KEYBOARDEVENTF_CREVICE_APP;
        }

        [Flags]
        public enum FLAGS : int
        {
            LLKHF_EXTENDED = 0x01,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN  = 0x20,
            LLKHF_UP       = 0x80,
        }

        public const uint KEYBOARDEVENTF_CREVICE_APP = 0xFFFFFF00;
        public const uint KEYBOARDEVENTF_TMASK = 0xFFFFFF00;

        public LowLevelKeyboardHook(Func<Event, KBDLLHOOKSTRUCT, Result> userCallback) :
            base
            (
                HookType.WH_KEYBOARD_LL,
                (wParam, lParam) =>
                {
                    var a = (Event)wParam;
                    var b = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                    return userCallback(a, b);
                }
            )
        {

        }
    }
}
