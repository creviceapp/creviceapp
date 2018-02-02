using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreviceApp
{
    using System.Drawing;
    using Crevice.Core.Context;
    using Crevice.Core.DSL;
    using Crevice.Core.Keys;
    using Crevice.Core.FSM;
    using Crevice.Core.Stroke;

    using CreviceApp.WinAPI.Window.Impl;

    public class LogicalSystemKeys : Keys<LogicalSystemKeys.LogicalKey> 
    {
        public override LogicalKey Create(int index)
            => new LogicalKey(index);

        public LogicalSystemKeys(int maxSize)
            : base(maxSize) { }

        public class LogicalKey : LogicalDoubleThrowKeys.LogicalKey
        {
            public LogicalKey(int keyId)
                : base(keyId) { }

            // todo implicit opeartor to LogicalPressEvent
        }
    }

    public class PhysicalSystemKeys : Keys<PhysicalSystemKeys.PhysicalKey>
    {
        public override PhysicalKey Create(int index)
            => new PhysicalKey(logicalKeys[index], index);

        private LogicalSystemKeys logicalKeys;

        public PhysicalSystemKeys(LogicalSystemKeys logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }

        public class PhysicalKey : PhysicalDoubleThrowKeys.PhysicalKey
        {
            public PhysicalKey(LogicalDoubleThrowKeys.LogicalKey logicalKey, int keyId)
                : base(logicalKey, keyId) { }

            // todo implicit opeartor to PhysicalPressEvent

            // todo implicit operater to Keys, int   
        }
    }

    public class SupportedKeys
    {
        private static SupportedKeys Instance = new SupportedKeys();

        private readonly LogicalKeyDeclaration logicalKeys;
        private readonly PhysicalKeyDeclaration physicalKeys;

        private SupportedKeys()
        {
            var logicalSingleThrowKeys = new LogicalSingleThrowKeys(10);
            var physicalSingleThrowKeys = new PhysicalSingleThrowKeys(logicalSingleThrowKeys);

            var logicalSystemKeys = new LogicalSystemKeys(255);
            var physicalSystemKeys = new PhysicalSystemKeys(logicalSystemKeys);

            logicalKeys = new LogicalKeyDeclaration(logicalSingleThrowKeys, logicalSystemKeys);
            physicalKeys = new PhysicalKeyDeclaration(physicalSingleThrowKeys, physicalSystemKeys);
        }

        public static LogicalKeyDeclaration Keys => Instance.logicalKeys;

        public static PhysicalKeyDeclaration PhysicalKeys => Instance.physicalKeys;

        public class LogicalKeyDeclaration
        {
            private readonly LogicalSingleThrowKeys LogicalSingleThrowKeys;
            private readonly LogicalSystemKeys LogicalSystemKeys;

            public LogicalSingleThrowKeys.LogicalKey WheelUp => LogicalSingleThrowKeys[1];
            public LogicalSingleThrowKeys.LogicalKey WheelDown => LogicalSingleThrowKeys[2];
            public LogicalSingleThrowKeys.LogicalKey WheelLeft => LogicalSingleThrowKeys[3];
            public LogicalSingleThrowKeys.LogicalKey WheelRight => LogicalSingleThrowKeys[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public LogicalSystemKeys.LogicalKey LeftButton => LogicalSystemKeys[1];
            public LogicalSystemKeys.LogicalKey RightButton => LogicalSystemKeys[2];
            public LogicalSystemKeys.LogicalKey MiddleButton => LogicalSystemKeys[4];
            public LogicalSystemKeys.LogicalKey X1Button => LogicalSystemKeys[5];
            public LogicalSystemKeys.LogicalKey X2Button => LogicalSystemKeys[6];

            public LogicalSystemKeys.LogicalKey LButton => LogicalSystemKeys[1];
            public LogicalSystemKeys.LogicalKey RButton => LogicalSystemKeys[2];
            public LogicalSystemKeys.LogicalKey Cancel => LogicalSystemKeys[3];
            public LogicalSystemKeys.LogicalKey MButton => LogicalSystemKeys[4];
            public LogicalSystemKeys.LogicalKey XButton1 => LogicalSystemKeys[5];
            public LogicalSystemKeys.LogicalKey XButton2 => LogicalSystemKeys[6];
            public LogicalSystemKeys.LogicalKey Back => LogicalSystemKeys[8];
            public LogicalSystemKeys.LogicalKey Tab => LogicalSystemKeys[9];
            public LogicalSystemKeys.LogicalKey LineFeed => LogicalSystemKeys[10];
            public LogicalSystemKeys.LogicalKey Clear => LogicalSystemKeys[12];
            public LogicalSystemKeys.LogicalKey Enter => LogicalSystemKeys[13];
            public LogicalSystemKeys.LogicalKey Return => LogicalSystemKeys[13];
            public LogicalSystemKeys.LogicalKey ShiftKey => LogicalSystemKeys[16];
            public LogicalSystemKeys.LogicalKey ControlKey => LogicalSystemKeys[17];
            public LogicalSystemKeys.LogicalKey Menu => LogicalSystemKeys[18];
            public LogicalSystemKeys.LogicalKey Pause => LogicalSystemKeys[19];
            public LogicalSystemKeys.LogicalKey CapsLock => LogicalSystemKeys[20];
            public LogicalSystemKeys.LogicalKey Capital => LogicalSystemKeys[20];
            public LogicalSystemKeys.LogicalKey KanaMode => LogicalSystemKeys[21];
            public LogicalSystemKeys.LogicalKey HangulMode => LogicalSystemKeys[21];
            public LogicalSystemKeys.LogicalKey JunjaMode => LogicalSystemKeys[23];
            public LogicalSystemKeys.LogicalKey FinalMode => LogicalSystemKeys[24];
            public LogicalSystemKeys.LogicalKey KanjiMode => LogicalSystemKeys[25];
            public LogicalSystemKeys.LogicalKey HanjaMode => LogicalSystemKeys[25];
            public LogicalSystemKeys.LogicalKey Escape => LogicalSystemKeys[27];
            public LogicalSystemKeys.LogicalKey IMEConvert => LogicalSystemKeys[28];
            public LogicalSystemKeys.LogicalKey IMENonconvert => LogicalSystemKeys[29];
            public LogicalSystemKeys.LogicalKey IMEAccept => LogicalSystemKeys[30];
            public LogicalSystemKeys.LogicalKey IMEModeChange => LogicalSystemKeys[31];
            public LogicalSystemKeys.LogicalKey Space => LogicalSystemKeys[32];
            public LogicalSystemKeys.LogicalKey Prior => LogicalSystemKeys[33];
            public LogicalSystemKeys.LogicalKey PageUp => LogicalSystemKeys[33];
            public LogicalSystemKeys.LogicalKey Next => LogicalSystemKeys[34];
            public LogicalSystemKeys.LogicalKey PageDown => LogicalSystemKeys[34];
            public LogicalSystemKeys.LogicalKey End => LogicalSystemKeys[35];
            public LogicalSystemKeys.LogicalKey Home => LogicalSystemKeys[36];
            public LogicalSystemKeys.LogicalKey Left => LogicalSystemKeys[37];
            public LogicalSystemKeys.LogicalKey Up => LogicalSystemKeys[38];
            public LogicalSystemKeys.LogicalKey Right => LogicalSystemKeys[39];
            public LogicalSystemKeys.LogicalKey Down => LogicalSystemKeys[40];
            public LogicalSystemKeys.LogicalKey Select => LogicalSystemKeys[41];
            public LogicalSystemKeys.LogicalKey Print => LogicalSystemKeys[42];
            public LogicalSystemKeys.LogicalKey Execute => LogicalSystemKeys[43];
            public LogicalSystemKeys.LogicalKey PrintScreen => LogicalSystemKeys[44];
            public LogicalSystemKeys.LogicalKey Snapshot => LogicalSystemKeys[44];
            public LogicalSystemKeys.LogicalKey Insert => LogicalSystemKeys[45];
            public LogicalSystemKeys.LogicalKey Delete => LogicalSystemKeys[46];
            public LogicalSystemKeys.LogicalKey Help => LogicalSystemKeys[47];
            public LogicalSystemKeys.LogicalKey D0 => LogicalSystemKeys[48];
            public LogicalSystemKeys.LogicalKey D1 => LogicalSystemKeys[49];
            public LogicalSystemKeys.LogicalKey D2 => LogicalSystemKeys[50];
            public LogicalSystemKeys.LogicalKey D3 => LogicalSystemKeys[51];
            public LogicalSystemKeys.LogicalKey D4 => LogicalSystemKeys[52];
            public LogicalSystemKeys.LogicalKey D5 => LogicalSystemKeys[53];
            public LogicalSystemKeys.LogicalKey D6 => LogicalSystemKeys[54];
            public LogicalSystemKeys.LogicalKey D7 => LogicalSystemKeys[55];
            public LogicalSystemKeys.LogicalKey D8 => LogicalSystemKeys[56];
            public LogicalSystemKeys.LogicalKey D9 => LogicalSystemKeys[57];
            public LogicalSystemKeys.LogicalKey A => LogicalSystemKeys[65];
            public LogicalSystemKeys.LogicalKey B => LogicalSystemKeys[66];
            public LogicalSystemKeys.LogicalKey C => LogicalSystemKeys[67];
            public LogicalSystemKeys.LogicalKey D => LogicalSystemKeys[68];
            public LogicalSystemKeys.LogicalKey E => LogicalSystemKeys[69];
            public LogicalSystemKeys.LogicalKey F => LogicalSystemKeys[70];
            public LogicalSystemKeys.LogicalKey G => LogicalSystemKeys[71];
            public LogicalSystemKeys.LogicalKey H => LogicalSystemKeys[72];
            public LogicalSystemKeys.LogicalKey I => LogicalSystemKeys[73];
            public LogicalSystemKeys.LogicalKey J => LogicalSystemKeys[74];
            public LogicalSystemKeys.LogicalKey K => LogicalSystemKeys[75];
            public LogicalSystemKeys.LogicalKey L => LogicalSystemKeys[76];
            public LogicalSystemKeys.LogicalKey M => LogicalSystemKeys[77];
            public LogicalSystemKeys.LogicalKey N => LogicalSystemKeys[78];
            public LogicalSystemKeys.LogicalKey O => LogicalSystemKeys[79];
            public LogicalSystemKeys.LogicalKey P => LogicalSystemKeys[80];
            public LogicalSystemKeys.LogicalKey Q => LogicalSystemKeys[81];
            public LogicalSystemKeys.LogicalKey R => LogicalSystemKeys[82];
            public LogicalSystemKeys.LogicalKey S => LogicalSystemKeys[83];
            public LogicalSystemKeys.LogicalKey T => LogicalSystemKeys[84];
            public LogicalSystemKeys.LogicalKey U => LogicalSystemKeys[85];
            public LogicalSystemKeys.LogicalKey V => LogicalSystemKeys[86];
            public LogicalSystemKeys.LogicalKey W => LogicalSystemKeys[87];
            public LogicalSystemKeys.LogicalKey X => LogicalSystemKeys[88];
            public LogicalSystemKeys.LogicalKey Y => LogicalSystemKeys[89];
            public LogicalSystemKeys.LogicalKey Z => LogicalSystemKeys[90];
            public LogicalSystemKeys.LogicalKey LWin => LogicalSystemKeys[91];
            public LogicalSystemKeys.LogicalKey RWin => LogicalSystemKeys[92];
            public LogicalSystemKeys.LogicalKey Apps => LogicalSystemKeys[93];
            public LogicalSystemKeys.LogicalKey Sleep => LogicalSystemKeys[95];
            public LogicalSystemKeys.LogicalKey NumPad0 => LogicalSystemKeys[96];
            public LogicalSystemKeys.LogicalKey NumPad1 => LogicalSystemKeys[97];
            public LogicalSystemKeys.LogicalKey NumPad2 => LogicalSystemKeys[98];
            public LogicalSystemKeys.LogicalKey NumPad3 => LogicalSystemKeys[99];
            public LogicalSystemKeys.LogicalKey NumPad4 => LogicalSystemKeys[100];
            public LogicalSystemKeys.LogicalKey NumPad5 => LogicalSystemKeys[101];
            public LogicalSystemKeys.LogicalKey NumPad6 => LogicalSystemKeys[102];
            public LogicalSystemKeys.LogicalKey NumPad7 => LogicalSystemKeys[103];
            public LogicalSystemKeys.LogicalKey NumPad8 => LogicalSystemKeys[104];
            public LogicalSystemKeys.LogicalKey NumPad9 => LogicalSystemKeys[105];
            public LogicalSystemKeys.LogicalKey Multiply => LogicalSystemKeys[106];
            public LogicalSystemKeys.LogicalKey Add => LogicalSystemKeys[107];
            public LogicalSystemKeys.LogicalKey Separator => LogicalSystemKeys[108];
            public LogicalSystemKeys.LogicalKey Subtract => LogicalSystemKeys[109];
            public LogicalSystemKeys.LogicalKey Decimal => LogicalSystemKeys[110];
            public LogicalSystemKeys.LogicalKey Divide => LogicalSystemKeys[111];
            public LogicalSystemKeys.LogicalKey F1 => LogicalSystemKeys[112];
            public LogicalSystemKeys.LogicalKey F2 => LogicalSystemKeys[113];
            public LogicalSystemKeys.LogicalKey F3 => LogicalSystemKeys[114];
            public LogicalSystemKeys.LogicalKey F4 => LogicalSystemKeys[115];
            public LogicalSystemKeys.LogicalKey F5 => LogicalSystemKeys[116];
            public LogicalSystemKeys.LogicalKey F6 => LogicalSystemKeys[117];
            public LogicalSystemKeys.LogicalKey F7 => LogicalSystemKeys[118];
            public LogicalSystemKeys.LogicalKey F8 => LogicalSystemKeys[119];
            public LogicalSystemKeys.LogicalKey F9 => LogicalSystemKeys[120];
            public LogicalSystemKeys.LogicalKey F10 => LogicalSystemKeys[121];
            public LogicalSystemKeys.LogicalKey F11 => LogicalSystemKeys[122];
            public LogicalSystemKeys.LogicalKey F12 => LogicalSystemKeys[123];
            public LogicalSystemKeys.LogicalKey F13 => LogicalSystemKeys[124];
            public LogicalSystemKeys.LogicalKey F14 => LogicalSystemKeys[125];
            public LogicalSystemKeys.LogicalKey F15 => LogicalSystemKeys[126];
            public LogicalSystemKeys.LogicalKey F16 => LogicalSystemKeys[127];
            public LogicalSystemKeys.LogicalKey F17 => LogicalSystemKeys[128];
            public LogicalSystemKeys.LogicalKey F18 => LogicalSystemKeys[129];
            public LogicalSystemKeys.LogicalKey F19 => LogicalSystemKeys[130];
            public LogicalSystemKeys.LogicalKey F20 => LogicalSystemKeys[131];
            public LogicalSystemKeys.LogicalKey F21 => LogicalSystemKeys[132];
            public LogicalSystemKeys.LogicalKey F22 => LogicalSystemKeys[133];
            public LogicalSystemKeys.LogicalKey F23 => LogicalSystemKeys[134];
            public LogicalSystemKeys.LogicalKey F24 => LogicalSystemKeys[135];
            public LogicalSystemKeys.LogicalKey NumLock => LogicalSystemKeys[144];
            public LogicalSystemKeys.LogicalKey Scroll => LogicalSystemKeys[145];
            public LogicalSystemKeys.LogicalKey LShiftKey => LogicalSystemKeys[160];
            public LogicalSystemKeys.LogicalKey RShiftKey => LogicalSystemKeys[161];
            public LogicalSystemKeys.LogicalKey LControlKey => LogicalSystemKeys[162];
            public LogicalSystemKeys.LogicalKey RControlKey => LogicalSystemKeys[163];
            public LogicalSystemKeys.LogicalKey LMenu => LogicalSystemKeys[164];
            public LogicalSystemKeys.LogicalKey RMenu => LogicalSystemKeys[165];
            public LogicalSystemKeys.LogicalKey BrowserBack => LogicalSystemKeys[166];
            public LogicalSystemKeys.LogicalKey BrowserForward => LogicalSystemKeys[167];
            public LogicalSystemKeys.LogicalKey BrowserRefresh => LogicalSystemKeys[168];
            public LogicalSystemKeys.LogicalKey BrowserStop => LogicalSystemKeys[169];
            public LogicalSystemKeys.LogicalKey BrowserSearch => LogicalSystemKeys[170];
            public LogicalSystemKeys.LogicalKey BrowserFavorites => LogicalSystemKeys[171];
            public LogicalSystemKeys.LogicalKey BrowserHome => LogicalSystemKeys[172];
            public LogicalSystemKeys.LogicalKey VolumeMute => LogicalSystemKeys[173];
            public LogicalSystemKeys.LogicalKey VolumeDown => LogicalSystemKeys[174];
            public LogicalSystemKeys.LogicalKey VolumeUp => LogicalSystemKeys[175];
            public LogicalSystemKeys.LogicalKey MediaNextTrack => LogicalSystemKeys[176];
            public LogicalSystemKeys.LogicalKey MediaPreviousTrack => LogicalSystemKeys[177];
            public LogicalSystemKeys.LogicalKey MediaStop => LogicalSystemKeys[178];
            public LogicalSystemKeys.LogicalKey MediaPlayPause => LogicalSystemKeys[179];
            public LogicalSystemKeys.LogicalKey LaunchMail => LogicalSystemKeys[180];
            public LogicalSystemKeys.LogicalKey SelectMedia => LogicalSystemKeys[181];
            public LogicalSystemKeys.LogicalKey LaunchApplication1 => LogicalSystemKeys[182];
            public LogicalSystemKeys.LogicalKey LaunchApplication2 => LogicalSystemKeys[183];
            public LogicalSystemKeys.LogicalKey Oem1 => LogicalSystemKeys[186];
            public LogicalSystemKeys.LogicalKey OemSemicolon => LogicalSystemKeys[186];
            public LogicalSystemKeys.LogicalKey Oemplus => LogicalSystemKeys[187];
            public LogicalSystemKeys.LogicalKey Oemcomma => LogicalSystemKeys[188];
            public LogicalSystemKeys.LogicalKey OemMinus => LogicalSystemKeys[189];
            public LogicalSystemKeys.LogicalKey OemPeriod => LogicalSystemKeys[190];
            public LogicalSystemKeys.LogicalKey OemQuestion => LogicalSystemKeys[191];
            public LogicalSystemKeys.LogicalKey Oem2 => LogicalSystemKeys[191];
            public LogicalSystemKeys.LogicalKey Oemtilde => LogicalSystemKeys[192];
            public LogicalSystemKeys.LogicalKey Oem3 => LogicalSystemKeys[192];
            public LogicalSystemKeys.LogicalKey Oem4 => LogicalSystemKeys[219];
            public LogicalSystemKeys.LogicalKey OemOpenBrackets => LogicalSystemKeys[219];
            public LogicalSystemKeys.LogicalKey OemPipe => LogicalSystemKeys[220];
            public LogicalSystemKeys.LogicalKey Oem5 => LogicalSystemKeys[220];
            public LogicalSystemKeys.LogicalKey Oem6 => LogicalSystemKeys[221];
            public LogicalSystemKeys.LogicalKey OemCloseBrackets => LogicalSystemKeys[221];
            public LogicalSystemKeys.LogicalKey Oem7 => LogicalSystemKeys[222];
            public LogicalSystemKeys.LogicalKey OemQuotes => LogicalSystemKeys[222];
            public LogicalSystemKeys.LogicalKey Oem8 => LogicalSystemKeys[223];
            public LogicalSystemKeys.LogicalKey Oem102 => LogicalSystemKeys[226];
            public LogicalSystemKeys.LogicalKey OemBackslash => LogicalSystemKeys[226];
            public LogicalSystemKeys.LogicalKey ProcessKey => LogicalSystemKeys[229];
            public LogicalSystemKeys.LogicalKey Packet => LogicalSystemKeys[231];
            public LogicalSystemKeys.LogicalKey Attn => LogicalSystemKeys[246];
            public LogicalSystemKeys.LogicalKey Crsel => LogicalSystemKeys[247];
            public LogicalSystemKeys.LogicalKey Exsel => LogicalSystemKeys[248];
            public LogicalSystemKeys.LogicalKey EraseEof => LogicalSystemKeys[249];
            public LogicalSystemKeys.LogicalKey Play => LogicalSystemKeys[250];
            public LogicalSystemKeys.LogicalKey Zoom => LogicalSystemKeys[251];
            public LogicalSystemKeys.LogicalKey NoName => LogicalSystemKeys[252];
            public LogicalSystemKeys.LogicalKey Pa1 => LogicalSystemKeys[253];
            public LogicalSystemKeys.LogicalKey OemClear => LogicalSystemKeys[254];

            public LogicalKeyDeclaration(
                LogicalSingleThrowKeys singleThrowKeys,
                LogicalSystemKeys logicalSystemKeys)
            {
                LogicalSingleThrowKeys = singleThrowKeys;
                LogicalSystemKeys = logicalSystemKeys;
            }
        }

        public class PhysicalKeyDeclaration
        {
            private readonly PhysicalSingleThrowKeys PhysicalSingleThrowKeys;
            private readonly PhysicalSystemKeys PhysicalSystemKeys;

            public PhysicalSingleThrowKeys.PhysicalKey WheelUp => PhysicalSingleThrowKeys[1];
            public PhysicalSingleThrowKeys.PhysicalKey WheelDown => PhysicalSingleThrowKeys[2];
            public PhysicalSingleThrowKeys.PhysicalKey WheelLeft => PhysicalSingleThrowKeys[3];
            public PhysicalSingleThrowKeys.PhysicalKey WheelRight => PhysicalSingleThrowKeys[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public PhysicalSystemKeys.PhysicalKey LeftButton => PhysicalSystemKeys[1];
            public PhysicalSystemKeys.PhysicalKey RightButton => PhysicalSystemKeys[2];
            public PhysicalSystemKeys.PhysicalKey MiddleButton => PhysicalSystemKeys[4];
            public PhysicalSystemKeys.PhysicalKey X1Button => PhysicalSystemKeys[5];
            public PhysicalSystemKeys.PhysicalKey X2Button => PhysicalSystemKeys[6];

            public PhysicalSystemKeys.PhysicalKey LButton => PhysicalSystemKeys[1];
            public PhysicalSystemKeys.PhysicalKey RButton => PhysicalSystemKeys[2];
            public PhysicalSystemKeys.PhysicalKey Cancel => PhysicalSystemKeys[3];
            public PhysicalSystemKeys.PhysicalKey MButton => PhysicalSystemKeys[4];
            public PhysicalSystemKeys.PhysicalKey XButton1 => PhysicalSystemKeys[5];
            public PhysicalSystemKeys.PhysicalKey XButton2 => PhysicalSystemKeys[6];
            public PhysicalSystemKeys.PhysicalKey Back => PhysicalSystemKeys[8];
            public PhysicalSystemKeys.PhysicalKey Tab => PhysicalSystemKeys[9];
            public PhysicalSystemKeys.PhysicalKey LineFeed => PhysicalSystemKeys[10];
            public PhysicalSystemKeys.PhysicalKey Clear => PhysicalSystemKeys[12];
            public PhysicalSystemKeys.PhysicalKey Enter => PhysicalSystemKeys[13];
            public PhysicalSystemKeys.PhysicalKey Return => PhysicalSystemKeys[13];
            public PhysicalSystemKeys.PhysicalKey ShiftKey => PhysicalSystemKeys[16];
            public PhysicalSystemKeys.PhysicalKey ControlKey => PhysicalSystemKeys[17];
            public PhysicalSystemKeys.PhysicalKey Menu => PhysicalSystemKeys[18];
            public PhysicalSystemKeys.PhysicalKey Pause => PhysicalSystemKeys[19];
            public PhysicalSystemKeys.PhysicalKey CapsLock => PhysicalSystemKeys[20];
            public PhysicalSystemKeys.PhysicalKey Capital => PhysicalSystemKeys[20];
            public PhysicalSystemKeys.PhysicalKey KanaMode => PhysicalSystemKeys[21];
            public PhysicalSystemKeys.PhysicalKey HangulMode => PhysicalSystemKeys[21];
            public PhysicalSystemKeys.PhysicalKey JunjaMode => PhysicalSystemKeys[23];
            public PhysicalSystemKeys.PhysicalKey FinalMode => PhysicalSystemKeys[24];
            public PhysicalSystemKeys.PhysicalKey KanjiMode => PhysicalSystemKeys[25];
            public PhysicalSystemKeys.PhysicalKey HanjaMode => PhysicalSystemKeys[25];
            public PhysicalSystemKeys.PhysicalKey Escape => PhysicalSystemKeys[27];
            public PhysicalSystemKeys.PhysicalKey IMEConvert => PhysicalSystemKeys[28];
            public PhysicalSystemKeys.PhysicalKey IMENonconvert => PhysicalSystemKeys[29];
            public PhysicalSystemKeys.PhysicalKey IMEAccept => PhysicalSystemKeys[30];
            public PhysicalSystemKeys.PhysicalKey IMEModeChange => PhysicalSystemKeys[31];
            public PhysicalSystemKeys.PhysicalKey Space => PhysicalSystemKeys[32];
            public PhysicalSystemKeys.PhysicalKey Prior => PhysicalSystemKeys[33];
            public PhysicalSystemKeys.PhysicalKey PageUp => PhysicalSystemKeys[33];
            public PhysicalSystemKeys.PhysicalKey Next => PhysicalSystemKeys[34];
            public PhysicalSystemKeys.PhysicalKey PageDown => PhysicalSystemKeys[34];
            public PhysicalSystemKeys.PhysicalKey End => PhysicalSystemKeys[35];
            public PhysicalSystemKeys.PhysicalKey Home => PhysicalSystemKeys[36];
            public PhysicalSystemKeys.PhysicalKey Left => PhysicalSystemKeys[37];
            public PhysicalSystemKeys.PhysicalKey Up => PhysicalSystemKeys[38];
            public PhysicalSystemKeys.PhysicalKey Right => PhysicalSystemKeys[39];
            public PhysicalSystemKeys.PhysicalKey Down => PhysicalSystemKeys[40];
            public PhysicalSystemKeys.PhysicalKey Select => PhysicalSystemKeys[41];
            public PhysicalSystemKeys.PhysicalKey Print => PhysicalSystemKeys[42];
            public PhysicalSystemKeys.PhysicalKey Execute => PhysicalSystemKeys[43];
            public PhysicalSystemKeys.PhysicalKey PrintScreen => PhysicalSystemKeys[44];
            public PhysicalSystemKeys.PhysicalKey Snapshot => PhysicalSystemKeys[44];
            public PhysicalSystemKeys.PhysicalKey Insert => PhysicalSystemKeys[45];
            public PhysicalSystemKeys.PhysicalKey Delete => PhysicalSystemKeys[46];
            public PhysicalSystemKeys.PhysicalKey Help => PhysicalSystemKeys[47];
            public PhysicalSystemKeys.PhysicalKey D0 => PhysicalSystemKeys[48];
            public PhysicalSystemKeys.PhysicalKey D1 => PhysicalSystemKeys[49];
            public PhysicalSystemKeys.PhysicalKey D2 => PhysicalSystemKeys[50];
            public PhysicalSystemKeys.PhysicalKey D3 => PhysicalSystemKeys[51];
            public PhysicalSystemKeys.PhysicalKey D4 => PhysicalSystemKeys[52];
            public PhysicalSystemKeys.PhysicalKey D5 => PhysicalSystemKeys[53];
            public PhysicalSystemKeys.PhysicalKey D6 => PhysicalSystemKeys[54];
            public PhysicalSystemKeys.PhysicalKey D7 => PhysicalSystemKeys[55];
            public PhysicalSystemKeys.PhysicalKey D8 => PhysicalSystemKeys[56];
            public PhysicalSystemKeys.PhysicalKey D9 => PhysicalSystemKeys[57];
            public PhysicalSystemKeys.PhysicalKey A => PhysicalSystemKeys[65];
            public PhysicalSystemKeys.PhysicalKey B => PhysicalSystemKeys[66];
            public PhysicalSystemKeys.PhysicalKey C => PhysicalSystemKeys[67];
            public PhysicalSystemKeys.PhysicalKey D => PhysicalSystemKeys[68];
            public PhysicalSystemKeys.PhysicalKey E => PhysicalSystemKeys[69];
            public PhysicalSystemKeys.PhysicalKey F => PhysicalSystemKeys[70];
            public PhysicalSystemKeys.PhysicalKey G => PhysicalSystemKeys[71];
            public PhysicalSystemKeys.PhysicalKey H => PhysicalSystemKeys[72];
            public PhysicalSystemKeys.PhysicalKey I => PhysicalSystemKeys[73];
            public PhysicalSystemKeys.PhysicalKey J => PhysicalSystemKeys[74];
            public PhysicalSystemKeys.PhysicalKey K => PhysicalSystemKeys[75];
            public PhysicalSystemKeys.PhysicalKey L => PhysicalSystemKeys[76];
            public PhysicalSystemKeys.PhysicalKey M => PhysicalSystemKeys[77];
            public PhysicalSystemKeys.PhysicalKey N => PhysicalSystemKeys[78];
            public PhysicalSystemKeys.PhysicalKey O => PhysicalSystemKeys[79];
            public PhysicalSystemKeys.PhysicalKey P => PhysicalSystemKeys[80];
            public PhysicalSystemKeys.PhysicalKey Q => PhysicalSystemKeys[81];
            public PhysicalSystemKeys.PhysicalKey R => PhysicalSystemKeys[82];
            public PhysicalSystemKeys.PhysicalKey S => PhysicalSystemKeys[83];
            public PhysicalSystemKeys.PhysicalKey T => PhysicalSystemKeys[84];
            public PhysicalSystemKeys.PhysicalKey U => PhysicalSystemKeys[85];
            public PhysicalSystemKeys.PhysicalKey V => PhysicalSystemKeys[86];
            public PhysicalSystemKeys.PhysicalKey W => PhysicalSystemKeys[87];
            public PhysicalSystemKeys.PhysicalKey X => PhysicalSystemKeys[88];
            public PhysicalSystemKeys.PhysicalKey Y => PhysicalSystemKeys[89];
            public PhysicalSystemKeys.PhysicalKey Z => PhysicalSystemKeys[90];
            public PhysicalSystemKeys.PhysicalKey LWin => PhysicalSystemKeys[91];
            public PhysicalSystemKeys.PhysicalKey RWin => PhysicalSystemKeys[92];
            public PhysicalSystemKeys.PhysicalKey Apps => PhysicalSystemKeys[93];
            public PhysicalSystemKeys.PhysicalKey Sleep => PhysicalSystemKeys[95];
            public PhysicalSystemKeys.PhysicalKey NumPad0 => PhysicalSystemKeys[96];
            public PhysicalSystemKeys.PhysicalKey NumPad1 => PhysicalSystemKeys[97];
            public PhysicalSystemKeys.PhysicalKey NumPad2 => PhysicalSystemKeys[98];
            public PhysicalSystemKeys.PhysicalKey NumPad3 => PhysicalSystemKeys[99];
            public PhysicalSystemKeys.PhysicalKey NumPad4 => PhysicalSystemKeys[100];
            public PhysicalSystemKeys.PhysicalKey NumPad5 => PhysicalSystemKeys[101];
            public PhysicalSystemKeys.PhysicalKey NumPad6 => PhysicalSystemKeys[102];
            public PhysicalSystemKeys.PhysicalKey NumPad7 => PhysicalSystemKeys[103];
            public PhysicalSystemKeys.PhysicalKey NumPad8 => PhysicalSystemKeys[104];
            public PhysicalSystemKeys.PhysicalKey NumPad9 => PhysicalSystemKeys[105];
            public PhysicalSystemKeys.PhysicalKey Multiply => PhysicalSystemKeys[106];
            public PhysicalSystemKeys.PhysicalKey Add => PhysicalSystemKeys[107];
            public PhysicalSystemKeys.PhysicalKey Separator => PhysicalSystemKeys[108];
            public PhysicalSystemKeys.PhysicalKey Subtract => PhysicalSystemKeys[109];
            public PhysicalSystemKeys.PhysicalKey Decimal => PhysicalSystemKeys[110];
            public PhysicalSystemKeys.PhysicalKey Divide => PhysicalSystemKeys[111];
            public PhysicalSystemKeys.PhysicalKey F1 => PhysicalSystemKeys[112];
            public PhysicalSystemKeys.PhysicalKey F2 => PhysicalSystemKeys[113];
            public PhysicalSystemKeys.PhysicalKey F3 => PhysicalSystemKeys[114];
            public PhysicalSystemKeys.PhysicalKey F4 => PhysicalSystemKeys[115];
            public PhysicalSystemKeys.PhysicalKey F5 => PhysicalSystemKeys[116];
            public PhysicalSystemKeys.PhysicalKey F6 => PhysicalSystemKeys[117];
            public PhysicalSystemKeys.PhysicalKey F7 => PhysicalSystemKeys[118];
            public PhysicalSystemKeys.PhysicalKey F8 => PhysicalSystemKeys[119];
            public PhysicalSystemKeys.PhysicalKey F9 => PhysicalSystemKeys[120];
            public PhysicalSystemKeys.PhysicalKey F10 => PhysicalSystemKeys[121];
            public PhysicalSystemKeys.PhysicalKey F11 => PhysicalSystemKeys[122];
            public PhysicalSystemKeys.PhysicalKey F12 => PhysicalSystemKeys[123];
            public PhysicalSystemKeys.PhysicalKey F13 => PhysicalSystemKeys[124];
            public PhysicalSystemKeys.PhysicalKey F14 => PhysicalSystemKeys[125];
            public PhysicalSystemKeys.PhysicalKey F15 => PhysicalSystemKeys[126];
            public PhysicalSystemKeys.PhysicalKey F16 => PhysicalSystemKeys[127];
            public PhysicalSystemKeys.PhysicalKey F17 => PhysicalSystemKeys[128];
            public PhysicalSystemKeys.PhysicalKey F18 => PhysicalSystemKeys[129];
            public PhysicalSystemKeys.PhysicalKey F19 => PhysicalSystemKeys[130];
            public PhysicalSystemKeys.PhysicalKey F20 => PhysicalSystemKeys[131];
            public PhysicalSystemKeys.PhysicalKey F21 => PhysicalSystemKeys[132];
            public PhysicalSystemKeys.PhysicalKey F22 => PhysicalSystemKeys[133];
            public PhysicalSystemKeys.PhysicalKey F23 => PhysicalSystemKeys[134];
            public PhysicalSystemKeys.PhysicalKey F24 => PhysicalSystemKeys[135];
            public PhysicalSystemKeys.PhysicalKey NumLock => PhysicalSystemKeys[144];
            public PhysicalSystemKeys.PhysicalKey Scroll => PhysicalSystemKeys[145];
            public PhysicalSystemKeys.PhysicalKey LShiftKey => PhysicalSystemKeys[160];
            public PhysicalSystemKeys.PhysicalKey RShiftKey => PhysicalSystemKeys[161];
            public PhysicalSystemKeys.PhysicalKey LControlKey => PhysicalSystemKeys[162];
            public PhysicalSystemKeys.PhysicalKey RControlKey => PhysicalSystemKeys[163];
            public PhysicalSystemKeys.PhysicalKey LMenu => PhysicalSystemKeys[164];
            public PhysicalSystemKeys.PhysicalKey RMenu => PhysicalSystemKeys[165];
            public PhysicalSystemKeys.PhysicalKey BrowserBack => PhysicalSystemKeys[166];
            public PhysicalSystemKeys.PhysicalKey BrowserForward => PhysicalSystemKeys[167];
            public PhysicalSystemKeys.PhysicalKey BrowserRefresh => PhysicalSystemKeys[168];
            public PhysicalSystemKeys.PhysicalKey BrowserStop => PhysicalSystemKeys[169];
            public PhysicalSystemKeys.PhysicalKey BrowserSearch => PhysicalSystemKeys[170];
            public PhysicalSystemKeys.PhysicalKey BrowserFavorites => PhysicalSystemKeys[171];
            public PhysicalSystemKeys.PhysicalKey BrowserHome => PhysicalSystemKeys[172];
            public PhysicalSystemKeys.PhysicalKey VolumeMute => PhysicalSystemKeys[173];
            public PhysicalSystemKeys.PhysicalKey VolumeDown => PhysicalSystemKeys[174];
            public PhysicalSystemKeys.PhysicalKey VolumeUp => PhysicalSystemKeys[175];
            public PhysicalSystemKeys.PhysicalKey MediaNextTrack => PhysicalSystemKeys[176];
            public PhysicalSystemKeys.PhysicalKey MediaPreviousTrack => PhysicalSystemKeys[177];
            public PhysicalSystemKeys.PhysicalKey MediaStop => PhysicalSystemKeys[178];
            public PhysicalSystemKeys.PhysicalKey MediaPlayPause => PhysicalSystemKeys[179];
            public PhysicalSystemKeys.PhysicalKey LaunchMail => PhysicalSystemKeys[180];
            public PhysicalSystemKeys.PhysicalKey SelectMedia => PhysicalSystemKeys[181];
            public PhysicalSystemKeys.PhysicalKey LaunchApplication1 => PhysicalSystemKeys[182];
            public PhysicalSystemKeys.PhysicalKey LaunchApplication2 => PhysicalSystemKeys[183];
            public PhysicalSystemKeys.PhysicalKey Oem1 => PhysicalSystemKeys[186];
            public PhysicalSystemKeys.PhysicalKey OemSemicolon => PhysicalSystemKeys[186];
            public PhysicalSystemKeys.PhysicalKey Oemplus => PhysicalSystemKeys[187];
            public PhysicalSystemKeys.PhysicalKey Oemcomma => PhysicalSystemKeys[188];
            public PhysicalSystemKeys.PhysicalKey OemMinus => PhysicalSystemKeys[189];
            public PhysicalSystemKeys.PhysicalKey OemPeriod => PhysicalSystemKeys[190];
            public PhysicalSystemKeys.PhysicalKey OemQuestion => PhysicalSystemKeys[191];
            public PhysicalSystemKeys.PhysicalKey Oem2 => PhysicalSystemKeys[191];
            public PhysicalSystemKeys.PhysicalKey Oemtilde => PhysicalSystemKeys[192];
            public PhysicalSystemKeys.PhysicalKey Oem3 => PhysicalSystemKeys[192];
            public PhysicalSystemKeys.PhysicalKey Oem4 => PhysicalSystemKeys[219];
            public PhysicalSystemKeys.PhysicalKey OemOpenBrackets => PhysicalSystemKeys[219];
            public PhysicalSystemKeys.PhysicalKey OemPipe => PhysicalSystemKeys[220];
            public PhysicalSystemKeys.PhysicalKey Oem5 => PhysicalSystemKeys[220];
            public PhysicalSystemKeys.PhysicalKey Oem6 => PhysicalSystemKeys[221];
            public PhysicalSystemKeys.PhysicalKey OemCloseBrackets => PhysicalSystemKeys[221];
            public PhysicalSystemKeys.PhysicalKey Oem7 => PhysicalSystemKeys[222];
            public PhysicalSystemKeys.PhysicalKey OemQuotes => PhysicalSystemKeys[222];
            public PhysicalSystemKeys.PhysicalKey Oem8 => PhysicalSystemKeys[223];
            public PhysicalSystemKeys.PhysicalKey Oem102 => PhysicalSystemKeys[226];
            public PhysicalSystemKeys.PhysicalKey OemBackslash => PhysicalSystemKeys[226];
            public PhysicalSystemKeys.PhysicalKey ProcessKey => PhysicalSystemKeys[229];
            public PhysicalSystemKeys.PhysicalKey Packet => PhysicalSystemKeys[231];
            public PhysicalSystemKeys.PhysicalKey Attn => PhysicalSystemKeys[246];
            public PhysicalSystemKeys.PhysicalKey Crsel => PhysicalSystemKeys[247];
            public PhysicalSystemKeys.PhysicalKey Exsel => PhysicalSystemKeys[248];
            public PhysicalSystemKeys.PhysicalKey EraseEof => PhysicalSystemKeys[249];
            public PhysicalSystemKeys.PhysicalKey Play => PhysicalSystemKeys[250];
            public PhysicalSystemKeys.PhysicalKey Zoom => PhysicalSystemKeys[251];
            public PhysicalSystemKeys.PhysicalKey NoName => PhysicalSystemKeys[252];
            public PhysicalSystemKeys.PhysicalKey Pa1 => PhysicalSystemKeys[253];
            public PhysicalSystemKeys.PhysicalKey OemClear => PhysicalSystemKeys[254];

            public PhysicalKeyDeclaration(PhysicalSingleThrowKeys singleThrowKeys, PhysicalSystemKeys systemKeys)
            {
                PhysicalSingleThrowKeys = singleThrowKeys;
                PhysicalSystemKeys = systemKeys;
            }
        }
    }

    public class CustomEvaluationContext : EvaluationContext
    {
        public readonly Point GestureStartPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomEvaluationContext(Point gestureStartPosition)
        {
            this.GestureStartPosition = gestureStartPosition;
            this.ForegroundWindow = new ForegroundWindowInfo();
            this.PointedWindow = new PointedWindowInfo(gestureStartPosition);
        }
    }

    public class CustomExecutionContext : ExecutionContext
    {
        public readonly Point GestureStartPosition;
        public readonly Point GestureEndPosition;
        public readonly ForegroundWindowInfo ForegroundWindow;
        public readonly PointedWindowInfo PointedWindow;

        public CustomExecutionContext(CustomEvaluationContext evaluationContext, Point gestureEndPosition)
        {
            this.GestureStartPosition = evaluationContext.GestureStartPosition;
            this.GestureEndPosition = gestureEndPosition;
            this.ForegroundWindow = evaluationContext.ForegroundWindow;
            this.PointedWindow = evaluationContext.PointedWindow;
        }
    }

    class CustomContextManager : ContextManager<CustomEvaluationContext, CustomExecutionContext>
    {
        public Point CursorPosition { get; set; }

        public override CustomEvaluationContext CreateEvaluateContext()
            => new CustomEvaluationContext(CursorPosition);

        public override CustomExecutionContext CreateExecutionContext(CustomEvaluationContext evaluationContext)
            => new CustomExecutionContext(evaluationContext, CursorPosition);

        /*
         override

        public virtual bool EvaluateWhenEvaluator(TEvalContext evalContext, WhenElement<TEvalContext, TExecContext> whenElement)
            => whenElement.WhenEvaluator(evalContext);

        public virtual void ExecuteExcutor(TExecContext execContext, ExecuteAction<TExecContext> executeAction)
            => executeAction(execContext);

         */
    }

    public class CustomRootElement : RootElement<CustomEvaluationContext, CustomExecutionContext>
    {

    }

    class CustomGestureMachine : GestureMachine<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        public CustomGestureMachine(CustomRootElement rootElement)
            : base(new GestureMachineConfig(), new CustomContextManager(), rootElement)
        {
            this.GestureCancelled += new GestureCancelledEventHandler((sender, e) => 
            {
                // ExecuteInBackground(ctx, RestorePrimaryButtonClickEvent());
            });

            this.GestureTimeout += new GestureTimeoutEventHandler((sender, e) => 
            {
                // ExecuteInBackground(ctx, RestorePrimaryButtonDownEvent());
            });


        }

        internal Action RestorePrimaryButtonPressEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftDown();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleDown();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightDown();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Down();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Down();
                }
            };
        }

        internal Action RestorePrimaryButtonClickEvent()
        {
            return () =>
            {
                if (primaryEvent == Def.Constant.LeftButtonDown)
                {
                    InputSender.LeftClick();
                }
                else if (primaryEvent == Def.Constant.MiddleButtonDown)
                {
                    InputSender.MiddleClick();
                }
                else if (primaryEvent == Def.Constant.RightButtonDown)
                {
                    InputSender.RightClick();
                }
                else if (primaryEvent == Def.Constant.X1ButtonDown)
                {
                    InputSender.X1Click();
                }
                else if (primaryEvent == Def.Constant.X2ButtonDown)
                {
                    InputSender.X2Click();
                }
            };
        }
    }
}
