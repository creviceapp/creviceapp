using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CreviceApp
{
    using Core.App;
    using WinAPI.WindowsHookEx;

    public class MouseGestureForm : Form
    {
        private bool _captureMouse = false;
        protected bool CaptureMouse
        {
            get { return _captureMouse; }
            set
            {
                if (_captureMouse != value)
                {
                    if (value)
                    {
                        mouseHook.SetHook();
                        _captureMouse = true;
                    }
                    else
                    {
                        mouseHook.Unhook();
                        _captureMouse = false;
                    }
                }
            }
        }

        private readonly LowLevelMouseHook mouseHook;
        protected readonly AppGlobal Global;
        public readonly UserScript UserScript;
        protected readonly ReloadableGestureMachine ReloadableGestureMachine;

        public MouseGestureForm(AppGlobal Global)
        {
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.Global = Global;
            this.UserScript = new UserScript(Global);
            this.ReloadableGestureMachine = new ReloadableGestureMachine(Global, UserScript);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ReloadableGestureMachine.Dispose();
        }

        protected const int WM_DISPLAYCHANGE = 0x007E;
        protected const int WM_POWERBROADCAST = 0x0218;

        protected const int PBT_APMQUERYSUSPEND = 0x0000;
        protected const int PBT_APMQUERYSTANDBY = 0x0001;
        protected const int PBT_APMQUERYSUSPENDFAILED = 0x0002;
        protected const int PBT_APMQUERYSTANDBYFAILED = 0x0003;
        protected const int PBT_APMSUSPEND = 0x0004;
        protected const int PBT_APMSTANDBY = 0x0005;
        protected const int PBT_APMRESUMECRITICAL = 0x0006;
        protected const int PBT_APMRESUMESUSPEND = 0x0007;
        protected const int PBT_APMRESUMESTANDBY = 0x0008;
        protected const int PBT_APMBATTERYLOW = 0x0009;
        protected const int PBT_APMPOWERSTATUSCHANGE = 0x000A;
        protected const int PBT_APMOEMEVENT = 0x000B;
        protected const int PBT_APMRESUMEAUTOMATIC = 0x0012;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    Verbose.Print("WndProc: WM_DISPLAYCHANGE");
                    ReloadableGestureMachine.Instance.Reset();
                    Verbose.Print("GestureMachine was reset.");
                    break;

                case WM_POWERBROADCAST:
                    int reason = m.WParam.ToInt32();
                    switch(reason)
                    {
                        case PBT_APMQUERYSUSPEND:
                            Verbose.Print("WndProc: PBT_APMQUERYSUSPEND");
                            break;
                        case PBT_APMQUERYSTANDBY:
                            Verbose.Print("WndProc: PBT_APMQUERYSTANDBY");
                            break;
                        case PBT_APMQUERYSUSPENDFAILED:
                            Verbose.Print("WndProc: PBT_APMQUERYSUSPENDFAILED");
                            break;
                        case PBT_APMQUERYSTANDBYFAILED:
                            Verbose.Print("WndProc: PBT_APMQUERYSTANDBYFAILED");
                            break;
                        case PBT_APMSUSPEND:
                            Verbose.Print("WndProc: PBT_APMSUSPEND");
                            break;
                        case PBT_APMSTANDBY:
                            Verbose.Print("WndProc: PBT_APMSTANDBY");
                            break;
                        case PBT_APMRESUMECRITICAL:
                            Verbose.Print("WndProc: PBT_APMRESUMECRITICAL");
                            break;
                        case PBT_APMRESUMESUSPEND:
                            Verbose.Print("WndProc: PBT_APMRESUMESUSPEND");
                            break;
                        case PBT_APMRESUMESTANDBY:
                            Verbose.Print("WndProc: PBT_APMRESUMESTANDBY");
                            break;
                        case PBT_APMBATTERYLOW:
                            Verbose.Print("WndProc: PBT_APMBATTERYLOW");
                            break;
                        case PBT_APMPOWERSTATUSCHANGE:
                            Verbose.Print("WndProc: PBT_APMPOWERSTATUSCHANGE");
                            break;
                        case PBT_APMOEMEVENT:
                            Verbose.Print("WndProc: PBT_APMOEMEVENT");
                            break;
                        case PBT_APMRESUMEAUTOMATIC:
                            Verbose.Print("WndProc: PBT_APMRESUMEAUTOMATIC");
                            break;
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        public WindowsHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            Debug.Print("MouseEvent: {0} | {1}",
                    Enum.GetName(typeof(LowLevelMouseHook.Event), evnt),
                    BitConverter.ToString(BitConverter.GetBytes((uint)data.dwExtraInfo))
                    );

            if (data.fromCreviceApp)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of CreviceApp",
                    Enum.GetName(typeof(LowLevelMouseHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }
            else if (data.fromTablet)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of Tablet",
                    Enum.GetName(typeof(LowLevelMouseHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }

            var point = new Point(data.pt.x, data.pt.y);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.Move, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.LeftButtonDown, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.LeftButtonUp, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.RightButtonDown, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.RightButtonUp, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.MiddleButtonDown, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.MiddleButtonUp, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelDown, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelUp, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X1ButtonDown, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X2ButtonDown, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.isXButton1)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X1ButtonUp, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.X2ButtonUp, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelRight, point));
                    }
                    else
                    {
                        return Convert(ReloadableGestureMachine.Instance.Input(Core.Def.Constant.WheelLeft, point));
                    }
            }
            return WindowsHook.Result.Transfer;
        }

        protected WindowsHook.Result Convert(bool consumed)
        {
            if (consumed)
            {
                return WindowsHook.Result.Cancel;
            }
            else
            {
                return WindowsHook.Result.Transfer;
            }
        }
    }
}
