using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.UserScript.Keys
{
    using Crevice.Core.Keys;
    using Crevice.Core.Stroke;


    public class LogicalMouseWheelKeySet : KeySet<LogicalMouseWheelKey>
    {
        public override LogicalMouseWheelKey Create(int index)
            => new LogicalMouseWheelKey(index);

        public LogicalMouseWheelKeySet()
            : base(5) { }

        public static string KeyIdToKeyName(int keyId)
        {
            switch (keyId)
            {
                case 1: return "WheelUp";
                case 2: return "WheelDown";
                case 3: return "WheelLeft";
                case 4: return "WheelRight";
            }
            return $"0x{keyId.ToString("x")}";
        }
    }

    public class LogicalMouseWheelKey : LogicalSingleThrowKey
    {
        public LogicalMouseWheelKey(int keyId)
            : base(keyId) { }

        public override string ToString()
            => $"LogicalMouseWheelKey({LogicalMouseWheelKeySet.KeyIdToKeyName(KeyId)})";
    }

    public class PhysicalMouseWheelKeySet : KeySet<PhysicalMouseWheelKey>
    {
        public override PhysicalMouseWheelKey Create(int index)
            => new PhysicalMouseWheelKey(logicalKeys[index], index);

        private LogicalMouseWheelKeySet logicalKeys;

        public PhysicalMouseWheelKeySet(LogicalMouseWheelKeySet logicalKeys)
            : base(logicalKeys.MaxSize)
        {
            this.logicalKeys = logicalKeys;
        }
    }

    public class PhysicalMouseWheelKey : PhysicalSingleThrowKey
    {
        public PhysicalMouseWheelKey(LogicalSingleThrowKey logicalKey, int keyId)
            : base(logicalKey, keyId) { }

        public override string ToString()
            => $"PhysicalMouseWheelKey({LogicalMouseWheelKeySet.KeyIdToKeyName(KeyId)})";
    }

    public class LogicalSystemKeySet : KeySet<LogicalSystemKey>
    {
        public override LogicalSystemKey Create(int index)
            => new LogicalSystemKey(index);

        public LogicalSystemKeySet()
            : base(256) { }

        public static string KeyIdToKeyName(int keyId)
        {
            switch (keyId)
            {
                case 0: return "None";
                case 1: return "LButton";
                case 2: return "RButton";
                case 3: return "Cancel";
                case 4: return "MButton";
                case 5: return "XButton1";
                case 6: return "XButton2";
                case 8: return "Back";
                case 9: return "Tab";
                case 10: return "LineFeed";
                case 12: return "Clear";
                case 13: return "Enter";
                case 16: return "ShiftKey";
                case 17: return "ControlKey";
                case 18: return "Menu";
                case 19: return "Pause";
                case 20: return "CapsLock";
                case 21: return "KanaMode";
                case 23: return "JunjaMode";
                case 24: return "FinalMode";
                case 25: return "KanjiMode";
                case 27: return "Escape";
                case 28: return "IMEConvert";
                case 29: return "IMENonconvert";
                case 30: return "IMEAccept";
                case 31: return "IMEModeChange";
                case 32: return "Space";
                case 33: return "PageUp";
                case 34: return "PageDown";
                case 35: return "End";
                case 36: return "Home";
                case 37: return "Left";
                case 38: return "Up";
                case 39: return "Right";
                case 40: return "Down";
                case 41: return "Select";
                case 42: return "Print";
                case 43: return "Execute";
                case 44: return "PrintScreen";
                case 45: return "Insert";
                case 46: return "Delete";
                case 47: return "Help";
                case 48: return "D0";
                case 49: return "D1";
                case 50: return "D2";
                case 51: return "D3";
                case 52: return "D4";
                case 53: return "D5";
                case 54: return "D6";
                case 55: return "D7";
                case 56: return "D8";
                case 57: return "D9";
                case 65: return "A";
                case 66: return "B";
                case 67: return "C";
                case 68: return "D";
                case 69: return "E";
                case 70: return "F";
                case 71: return "G";
                case 72: return "H";
                case 73: return "I";
                case 74: return "J";
                case 75: return "K";
                case 76: return "L";
                case 77: return "M";
                case 78: return "N";
                case 79: return "O";
                case 80: return "P";
                case 81: return "Q";
                case 82: return "R";
                case 83: return "S";
                case 84: return "T";
                case 85: return "U";
                case 86: return "V";
                case 87: return "W";
                case 88: return "X";
                case 89: return "Y";
                case 90: return "Z";
                case 91: return "LWin";
                case 92: return "RWin";
                case 93: return "Apps";
                case 95: return "Sleep";
                case 96: return "NumPad0";
                case 97: return "NumPad1";
                case 98: return "NumPad2";
                case 99: return "NumPad3";
                case 100: return "NumPad4";
                case 101: return "NumPad5";
                case 102: return "NumPad6";
                case 103: return "NumPad7";
                case 104: return "NumPad8";
                case 105: return "NumPad9";
                case 106: return "Multiply";
                case 107: return "Add";
                case 108: return "Separator";
                case 109: return "Subtract";
                case 110: return "Decimal";
                case 111: return "Divide";
                case 112: return "F1";
                case 113: return "F2";
                case 114: return "F3";
                case 115: return "F4";
                case 116: return "F5";
                case 117: return "F6";
                case 118: return "F7";
                case 119: return "F8";
                case 120: return "F9";
                case 121: return "F10";
                case 122: return "F11";
                case 123: return "F12";
                case 124: return "F13";
                case 125: return "F14";
                case 126: return "F15";
                case 127: return "F16";
                case 128: return "F17";
                case 129: return "F18";
                case 130: return "F19";
                case 131: return "F20";
                case 132: return "F21";
                case 133: return "F22";
                case 134: return "F23";
                case 135: return "F24";
                case 144: return "NumLock";
                case 145: return "Scroll";
                case 160: return "LShiftKey";
                case 161: return "RShiftKey";
                case 162: return "LControlKey";
                case 163: return "RControlKey";
                case 164: return "LMenu";
                case 165: return "RMenu";
                case 166: return "BrowserBack";
                case 167: return "BrowserForward";
                case 168: return "BrowserRefresh";
                case 169: return "BrowserStop";
                case 170: return "BrowserSearch";
                case 171: return "BrowserFavorites";
                case 172: return "BrowserHome";
                case 173: return "VolumeMute";
                case 174: return "VolumeDown";
                case 175: return "VolumeUp";
                case 176: return "MediaNextTrack";
                case 177: return "MediaPreviousTrack";
                case 178: return "MediaStop";
                case 179: return "MediaPlayPause";
                case 180: return "LaunchMail";
                case 181: return "SelectMedia";
                case 182: return "LaunchApplication1";
                case 183: return "LaunchApplication2";
                case 186: return "Oem1";
                case 187: return "Oemplus";
                case 188: return "Oemcomma";
                case 189: return "OemMinus";
                case 190: return "OemPeriod";
                case 191: return "Oem2";
                case 192: return "Oem3";
                case 219: return "Oem4";
                case 220: return "Oem5";
                case 221: return "Oem6";
                case 222: return "Oem7";
                case 223: return "Oem8";
                case 226: return "Oem102";
                case 229: return "ProcessKey";
                case 231: return "Packet";
                case 246: return "Attn";
                case 247: return "Crsel";
                case 248: return "Exsel";
                case 249: return "EraseEof";
                case 250: return "Play";
                case 251: return "Zoom";
                case 252: return "NoName";
                case 253: return "Pa1";
                case 254: return "OemClear";
            }
            return $"0x{keyId.ToString("x")}";
        }
    }

    public class LogicalSystemKey : LogicalDoubleThrowKey
    {
        public LogicalSystemKey(int keyId)
            : base(keyId) { }
        
        public static implicit operator int(LogicalSystemKey key)
            => key.KeyId;

        public override string ToString()
            => $"LogicalSystemKey({LogicalSystemKeySet.KeyIdToKeyName(KeyId)})";
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
        
        public static implicit operator int(PhysicalSystemKey key)
            => key.KeyId;

        public override string ToString()
            => $"PhysicalSystemKey({LogicalSystemKeySet.KeyIdToKeyName(KeyId)})";
    }

    public class SupportedKeys
    {
        private static SupportedKeys Instance = new SupportedKeys();

        private readonly LogicalKeyDeclaration logicalKeys;
        private readonly PhysicalKeyDeclaration physicalKeys;

        private SupportedKeys()
        {
            var logicalMouseWheelwKeys = new LogicalMouseWheelKeySet();
            var physicalMouseWheelKeys = new PhysicalMouseWheelKeySet(logicalMouseWheelwKeys);

            var logicalSystemKeys = new LogicalSystemKeySet();
            var physicalSystemKeys = new PhysicalSystemKeySet(logicalSystemKeys);

            logicalKeys = new LogicalKeyDeclaration(logicalMouseWheelwKeys, logicalSystemKeys);
            physicalKeys = new PhysicalKeyDeclaration(physicalMouseWheelKeys, physicalSystemKeys);
        }

        public static LogicalKeyDeclaration Keys => Instance.logicalKeys;

        public static PhysicalKeyDeclaration PhysicalKeys => Instance.physicalKeys;

        public class KeyDeclaration<TKeyA, TKeyB>
            where TKeyA : KeyType
            where TKeyB : KeyType
        {
            public TKeyB this[int keyCode] => SystemKeySet[keyCode];

            public readonly KeySet<TKeyA> SingleThrowKeySet;
            public readonly KeySet<TKeyB> SystemKeySet;

            public KeyDeclaration(
                KeySet<TKeyA> singleThrowKeys,
                KeySet<TKeyB> systemKeys)
            {
                SingleThrowKeySet = singleThrowKeys;
                SystemKeySet = systemKeys;
            }

            public TKeyA WheelUp => SingleThrowKeySet[1];
            public TKeyA WheelDown => SingleThrowKeySet[2];
            public TKeyA WheelLeft => SingleThrowKeySet[3];
            public TKeyA WheelRight => SingleThrowKeySet[4];

            public StrokeDirection MoveUp => StrokeDirection.Up;
            public StrokeDirection MoveDown => StrokeDirection.Down;
            public StrokeDirection MoveLeft => StrokeDirection.Left;
            public StrokeDirection MoveRight => StrokeDirection.Right;

            public int KeyCode = 0x0000FFFF;
            public int Modifiers = unchecked((int)0xFFFF0000);
            public int Shift = 0x00010000;
            public int Control = 0x00020000;
            public int Alt = 0x00040000;

            public TKeyB None => SystemKeySet[0];
            public TKeyB LButton => SystemKeySet[1];
            public TKeyB RButton => SystemKeySet[2];
            public TKeyB Cancel => SystemKeySet[3];
            public TKeyB MButton => SystemKeySet[4];
            public TKeyB XButton1 => SystemKeySet[5];
            public TKeyB XButton2 => SystemKeySet[6];
            public TKeyB Back => SystemKeySet[8];
            public TKeyB Tab => SystemKeySet[9];
            public TKeyB LineFeed => SystemKeySet[10];
            public TKeyB Clear => SystemKeySet[12];
            public TKeyB Enter => SystemKeySet[13];
            public TKeyB Return => SystemKeySet[13];
            public TKeyB ShiftKey => SystemKeySet[16];
            public TKeyB ControlKey => SystemKeySet[17];
            public TKeyB Menu => SystemKeySet[18];
            public TKeyB Pause => SystemKeySet[19];
            public TKeyB CapsLock => SystemKeySet[20];
            public TKeyB Capital => SystemKeySet[20];
            public TKeyB KanaMode => SystemKeySet[21];
            public TKeyB HangulMode => SystemKeySet[21];
            public TKeyB JunjaMode => SystemKeySet[23];
            public TKeyB FinalMode => SystemKeySet[24];
            public TKeyB KanjiMode => SystemKeySet[25];
            public TKeyB HanjaMode => SystemKeySet[25];
            public TKeyB Escape => SystemKeySet[27];
            public TKeyB IMEConvert => SystemKeySet[28];
            public TKeyB IMENonconvert => SystemKeySet[29];
            public TKeyB IMEAccept => SystemKeySet[30];
            public TKeyB IMEModeChange => SystemKeySet[31];
            public TKeyB Space => SystemKeySet[32];
            public TKeyB Prior => SystemKeySet[33];
            public TKeyB PageUp => SystemKeySet[33];
            public TKeyB Next => SystemKeySet[34];
            public TKeyB PageDown => SystemKeySet[34];
            public TKeyB End => SystemKeySet[35];
            public TKeyB Home => SystemKeySet[36];
            public TKeyB Left => SystemKeySet[37];
            public TKeyB Up => SystemKeySet[38];
            public TKeyB Right => SystemKeySet[39];
            public TKeyB Down => SystemKeySet[40];
            public TKeyB Select => SystemKeySet[41];
            public TKeyB Print => SystemKeySet[42];
            public TKeyB Execute => SystemKeySet[43];
            public TKeyB PrintScreen => SystemKeySet[44];
            public TKeyB Snapshot => SystemKeySet[44];
            public TKeyB Insert => SystemKeySet[45];
            public TKeyB Delete => SystemKeySet[46];
            public TKeyB Help => SystemKeySet[47];
            public TKeyB D0 => SystemKeySet[48];
            public TKeyB D1 => SystemKeySet[49];
            public TKeyB D2 => SystemKeySet[50];
            public TKeyB D3 => SystemKeySet[51];
            public TKeyB D4 => SystemKeySet[52];
            public TKeyB D5 => SystemKeySet[53];
            public TKeyB D6 => SystemKeySet[54];
            public TKeyB D7 => SystemKeySet[55];
            public TKeyB D8 => SystemKeySet[56];
            public TKeyB D9 => SystemKeySet[57];
            public TKeyB A => SystemKeySet[65];
            public TKeyB B => SystemKeySet[66];
            public TKeyB C => SystemKeySet[67];
            public TKeyB D => SystemKeySet[68];
            public TKeyB E => SystemKeySet[69];
            public TKeyB F => SystemKeySet[70];
            public TKeyB G => SystemKeySet[71];
            public TKeyB H => SystemKeySet[72];
            public TKeyB I => SystemKeySet[73];
            public TKeyB J => SystemKeySet[74];
            public TKeyB K => SystemKeySet[75];
            public TKeyB L => SystemKeySet[76];
            public TKeyB M => SystemKeySet[77];
            public TKeyB N => SystemKeySet[78];
            public TKeyB O => SystemKeySet[79];
            public TKeyB P => SystemKeySet[80];
            public TKeyB Q => SystemKeySet[81];
            public TKeyB R => SystemKeySet[82];
            public TKeyB S => SystemKeySet[83];
            public TKeyB T => SystemKeySet[84];
            public TKeyB U => SystemKeySet[85];
            public TKeyB V => SystemKeySet[86];
            public TKeyB W => SystemKeySet[87];
            public TKeyB X => SystemKeySet[88];
            public TKeyB Y => SystemKeySet[89];
            public TKeyB Z => SystemKeySet[90];
            public TKeyB LWin => SystemKeySet[91];
            public TKeyB RWin => SystemKeySet[92];
            public TKeyB Apps => SystemKeySet[93];
            public TKeyB Sleep => SystemKeySet[95];
            public TKeyB NumPad0 => SystemKeySet[96];
            public TKeyB NumPad1 => SystemKeySet[97];
            public TKeyB NumPad2 => SystemKeySet[98];
            public TKeyB NumPad3 => SystemKeySet[99];
            public TKeyB NumPad4 => SystemKeySet[100];
            public TKeyB NumPad5 => SystemKeySet[101];
            public TKeyB NumPad6 => SystemKeySet[102];
            public TKeyB NumPad7 => SystemKeySet[103];
            public TKeyB NumPad8 => SystemKeySet[104];
            public TKeyB NumPad9 => SystemKeySet[105];
            public TKeyB Multiply => SystemKeySet[106];
            public TKeyB Add => SystemKeySet[107];
            public TKeyB Separator => SystemKeySet[108];
            public TKeyB Subtract => SystemKeySet[109];
            public TKeyB Decimal => SystemKeySet[110];
            public TKeyB Divide => SystemKeySet[111];
            public TKeyB F1 => SystemKeySet[112];
            public TKeyB F2 => SystemKeySet[113];
            public TKeyB F3 => SystemKeySet[114];
            public TKeyB F4 => SystemKeySet[115];
            public TKeyB F5 => SystemKeySet[116];
            public TKeyB F6 => SystemKeySet[117];
            public TKeyB F7 => SystemKeySet[118];
            public TKeyB F8 => SystemKeySet[119];
            public TKeyB F9 => SystemKeySet[120];
            public TKeyB F10 => SystemKeySet[121];
            public TKeyB F11 => SystemKeySet[122];
            public TKeyB F12 => SystemKeySet[123];
            public TKeyB F13 => SystemKeySet[124];
            public TKeyB F14 => SystemKeySet[125];
            public TKeyB F15 => SystemKeySet[126];
            public TKeyB F16 => SystemKeySet[127];
            public TKeyB F17 => SystemKeySet[128];
            public TKeyB F18 => SystemKeySet[129];
            public TKeyB F19 => SystemKeySet[130];
            public TKeyB F20 => SystemKeySet[131];
            public TKeyB F21 => SystemKeySet[132];
            public TKeyB F22 => SystemKeySet[133];
            public TKeyB F23 => SystemKeySet[134];
            public TKeyB F24 => SystemKeySet[135];
            public TKeyB NumLock => SystemKeySet[144];
            public TKeyB Scroll => SystemKeySet[145];
            public TKeyB LShiftKey => SystemKeySet[160];
            public TKeyB RShiftKey => SystemKeySet[161];
            public TKeyB LControlKey => SystemKeySet[162];
            public TKeyB RControlKey => SystemKeySet[163];
            public TKeyB LMenu => SystemKeySet[164];
            public TKeyB RMenu => SystemKeySet[165];
            public TKeyB BrowserBack => SystemKeySet[166];
            public TKeyB BrowserForward => SystemKeySet[167];
            public TKeyB BrowserRefresh => SystemKeySet[168];
            public TKeyB BrowserStop => SystemKeySet[169];
            public TKeyB BrowserSearch => SystemKeySet[170];
            public TKeyB BrowserFavorites => SystemKeySet[171];
            public TKeyB BrowserHome => SystemKeySet[172];
            public TKeyB VolumeMute => SystemKeySet[173];
            public TKeyB VolumeDown => SystemKeySet[174];
            public TKeyB VolumeUp => SystemKeySet[175];
            public TKeyB MediaNextTrack => SystemKeySet[176];
            public TKeyB MediaPreviousTrack => SystemKeySet[177];
            public TKeyB MediaStop => SystemKeySet[178];
            public TKeyB MediaPlayPause => SystemKeySet[179];
            public TKeyB LaunchMail => SystemKeySet[180];
            public TKeyB SelectMedia => SystemKeySet[181];
            public TKeyB LaunchApplication1 => SystemKeySet[182];
            public TKeyB LaunchApplication2 => SystemKeySet[183];
            public TKeyB Oem1 => SystemKeySet[186];
            public TKeyB OemSemicolon => SystemKeySet[186];
            public TKeyB Oemplus => SystemKeySet[187];
            public TKeyB Oemcomma => SystemKeySet[188];
            public TKeyB OemMinus => SystemKeySet[189];
            public TKeyB OemPeriod => SystemKeySet[190];
            public TKeyB OemQuestion => SystemKeySet[191];
            public TKeyB Oem2 => SystemKeySet[191];
            public TKeyB Oemtilde => SystemKeySet[192];
            public TKeyB Oem3 => SystemKeySet[192];
            public TKeyB Oem4 => SystemKeySet[219];
            public TKeyB OemOpenBrackets => SystemKeySet[219];
            public TKeyB OemPipe => SystemKeySet[220];
            public TKeyB Oem5 => SystemKeySet[220];
            public TKeyB Oem6 => SystemKeySet[221];
            public TKeyB OemCloseBrackets => SystemKeySet[221];
            public TKeyB Oem7 => SystemKeySet[222];
            public TKeyB OemQuotes => SystemKeySet[222];
            public TKeyB Oem8 => SystemKeySet[223];
            public TKeyB Oem102 => SystemKeySet[226];
            public TKeyB OemBackslash => SystemKeySet[226];
            public TKeyB ProcessKey => SystemKeySet[229];
            public TKeyB Packet => SystemKeySet[231];
            public TKeyB Attn => SystemKeySet[246];
            public TKeyB Crsel => SystemKeySet[247];
            public TKeyB Exsel => SystemKeySet[248];
            public TKeyB EraseEof => SystemKeySet[249];
            public TKeyB Play => SystemKeySet[250];
            public TKeyB Zoom => SystemKeySet[251];
            public TKeyB NoName => SystemKeySet[252];
            public TKeyB Pa1 => SystemKeySet[253];
            public TKeyB OemClear => SystemKeySet[254];
        }

        public class LogicalKeyDeclaration : KeyDeclaration<LogicalMouseWheelKey, LogicalSystemKey>
        {
            public LogicalKeyDeclaration(
                LogicalMouseWheelKeySet singleThrowKeys,
                LogicalSystemKeySet logicalSystemKeys)
                : base(singleThrowKeys, logicalSystemKeys)
            {

            }
        }

        public class PhysicalKeyDeclaration : KeyDeclaration<PhysicalMouseWheelKey, PhysicalSystemKey>
        {
            public PhysicalKeyDeclaration(
                PhysicalMouseWheelKeySet singleThrowKeys,
                PhysicalSystemKeySet systemKeys)
                : base(singleThrowKeys, systemKeys)
            {

            }
        }
    }
}
