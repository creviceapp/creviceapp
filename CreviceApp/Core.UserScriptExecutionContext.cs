using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreviceApp.Core
{
    using VK = WinAPI.SendInput.VirtualKeys;

    public class UserScriptExecutionContext
    {
        public const ushort VK_LBUTTON              = VK.VK_LBUTTON;
        public const ushort VK_RBUTTON              = VK.VK_RBUTTON;
        public const ushort VK_CANCEL               = VK.VK_CANCEL;
        public const ushort VK_MBUTTON              = VK.VK_MBUTTON;
        public const ushort VK_XBUTTON1             = VK.VK_XBUTTON1;
        public const ushort VK_XBUTTON2             = VK.VK_XBUTTON2;
        public const ushort VK_BACK                 = VK.VK_BACK;
        public const ushort VK_TAB                  = VK.VK_TAB;
        public const ushort VK_CLEAR                = VK.VK_CLEAR;
        public const ushort VK_RETURN               = VK.VK_RETURN;
        public const ushort VK_SHIFT                = VK.VK_SHIFT;
        public const ushort VK_CONTROL              = VK.VK_CONTROL;
        public const ushort VK_MENU                 = VK.VK_MENU;
        public const ushort VK_PAUSE                = VK.VK_PAUSE;
        public const ushort VK_CAPITAL              = VK.VK_CAPITAL;
        public const ushort VK_KANA                 = VK.VK_KANA;
        public const ushort VK_HANGUEL              = VK.VK_HANGUEL;
        public const ushort VK_HANGUL               = VK.VK_HANGUL;
        public const ushort VK_JUNJA                = VK.VK_JUNJA;
        public const ushort VK_FINAL                = VK.VK_FINAL;
        public const ushort VK_HANJA                = VK.VK_HANJA;
        public const ushort VK_KANJI                = VK.VK_KANJI;
        public const ushort VK_ESCAPE               = VK.VK_ESCAPE;
        public const ushort VK_CONVERT              = VK.VK_CONVERT;
        public const ushort VK_NONCONVERT           = VK.VK_NONCONVERT;
        public const ushort VK_ACCEPT               = VK.VK_ACCEPT;
        public const ushort VK_MODECHANGE           = VK.VK_MODECHANGE;
        public const ushort VK_SPACE                = VK.VK_SPACE;
        public const ushort VK_PRIOR                = VK.VK_PRIOR;
        public const ushort VK_NEXT                 = VK.VK_NEXT;
        public const ushort VK_END                  = VK.VK_END;
        public const ushort VK_HOME                 = VK.VK_HOME;
        public const ushort VK_LEFT                 = VK.VK_LEFT;
        public const ushort VK_UP                   = VK.VK_UP;
        public const ushort VK_RIGHT                = VK.VK_RIGHT;
        public const ushort VK_DOWN                 = VK.VK_DOWN;
        public const ushort VK_SELECT               = VK.VK_SELECT;
        public const ushort VK_PRINT                = VK.VK_PRINT;
        public const ushort VK_EXECUTE              = VK.VK_EXECUTE;
        public const ushort VK_SNAPSHOT             = VK.VK_SNAPSHOT;
        public const ushort VK_INSERT               = VK.VK_INSERT;
        public const ushort VK_DELETE               = VK.VK_DELETE;
        public const ushort VK_HELP                 = VK.VK_HELP;
        public const ushort VK_0                    = VK.VK_0;
        public const ushort VK_1                    = VK.VK_1;
        public const ushort VK_2                    = VK.VK_2;
        public const ushort VK_3                    = VK.VK_3;
        public const ushort VK_4                    = VK.VK_4;
        public const ushort VK_5                    = VK.VK_5;
        public const ushort VK_6                    = VK.VK_6;
        public const ushort VK_7                    = VK.VK_7;
        public const ushort VK_8                    = VK.VK_8;
        public const ushort VK_9                    = VK.VK_9;
        public const ushort VK_A                    = VK.VK_A;
        public const ushort VK_B                    = VK.VK_B;
        public const ushort VK_C                    = VK.VK_C;
        public const ushort VK_D                    = VK.VK_D;
        public const ushort VK_E                    = VK.VK_E;
        public const ushort VK_F                    = VK.VK_F;
        public const ushort VK_G                    = VK.VK_G;
        public const ushort VK_H                    = VK.VK_H;
        public const ushort VK_I                    = VK.VK_I;
        public const ushort VK_J                    = VK.VK_J;
        public const ushort VK_K                    = VK.VK_K;
        public const ushort VK_L                    = VK.VK_L;
        public const ushort VK_M                    = VK.VK_M;
        public const ushort VK_N                    = VK.VK_N;
        public const ushort VK_O                    = VK.VK_O;
        public const ushort VK_P                    = VK.VK_P;
        public const ushort VK_Q                    = VK.VK_Q;
        public const ushort VK_R                    = VK.VK_R;
        public const ushort VK_S                    = VK.VK_S;
        public const ushort VK_T                    = VK.VK_T;
        public const ushort VK_U                    = VK.VK_U;
        public const ushort VK_V                    = VK.VK_V;
        public const ushort VK_W                    = VK.VK_W;
        public const ushort VK_X                    = VK.VK_X;
        public const ushort VK_Y                    = VK.VK_Y;
        public const ushort VK_Z                    = VK.VK_Z;
        public const ushort VK_LWIN                 = VK.VK_LWIN;
        public const ushort VK_RWIN                 = VK.VK_RWIN;
        public const ushort VK_APPS                 = VK.VK_APPS;
        public const ushort VK_SLEEP                = VK.VK_SLEEP;
        public const ushort VK_NUMPAD0              = VK.VK_NUMPAD0;
        public const ushort VK_NUMPAD1              = VK.VK_NUMPAD1;
        public const ushort VK_NUMPAD2              = VK.VK_NUMPAD2;
        public const ushort VK_NUMPAD3              = VK.VK_NUMPAD3;
        public const ushort VK_NUMPAD4              = VK.VK_NUMPAD4;
        public const ushort VK_NUMPAD5              = VK.VK_NUMPAD5;
        public const ushort VK_NUMPAD6              = VK.VK_NUMPAD6;
        public const ushort VK_NUMPAD7              = VK.VK_NUMPAD7;
        public const ushort VK_NUMPAD8              = VK.VK_NUMPAD8;
        public const ushort VK_NUMPAD9              = VK.VK_NUMPAD9;
        public const ushort VK_MULTIPLY             = VK.VK_MULTIPLY;
        public const ushort VK_ADD                  = VK.VK_ADD;
        public const ushort VK_SEPARATOR            = VK.VK_SEPARATOR;
        public const ushort VK_SUBTRACT             = VK.VK_SUBTRACT;
        public const ushort VK_DECIMAL              = VK.VK_DECIMAL;
        public const ushort VK_DIVIDE               = VK.VK_DIVIDE;
        public const ushort VK_F1                   = VK.VK_F1;
        public const ushort VK_F2                   = VK.VK_F2;
        public const ushort VK_F3                   = VK.VK_F3;
        public const ushort VK_F4                   = VK.VK_F4;
        public const ushort VK_F5                   = VK.VK_F5;
        public const ushort VK_F6                   = VK.VK_F6;
        public const ushort VK_F7                   = VK.VK_F7;
        public const ushort VK_F8                   = VK.VK_F8;
        public const ushort VK_F9                   = VK.VK_F9;
        public const ushort VK_F10                  = VK.VK_F10;
        public const ushort VK_F11                  = VK.VK_F11;
        public const ushort VK_F12                  = VK.VK_F12;
        public const ushort VK_F13                  = VK.VK_F13;
        public const ushort VK_F14                  = VK.VK_F14;
        public const ushort VK_F15                  = VK.VK_F15;
        public const ushort VK_F16                  = VK.VK_F16;
        public const ushort VK_F17                  = VK.VK_F17;
        public const ushort VK_F18                  = VK.VK_F18;
        public const ushort VK_F19                  = VK.VK_F19;
        public const ushort VK_F20                  = VK.VK_F20;
        public const ushort VK_F21                  = VK.VK_F21;
        public const ushort VK_F22                  = VK.VK_F22;
        public const ushort VK_F23                  = VK.VK_F23;
        public const ushort VK_F24                  = VK.VK_F24;
        public const ushort VK_NUMLOCK              = VK.VK_NUMLOCK;
        public const ushort VK_SCROLL               = VK.VK_SCROLL;
        public const ushort VK_LSHIFT               = VK.VK_LSHIFT;
        public const ushort VK_RSHIFT               = VK.VK_RSHIFT;
        public const ushort VK_LCONTROL             = VK.VK_LCONTROL;
        public const ushort VK_RCONTROL             = VK.VK_RCONTROL;
        public const ushort VK_LMENU                = VK.VK_LMENU;
        public const ushort VK_RMENU                = VK.VK_RMENU;
        public const ushort VK_BROWSER_BACK         = VK.VK_BROWSER_BACK;
        public const ushort VK_BROWSER_FORWARD      = VK.VK_BROWSER_FORWARD;
        public const ushort VK_BROWSER_REFRESH      = VK.VK_BROWSER_REFRESH;
        public const ushort VK_BROWSER_STOP         = VK.VK_BROWSER_STOP;
        public const ushort VK_BROWSER_SEARCH       = VK.VK_BROWSER_SEARCH;
        public const ushort VK_BROWSER_FAVORITES    = VK.VK_BROWSER_FAVORITES;
        public const ushort VK_BROWSER_HOME         = VK.VK_BROWSER_HOME;
        public const ushort VK_VOLUME_MUTE          = VK.VK_VOLUME_MUTE;
        public const ushort VK_VOLUME_DOWN          = VK.VK_VOLUME_DOWN;
        public const ushort VK_VOLUME_UP            = VK.VK_VOLUME_UP;
        public const ushort VK_MEDIA_NEXT_TRACK     = VK.VK_MEDIA_NEXT_TRACK;
        public const ushort VK_MEDIA_PREV_TRACK     = VK.VK_MEDIA_PREV_TRACK;
        public const ushort VK_MEDIA_STOP           = VK.VK_MEDIA_STOP;
        public const ushort VK_MEDIA_PLAY_PAUSE     = VK.VK_MEDIA_PLAY_PAUSE;
        public const ushort VK_LAUNCH_MAIL          = VK.VK_LAUNCH_MAIL;
        public const ushort VK_LAUNCH_MEDIA_SELECT  = VK.VK_LAUNCH_MEDIA_SELECT;
        public const ushort VK_LAUNCH_APP1          = VK.VK_LAUNCH_APP1;
        public const ushort VK_LAUNCH_APP2          = VK.VK_LAUNCH_APP2;
        public const ushort VK_OEM_1                = VK.VK_OEM_1;
        public const ushort VK_OEM_COMMA            = VK.VK_OEM_COMMA;
        public const ushort VK_OEM_MINUS            = VK.VK_OEM_MINUS;
        public const ushort VK_OEM_PERIOD           = VK.VK_OEM_PERIOD;
        public const ushort VK_OEM_2                = VK.VK_OEM_2;
        public const ushort VK_OEM_3                = VK.VK_OEM_3;
        public const ushort VK_OEM_4                = VK.VK_OEM_4;
        public const ushort VK_OEM_5                = VK.VK_OEM_5;
        public const ushort VK_OEM_6                = VK.VK_OEM_6;
        public const ushort VK_OEM_7                = VK.VK_OEM_7;
        public const ushort VK_OEM_8                = VK.VK_OEM_8;
        public const ushort VK_OEM_102              = VK.VK_OEM_102;
        public const ushort VK_PROCESSKEY           = VK.VK_PROCESSKEY;
        public const ushort VK_PACKET               = VK.VK_PACKET;
        public const ushort VK_ATTN                 = VK.VK_ATTN;
        public const ushort VK_CRSEL                = VK.VK_CRSEL;
        public const ushort VK_EXSEL                = VK.VK_EXSEL;
        public const ushort VK_EREOF                = VK.VK_EREOF;
        public const ushort VK_PLAY                 = VK.VK_PLAY;
        public const ushort VK_ZOOM                 = VK.VK_ZOOM;
        public const ushort VK_NONAME               = VK.VK_NONAME;
        public const ushort VK_PA1                  = VK.VK_PA1;
        public const ushort VK_OEM_CLEAR            = VK.VK_OEM_CLEAR;

        public readonly DSL.Def.LeftButton   LeftButton   = DSL.Def.Constant.LeftButton;
        public readonly DSL.Def.MiddleButton MiddleButton = DSL.Def.Constant.MiddleButton;
        public readonly DSL.Def.RightButton  RightButton  = DSL.Def.Constant.RightButton;
        public readonly DSL.Def.WheelDown    WheelDown    = DSL.Def.Constant.WheelDown;
        public readonly DSL.Def.WheelUp      WheelUp      = DSL.Def.Constant.WheelUp;
        public readonly DSL.Def.WheelLeft    WheelLeft    = DSL.Def.Constant.WheelLeft;
        public readonly DSL.Def.WheelRight   WheelRight   = DSL.Def.Constant.WheelRight;
        public readonly DSL.Def.X1Button     X1Button     = DSL.Def.Constant.X1Button;
        public readonly DSL.Def.X2Button     X2Button     = DSL.Def.Constant.X2Button;

        public readonly DSL.Def.MoveUp    MoveUp    = DSL.Def.Constant.MoveUp;
        public readonly DSL.Def.MoveDown  MoveDown  = DSL.Def.Constant.MoveDown;
        public readonly DSL.Def.MoveLeft  MoveLeft  = DSL.Def.Constant.MoveLeft;
        public readonly DSL.Def.MoveRight MoveRight = DSL.Def.Constant.MoveRight;

        public readonly WinAPI.SendInput.SingleInputSender SendInput = new WinAPI.SendInput.SingleInputSender();
        public readonly WinAPI.CoreAudioAPI.VolumeControl WaveVolume = new WinAPI.CoreAudioAPI.VolumeControl();
        
        private readonly DSL.Root root = new DSL.Root();

        private readonly AppGlobal Global;
        
        public UserScriptExecutionContext(AppGlobal Global)
        {
            this.Global = Global;
        }
        
        public IEnumerable<GestureDefinition> GetGestureDefinition()
        {
            return DSLTreeParser.TreeToGestureDefinition(root)
                .Where(x => x.IsComplete)
                .ToList();
        }

        public DSL.WhenElement @when(DSL.Def.WhenFunc func)
        {
            return root.@when(func);
        }

        public void Tooltip(string text)
        {
            Tooltip(text, Global.UserConfig.UI.TooltipPositionBinding(Cursor.Position));
        }

        public void Tooltip(string text, Point point)
        {
            Tooltip(text, point, Global.UserConfig.UI.TooltipTimeout);
        }

        public void Tooltip(string text, Point point, int duration)
        {
            Global.MainForm.ShowTooltip(text, point, duration);
        }

        public void Baloon(string text)
        {
            Baloon(text, Global.UserConfig.UI.BaloonTimeout);
        }

        public void Baloon(string text, int timeout)
        {
            Global.MainForm.ShowBaloon(text, "", ToolTipIcon.None, timeout);
        }
    }
}
