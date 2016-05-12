using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

    public class LowLevelMouseHook : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Procedure callback, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);
        
        private delegate IntPtr Procedure(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate Result UserProcedure(Event e, MSLLHOOKSTRUCT data);

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
        
        private readonly Object hookLock = new Object();
        private readonly UserProcedure userProcedure;

        private IntPtr hHook = IntPtr.Zero;
        
        public LowLevelMouseHook(UserProcedure userProcedure)
        {
            this.userProcedure = userProcedure;
        }

        public bool Activated()
        {
            return hHook != IntPtr.Zero;
        }

        public void SetHook()
        {
            lock (hookLock)
            {
                if (Activated())
                {
                    throw new InvalidOperationException();
                }
                var hInstance = Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]);
                Debug.Print("hInstance: 0x{0:X}", hInstance.ToInt64());
                Debug.Print("trying to set a hook(WH_MOUSE_LL)");
                hHook = SetWindowsHookEx(WH_MOUSE_LL, MouseHookProc, hInstance, 0);
                Debug.Print("hHook: 0x{0:X}", hHook.ToInt64());
                if (!Activated())
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Debug.Print("failed to set a hook(WH_MOUSE_LL), ErrorCode: {0}", errorCode);
                    throw new Win32Exception(errorCode);
                }
            }
        }

        public void Unhook()
        {
            lock (hookLock)
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
            }
        }

        public IntPtr MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            lock (hookLock)
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
            lock (hookLock)
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
