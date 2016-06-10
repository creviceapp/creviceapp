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
        const int WM_DISPLAYCHANGE = 0x007E;

        readonly LowLevelMouseHook MouseHook;
        readonly Core.FSM.GestureMachine GestureMachine;

        public Form1()
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new Logging.CustomConsoleTraceListener());

            var gestureDef = new List<Core.FSM.GestureDefinition>() {
                new Core.FSM.ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelUp,
                    () => 
                    {
                        new InputSequenceBuilder()
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_CONTROL)
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_SHIFT)
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_TAB)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_TAB)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_SHIFT)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_CONTROL)
                            .Send();
                    }),
                new Core.FSM.ButtonGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    DSL.Def.Constant.WheelDown,
                    () =>
                    {
                        new InputSequenceBuilder()
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_CONTROL)
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_TAB)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_TAB)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_CONTROL)
                            .Send();
                    }),
                new Core.FSM.StrokeGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    new Core.Def.Stroke(new List<Core.Def.Direction>() { Core.Def.Direction.Up }),
                    () =>
                    {
                        new InputSequenceBuilder()
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_HOME)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_HOME)
                            .Send();
                    }),
                new Core.FSM.StrokeGestureDefinition(
                    () => { return true; },
                    DSL.Def.Constant.RightButton,
                    new Core.Def.Stroke(new List<Core.Def.Direction>() { Core.Def.Direction.Down }),
                    () =>
                    {
                        new InputSequenceBuilder()
                            .ExtendedKeyDown(InputSender.VirtualKeys.VK_END)
                            .ExtendedKeyUp(InputSender.VirtualKeys.VK_END)
                            .Send();
                    })
            };

            FormClosing += OnClosing;
            MouseHook = new LowLevelMouseHook(MouseProc);
            GestureMachine = new Core.FSM.GestureMachine(gestureDef);

            MouseHook.SetHook();

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    GestureMachine.Reset();
                    break;
            }
            base.WndProc(ref m);
        }

        public LowLevelMouseHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            if (data.fromCreviceApp)
            {
                Debug.Print("{0} ignored because this event has the signature of CreviceApp", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt));
                return WindowsHook.Result.Determine;
            }

            /*
            foreach (var window in new List<WinAPI.Application.WindowInfo>() {
                new WinAPI.Application.ForegroundWindowInfo(),
                new WinAPI.Application.OnCursorWindowInfo(data.pt.x, data.pt.y) })
            {
                Debug.Print("{0}", window.GetType().Name);
                Debug.Print("Handle: 0x{0:X}", window.Handle.ToInt64());
                Debug.Print("Id: 0x{0:X}", window.Id);
                Debug.Print("Class name: {0}", window.ClassName);
                //Debug.Print("Parent: {0}", window.Parent);
                Debug.Print("Process id: 0x{0:X}", window.ProcessId);
                Debug.Print("Thread id: 0x{0:X}", window.ThreadId);
                Debug.Print("Text: {0}", window.Text);
                Debug.Print("Name: {0}", window.Module.Name);
                Debug.Print("Path: {0}", window.Module.Path);
            }
            */

            switch(evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return Convert(GestureMachine.Input(Core.Def.Constant.Move, data.pt));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonDown, data.pt));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonUp, data.pt));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonDown, data.pt));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonUp, data.pt));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonDown, data.pt));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonUp, data.pt));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelDown, data.pt));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelUp, data.pt));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonDown, data.pt));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonDown, data.pt));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonUp, data.pt));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonUp, data.pt));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelRight, data.pt));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelLeft, data.pt));
                    }
            }
            return LowLevelMouseHook.Result.Transfer;
        } 

        private LowLevelMouseHook.Result Convert(bool consumed)
        {
            if (consumed)
            {
                return LowLevelMouseHook.Result.Cancel;
            }
            else
            {
                return LowLevelMouseHook.Result.Transfer;
            }
        }

        public void OnClosing(object sender, CancelEventArgs e)
        {
            MouseHook.Unhook();
            GestureMachine.Dispose();
        }
    }
}
