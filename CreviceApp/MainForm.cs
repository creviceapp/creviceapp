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

        private readonly UI.TooltipNotifier tooltip;
        private readonly LowLevelMouseHook mouseHook;
        private readonly Core.FSM.GestureMachine GestureMachine;

        public MainForm()
        {
            this.tooltip = new UI.TooltipNotifier(this);
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.GestureMachine = new Core.FSM.GestureMachine(new User.UserConfig().GetGestureDefinition());
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
                Debug.Print("{0} was ignored because this event has the signature of CreviceApp", 
                    Enum.GetName(typeof(LowLevelMouseHook.Event), 
                    evnt));
                return WindowsHook.Result.Determine;
            }

            var point = new Point(data.pt.x, data.pt.y);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return Convert(GestureMachine.Input(Core.Def.Constant.Move, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonDown, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.LeftButtonUp, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonDown, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.RightButtonUp, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonDown, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return Convert(GestureMachine.Input(Core.Def.Constant.MiddleButtonUp, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelDown, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelUp, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonDown, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonDown, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X1ButtonUp, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.X2ButtonUp, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelRight, point));
                    }
                    else
                    {
                        return Convert(GestureMachine.Input(Core.Def.Constant.WheelLeft, point));
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
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                mouseHook.SetHook();
            }
            catch (Win32Exception)
            {
                MessageBox.Show("SetWindowsHookEX(WH_MOUSE_LL) was failed.",
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            try
            {
                mouseHook.Unhook();
            }
            catch(Win32Exception)
            {
                MessageBox.Show("UnhookWindowsHookEx(WH_MOUSE_LL) was failed.",
                    "Fatal error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            tooltip.Dispose();
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
            var invoker = (MethodInvoker)delegate()
            {
                notifyIcon1.BalloonTipText = text;
                notifyIcon1.ShowBalloonTip(timeout);
            };
            InvokeProperly(invoker);
        }
    }
}
