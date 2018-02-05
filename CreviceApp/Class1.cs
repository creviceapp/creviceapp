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
    using Crevice.Core.Events;

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

        // todo implicit operater to Keys, int   
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
            public readonly LogicalSingleThrowKeySet LogicalSingleThrowKeySet;
            public readonly LogicalSystemKeySet LogicalSystemKeySet;
        
            public LogicalSingleThrowKey WheelUp => LogicalSingleThrowKeySet[1];
            public LogicalSingleThrowKey WheelDown => LogicalSingleThrowKeySet[2];
            public LogicalSingleThrowKey WheelLeft => LogicalSingleThrowKeySet[3];
            public LogicalSingleThrowKey WheelRight => LogicalSingleThrowKeySet[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public LogicalSystemKey LeftButton => LogicalSystemKeySet[1];
            public LogicalSystemKey RightButton => LogicalSystemKeySet[2];
            public LogicalSystemKey MiddleButton => LogicalSystemKeySet[4];
            public LogicalSystemKey X1Button => LogicalSystemKeySet[5];
            public LogicalSystemKey X2Button => LogicalSystemKeySet[6];

            public LogicalSystemKey None => LogicalSystemKeySet[0];
            public LogicalSystemKey LButton => LogicalSystemKeySet[1];
            public LogicalSystemKey RButton => LogicalSystemKeySet[2];
            public LogicalSystemKey Cancel => LogicalSystemKeySet[3];
            public LogicalSystemKey MButton => LogicalSystemKeySet[4];
            public LogicalSystemKey XButton1 => LogicalSystemKeySet[5];
            public LogicalSystemKey XButton2 => LogicalSystemKeySet[6];

            public LogicalSystemKey VirtualKey(int virtualKey) => LogicalSystemKeySet[virtualKey];

            public LogicalSystemKey Back => LogicalSystemKeySet[8];
            public LogicalSystemKey Tab => LogicalSystemKeySet[9];
            public LogicalSystemKey LineFeed => LogicalSystemKeySet[10];
            public LogicalSystemKey Clear => LogicalSystemKeySet[12];
            public LogicalSystemKey Enter => LogicalSystemKeySet[13];
            public LogicalSystemKey Return => LogicalSystemKeySet[13];
            public LogicalSystemKey ShiftKey => LogicalSystemKeySet[16];
            public LogicalSystemKey ControlKey => LogicalSystemKeySet[17];
            public LogicalSystemKey Menu => LogicalSystemKeySet[18];
            public LogicalSystemKey Pause => LogicalSystemKeySet[19];
            public LogicalSystemKey CapsLock => LogicalSystemKeySet[20];
            public LogicalSystemKey Capital => LogicalSystemKeySet[20];
            public LogicalSystemKey KanaMode => LogicalSystemKeySet[21];
            public LogicalSystemKey HangulMode => LogicalSystemKeySet[21];
            public LogicalSystemKey JunjaMode => LogicalSystemKeySet[23];
            public LogicalSystemKey FinalMode => LogicalSystemKeySet[24];
            public LogicalSystemKey KanjiMode => LogicalSystemKeySet[25];
            public LogicalSystemKey HanjaMode => LogicalSystemKeySet[25];
            public LogicalSystemKey Escape => LogicalSystemKeySet[27];
            public LogicalSystemKey IMEConvert => LogicalSystemKeySet[28];
            public LogicalSystemKey IMENonconvert => LogicalSystemKeySet[29];
            public LogicalSystemKey IMEAccept => LogicalSystemKeySet[30];
            public LogicalSystemKey IMEModeChange => LogicalSystemKeySet[31];
            public LogicalSystemKey Space => LogicalSystemKeySet[32];
            public LogicalSystemKey Prior => LogicalSystemKeySet[33];
            public LogicalSystemKey PageUp => LogicalSystemKeySet[33];
            public LogicalSystemKey Next => LogicalSystemKeySet[34];
            public LogicalSystemKey PageDown => LogicalSystemKeySet[34];
            public LogicalSystemKey End => LogicalSystemKeySet[35];
            public LogicalSystemKey Home => LogicalSystemKeySet[36];
            public LogicalSystemKey Left => LogicalSystemKeySet[37];
            public LogicalSystemKey Up => LogicalSystemKeySet[38];
            public LogicalSystemKey Right => LogicalSystemKeySet[39];
            public LogicalSystemKey Down => LogicalSystemKeySet[40];
            public LogicalSystemKey Select => LogicalSystemKeySet[41];
            public LogicalSystemKey Print => LogicalSystemKeySet[42];
            public LogicalSystemKey Execute => LogicalSystemKeySet[43];
            public LogicalSystemKey PrintScreen => LogicalSystemKeySet[44];
            public LogicalSystemKey Snapshot => LogicalSystemKeySet[44];
            public LogicalSystemKey Insert => LogicalSystemKeySet[45];
            public LogicalSystemKey Delete => LogicalSystemKeySet[46];
            public LogicalSystemKey Help => LogicalSystemKeySet[47];
            public LogicalSystemKey D0 => LogicalSystemKeySet[48];
            public LogicalSystemKey D1 => LogicalSystemKeySet[49];
            public LogicalSystemKey D2 => LogicalSystemKeySet[50];
            public LogicalSystemKey D3 => LogicalSystemKeySet[51];
            public LogicalSystemKey D4 => LogicalSystemKeySet[52];
            public LogicalSystemKey D5 => LogicalSystemKeySet[53];
            public LogicalSystemKey D6 => LogicalSystemKeySet[54];
            public LogicalSystemKey D7 => LogicalSystemKeySet[55];
            public LogicalSystemKey D8 => LogicalSystemKeySet[56];
            public LogicalSystemKey D9 => LogicalSystemKeySet[57];
            public LogicalSystemKey A => LogicalSystemKeySet[65];
            public LogicalSystemKey B => LogicalSystemKeySet[66];
            public LogicalSystemKey C => LogicalSystemKeySet[67];
            public LogicalSystemKey D => LogicalSystemKeySet[68];
            public LogicalSystemKey E => LogicalSystemKeySet[69];
            public LogicalSystemKey F => LogicalSystemKeySet[70];
            public LogicalSystemKey G => LogicalSystemKeySet[71];
            public LogicalSystemKey H => LogicalSystemKeySet[72];
            public LogicalSystemKey I => LogicalSystemKeySet[73];
            public LogicalSystemKey J => LogicalSystemKeySet[74];
            public LogicalSystemKey K => LogicalSystemKeySet[75];
            public LogicalSystemKey L => LogicalSystemKeySet[76];
            public LogicalSystemKey M => LogicalSystemKeySet[77];
            public LogicalSystemKey N => LogicalSystemKeySet[78];
            public LogicalSystemKey O => LogicalSystemKeySet[79];
            public LogicalSystemKey P => LogicalSystemKeySet[80];
            public LogicalSystemKey Q => LogicalSystemKeySet[81];
            public LogicalSystemKey R => LogicalSystemKeySet[82];
            public LogicalSystemKey S => LogicalSystemKeySet[83];
            public LogicalSystemKey T => LogicalSystemKeySet[84];
            public LogicalSystemKey U => LogicalSystemKeySet[85];
            public LogicalSystemKey V => LogicalSystemKeySet[86];
            public LogicalSystemKey W => LogicalSystemKeySet[87];
            public LogicalSystemKey X => LogicalSystemKeySet[88];
            public LogicalSystemKey Y => LogicalSystemKeySet[89];
            public LogicalSystemKey Z => LogicalSystemKeySet[90];
            public LogicalSystemKey LWin => LogicalSystemKeySet[91];
            public LogicalSystemKey RWin => LogicalSystemKeySet[92];
            public LogicalSystemKey Apps => LogicalSystemKeySet[93];
            public LogicalSystemKey Sleep => LogicalSystemKeySet[95];
            public LogicalSystemKey NumPad0 => LogicalSystemKeySet[96];
            public LogicalSystemKey NumPad1 => LogicalSystemKeySet[97];
            public LogicalSystemKey NumPad2 => LogicalSystemKeySet[98];
            public LogicalSystemKey NumPad3 => LogicalSystemKeySet[99];
            public LogicalSystemKey NumPad4 => LogicalSystemKeySet[100];
            public LogicalSystemKey NumPad5 => LogicalSystemKeySet[101];
            public LogicalSystemKey NumPad6 => LogicalSystemKeySet[102];
            public LogicalSystemKey NumPad7 => LogicalSystemKeySet[103];
            public LogicalSystemKey NumPad8 => LogicalSystemKeySet[104];
            public LogicalSystemKey NumPad9 => LogicalSystemKeySet[105];
            public LogicalSystemKey Multiply => LogicalSystemKeySet[106];
            public LogicalSystemKey Add => LogicalSystemKeySet[107];
            public LogicalSystemKey Separator => LogicalSystemKeySet[108];
            public LogicalSystemKey Subtract => LogicalSystemKeySet[109];
            public LogicalSystemKey Decimal => LogicalSystemKeySet[110];
            public LogicalSystemKey Divide => LogicalSystemKeySet[111];
            public LogicalSystemKey F1 => LogicalSystemKeySet[112];
            public LogicalSystemKey F2 => LogicalSystemKeySet[113];
            public LogicalSystemKey F3 => LogicalSystemKeySet[114];
            public LogicalSystemKey F4 => LogicalSystemKeySet[115];
            public LogicalSystemKey F5 => LogicalSystemKeySet[116];
            public LogicalSystemKey F6 => LogicalSystemKeySet[117];
            public LogicalSystemKey F7 => LogicalSystemKeySet[118];
            public LogicalSystemKey F8 => LogicalSystemKeySet[119];
            public LogicalSystemKey F9 => LogicalSystemKeySet[120];
            public LogicalSystemKey F10 => LogicalSystemKeySet[121];
            public LogicalSystemKey F11 => LogicalSystemKeySet[122];
            public LogicalSystemKey F12 => LogicalSystemKeySet[123];
            public LogicalSystemKey F13 => LogicalSystemKeySet[124];
            public LogicalSystemKey F14 => LogicalSystemKeySet[125];
            public LogicalSystemKey F15 => LogicalSystemKeySet[126];
            public LogicalSystemKey F16 => LogicalSystemKeySet[127];
            public LogicalSystemKey F17 => LogicalSystemKeySet[128];
            public LogicalSystemKey F18 => LogicalSystemKeySet[129];
            public LogicalSystemKey F19 => LogicalSystemKeySet[130];
            public LogicalSystemKey F20 => LogicalSystemKeySet[131];
            public LogicalSystemKey F21 => LogicalSystemKeySet[132];
            public LogicalSystemKey F22 => LogicalSystemKeySet[133];
            public LogicalSystemKey F23 => LogicalSystemKeySet[134];
            public LogicalSystemKey F24 => LogicalSystemKeySet[135];
            public LogicalSystemKey NumLock => LogicalSystemKeySet[144];
            public LogicalSystemKey Scroll => LogicalSystemKeySet[145];
            public LogicalSystemKey LShiftKey => LogicalSystemKeySet[160];
            public LogicalSystemKey RShiftKey => LogicalSystemKeySet[161];
            public LogicalSystemKey LControlKey => LogicalSystemKeySet[162];
            public LogicalSystemKey RControlKey => LogicalSystemKeySet[163];
            public LogicalSystemKey LMenu => LogicalSystemKeySet[164];
            public LogicalSystemKey RMenu => LogicalSystemKeySet[165];
            public LogicalSystemKey BrowserBack => LogicalSystemKeySet[166];
            public LogicalSystemKey BrowserForward => LogicalSystemKeySet[167];
            public LogicalSystemKey BrowserRefresh => LogicalSystemKeySet[168];
            public LogicalSystemKey BrowserStop => LogicalSystemKeySet[169];
            public LogicalSystemKey BrowserSearch => LogicalSystemKeySet[170];
            public LogicalSystemKey BrowserFavorites => LogicalSystemKeySet[171];
            public LogicalSystemKey BrowserHome => LogicalSystemKeySet[172];
            public LogicalSystemKey VolumeMute => LogicalSystemKeySet[173];
            public LogicalSystemKey VolumeDown => LogicalSystemKeySet[174];
            public LogicalSystemKey VolumeUp => LogicalSystemKeySet[175];
            public LogicalSystemKey MediaNextTrack => LogicalSystemKeySet[176];
            public LogicalSystemKey MediaPreviousTrack => LogicalSystemKeySet[177];
            public LogicalSystemKey MediaStop => LogicalSystemKeySet[178];
            public LogicalSystemKey MediaPlayPause => LogicalSystemKeySet[179];
            public LogicalSystemKey LaunchMail => LogicalSystemKeySet[180];
            public LogicalSystemKey SelectMedia => LogicalSystemKeySet[181];
            public LogicalSystemKey LaunchApplication1 => LogicalSystemKeySet[182];
            public LogicalSystemKey LaunchApplication2 => LogicalSystemKeySet[183];
            public LogicalSystemKey Oem1 => LogicalSystemKeySet[186];
            public LogicalSystemKey OemSemicolon => LogicalSystemKeySet[186];
            public LogicalSystemKey Oemplus => LogicalSystemKeySet[187];
            public LogicalSystemKey Oemcomma => LogicalSystemKeySet[188];
            public LogicalSystemKey OemMinus => LogicalSystemKeySet[189];
            public LogicalSystemKey OemPeriod => LogicalSystemKeySet[190];
            public LogicalSystemKey OemQuestion => LogicalSystemKeySet[191];
            public LogicalSystemKey Oem2 => LogicalSystemKeySet[191];
            public LogicalSystemKey Oemtilde => LogicalSystemKeySet[192];
            public LogicalSystemKey Oem3 => LogicalSystemKeySet[192];
            public LogicalSystemKey Oem4 => LogicalSystemKeySet[219];
            public LogicalSystemKey OemOpenBrackets => LogicalSystemKeySet[219];
            public LogicalSystemKey OemPipe => LogicalSystemKeySet[220];
            public LogicalSystemKey Oem5 => LogicalSystemKeySet[220];
            public LogicalSystemKey Oem6 => LogicalSystemKeySet[221];
            public LogicalSystemKey OemCloseBrackets => LogicalSystemKeySet[221];
            public LogicalSystemKey Oem7 => LogicalSystemKeySet[222];
            public LogicalSystemKey OemQuotes => LogicalSystemKeySet[222];
            public LogicalSystemKey Oem8 => LogicalSystemKeySet[223];
            public LogicalSystemKey Oem102 => LogicalSystemKeySet[226];
            public LogicalSystemKey OemBackslash => LogicalSystemKeySet[226];
            public LogicalSystemKey ProcessKey => LogicalSystemKeySet[229];
            public LogicalSystemKey Packet => LogicalSystemKeySet[231];
            public LogicalSystemKey Attn => LogicalSystemKeySet[246];
            public LogicalSystemKey Crsel => LogicalSystemKeySet[247];
            public LogicalSystemKey Exsel => LogicalSystemKeySet[248];
            public LogicalSystemKey EraseEof => LogicalSystemKeySet[249];
            public LogicalSystemKey Play => LogicalSystemKeySet[250];
            public LogicalSystemKey Zoom => LogicalSystemKeySet[251];
            public LogicalSystemKey NoName => LogicalSystemKeySet[252];
            public LogicalSystemKey Pa1 => LogicalSystemKeySet[253];
            public LogicalSystemKey OemClear => LogicalSystemKeySet[254];

            public LogicalKeyDeclaration(
                LogicalSingleThrowKeySet singleThrowKeys,
                LogicalSystemKeySet logicalSystemKeys)
            {
                LogicalSingleThrowKeySet = singleThrowKeys;
                LogicalSystemKeySet = logicalSystemKeys;
            }
        }

        public class PhysicalKeyDeclaration
        {
            public readonly PhysicalSingleThrowKeySet PhysicalSingleThrowKeySet;
            public readonly PhysicalSystemKeySet PhysicalSystemKeySet;

            public readonly Crevice.Core.Events.NullEvent NullEvent = new Crevice.Core.Events.NullEvent();

            public PhysicalSingleThrowKey WheelUp => PhysicalSingleThrowKeySet[1];
            public PhysicalSingleThrowKey WheelDown => PhysicalSingleThrowKeySet[2];
            public PhysicalSingleThrowKey WheelLeft => PhysicalSingleThrowKeySet[3];
            public PhysicalSingleThrowKey WheelRight => PhysicalSingleThrowKeySet[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public PhysicalSystemKey LeftButton => PhysicalSystemKeySet[1];
            public PhysicalSystemKey RightButton => PhysicalSystemKeySet[2];
            public PhysicalSystemKey MiddleButton => PhysicalSystemKeySet[4];
            public PhysicalSystemKey X1Button => PhysicalSystemKeySet[5];
            public PhysicalSystemKey X2Button => PhysicalSystemKeySet[6];

            // todo this[] に
            public PhysicalSystemKey VirtualKey(int virtualKey) => PhysicalSystemKeySet[virtualKey];

            public PhysicalSystemKey None => PhysicalSystemKeySet[0];
            public PhysicalSystemKey LButton => PhysicalSystemKeySet[1];
            public PhysicalSystemKey RButton => PhysicalSystemKeySet[2];
            public PhysicalSystemKey Cancel => PhysicalSystemKeySet[3];
            public PhysicalSystemKey MButton => PhysicalSystemKeySet[4];
            public PhysicalSystemKey XButton1 => PhysicalSystemKeySet[5];
            public PhysicalSystemKey XButton2 => PhysicalSystemKeySet[6];
            public PhysicalSystemKey Back => PhysicalSystemKeySet[8];
            public PhysicalSystemKey Tab => PhysicalSystemKeySet[9];
            public PhysicalSystemKey LineFeed => PhysicalSystemKeySet[10];
            public PhysicalSystemKey Clear => PhysicalSystemKeySet[12];
            public PhysicalSystemKey Enter => PhysicalSystemKeySet[13];
            public PhysicalSystemKey Return => PhysicalSystemKeySet[13];
            public PhysicalSystemKey ShiftKey => PhysicalSystemKeySet[16];
            public PhysicalSystemKey ControlKey => PhysicalSystemKeySet[17];
            public PhysicalSystemKey Menu => PhysicalSystemKeySet[18];
            public PhysicalSystemKey Pause => PhysicalSystemKeySet[19];
            public PhysicalSystemKey CapsLock => PhysicalSystemKeySet[20];
            public PhysicalSystemKey Capital => PhysicalSystemKeySet[20];
            public PhysicalSystemKey KanaMode => PhysicalSystemKeySet[21];
            public PhysicalSystemKey HangulMode => PhysicalSystemKeySet[21];
            public PhysicalSystemKey JunjaMode => PhysicalSystemKeySet[23];
            public PhysicalSystemKey FinalMode => PhysicalSystemKeySet[24];
            public PhysicalSystemKey KanjiMode => PhysicalSystemKeySet[25];
            public PhysicalSystemKey HanjaMode => PhysicalSystemKeySet[25];
            public PhysicalSystemKey Escape => PhysicalSystemKeySet[27];
            public PhysicalSystemKey IMEConvert => PhysicalSystemKeySet[28];
            public PhysicalSystemKey IMENonconvert => PhysicalSystemKeySet[29];
            public PhysicalSystemKey IMEAccept => PhysicalSystemKeySet[30];
            public PhysicalSystemKey IMEModeChange => PhysicalSystemKeySet[31];
            public PhysicalSystemKey Space => PhysicalSystemKeySet[32];
            public PhysicalSystemKey Prior => PhysicalSystemKeySet[33];
            public PhysicalSystemKey PageUp => PhysicalSystemKeySet[33];
            public PhysicalSystemKey Next => PhysicalSystemKeySet[34];
            public PhysicalSystemKey PageDown => PhysicalSystemKeySet[34];
            public PhysicalSystemKey End => PhysicalSystemKeySet[35];
            public PhysicalSystemKey Home => PhysicalSystemKeySet[36];
            public PhysicalSystemKey Left => PhysicalSystemKeySet[37];
            public PhysicalSystemKey Up => PhysicalSystemKeySet[38];
            public PhysicalSystemKey Right => PhysicalSystemKeySet[39];
            public PhysicalSystemKey Down => PhysicalSystemKeySet[40];
            public PhysicalSystemKey Select => PhysicalSystemKeySet[41];
            public PhysicalSystemKey Print => PhysicalSystemKeySet[42];
            public PhysicalSystemKey Execute => PhysicalSystemKeySet[43];
            public PhysicalSystemKey PrintScreen => PhysicalSystemKeySet[44];
            public PhysicalSystemKey Snapshot => PhysicalSystemKeySet[44];
            public PhysicalSystemKey Insert => PhysicalSystemKeySet[45];
            public PhysicalSystemKey Delete => PhysicalSystemKeySet[46];
            public PhysicalSystemKey Help => PhysicalSystemKeySet[47];
            public PhysicalSystemKey D0 => PhysicalSystemKeySet[48];
            public PhysicalSystemKey D1 => PhysicalSystemKeySet[49];
            public PhysicalSystemKey D2 => PhysicalSystemKeySet[50];
            public PhysicalSystemKey D3 => PhysicalSystemKeySet[51];
            public PhysicalSystemKey D4 => PhysicalSystemKeySet[52];
            public PhysicalSystemKey D5 => PhysicalSystemKeySet[53];
            public PhysicalSystemKey D6 => PhysicalSystemKeySet[54];
            public PhysicalSystemKey D7 => PhysicalSystemKeySet[55];
            public PhysicalSystemKey D8 => PhysicalSystemKeySet[56];
            public PhysicalSystemKey D9 => PhysicalSystemKeySet[57];
            public PhysicalSystemKey A => PhysicalSystemKeySet[65];
            public PhysicalSystemKey B => PhysicalSystemKeySet[66];
            public PhysicalSystemKey C => PhysicalSystemKeySet[67];
            public PhysicalSystemKey D => PhysicalSystemKeySet[68];
            public PhysicalSystemKey E => PhysicalSystemKeySet[69];
            public PhysicalSystemKey F => PhysicalSystemKeySet[70];
            public PhysicalSystemKey G => PhysicalSystemKeySet[71];
            public PhysicalSystemKey H => PhysicalSystemKeySet[72];
            public PhysicalSystemKey I => PhysicalSystemKeySet[73];
            public PhysicalSystemKey J => PhysicalSystemKeySet[74];
            public PhysicalSystemKey K => PhysicalSystemKeySet[75];
            public PhysicalSystemKey L => PhysicalSystemKeySet[76];
            public PhysicalSystemKey M => PhysicalSystemKeySet[77];
            public PhysicalSystemKey N => PhysicalSystemKeySet[78];
            public PhysicalSystemKey O => PhysicalSystemKeySet[79];
            public PhysicalSystemKey P => PhysicalSystemKeySet[80];
            public PhysicalSystemKey Q => PhysicalSystemKeySet[81];
            public PhysicalSystemKey R => PhysicalSystemKeySet[82];
            public PhysicalSystemKey S => PhysicalSystemKeySet[83];
            public PhysicalSystemKey T => PhysicalSystemKeySet[84];
            public PhysicalSystemKey U => PhysicalSystemKeySet[85];
            public PhysicalSystemKey V => PhysicalSystemKeySet[86];
            public PhysicalSystemKey W => PhysicalSystemKeySet[87];
            public PhysicalSystemKey X => PhysicalSystemKeySet[88];
            public PhysicalSystemKey Y => PhysicalSystemKeySet[89];
            public PhysicalSystemKey Z => PhysicalSystemKeySet[90];
            public PhysicalSystemKey LWin => PhysicalSystemKeySet[91];
            public PhysicalSystemKey RWin => PhysicalSystemKeySet[92];
            public PhysicalSystemKey Apps => PhysicalSystemKeySet[93];
            public PhysicalSystemKey Sleep => PhysicalSystemKeySet[95];
            public PhysicalSystemKey NumPad0 => PhysicalSystemKeySet[96];
            public PhysicalSystemKey NumPad1 => PhysicalSystemKeySet[97];
            public PhysicalSystemKey NumPad2 => PhysicalSystemKeySet[98];
            public PhysicalSystemKey NumPad3 => PhysicalSystemKeySet[99];
            public PhysicalSystemKey NumPad4 => PhysicalSystemKeySet[100];
            public PhysicalSystemKey NumPad5 => PhysicalSystemKeySet[101];
            public PhysicalSystemKey NumPad6 => PhysicalSystemKeySet[102];
            public PhysicalSystemKey NumPad7 => PhysicalSystemKeySet[103];
            public PhysicalSystemKey NumPad8 => PhysicalSystemKeySet[104];
            public PhysicalSystemKey NumPad9 => PhysicalSystemKeySet[105];
            public PhysicalSystemKey Multiply => PhysicalSystemKeySet[106];
            public PhysicalSystemKey Add => PhysicalSystemKeySet[107];
            public PhysicalSystemKey Separator => PhysicalSystemKeySet[108];
            public PhysicalSystemKey Subtract => PhysicalSystemKeySet[109];
            public PhysicalSystemKey Decimal => PhysicalSystemKeySet[110];
            public PhysicalSystemKey Divide => PhysicalSystemKeySet[111];
            public PhysicalSystemKey F1 => PhysicalSystemKeySet[112];
            public PhysicalSystemKey F2 => PhysicalSystemKeySet[113];
            public PhysicalSystemKey F3 => PhysicalSystemKeySet[114];
            public PhysicalSystemKey F4 => PhysicalSystemKeySet[115];
            public PhysicalSystemKey F5 => PhysicalSystemKeySet[116];
            public PhysicalSystemKey F6 => PhysicalSystemKeySet[117];
            public PhysicalSystemKey F7 => PhysicalSystemKeySet[118];
            public PhysicalSystemKey F8 => PhysicalSystemKeySet[119];
            public PhysicalSystemKey F9 => PhysicalSystemKeySet[120];
            public PhysicalSystemKey F10 => PhysicalSystemKeySet[121];
            public PhysicalSystemKey F11 => PhysicalSystemKeySet[122];
            public PhysicalSystemKey F12 => PhysicalSystemKeySet[123];
            public PhysicalSystemKey F13 => PhysicalSystemKeySet[124];
            public PhysicalSystemKey F14 => PhysicalSystemKeySet[125];
            public PhysicalSystemKey F15 => PhysicalSystemKeySet[126];
            public PhysicalSystemKey F16 => PhysicalSystemKeySet[127];
            public PhysicalSystemKey F17 => PhysicalSystemKeySet[128];
            public PhysicalSystemKey F18 => PhysicalSystemKeySet[129];
            public PhysicalSystemKey F19 => PhysicalSystemKeySet[130];
            public PhysicalSystemKey F20 => PhysicalSystemKeySet[131];
            public PhysicalSystemKey F21 => PhysicalSystemKeySet[132];
            public PhysicalSystemKey F22 => PhysicalSystemKeySet[133];
            public PhysicalSystemKey F23 => PhysicalSystemKeySet[134];
            public PhysicalSystemKey F24 => PhysicalSystemKeySet[135];
            public PhysicalSystemKey NumLock => PhysicalSystemKeySet[144];
            public PhysicalSystemKey Scroll => PhysicalSystemKeySet[145];
            public PhysicalSystemKey LShiftKey => PhysicalSystemKeySet[160];
            public PhysicalSystemKey RShiftKey => PhysicalSystemKeySet[161];
            public PhysicalSystemKey LControlKey => PhysicalSystemKeySet[162];
            public PhysicalSystemKey RControlKey => PhysicalSystemKeySet[163];
            public PhysicalSystemKey LMenu => PhysicalSystemKeySet[164];
            public PhysicalSystemKey RMenu => PhysicalSystemKeySet[165];
            public PhysicalSystemKey BrowserBack => PhysicalSystemKeySet[166];
            public PhysicalSystemKey BrowserForward => PhysicalSystemKeySet[167];
            public PhysicalSystemKey BrowserRefresh => PhysicalSystemKeySet[168];
            public PhysicalSystemKey BrowserStop => PhysicalSystemKeySet[169];
            public PhysicalSystemKey BrowserSearch => PhysicalSystemKeySet[170];
            public PhysicalSystemKey BrowserFavorites => PhysicalSystemKeySet[171];
            public PhysicalSystemKey BrowserHome => PhysicalSystemKeySet[172];
            public PhysicalSystemKey VolumeMute => PhysicalSystemKeySet[173];
            public PhysicalSystemKey VolumeDown => PhysicalSystemKeySet[174];
            public PhysicalSystemKey VolumeUp => PhysicalSystemKeySet[175];
            public PhysicalSystemKey MediaNextTrack => PhysicalSystemKeySet[176];
            public PhysicalSystemKey MediaPreviousTrack => PhysicalSystemKeySet[177];
            public PhysicalSystemKey MediaStop => PhysicalSystemKeySet[178];
            public PhysicalSystemKey MediaPlayPause => PhysicalSystemKeySet[179];
            public PhysicalSystemKey LaunchMail => PhysicalSystemKeySet[180];
            public PhysicalSystemKey SelectMedia => PhysicalSystemKeySet[181];
            public PhysicalSystemKey LaunchApplication1 => PhysicalSystemKeySet[182];
            public PhysicalSystemKey LaunchApplication2 => PhysicalSystemKeySet[183];
            public PhysicalSystemKey Oem1 => PhysicalSystemKeySet[186];
            public PhysicalSystemKey OemSemicolon => PhysicalSystemKeySet[186];
            public PhysicalSystemKey Oemplus => PhysicalSystemKeySet[187];
            public PhysicalSystemKey Oemcomma => PhysicalSystemKeySet[188];
            public PhysicalSystemKey OemMinus => PhysicalSystemKeySet[189];
            public PhysicalSystemKey OemPeriod => PhysicalSystemKeySet[190];
            public PhysicalSystemKey OemQuestion => PhysicalSystemKeySet[191];
            public PhysicalSystemKey Oem2 => PhysicalSystemKeySet[191];
            public PhysicalSystemKey Oemtilde => PhysicalSystemKeySet[192];
            public PhysicalSystemKey Oem3 => PhysicalSystemKeySet[192];
            public PhysicalSystemKey Oem4 => PhysicalSystemKeySet[219];
            public PhysicalSystemKey OemOpenBrackets => PhysicalSystemKeySet[219];
            public PhysicalSystemKey OemPipe => PhysicalSystemKeySet[220];
            public PhysicalSystemKey Oem5 => PhysicalSystemKeySet[220];
            public PhysicalSystemKey Oem6 => PhysicalSystemKeySet[221];
            public PhysicalSystemKey OemCloseBrackets => PhysicalSystemKeySet[221];
            public PhysicalSystemKey Oem7 => PhysicalSystemKeySet[222];
            public PhysicalSystemKey OemQuotes => PhysicalSystemKeySet[222];
            public PhysicalSystemKey Oem8 => PhysicalSystemKeySet[223];
            public PhysicalSystemKey Oem102 => PhysicalSystemKeySet[226];
            public PhysicalSystemKey OemBackslash => PhysicalSystemKeySet[226];
            public PhysicalSystemKey ProcessKey => PhysicalSystemKeySet[229];
            public PhysicalSystemKey Packet => PhysicalSystemKeySet[231];
            public PhysicalSystemKey Attn => PhysicalSystemKeySet[246];
            public PhysicalSystemKey Crsel => PhysicalSystemKeySet[247];
            public PhysicalSystemKey Exsel => PhysicalSystemKeySet[248];
            public PhysicalSystemKey EraseEof => PhysicalSystemKeySet[249];
            public PhysicalSystemKey Play => PhysicalSystemKeySet[250];
            public PhysicalSystemKey Zoom => PhysicalSystemKeySet[251];
            public PhysicalSystemKey NoName => PhysicalSystemKeySet[252];
            public PhysicalSystemKey Pa1 => PhysicalSystemKeySet[253];
            public PhysicalSystemKey OemClear => PhysicalSystemKeySet[254];

            public PhysicalKeyDeclaration(PhysicalSingleThrowKeySet singleThrowKeys, PhysicalSystemKeySet systemKeys)
            {
                PhysicalSingleThrowKeySet = singleThrowKeys;
                PhysicalSystemKeySet = systemKeys;
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

        public override bool Evaluate(
            CustomEvaluationContext evalContext,
            WhenElement<CustomEvaluationContext, CustomExecutionContext> whenElement)
        {
            var task = Task.Factory.StartNew(() => 
            {
                return whenElement.WhenEvaluator(evalContext);
            });
            try
            {
                return task.Wait(20);
            }
            catch(Exception)
            {
                return false;
            }
        }

        public override void Execute(
            CustomExecutionContext execContext, 
            ExecuteAction<CustomExecutionContext> executeAction)
        {
            Task.Factory.StartNew(() =>
            {
                executeAction(execContext);
            });
        }
    }
    
    public class CustomRootElement : RootElement<CustomEvaluationContext, CustomExecutionContext>
    {

    }

    public class CustomCallbackManager : CallbackManager<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        private readonly TaskFactory SystemKeyRestorationTaskFactory = Task.Factory;

        private readonly WinAPI.SendInput.SingleInputSender SingleInputSender = new WinAPI.SendInput.SingleInputSender();

        public override void OnGestureCancelled(
            StateN<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext> stateN)
        {
            Verbose.Print("GestureCancelled");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressAndReleaseEvent(systemKey));
            base.OnGestureCancelled(stateN);
        }

        public override void OnGestureTimeout(
            StateN<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext> stateN)
        {
            Verbose.Print("GestureTimeout");
            var systemKey = stateN.NormalEndTrigger.PhysicalKey as PhysicalSystemKey;
            ExecuteInBackground(SystemKeyRestorationTaskFactory, RestoreKeyPressEvent(systemKey));
            base.OnGestureTimeout(stateN);
        }

        public override void OnMachineReset(
            IState state)
        {
            Verbose.Print("MachineReset");
            base.OnMachineReset(state);
        }

        protected internal void ExecuteInBackground(TaskFactory taskFactory, Action action)
            => taskFactory.StartNew(action);

        internal Action RestoreKeyPressEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
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
                else
                {
                    SingleInputSender.ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId);
                }
            };
        }

        internal Action RestoreKeyPressAndReleaseEvent(PhysicalSystemKey systemKey)
        {
            return () =>
            {
                if (systemKey == SupportedKeys.PhysicalKeys.None)
                {

                }
                else if (systemKey == SupportedKeys.PhysicalKeys.LeftButton)
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
                else
                {
                    SingleInputSender.Multiple()
                        .ExtendedKeyDownWithScanCode((ushort)systemKey.KeyId)
                        .ExtendedKeyUpWithScanCode((ushort)systemKey.KeyId)
                        .Send();
                }
            };
        }
    }

    public class NullGestureMachine : CustomGestureMachine
    {
        public NullGestureMachine() : base(new CustomRootElement()) { }
    }

    public class CustomGestureMachine : GestureMachine<GestureMachineConfig, CustomContextManager, CustomEvaluationContext, CustomExecutionContext>
    {
        public CustomGestureMachine(CustomRootElement rootElement)
            : base(new GestureMachineConfig(), new CustomCallbackManager(), new CustomContextManager(), rootElement)
        {

        }

        public override bool Input(IPhysicalEvent evnt, Point? point)
        {
            lock (lockObject)
            {
                if (point.HasValue)
                {
                    ContextManager.CursorPosition = point.Value;
                }
                return base.Input(evnt, point);
            }
        }
    }
}
