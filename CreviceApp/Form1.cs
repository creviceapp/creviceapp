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

        public Form1()
        {
            winApp = new WindowsApplication();
            hook = new LowLevelMouseHook(MouseProc);
            hook.SetHook();


            var eventSender = new EventSender();
            Task.Run(() => {
                Thread.Sleep(1000);
                eventSender.LeftDown();
                eventSender.LeftUp();
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
            var app = winApp.GetOnCursor(data.pt.x, data.pt.y);
            Debug.Print("process path: {0}", app.path);
            Debug.Print("process name: {0}", app.name);

            Debug.Print("dwExtraInfo: {0}", BitConverter.ToString(BitConverter.GetBytes(data.dwExtraInfo.ToUInt64())));
            Debug.Print("time: {0}", data.time);
            Debug.Print("fromCreviceApp: {0}", data.fromCreviceApp);
            Debug.Print("fromTablet: {0}", data.fromTablet);

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
                    Debug.Print("{0}: delta={1}", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt), data.mouseData.higher);
                    break;
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    Debug.Print("{0}: type={1}", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt), data.mouseData.higher);
                    break;
                default:
                    Debug.Print("{0}", evnt);
                    break;
            }
            return LowLevelMouseHook.Result.Transfer;
        } 
    }
}
