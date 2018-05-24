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

namespace Crevice.UI
{
    using Crevice.Core.FSM;
    using Crevice.Logging;
    using Crevice.UserScript.Keys;
    using WinAPI.WindowsHookEx;

    public class MouseGestureForm : Form
    {
        private bool _hookEnabled = false;
        protected bool HookEnabled
        {
            get =>_hookEnabled;
            set
            {
                if (_hookEnabled != value)
                {
                    if (value)
                    {
                        KeyboardHook.SetHook();
                        MouseHook.SetHook();
                        _hookEnabled = true;
                    }
                    else
                    {
                        KeyboardHook.Unhook();
                        MouseHook.Unhook();
                        _hookEnabled = false;
                    }
                }
            }
        }

        private readonly Core.Events.NullEvent NullEvent = new Core.Events.NullEvent();
        
        public virtual IGestureMachine GestureMachine { get; }

        private readonly LowLevelKeyboardHook KeyboardHook;
        private readonly LowLevelMouseHook MouseHook;
        
        public MouseGestureForm()
        {
            KeyboardHook = new LowLevelKeyboardHook(KeyboardProc);
            MouseHook = new LowLevelMouseHook(MouseProc);
        }
        
        protected const int WM_DISPLAYCHANGE = 0x007E;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DISPLAYCHANGE:
                    if (GestureMachine != null)
                    {
                        Verbose.Print("WndProc: WM_DISPLAYCHANGE");
                        GestureMachine?.Reset();
                        Verbose.Print("GestureMachine was reset.");
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        public WindowsHook.Result KeyboardProc(LowLevelKeyboardHook.Event evnt, LowLevelKeyboardHook.KBDLLHOOKSTRUCT data)
        {
            if (data.FromCreviceApp)
            {
                Verbose.Print($"KeyboardEvent(vkCode={data.vkCode}, " +
                    $"event={Enum.GetName(typeof(LowLevelKeyboardHook.Event), evnt)}, " +
                    $"dwExtraInfo={BitConverter.ToString(BitConverter.GetBytes((int)data.dwExtraInfo))}) " +
                    $"was passed to the next hook because this event has the signature of CreviceApp");
                return WindowsHook.Result.Transfer;
            }

            var keyCode = data.vkCode;
            if (keyCode < 8 || keyCode > 255)
            {
                return WindowsHook.Result.Transfer;
            }

            var key = SupportedKeys.PhysicalKeys[keyCode];

            switch (evnt)
            {
                case LowLevelKeyboardHook.Event.WM_KEYDOWN:
                    return ToHookResult(GestureMachine.Input(key.PressEvent));

                case LowLevelKeyboardHook.Event.WM_SYSKEYDOWN:
                    return ToHookResult(GestureMachine.Input(key.PressEvent));

                case LowLevelKeyboardHook.Event.WM_KEYUP:
                    return ToHookResult(GestureMachine.Input(key.ReleaseEvent));

                case LowLevelKeyboardHook.Event.WM_SYSKEYUP:
                    return ToHookResult(GestureMachine.Input(key.ReleaseEvent));
            }
            return WindowsHook.Result.Transfer;
        }

        public WindowsHook.Result MouseProc(LowLevelMouseHook.Event evnt, LowLevelMouseHook.MSLLHOOKSTRUCT data)
        {
            if (data.FromCreviceApp)
            {
                Verbose.Print($"KeyboardEvent(event={Enum.GetName(typeof(LowLevelKeyboardHook.Event), evnt)}, " +
                    $"dwExtraInfo={BitConverter.ToString(BitConverter.GetBytes((int)data.dwExtraInfo))}) " +
                    "was passed to the next hook because this event has the signature of CreviceApp");
                return WindowsHook.Result.Transfer;
            }
            else if (data.FromTablet)
            {
                Verbose.Print($"KeyboardEvent(event={Enum.GetName(typeof(LowLevelKeyboardHook.Event), evnt)}, " +
                    $"dwExtraInfo={BitConverter.ToString(BitConverter.GetBytes((int)data.dwExtraInfo))}) " +
                   "was passed to the next hook because this event has the signature of Tablet");
                return WindowsHook.Result.Transfer;
            }

            var point = new Point(data.pt.x, data.pt.y);

            switch (evnt)
            {
                case LowLevelMouseHook.Event.WM_MOUSEMOVE:
                    return ToHookResult(GestureMachine.Input(NullEvent, point));
                case LowLevelMouseHook.Event.WM_LBUTTONDOWN:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.LButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_LBUTTONUP:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.LButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_RBUTTONDOWN:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_RBUTTONUP:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.RButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_MBUTTONDOWN:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.MButton.PressEvent, point));
                case LowLevelMouseHook.Event.WM_MBUTTONUP:
                    return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.MButton.ReleaseEvent, point));
                case LowLevelMouseHook.Event.WM_MOUSEWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.WheelDown.FireEvent, point));
                    }
                    else
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.WheelUp.FireEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONDOWN:
                    if (data.mouseData.asXButton.IsXButton1)
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.XButton1.PressEvent, point));
                    }
                    else
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.XButton2.PressEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_XBUTTONUP:
                    if (data.mouseData.asXButton.IsXButton1)
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.XButton1.ReleaseEvent, point));
                    }
                    else
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.XButton2.ReleaseEvent, point));
                    }
                case LowLevelMouseHook.Event.WM_MOUSEHWHEEL:
                    if (data.mouseData.asWheelDelta.delta < 0)
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.WheelRight.FireEvent, point));
                    }
                    else
                    {
                        return ToHookResult(GestureMachine.Input(SupportedKeys.PhysicalKeys.WheelLeft.FireEvent, point));
                    }
            }
            return WindowsHook.Result.Transfer;
        }

        protected WindowsHook.Result ToHookResult(bool consumed)
            => consumed ? WindowsHook.Result.Cancel : WindowsHook.Result.Transfer;
    }
}
