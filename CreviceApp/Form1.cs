using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    public partial class Form1 : Form
    {
        readonly WindowsApplication winApp;
        readonly LowLevelMouseHook hook;
        Core.Stroke.StrokeWatcher strokeWatcher;

        public Form1()
        {
            winApp = new WindowsApplication();
            hook = new LowLevelMouseHook(MouseProc);
            hook.SetHook();

            Task.Run(() => {
                var inputSender = new SingleInputSender();

                Thread.Sleep(5000);
                /*
                inputSender.LeftDown();
                inputSender.LeftUp();
                inputSender.RightDown();
                inputSender.RightUp();
                */

                ushort VK_MENU    = 0x12;
                ushort VK_TAB     = 0x09;
                ushort VK_CONTROL = 0x11;
                ushort N_0        = 0x30;
                ushort VK_LWIN    = 0x5B;

                /*
                inputSender.ExtendedKeyDown(N_0);
                inputSender.ExtendedKeyUp(N_0);

                Thread.Sleep(1000);

                inputSender.ExtendedKeyDown(VK_CONTROL);
                Thread.Sleep(1000);
                inputSender.ExtendedKeyDown(VK_TAB);
                Thread.Sleep(1000);
                inputSender.ExtendedKeyUp(VK_TAB);
                Thread.Sleep(1000);
                inputSender.ExtendedKeyUp(VK_CONTROL);

                inputSender.ExtendedKeyDown(N_0);
                inputSender.ExtendedKeyUp(N_0);

                */

                /*
                new InputSequenceBuilder()
                  .KeyDownWithScanCode(VK_CONTROL)
                  .KeyDownWithScanCode(VK_TAB)
                  .KeyUpWithScanCode(VK_TAB)
                  .KeyUpWithScanCode(VK_CONTROL)
                  .Send();
                */

                /*
                Thread.Sleep(5000);
                inputSender.KeyDown(VK_LWIN);
                inputSender.KeyUp(VK_LWIN);
                Thread.Sleep(5000);
                inputSender.ExtendedKeyDown(VK_LWIN);
                inputSender.ExtendedKeyUp(VK_LWIN);
                Thread.Sleep(5000);
                inputSender.ExtendedKeyDownWithScanCode(VK_LWIN);
                inputSender.ExtendedKeyUpWithScanCode(VK_LWIN);
                */

                //inputSender.UnicodeKeyStroke("🍣🍣🍣🍣");
                inputSender.Wheel(2400);
            });

            InitializeComponent();
        }

        /**
         * 
         * APP     : App((x) => {})  ( ON )
         * 
         * ON      : @on(BUTTON)     ( DO | IF | STROKE )
         * 
         * IF      : @if(BUTTON)     ( DO )
         * 
         * DO      : @do((x) => {}) 
         * 
         * STROKE  : @stroke(MOVE *) ( BY )
         * 
         * BY      : @by(BUTTON)     ( DO )
         * 
         * BUTTON  : L | M | R | X1 | X2 | W_UP | W_DOWN | W_LEFT | W_RIGHT
         * 
         * MOVE    : MOVE_UP | MOVE_DOWN | MOVE_LEFT | MOVE_RIGHT
         *
         */

        public LowLevelMouseHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            /*
            var app = winApp.GetOnCursor(data.pt.x, data.pt.y);
            Debug.Print("process path: {0}", app.path);
            Debug.Print("process name: {0}", app.name);

            Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(data.dwExtraInfo.ToUInt64())));
            Debug.Print("time: {0}", data.time);
            Debug.Print("fromCreviceApp: {0}", data.fromCreviceApp);
            Debug.Print("fromTablet: {0}", data.fromTablet);
            */

            if (strokeWatcher != null)
            {
                strokeWatcher.Queue(data.pt);
            }

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    strokeWatcher = new Core.Stroke.StrokeWatcher(10, 20, 10, 10);
                    break;
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    Debug.Print("Stroke: {0}", strokeWatcher.GetStorke());
                    strokeWatcher.Dispose();
                    strokeWatcher = null;
                    break;
            }

            /*
            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    Debug.Print("{0}: x={1}, y={2}", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt), data.pt.x, data.pt.y);
                    break;
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    Debug.Print("{0}: delta={1}", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt), data.mouseData.asWheelDelta.delta);
                    break;
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    Debug.Print("{0}: type={1}", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt), data.mouseData.asXButton.type);
                    break;
                default:
                    Debug.Print("{0}", evnt);
                    break;
            }
            */
            return LowLevelMouseHook.Result.Transfer;
        } 
    }
}
