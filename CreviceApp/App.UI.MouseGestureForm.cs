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


namespace CreviceApp.App
{
    using WinAPI.WindowsHookEx;

    public class MouseGestureForm : Form
    {
        private bool _enableHook = false;
        protected bool EnableHook
        {
            get { return _enableHook; }
            set
            {
                if (_enableHook != value)
                {
                    if (value)
                    {
                        keyboardHook.SetHook();
                        mouseHook.SetHook();
                        _enableHook = true;
                    }
                    else
                    {
                        keyboardHook.Unhook();
                        mouseHook.Unhook();
                        _enableHook = false;
                    }
                }
            }
        }

        private readonly Crevice.Core.Events.NullEvent nullEvent = new Crevice.Core.Events.NullEvent();

        private readonly LowLevelKeyboardHook keyboardHook;
        private readonly LowLevelMouseHook mouseHook;
        protected readonly AppConfig appConfig;
        protected readonly Core.ReloadableGestureMachine reloadableGestureMachine;

        public MouseGestureForm(AppConfig appConfig)
        {
            this.keyboardHook = new LowLevelKeyboardHook(KeyboardProc);
            this.mouseHook = new LowLevelMouseHook(MouseProc);
            this.appConfig = appConfig;
            this.reloadableGestureMachine = new Core.ReloadableGestureMachine(appConfig);
        }
        
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            reloadableGestureMachine.Dispose();
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
                    reloadableGestureMachine.Instance.Reset();
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

        public WindowsHook.Result KeyboardProc(LowLevelKeyboardHook.Event evnt, LowLevelKeyboardHook.KBDLLHOOKSTRUCT data)
        {
            Debug.Print("KeyboardEvent: {0} - {1} | {2}",
                    data.vkCode,
                    Enum.GetName(typeof(LowLevelKeyboardHook.Event), evnt),
                    BitConverter.ToString(BitConverter.GetBytes((uint)data.dwExtraInfo))
                    );

            if (data.FromCreviceApp)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of CreviceApp",
                    Enum.GetName(typeof(LowLevelKeyboardHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }

            var keyCode = data.vkCode;
            if (keyCode < 8 || keyCode > 255)
            {
                return WindowsHook.Result.Transfer;
            }

            var key = SupportedKeys.PhysicalKeys[(int)keyCode];

            switch (evnt)
            {
                case LowLevelKeyboardHook.Event.WM_KEYDOWN:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(key.PressEvent));

                case LowLevelKeyboardHook.Event.WM_SYSKEYDOWN:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(key.PressEvent));

                case LowLevelKeyboardHook.Event.WM_KEYUP:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(key.ReleaseEvent));

                case LowLevelKeyboardHook.Event.WM_SYSKEYUP:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(key.ReleaseEvent));
            }
            return WindowsHook.Result.Transfer;
        }

        public WindowsHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            Debug.Print("MouseEvent: {0} | {1}",
                    Enum.GetName(typeof(LowLevelMouseHook.Event), evnt),
                    BitConverter.ToString(BitConverter.GetBytes((uint)data.dwExtraInfo))
                    );

            if (data.FromCreviceApp)
            {
                Debug.Print("{0} was passed to the next hook because this event has the signature of CreviceApp",
                    Enum.GetName(typeof(LowLevelMouseHook.Event),
                    evnt));
                return WindowsHook.Result.Transfer;
            }
            else if (data.FromTablet)
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
                    return ToHookResult(reloadableGestureMachine.Instance.Input(nullEvent, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.LeftButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.LeftButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.RightButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.RightButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.MiddleButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.MiddleButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.WheelDown.FireEvent, point));
                    }
                    else
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.WheelUp.FireEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.IsXButton1)
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.X1Button.PressEvent, point));
                    }
                    else
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.X2Button.PressEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.IsXButton1)
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.X1Button.ReleaseEvent, point));
                    }
                    else
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.X2Button.ReleaseEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.WheelRight.FireEvent, point));
                    }
                    else
                    {
                        return ToHookResult(reloadableGestureMachine.Instance.Input(SupportedKeys.PhysicalKeys.WheelLeft.FireEvent, point));
                    }
            }
            return WindowsHook.Result.Transfer;
        }

        protected WindowsHook.Result ToHookResult(bool consumed)
            => consumed ? WindowsHook.Result.Cancel : WindowsHook.Result.Transfer;
    }
}
