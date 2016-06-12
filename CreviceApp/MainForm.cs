using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp
{
    using WinAPI.WindowsHookEx;

    public partial class MainForm : Form
    {
        private static readonly MainForm instance = new MainForm();
        public static MainForm Instance
        {
            get
            {
                return instance;
            }
        }

        private const int WM_DISPLAYCHANGE = 0x007E;

        private readonly LowLevelMouseHook mouseHook;
        private readonly Core.FSM.GestureMachine GestureMachine;

        private readonly UI.TooltipNotifier tooltip;

        public MainForm()
        {
            Trace.Listeners.Clear();
            Trace.Listeners.Add(new Logging.CustomConsoleTraceListener());

            this.FormClosing += OnClosing;
            this.tooltip = new UI.TooltipNotifier(this);
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.GestureMachine = new Core.FSM.GestureMachine(new User.UserConfig().GetGestureDefinition());

            try
            {
                mouseHook.SetHook();
            }
            catch(Win32Exception)
            {
                MessageBox.Show("Fatal error: SetWindowsHookEX(WH_MOUSE_LL) was failed.");
                Application.Exit();
            }

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
                Debug.Print("{0} was ignored because this event has the signature of CreviceApp", Enum.GetName(typeof(LowLevelMouseHook.Event), evnt));
                return WindowsHook.Result.Determine;
            }

            switch (evnt)
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
            mouseHook.Unhook();
            GestureMachine.Dispose();
        }

        private void InvokeProperly(MethodInvoker invoker)
        {
            if (InvokeRequired)
            {
                Invoke(invoker);
            }
            else
            {
                invoker.Invoke();
            }
        }

        public void ShowTooltip(string text, Point point, int duration)
        {
            var invoker = (MethodInvoker)delegate()
            {
                tooltip.Show(text, point, duration);
            };
            InvokeProperly(invoker);
        }

        public void ShowBaloon(string text, int timeout)
        {
            var invoker = (MethodInvoker)delegate ()
            {
                notifyIcon1.BalloonTipText = text;
                notifyIcon1.ShowBalloonTip(timeout);
            };
            InvokeProperly(invoker);
        }
    }
}
