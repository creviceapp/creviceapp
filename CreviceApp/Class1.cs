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

    public class LogicalSystemKeySet : KeySet<LogicalSystemKey> 
    {
        public override LogicalSystemKey Create(int index)
            => new LogicalSystemKey(index);

        public LogicalSystemKeySet(int maxSize)
            : base(maxSize) { }

    }

    public class LogicalSystemKey : LogicalDoubleThrowKey
    {
        public LogicalSystemKey(int keyId)
            : base(keyId) { }
    }

    public class PhysicalSystemKeySet : KeySet<PhysicalSystemKey>
    {
        public override PhysicalSystemKey Create(int index)
            => new PhysicalSystemKey(logicalKeys[index], index);

        private LogicalSystemKeySet logicalKeys;

        public PhysicalSystemKeySet(LogicalSystemKeySet logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }

    }

    public class PhysicalSystemKey : PhysicalDoubleThrowKey
    {
        public PhysicalSystemKey(LogicalDoubleThrowKey logicalKey, int keyId)
            : base(logicalKey, keyId) { }

        // todo implicit operater to Keys, int   
    }

    public class SupportedKeys
    {
        private static SupportedKeys Instance = new SupportedKeys();

        private readonly LogicalKeyDeclaration logicalKeys;
        private readonly PhysicalKeyDeclaration physicalKeys;

        private SupportedKeys()
        {
            var logicalSingleThrowKeys = new LogicalSingleThrowKeySet(10);
            var physicalSingleThrowKeys = new PhysicalSingleThrowKeySet(logicalSingleThrowKeys);

            var logicalSystemKeys = new LogicalSystemKeySet(255);
            var physicalSystemKeys = new PhysicalSystemKeySet(logicalSystemKeys);

            logicalKeys = new LogicalKeyDeclaration(logicalSingleThrowKeys, logicalSystemKeys);
            physicalKeys = new PhysicalKeyDeclaration(physicalSingleThrowKeys, physicalSystemKeys);
        }

        public static LogicalKeyDeclaration Keys => Instance.logicalKeys;

        public static PhysicalKeyDeclaration PhysicalKeys => Instance.physicalKeys;

        public class LogicalKeyDeclaration
        {
            private readonly LogicalSingleThrowKeySet LogicalSingleThrowKeys;
            private readonly LogicalSystemKeySet LogicalSystemKeys;

            public LogicalSingleThrowKey WheelUp => LogicalSingleThrowKeys[1];
            public LogicalSingleThrowKey WheelDown => LogicalSingleThrowKeys[2];
            public LogicalSingleThrowKey WheelLeft => LogicalSingleThrowKeys[3];
            public LogicalSingleThrowKey WheelRight => LogicalSingleThrowKeys[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public LogicalSystemKey LeftButton => LogicalSystemKeys[1];
            public LogicalSystemKey RightButton => LogicalSystemKeys[2];
            public LogicalSystemKey MiddleButton => LogicalSystemKeys[4];
            public LogicalSystemKey X1Button => LogicalSystemKeys[5];
            public LogicalSystemKey X2Button => LogicalSystemKeys[6];

            public LogicalSystemKey LButton => LogicalSystemKeys[1];
            public LogicalSystemKey RButton => LogicalSystemKeys[2];
            public LogicalSystemKey Cancel => LogicalSystemKeys[3];
            public LogicalSystemKey MButton => LogicalSystemKeys[4];
            public LogicalSystemKey XButton1 => LogicalSystemKeys[5];
            public LogicalSystemKey XButton2 => LogicalSystemKeys[6];
            public LogicalSystemKey Back => LogicalSystemKeys[8];
            public LogicalSystemKey Tab => LogicalSystemKeys[9];
            public LogicalSystemKey LineFeed => LogicalSystemKeys[10];
            public LogicalSystemKey Clear => LogicalSystemKeys[12];
            public LogicalSystemKey Enter => LogicalSystemKeys[13];
            public LogicalSystemKey Return => LogicalSystemKeys[13];
            public LogicalSystemKey ShiftKey => LogicalSystemKeys[16];
            public LogicalSystemKey ControlKey => LogicalSystemKeys[17];
            public LogicalSystemKey Menu => LogicalSystemKeys[18];
            public LogicalSystemKey Pause => LogicalSystemKeys[19];
            public LogicalSystemKey CapsLock => LogicalSystemKeys[20];
            public LogicalSystemKey Capital => LogicalSystemKeys[20];
            public LogicalSystemKey KanaMode => LogicalSystemKeys[21];
            public LogicalSystemKey HangulMode => LogicalSystemKeys[21];
            public LogicalSystemKey JunjaMode => LogicalSystemKeys[23];
            public LogicalSystemKey FinalMode => LogicalSystemKeys[24];
            public LogicalSystemKey KanjiMode => LogicalSystemKeys[25];
            public LogicalSystemKey HanjaMode => LogicalSystemKeys[25];
            public LogicalSystemKey Escape => LogicalSystemKeys[27];
            public LogicalSystemKey IMEConvert => LogicalSystemKeys[28];
            public LogicalSystemKey IMENonconvert => LogicalSystemKeys[29];
            public LogicalSystemKey IMEAccept => LogicalSystemKeys[30];
            public LogicalSystemKey IMEModeChange => LogicalSystemKeys[31];
            public LogicalSystemKey Space => LogicalSystemKeys[32];
            public LogicalSystemKey Prior => LogicalSystemKeys[33];
            public LogicalSystemKey PageUp => LogicalSystemKeys[33];
            public LogicalSystemKey Next => LogicalSystemKeys[34];
            public LogicalSystemKey PageDown => LogicalSystemKeys[34];
            public LogicalSystemKey End => LogicalSystemKeys[35];
            public LogicalSystemKey Home => LogicalSystemKeys[36];
            public LogicalSystemKey Left => LogicalSystemKeys[37];
            public LogicalSystemKey Up => LogicalSystemKeys[38];
            public LogicalSystemKey Right => LogicalSystemKeys[39];
            public LogicalSystemKey Down => LogicalSystemKeys[40];
            public LogicalSystemKey Select => LogicalSystemKeys[41];
            public LogicalSystemKey Print => LogicalSystemKeys[42];
            public LogicalSystemKey Execute => LogicalSystemKeys[43];
            public LogicalSystemKey PrintScreen => LogicalSystemKeys[44];
            public LogicalSystemKey Snapshot => LogicalSystemKeys[44];
            public LogicalSystemKey Insert => LogicalSystemKeys[45];
            public LogicalSystemKey Delete => LogicalSystemKeys[46];
            public LogicalSystemKey Help => LogicalSystemKeys[47];
            public LogicalSystemKey D0 => LogicalSystemKeys[48];
            public LogicalSystemKey D1 => LogicalSystemKeys[49];
            public LogicalSystemKey D2 => LogicalSystemKeys[50];
            public LogicalSystemKey D3 => LogicalSystemKeys[51];
            public LogicalSystemKey D4 => LogicalSystemKeys[52];
            public LogicalSystemKey D5 => LogicalSystemKeys[53];
            public LogicalSystemKey D6 => LogicalSystemKeys[54];
            public LogicalSystemKey D7 => LogicalSystemKeys[55];
            public LogicalSystemKey D8 => LogicalSystemKeys[56];
            public LogicalSystemKey D9 => LogicalSystemKeys[57];
            public LogicalSystemKey A => LogicalSystemKeys[65];
            public LogicalSystemKey B => LogicalSystemKeys[66];
            public LogicalSystemKey C => LogicalSystemKeys[67];
            public LogicalSystemKey D => LogicalSystemKeys[68];
            public LogicalSystemKey E => LogicalSystemKeys[69];
            public LogicalSystemKey F => LogicalSystemKeys[70];
            public LogicalSystemKey G => LogicalSystemKeys[71];
            public LogicalSystemKey H => LogicalSystemKeys[72];
            public LogicalSystemKey I => LogicalSystemKeys[73];
            public LogicalSystemKey J => LogicalSystemKeys[74];
            public LogicalSystemKey K => LogicalSystemKeys[75];
            public LogicalSystemKey L => LogicalSystemKeys[76];
            public LogicalSystemKey M => LogicalSystemKeys[77];
            public LogicalSystemKey N => LogicalSystemKeys[78];
            public LogicalSystemKey O => LogicalSystemKeys[79];
            public LogicalSystemKey P => LogicalSystemKeys[80];
            public LogicalSystemKey Q => LogicalSystemKeys[81];
            public LogicalSystemKey R => LogicalSystemKeys[82];
            public LogicalSystemKey S => LogicalSystemKeys[83];
            public LogicalSystemKey T => LogicalSystemKeys[84];
            public LogicalSystemKey U => LogicalSystemKeys[85];
            public LogicalSystemKey V => LogicalSystemKeys[86];
            public LogicalSystemKey W => LogicalSystemKeys[87];
            public LogicalSystemKey X => LogicalSystemKeys[88];
            public LogicalSystemKey Y => LogicalSystemKeys[89];
            public LogicalSystemKey Z => LogicalSystemKeys[90];
            public LogicalSystemKey LWin => LogicalSystemKeys[91];
            public LogicalSystemKey RWin => LogicalSystemKeys[92];
            public LogicalSystemKey Apps => LogicalSystemKeys[93];
            public LogicalSystemKey Sleep => LogicalSystemKeys[95];
            public LogicalSystemKey NumPad0 => LogicalSystemKeys[96];
            public LogicalSystemKey NumPad1 => LogicalSystemKeys[97];
            public LogicalSystemKey NumPad2 => LogicalSystemKeys[98];
            public LogicalSystemKey NumPad3 => LogicalSystemKeys[99];
            public LogicalSystemKey NumPad4 => LogicalSystemKeys[100];
            public LogicalSystemKey NumPad5 => LogicalSystemKeys[101];
            public LogicalSystemKey NumPad6 => LogicalSystemKeys[102];
            public LogicalSystemKey NumPad7 => LogicalSystemKeys[103];
            public LogicalSystemKey NumPad8 => LogicalSystemKeys[104];
            public LogicalSystemKey NumPad9 => LogicalSystemKeys[105];
            public LogicalSystemKey Multiply => LogicalSystemKeys[106];
            public LogicalSystemKey Add => LogicalSystemKeys[107];
            public LogicalSystemKey Separator => LogicalSystemKeys[108];
            public LogicalSystemKey Subtract => LogicalSystemKeys[109];
            public LogicalSystemKey Decimal => LogicalSystemKeys[110];
            public LogicalSystemKey Divide => LogicalSystemKeys[111];
            public LogicalSystemKey F1 => LogicalSystemKeys[112];
            public LogicalSystemKey F2 => LogicalSystemKeys[113];
            public LogicalSystemKey F3 => LogicalSystemKeys[114];
            public LogicalSystemKey F4 => LogicalSystemKeys[115];
            public LogicalSystemKey F5 => LogicalSystemKeys[116];
            public LogicalSystemKey F6 => LogicalSystemKeys[117];
            public LogicalSystemKey F7 => LogicalSystemKeys[118];
            public LogicalSystemKey F8 => LogicalSystemKeys[119];
            public LogicalSystemKey F9 => LogicalSystemKeys[120];
            public LogicalSystemKey F10 => LogicalSystemKeys[121];
            public LogicalSystemKey F11 => LogicalSystemKeys[122];
            public LogicalSystemKey F12 => LogicalSystemKeys[123];
            public LogicalSystemKey F13 => LogicalSystemKeys[124];
            public LogicalSystemKey F14 => LogicalSystemKeys[125];
            public LogicalSystemKey F15 => LogicalSystemKeys[126];
            public LogicalSystemKey F16 => LogicalSystemKeys[127];
            public LogicalSystemKey F17 => LogicalSystemKeys[128];
            public LogicalSystemKey F18 => LogicalSystemKeys[129];
            public LogicalSystemKey F19 => LogicalSystemKeys[130];
            public LogicalSystemKey F20 => LogicalSystemKeys[131];
            public LogicalSystemKey F21 => LogicalSystemKeys[132];
            public LogicalSystemKey F22 => LogicalSystemKeys[133];
            public LogicalSystemKey F23 => LogicalSystemKeys[134];
            public LogicalSystemKey F24 => LogicalSystemKeys[135];
            public LogicalSystemKey NumLock => LogicalSystemKeys[144];
            public LogicalSystemKey Scroll => LogicalSystemKeys[145];
            public LogicalSystemKey LShiftKey => LogicalSystemKeys[160];
            public LogicalSystemKey RShiftKey => LogicalSystemKeys[161];
            public LogicalSystemKey LControlKey => LogicalSystemKeys[162];
            public LogicalSystemKey RControlKey => LogicalSystemKeys[163];
            public LogicalSystemKey LMenu => LogicalSystemKeys[164];
            public LogicalSystemKey RMenu => LogicalSystemKeys[165];
            public LogicalSystemKey BrowserBack => LogicalSystemKeys[166];
            public LogicalSystemKey BrowserForward => LogicalSystemKeys[167];
            public LogicalSystemKey BrowserRefresh => LogicalSystemKeys[168];
            public LogicalSystemKey BrowserStop => LogicalSystemKeys[169];
            public LogicalSystemKey BrowserSearch => LogicalSystemKeys[170];
            public LogicalSystemKey BrowserFavorites => LogicalSystemKeys[171];
            public LogicalSystemKey BrowserHome => LogicalSystemKeys[172];
            public LogicalSystemKey VolumeMute => LogicalSystemKeys[173];
            public LogicalSystemKey VolumeDown => LogicalSystemKeys[174];
            public LogicalSystemKey VolumeUp => LogicalSystemKeys[175];
            public LogicalSystemKey MediaNextTrack => LogicalSystemKeys[176];
            public LogicalSystemKey MediaPreviousTrack => LogicalSystemKeys[177];
            public LogicalSystemKey MediaStop => LogicalSystemKeys[178];
            public LogicalSystemKey MediaPlayPause => LogicalSystemKeys[179];
            public LogicalSystemKey LaunchMail => LogicalSystemKeys[180];
            public LogicalSystemKey SelectMedia => LogicalSystemKeys[181];
            public LogicalSystemKey LaunchApplication1 => LogicalSystemKeys[182];
            public LogicalSystemKey LaunchApplication2 => LogicalSystemKeys[183];
            public LogicalSystemKey Oem1 => LogicalSystemKeys[186];
            public LogicalSystemKey OemSemicolon => LogicalSystemKeys[186];
            public LogicalSystemKey Oemplus => LogicalSystemKeys[187];
            public LogicalSystemKey Oemcomma => LogicalSystemKeys[188];
            public LogicalSystemKey OemMinus => LogicalSystemKeys[189];
            public LogicalSystemKey OemPeriod => LogicalSystemKeys[190];
            public LogicalSystemKey OemQuestion => LogicalSystemKeys[191];
            public LogicalSystemKey Oem2 => LogicalSystemKeys[191];
            public LogicalSystemKey Oemtilde => LogicalSystemKeys[192];
            public LogicalSystemKey Oem3 => LogicalSystemKeys[192];
            public LogicalSystemKey Oem4 => LogicalSystemKeys[219];
            public LogicalSystemKey OemOpenBrackets => LogicalSystemKeys[219];
            public LogicalSystemKey OemPipe => LogicalSystemKeys[220];
            public LogicalSystemKey Oem5 => LogicalSystemKeys[220];
            public LogicalSystemKey Oem6 => LogicalSystemKeys[221];
            public LogicalSystemKey OemCloseBrackets => LogicalSystemKeys[221];
            public LogicalSystemKey Oem7 => LogicalSystemKeys[222];
            public LogicalSystemKey OemQuotes => LogicalSystemKeys[222];
            public LogicalSystemKey Oem8 => LogicalSystemKeys[223];
            public LogicalSystemKey Oem102 => LogicalSystemKeys[226];
            public LogicalSystemKey OemBackslash => LogicalSystemKeys[226];
            public LogicalSystemKey ProcessKey => LogicalSystemKeys[229];
            public LogicalSystemKey Packet => LogicalSystemKeys[231];
            public LogicalSystemKey Attn => LogicalSystemKeys[246];
            public LogicalSystemKey Crsel => LogicalSystemKeys[247];
            public LogicalSystemKey Exsel => LogicalSystemKeys[248];
            public LogicalSystemKey EraseEof => LogicalSystemKeys[249];
            public LogicalSystemKey Play => LogicalSystemKeys[250];
            public LogicalSystemKey Zoom => LogicalSystemKeys[251];
            public LogicalSystemKey NoName => LogicalSystemKeys[252];
            public LogicalSystemKey Pa1 => LogicalSystemKeys[253];
            public LogicalSystemKey OemClear => LogicalSystemKeys[254];

            public LogicalKeyDeclaration(
                LogicalSingleThrowKeySet singleThrowKeys,
                LogicalSystemKeySet logicalSystemKeys)
            {
                LogicalSingleThrowKeys = singleThrowKeys;
                LogicalSystemKeys = logicalSystemKeys;
            }
        }

        public class PhysicalKeyDeclaration
        {
            private readonly PhysicalSingleThrowKeySet PhysicalSingleThrowKeys;
            private readonly PhysicalSystemKeySet PhysicalSystemKeys;

            public readonly Crevice.Core.Events.NullEvent NullEvent = new Crevice.Core.Events.NullEvent();

            public PhysicalSingleThrowKey WheelUp => PhysicalSingleThrowKeys[1];
            public PhysicalSingleThrowKey WheelDown => PhysicalSingleThrowKeys[2];
            public PhysicalSingleThrowKey WheelLeft => PhysicalSingleThrowKeys[3];
            public PhysicalSingleThrowKey WheelRight => PhysicalSingleThrowKeys[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public PhysicalSystemKey LeftButton => PhysicalSystemKeys[1];
            public PhysicalSystemKey RightButton => PhysicalSystemKeys[2];
            public PhysicalSystemKey MiddleButton => PhysicalSystemKeys[4];
            public PhysicalSystemKey X1Button => PhysicalSystemKeys[5];
            public PhysicalSystemKey X2Button => PhysicalSystemKeys[6];

            public PhysicalSystemKey LButton => PhysicalSystemKeys[1];
            public PhysicalSystemKey RButton => PhysicalSystemKeys[2];
            public PhysicalSystemKey Cancel => PhysicalSystemKeys[3];
            public PhysicalSystemKey MButton => PhysicalSystemKeys[4];
            public PhysicalSystemKey XButton1 => PhysicalSystemKeys[5];
            public PhysicalSystemKey XButton2 => PhysicalSystemKeys[6];
            public PhysicalSystemKey Back => PhysicalSystemKeys[8];
            public PhysicalSystemKey Tab => PhysicalSystemKeys[9];
            public PhysicalSystemKey LineFeed => PhysicalSystemKeys[10];
            public PhysicalSystemKey Clear => PhysicalSystemKeys[12];
            public PhysicalSystemKey Enter => PhysicalSystemKeys[13];
            public PhysicalSystemKey Return => PhysicalSystemKeys[13];
            public PhysicalSystemKey ShiftKey => PhysicalSystemKeys[16];
            public PhysicalSystemKey ControlKey => PhysicalSystemKeys[17];
            public PhysicalSystemKey Menu => PhysicalSystemKeys[18];
            public PhysicalSystemKey Pause => PhysicalSystemKeys[19];
            public PhysicalSystemKey CapsLock => PhysicalSystemKeys[20];
            public PhysicalSystemKey Capital => PhysicalSystemKeys[20];
            public PhysicalSystemKey KanaMode => PhysicalSystemKeys[21];
            public PhysicalSystemKey HangulMode => PhysicalSystemKeys[21];
            public PhysicalSystemKey JunjaMode => PhysicalSystemKeys[23];
            public PhysicalSystemKey FinalMode => PhysicalSystemKeys[24];
            public PhysicalSystemKey KanjiMode => PhysicalSystemKeys[25];
            public PhysicalSystemKey HanjaMode => PhysicalSystemKeys[25];
            public PhysicalSystemKey Escape => PhysicalSystemKeys[27];
            public PhysicalSystemKey IMEConvert => PhysicalSystemKeys[28];
            public PhysicalSystemKey IMENonconvert => PhysicalSystemKeys[29];
            public PhysicalSystemKey IMEAccept => PhysicalSystemKeys[30];
            public PhysicalSystemKey IMEModeChange => PhysicalSystemKeys[31];
            public PhysicalSystemKey Space => PhysicalSystemKeys[32];
            public PhysicalSystemKey Prior => PhysicalSystemKeys[33];
            public PhysicalSystemKey PageUp => PhysicalSystemKeys[33];
            public PhysicalSystemKey Next => PhysicalSystemKeys[34];
            public PhysicalSystemKey PageDown => PhysicalSystemKeys[34];
            public PhysicalSystemKey End => PhysicalSystemKeys[35];
            public PhysicalSystemKey Home => PhysicalSystemKeys[36];
            public PhysicalSystemKey Left => PhysicalSystemKeys[37];
            public PhysicalSystemKey Up => PhysicalSystemKeys[38];
            public PhysicalSystemKey Right => PhysicalSystemKeys[39];
            public PhysicalSystemKey Down => PhysicalSystemKeys[40];
            public PhysicalSystemKey Select => PhysicalSystemKeys[41];
            public PhysicalSystemKey Print => PhysicalSystemKeys[42];
            public PhysicalSystemKey Execute => PhysicalSystemKeys[43];
            public PhysicalSystemKey PrintScreen => PhysicalSystemKeys[44];
            public PhysicalSystemKey Snapshot => PhysicalSystemKeys[44];
            public PhysicalSystemKey Insert => PhysicalSystemKeys[45];
            public PhysicalSystemKey Delete => PhysicalSystemKeys[46];
            public PhysicalSystemKey Help => PhysicalSystemKeys[47];
            public PhysicalSystemKey D0 => PhysicalSystemKeys[48];
            public PhysicalSystemKey D1 => PhysicalSystemKeys[49];
            public PhysicalSystemKey D2 => PhysicalSystemKeys[50];
            public PhysicalSystemKey D3 => PhysicalSystemKeys[51];
            public PhysicalSystemKey D4 => PhysicalSystemKeys[52];
            public PhysicalSystemKey D5 => PhysicalSystemKeys[53];
            public PhysicalSystemKey D6 => PhysicalSystemKeys[54];
            public PhysicalSystemKey D7 => PhysicalSystemKeys[55];
            public PhysicalSystemKey D8 => PhysicalSystemKeys[56];
            public PhysicalSystemKey D9 => PhysicalSystemKeys[57];
            public PhysicalSystemKey A => PhysicalSystemKeys[65];
            public PhysicalSystemKey B => PhysicalSystemKeys[66];
            public PhysicalSystemKey C => PhysicalSystemKeys[67];
            public PhysicalSystemKey D => PhysicalSystemKeys[68];
            public PhysicalSystemKey E => PhysicalSystemKeys[69];
            public PhysicalSystemKey F => PhysicalSystemKeys[70];
            public PhysicalSystemKey G => PhysicalSystemKeys[71];
            public PhysicalSystemKey H => PhysicalSystemKeys[72];
            public PhysicalSystemKey I => PhysicalSystemKeys[73];
            public PhysicalSystemKey J => PhysicalSystemKeys[74];
            public PhysicalSystemKey K => PhysicalSystemKeys[75];
            public PhysicalSystemKey L => PhysicalSystemKeys[76];
            public PhysicalSystemKey M => PhysicalSystemKeys[77];
            public PhysicalSystemKey N => PhysicalSystemKeys[78];
            public PhysicalSystemKey O => PhysicalSystemKeys[79];
            public PhysicalSystemKey P => PhysicalSystemKeys[80];
            public PhysicalSystemKey Q => PhysicalSystemKeys[81];
            public PhysicalSystemKey R => PhysicalSystemKeys[82];
            public PhysicalSystemKey S => PhysicalSystemKeys[83];
            public PhysicalSystemKey T => PhysicalSystemKeys[84];
            public PhysicalSystemKey U => PhysicalSystemKeys[85];
            public PhysicalSystemKey V => PhysicalSystemKeys[86];
            public PhysicalSystemKey W => PhysicalSystemKeys[87];
            public PhysicalSystemKey X => PhysicalSystemKeys[88];
            public PhysicalSystemKey Y => PhysicalSystemKeys[89];
            public PhysicalSystemKey Z => PhysicalSystemKeys[90];
            public PhysicalSystemKey LWin => PhysicalSystemKeys[91];
            public PhysicalSystemKey RWin => PhysicalSystemKeys[92];
            public PhysicalSystemKey Apps => PhysicalSystemKeys[93];
            public PhysicalSystemKey Sleep => PhysicalSystemKeys[95];
            public PhysicalSystemKey NumPad0 => PhysicalSystemKeys[96];
            public PhysicalSystemKey NumPad1 => PhysicalSystemKeys[97];
            public PhysicalSystemKey NumPad2 => PhysicalSystemKeys[98];
            public PhysicalSystemKey NumPad3 => PhysicalSystemKeys[99];
            public PhysicalSystemKey NumPad4 => PhysicalSystemKeys[100];
            public PhysicalSystemKey NumPad5 => PhysicalSystemKeys[101];
            public PhysicalSystemKey NumPad6 => PhysicalSystemKeys[102];
            public PhysicalSystemKey NumPad7 => PhysicalSystemKeys[103];
            public PhysicalSystemKey NumPad8 => PhysicalSystemKeys[104];
            public PhysicalSystemKey NumPad9 => PhysicalSystemKeys[105];
            public PhysicalSystemKey Multiply => PhysicalSystemKeys[106];
            public PhysicalSystemKey Add => PhysicalSystemKeys[107];
            public PhysicalSystemKey Separator => PhysicalSystemKeys[108];
            public PhysicalSystemKey Subtract => PhysicalSystemKeys[109];
            public PhysicalSystemKey Decimal => PhysicalSystemKeys[110];
            public PhysicalSystemKey Divide => PhysicalSystemKeys[111];
            public PhysicalSystemKey F1 => PhysicalSystemKeys[112];
            public PhysicalSystemKey F2 => PhysicalSystemKeys[113];
            public PhysicalSystemKey F3 => PhysicalSystemKeys[114];
            public PhysicalSystemKey F4 => PhysicalSystemKeys[115];
            public PhysicalSystemKey F5 => PhysicalSystemKeys[116];
            public PhysicalSystemKey F6 => PhysicalSystemKeys[117];
            public PhysicalSystemKey F7 => PhysicalSystemKeys[118];
            public PhysicalSystemKey F8 => PhysicalSystemKeys[119];
            public PhysicalSystemKey F9 => PhysicalSystemKeys[120];
            public PhysicalSystemKey F10 => PhysicalSystemKeys[121];
            public PhysicalSystemKey F11 => PhysicalSystemKeys[122];
            public PhysicalSystemKey F12 => PhysicalSystemKeys[123];
            public PhysicalSystemKey F13 => PhysicalSystemKeys[124];
            public PhysicalSystemKey F14 => PhysicalSystemKeys[125];
            public PhysicalSystemKey F15 => PhysicalSystemKeys[126];
            public PhysicalSystemKey F16 => PhysicalSystemKeys[127];
            public PhysicalSystemKey F17 => PhysicalSystemKeys[128];
            public PhysicalSystemKey F18 => PhysicalSystemKeys[129];
            public PhysicalSystemKey F19 => PhysicalSystemKeys[130];
            public PhysicalSystemKey F20 => PhysicalSystemKeys[131];
            public PhysicalSystemKey F21 => PhysicalSystemKeys[132];
            public PhysicalSystemKey F22 => PhysicalSystemKeys[133];
            public PhysicalSystemKey F23 => PhysicalSystemKeys[134];
            public PhysicalSystemKey F24 => PhysicalSystemKeys[135];
            public PhysicalSystemKey NumLock => PhysicalSystemKeys[144];
            public PhysicalSystemKey Scroll => PhysicalSystemKeys[145];
            public PhysicalSystemKey LShiftKey => PhysicalSystemKeys[160];
            public PhysicalSystemKey RShiftKey => PhysicalSystemKeys[161];
            public PhysicalSystemKey LControlKey => PhysicalSystemKeys[162];
            public PhysicalSystemKey RControlKey => PhysicalSystemKeys[163];
            public PhysicalSystemKey LMenu => PhysicalSystemKeys[164];
            public PhysicalSystemKey RMenu => PhysicalSystemKeys[165];
            public PhysicalSystemKey BrowserBack => PhysicalSystemKeys[166];
            public PhysicalSystemKey BrowserForward => PhysicalSystemKeys[167];
            public PhysicalSystemKey BrowserRefresh => PhysicalSystemKeys[168];
            public PhysicalSystemKey BrowserStop => PhysicalSystemKeys[169];
            public PhysicalSystemKey BrowserSearch => PhysicalSystemKeys[170];
            public PhysicalSystemKey BrowserFavorites => PhysicalSystemKeys[171];
            public PhysicalSystemKey BrowserHome => PhysicalSystemKeys[172];
            public PhysicalSystemKey VolumeMute => PhysicalSystemKeys[173];
            public PhysicalSystemKey VolumeDown => PhysicalSystemKeys[174];
            public PhysicalSystemKey VolumeUp => PhysicalSystemKeys[175];
            public PhysicalSystemKey MediaNextTrack => PhysicalSystemKeys[176];
            public PhysicalSystemKey MediaPreviousTrack => PhysicalSystemKeys[177];
            public PhysicalSystemKey MediaStop => PhysicalSystemKeys[178];
            public PhysicalSystemKey MediaPlayPause => PhysicalSystemKeys[179];
            public PhysicalSystemKey LaunchMail => PhysicalSystemKeys[180];
            public PhysicalSystemKey SelectMedia => PhysicalSystemKeys[181];
            public PhysicalSystemKey LaunchApplication1 => PhysicalSystemKeys[182];
            public PhysicalSystemKey LaunchApplication2 => PhysicalSystemKeys[183];
            public PhysicalSystemKey Oem1 => PhysicalSystemKeys[186];
            public PhysicalSystemKey OemSemicolon => PhysicalSystemKeys[186];
            public PhysicalSystemKey Oemplus => PhysicalSystemKeys[187];
            public PhysicalSystemKey Oemcomma => PhysicalSystemKeys[188];
            public PhysicalSystemKey OemMinus => PhysicalSystemKeys[189];
            public PhysicalSystemKey OemPeriod => PhysicalSystemKeys[190];
            public PhysicalSystemKey OemQuestion => PhysicalSystemKeys[191];
            public PhysicalSystemKey Oem2 => PhysicalSystemKeys[191];
            public PhysicalSystemKey Oemtilde => PhysicalSystemKeys[192];
            public PhysicalSystemKey Oem3 => PhysicalSystemKeys[192];
            public PhysicalSystemKey Oem4 => PhysicalSystemKeys[219];
            public PhysicalSystemKey OemOpenBrackets => PhysicalSystemKeys[219];
            public PhysicalSystemKey OemPipe => PhysicalSystemKeys[220];
            public PhysicalSystemKey Oem5 => PhysicalSystemKeys[220];
            public PhysicalSystemKey Oem6 => PhysicalSystemKeys[221];
            public PhysicalSystemKey OemCloseBrackets => PhysicalSystemKeys[221];
            public PhysicalSystemKey Oem7 => PhysicalSystemKeys[222];
            public PhysicalSystemKey OemQuotes => PhysicalSystemKeys[222];
            public PhysicalSystemKey Oem8 => PhysicalSystemKeys[223];
            public PhysicalSystemKey Oem102 => PhysicalSystemKeys[226];
            public PhysicalSystemKey OemBackslash => PhysicalSystemKeys[226];
            public PhysicalSystemKey ProcessKey => PhysicalSystemKeys[229];
            public PhysicalSystemKey Packet => PhysicalSystemKeys[231];
            public PhysicalSystemKey Attn => PhysicalSystemKeys[246];
            public PhysicalSystemKey Crsel => PhysicalSystemKeys[247];
            public PhysicalSystemKey Exsel => PhysicalSystemKeys[248];
            public PhysicalSystemKey EraseEof => PhysicalSystemKeys[249];
            public PhysicalSystemKey Play => PhysicalSystemKeys[250];
            public PhysicalSystemKey Zoom => PhysicalSystemKeys[251];
            public PhysicalSystemKey NoName => PhysicalSystemKeys[252];
            public PhysicalSystemKey Pa1 => PhysicalSystemKeys[253];
            public PhysicalSystemKey OemClear => PhysicalSystemKeys[254];

            public PhysicalKeyDeclaration(PhysicalSingleThrowKeySet singleThrowKeys, PhysicalSystemKeySet systemKeys)
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

    public class CustomContextManager : ContextManager<CustomEvaluationContext, CustomExecutionContext>
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

    public class NullGestureMachine : CustomGestureMachine
    {
        public NullGestureMachine() : base(new CustomRootElement()) { }
    }

    public class CustomGestureMachine : GestureMachine<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        private readonly WinAPI.SendInput.SingleInputSender SingleInputSender = new WinAPI.SendInput.SingleInputSender();

        private readonly TaskFactory UserActionTaskFactory = Task.Factory;

        public CustomGestureMachine(CustomRootElement rootElement)
            : base(new GestureMachineConfig(), new CustomContextManager(), rootElement)
        {
            this.GestureCancelled += new GestureCancelledEventHandler((sender, e) => 
            {
                var systemKey = e.LastState.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
                ExecuteInBackground(RestorePrimaryButtonClickEvent(systemKey));
                Verbose.Print("GestureCancelled");
            });

            this.GestureTimeout += new GestureTimeoutEventHandler((sender, e) =>
            {
                var systemKey = e.LastState.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
                ExecuteInBackground(RestorePrimaryButtonPressEvent(systemKey));
                Verbose.Print("GestureTimeout");
            });

            this.MachineReset += new MachineResetEventHandler((sender, e) => 
            {
                Verbose.Print("MachineReset");
            });
        }

        protected internal void ExecuteInBackground(Action action)
            => UserActionTaskFactory.StartNew(action);

        internal Action RestorePrimaryButtonPressEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
                {
                    SingleInputSender.LeftDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RightButton)
                {
                    SingleInputSender.RightDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MiddleButton)
                {
                    SingleInputSender.MiddleDown();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X1Button)
                {
                    SingleInputSender.X1Down();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X2Button)
                {
                    SingleInputSender.X2Down();
                }
            };
        }

        internal Action RestorePrimaryButtonClickEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
                {
                    SingleInputSender.LeftClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.RightButton)
                {
                    SingleInputSender.RightClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.MiddleButton)
                {
                    SingleInputSender.MiddleClick();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X1Button)
                {
                    SingleInputSender.X1Click();
                }
                else if (systemKey == SupportedKeys.PhysicalKeys.X2Button)
                {
                    SingleInputSender.X2Click();
                }
            };
        }
    }
}
