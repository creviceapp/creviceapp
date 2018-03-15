
# Appendix 1. Complete list of some API

## Complete list of Keys

| Property Name | Value | Description
|-----|-----|-----
WheelUp | -
WheelDown | -
WheelLeft | -
WheelRight | -
MoveUp | -
MoveDown | -
MoveLeft | -
MoveRight | -
KeyCode | 0x0000FFFF
Modifiers | 0xFFFF0000
Shift | 0x00010000
Control | 0x00020000
Alt | 0x00040000
None | 0x00
LButton | 0x01
RButton | 0x02
Cancel | 0x03
MButton | 0x04
XButton1 | 0x05
XButton2 | 0x06
Back | 0x08
Tab | 0x09
LineFeed | 0x0A
Clear | 0x0C
Enter | 0x0D
Return | 0x0D
ShiftKey | 0x10
ControlKey | 0x11
Menu | 0x12
Pause | 0x13
CapsLock | 0x14
Capital | 0x14
KanaMode | 0x15
HangulMode | 0x15
JunjaMode | 0x17
FinalMode | 0x18
KanjiMode | 0x19
HanjaMode | 0x19
Escape | 0x1B
IMEConvert | 0x1C
IMENonconvert | 0x1D
IMEAccept | 0x1E
IMEModeChange | 0x1F
Space | 0x20
Prior | 0x21
PageUp | 0x21
Next | 0x22
PageDown | 0x22
End | 0x23
Home | 0x24
Left | 0x25
Up | 0x26
Right | 0x27
Down | 0x28
Select | 0x29
Print | 0x2A
Execute | 0x2B
PrintScreen | 0x2C
Snapshot | 0x2C
Insert | 0x2D
Delete | 0x2E
Help | 0x2F
D0 | 0x30
D1 | 0x31
D2 | 0x32
D3 | 0x33
D4 | 0x34
D5 | 0x35
D6 | 0x36
D7 | 0x37
D8 | 0x38
D9 | 0x39
A | 0x41
B | 0x42
C | 0x43
D | 0x44
E | 0x45
F | 0x46
G | 0x47
H | 0x48
I | 0x49
J | 0x4A
K | 0x4B
L | 0x4C
M | 0x4D
N | 0x4E
O | 0x4F
P | 0x50
Q | 0x51
R | 0x52
S | 0x53
T | 0x54
U | 0x55
V | 0x56
W | 0x57
X | 0x58
Y | 0x59
Z | 0x5A
LWin | 0x5B
RWin | 0x5C
Apps | 0x5D
Sleep | 0x5F
NumPad0 | 0x60
NumPad1 | 0x61
NumPad2 | 0x62
NumPad3 | 0x63
NumPad4 | 0x64
NumPad5 | 0x65
NumPad6 | 0x66
NumPad7 | 0x67
NumPad8 | 0x68
NumPad9 | 0x69
Multiply | 0x6A
Add | 0x6B
Separator | 0x6C
Subtract | 0x6D
Decimal | 0x6E
Divide | 0x6F
F1 | 0x70
F2 | 0x71
F3 | 0x72
F4 | 0x73
F5 | 0x74
F6 | 0x75
F7 | 0x76
F8 | 0x77
F9 | 0x78
F10 | 0x79
F11 | 0x7A
F12 | 0x7B
F13 | 0x7C
F14 | 0x7D
F15 | 0x7E
F16 | 0x7F
F17 | 0x80
F18 | 0x81
F19 | 0x82
F20 | 0x83
F21 | 0x84
F22 | 0x85
F23 | 0x86
F24 | 0x87
NumLock | 0x90
Scroll | 0x91
LShiftKey | 0xA0
RShiftKey | 0xA1
LControlKey | 0xA2
RControlKey | 0xA3
LMenu | 0xA4
RMenu | 0xA5
BrowserBack | 0xA6
BrowserForward | 0xA7
BrowserRefresh | 0xA8
BrowserStop | 0xA9
BrowserSearch | 0xAA
BrowserFavorites | 0xAB
BrowserHome | 0xAC
VolumeMute | 0xAD
VolumeDown | 0xAE
VolumeUp | 0xAF
MediaNextTrack | 0xB0
MediaPreviousTrack | 0xB1
MediaStop | 0xB2
MediaPlayPause | 0xB3
LaunchMail | 0xB4
SelectMedia | 0xB5
LaunchApplication1 | 0xB6
LaunchApplication2 | 0xB7
Oem1 | 0xBA
OemSemicolon | 0xBA
Oemplus | 0xBB
Oemcomma | 0xBC
OemMinus | 0xBD
OemPeriod | 0xBE
OemQuestion | 0xBF
Oem2 | 0xBF
Oemtilde | 0xC0
Oem3 | 0xC0
Oem4 | 0xDB
OemOpenBrackets | 0xDB
OemPipe | 0xDC
Oem5 | 0xDC
Oem6 | 0xDD
OemCloseBrackets | 0xDD
Oem7 | 0xDE
OemQuotes | 0xDE
Oem8 | 0xDF
Oem102 | 0xE2
OemBackslash | 0xE2
ProcessKey | 0xE5
Packet | 0xE7
Attn | 0xF6
Crsel | 0xF7
Exsel | 0xF8
EraseEof | 0xF9
Play | 0xFA
Zoom | 0xFB
NoName | 0xFC
Pa1 | 0xFD
OemClear | 0xFE

## Complete list of Crevice.WinAPI.Constants.VirtualKeys

<!--
src: CreviceApp\WinAPI.Constants.VirtualKeys.cs
\s+public const (.+?)(VK.+?)\s+=(.+); // (.+)
\1| \2 |\3 | \4

\s+?//\s+?-\s+?(0x.+?)\s+?// (.+)
- | - | \1 | \2
-->


| Property Name | Value | Description
|-----|-----|-----
 VK_LBUTTON | 0x01 | Left mouse button
 VK_RBUTTON | 0x02 | Right mouse button
 VK_CANCEL | 0x03 | Control-break processing
 VK_MBUTTON | 0x04 | Middle mouse button (three-button mouse)
 VK_XBUTTON1 | 0x05 | X1 mouse button
 VK_XBUTTON2 | 0x06 | X2 mouse button
 - | 0x07 | Undefined
 VK_BACK | 0x08 | BACKSPACE key
 VK_TAB | 0x09 | TAB key
 - | 0x0A-0B | Reserved
 VK_CLEAR | 0x0C | CLEAR key
 VK_RETURN | 0x0D | ENTER key
 - | 0x0E-0F | Undefined
 VK_SHIFT | 0x10 | SHIFT key
 VK_CONTROL | 0x11 | CTRL key
 VK_MENU | 0x12 | ALT key
 VK_PAUSE | 0x13 | PAUSE key
 VK_CAPITAL | 0x14 | CAPS LOCK key
 VK_KANA | 0x15 | IME Kana mode
 VK_HANGUEL | 0x15 | IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
 VK_HANGUL | 0x15 | IME Hangul mode
 - | 0x16 | Undefined
 VK_JUNJA | 0x17 | IME Junja mode
 VK_FINAL | 0x18 | IME final mode
 VK_HANJA | 0x19 | IME Hanja mode
 VK_KANJI | 0x19 | IME Kanji mode
 - | 0x1A | Undefined
 VK_ESCAPE | 0x1B | ESC key
 VK_CONVERT | 0x1C | IME convert
 VK_NONCONVERT | 0x1D | IME nonconvert
 VK_ACCEPT | 0x1E | IME accept
 VK_MODECHANGE | 0x1F | IME mode change request
 VK_SPACE | 0x20 | SPACEBAR
 VK_PRIOR | 0x21 | PAGE UP key
 VK_NEXT | 0x22 | PAGE DOWN key
 VK_END | 0x23 | END key
 VK_HOME | 0x24 | HOME key
 VK_LEFT | 0x25 | LEFT ARROW key
 VK_UP | 0x26 | UP ARROW key
 VK_RIGHT | 0x27 | RIGHT ARROW key
 VK_DOWN | 0x28 | DOWN ARROW key
 VK_SELECT | 0x29 | SELECT key
 VK_PRINT | 0x2A | PRINT key
 VK_EXECUTE | 0x2B | EXECUTE key
 VK_SNAPSHOT | 0x2C | PRINT SCREEN key
 VK_INSERT | 0x2D | INS key
 VK_DELETE | 0x2E | DEL key
 VK_HELP | 0x2F | HELP key
 VK_0 | 0x30 | 0 key
 VK_1 | 0x31 | 1 key
 VK_2 | 0x32 | 2 key
 VK_3 | 0x33 | 3 key
 VK_4 | 0x34 | 4 key
 VK_5 | 0x35 | 5 key
 VK_6 | 0x36 | 6 key
 VK_7 | 0x37 | 7 key
 VK_8 | 0x38 | 8 key
 VK_9 | 0x39 | 9 key
 - | 0x3A-40 | Undefined
 VK_A | 0x41 | A key
 VK_B | 0x42 | B key
 VK_C | 0x43 | C key
 VK_D | 0x44 | D key
 VK_E | 0x45 | E key
 VK_F | 0x46 | F key
 VK_G | 0x47 | G key
 VK_H | 0x48 | H key
 VK_I | 0x49 | I key
 VK_J | 0x4A | J key
 VK_K | 0x4B | K key
 VK_L | 0x4C | L key
 VK_M | 0x4D | M key
 VK_N | 0x4E | N key
 VK_O | 0x4F | O key
 VK_P | 0x50 | P key
 VK_Q | 0x51 | Q key
 VK_R | 0x52 | R key
 VK_S | 0x53 | S key
 VK_T | 0x54 | T key
 VK_U | 0x55 | U key
 VK_V | 0x56 | V key
 VK_W | 0x57 | W key
 VK_X | 0x58 | X key
 VK_Y | 0x59 | Y key
 VK_Z | 0x5A | Z key
 VK_LWIN | 0x5B | Left Windows key (Natural keyboard)
 VK_RWIN | 0x5C | Right Windows key (Natural keyboard)
 VK_APPS | 0x5D | Applications key (Natural keyboard)
 - | 0x5E | Reserved
 VK_SLEEP | 0x5F | Computer Sleep key
 VK_NUMPAD0 | 0x60 | Numeric keypad 0 key
 VK_NUMPAD1 | 0x61 | Numeric keypad 1 key
 VK_NUMPAD2 | 0x62 | Numeric keypad 2 key
 VK_NUMPAD3 | 0x63 | Numeric keypad 3 key
 VK_NUMPAD4 | 0x64 | Numeric keypad 4 key
 VK_NUMPAD5 | 0x65 | Numeric keypad 5 key
 VK_NUMPAD6 | 0x66 | Numeric keypad 6 key
 VK_NUMPAD7 | 0x67 | Numeric keypad 7 key
 VK_NUMPAD8 | 0x68 | Numeric keypad 8 key
 VK_NUMPAD9 | 0x69 | Numeric keypad 9 key
 VK_MULTIPLY | 0x6A | Multiply key
 VK_ADD | 0x6B | Add key
 VK_SEPARATOR | 0x6C | Separator key
 VK_SUBTRACT | 0x6D | Subtract key
 VK_DECIMAL | 0x6E | Decimal key
 VK_DIVIDE | 0x6F | Divide key
 VK_F1 | 0x70 | F1 key
 VK_F2 | 0x71 | F2 key
 VK_F3 | 0x72 | F3 key
 VK_F4 | 0x73 | F4 key
 VK_F5 | 0x74 | F5 key
 VK_F6 | 0x75 | F6 key
 VK_F7 | 0x76 | F7 key
 VK_F8 | 0x77 | F8 key
 VK_F9 | 0x78 | F9 key
 VK_F10 | 0x79 | F10 key
 VK_F11 | 0x7A | F11 key
 VK_F12 | 0x7B | F12 key
 VK_F13 | 0x7C | F13 key
 VK_F14 | 0x7D | F14 key
 VK_F15 | 0x7E | F15 key
 VK_F16 | 0x7F | F16 key
 VK_F17 | 0x80 | F17 key
 VK_F18 | 0x81 | F18 key
 VK_F19 | 0x82 | F19 key
 VK_F20 | 0x83 | F20 key
 VK_F21 | 0x84 | F21 key
 VK_F22 | 0x85 | F22 key
 VK_F23 | 0x86 | F23 key
 VK_F24 | 0x87 | F24 key
 - | 0x88-8F | Unassigned
 VK_NUMLOCK | 0x90 | NUM LOCK key
 VK_SCROLL | 0x91 | SCROLL LOCK key
 - |0x92-96 | OEM specific
 - | 0x97-9F | Unassigned
 VK_LSHIFT | 0xA0 | Left SHIFT key
 VK_RSHIFT | 0xA1 | Right SHIFT key
 VK_LCONTROL | 0xA2 | Left CONTROL key
 VK_RCONTROL | 0xA3 | Right CONTROL key
 VK_LMENU | 0xA4 | Left MENU key
 VK_RMENU | 0xA5 | Right MENU key
 VK_BROWSER_BACK | 0xA6 | Browser Back key
 VK_BROWSER_FORWARD | 0xA7 | Browser Forward key
 VK_BROWSER_REFRESH | 0xA8 | Browser Refresh key
 VK_BROWSER_STOP | 0xA9 | Browser Stop key
 VK_BROWSER_SEARCH | 0xAA | Browser Search key
 VK_BROWSER_FAVORITES | 0xAB | Browser Favorites key
 VK_BROWSER_HOME | 0xAC | Browser Start and Home key
 VK_VOLUME_MUTE | 0xAD | Volume Mute key
 VK_VOLUME_DOWN | 0xAE | Volume Down key
 VK_VOLUME_UP | 0xAF | Volume Up key
 VK_MEDIA_NEXT_TRACK | 0xB0 | Next Track key
 VK_MEDIA_PREV_TRACK | 0xB1 | Previous Track key
 VK_MEDIA_STOP | 0xB2 | Stop Media key
 VK_MEDIA_PLAY_PAUSE | 0xB3 | Play/Pause Media key
 VK_LAUNCH_MAIL | 0xB4 | Start Mail key
 VK_LAUNCH_MEDIA_SELECT | 0xB5 | Select Media key
 VK_LAUNCH_APP1 | 0xB6 | Start Application 1 key
 VK_LAUNCH_APP2 | 0xB7 | Start Application 2 key
 - | 0xB8-B9 | Reserved
 VK_OEM_1 | 0xBA | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key
 VK_OEM_PLUS | 0xBB |  For any country/region, the '+' key
 VK_OEM_COMMA | 0xBC | For any country/region, the ',' key
 VK_OEM_MINUS | 0xBD | For any country/region, the '-' key
 VK_OEM_PERIOD | 0xBE | For any country/region, the '.' key
 VK_OEM_2 | 0xBF | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key
 VK_OEM_3 | 0xC0 | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key
 - | 0xC1-D7 | Reserved
 - | 0xD8-DA | Unassigned
 VK_OEM_4 | 0xDB | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key
 VK_OEM_5 | 0xDC | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\|' key
 VK_OEM_6 | 0xDD | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key
 VK_OEM_7 | 0xDE | Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key
 VK_OEM_8 | 0xDF | Used for miscellaneous characters; it can vary by keyboard.
 - | 0xE0 | Reserved
 - | 0xE1 | OEM specific
 VK_OEM_102 | 0xE2 | Either the angle bracket key or the backslash key on the RT 102-key keyboard
 - | 0xE3-E4 | OEM specific
 VK_PROCESSKEY | 0xE5 | IME PROCESS key
 - | 0xE6 | OEM specific
 VK_PACKET | 0xE7 | Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
 - | 0xE8 | Unassigned
 - | 0xE9-F5 | OEM specific
 VK_ATTN | 0xF6 | Attn key
 VK_CRSEL | 0xF7 | CrSel key
 VK_EXSEL | 0xF8 | ExSel key
 VK_EREOF | 0xF9 | Erase EOF key
 VK_PLAY | 0xFA | Play key
 VK_ZOOM | 0xFB | Zoom key
 VK_NONAME | 0xFC | Reserved
 VK_PA1 | 0xFD | PA1 key
 VK_OEM_CLEAR | 0xFE | Clear Key

## Crevice.WinAPI.Constants.WindowsMessages

<!-- 
src: CreviceApp\WinAPI.Constants.WindowsMessages.cs
\s+public const (.+?)(WM.+?)=(.+);
\1| \2|\3
-->

| Property Name | Value 
|-----|-----
 WM_NULL | 0x0000
 WM_CREATE | 0x0001
 WM_DESTROY | 0x0002
 WM_MOVE | 0x0003
 WM_SIZE | 0x0005
 WM_ACTIVATE | 0x0006
 WM_SETFOCUS | 0x0007
 WM_KILLFOCUS | 0x0008
 WM_ENABLE | 0x000A
 WM_SETREDRAW | 0x000B
 WM_SETTEXT | 0x000C
 WM_GETTEXT | 0x000D
 WM_GETTEXTLENGTH | 0x000E
 WM_PAINT | 0x000F
 WM_CLOSE | 0x0010
 WM_QUERYENDSESSION | 0x0011
 WM_QUERYOPEN | 0x0013
 WM_ENDSESSION | 0x0016
 WM_QUIT | 0x0012
 WM_ERASEBKGND | 0x0014
 WM_SYSCOLORCHANGE | 0x0015
 WM_SHOWWINDOW | 0x0018
 WM_WININICHANGE | 0x001A
 WM_SETTINGCHANGE | WM_WININICHANGE
 WM_DEVMODECHANGE | 0x001B
 WM_ACTIVATEAPP | 0x001C
 WM_FONTCHANGE | 0x001D
 WM_TIMECHANGE | 0x001E
 WM_CANCELMODE | 0x001F
 WM_SETCURSOR | 0x0020
 WM_MOUSEACTIVATE | 0x0021
 WM_CHILDACTIVATE | 0x0022
 WM_QUEUESYNC | 0x0023
 WM_GETMINMAXINFO | 0x0024
 WM_PAINTICON | 0x0026
 WM_ICONERASEBKGND | 0x0027
 WM_NEXTDLGCTL | 0x0028
 WM_SPOOLERSTATUS | 0x002A
 WM_DRAWITEM | 0x002B
 WM_MEASUREITEM | 0x002C
 WM_DELETEITEM | 0x002D
 WM_VKEYTOITEM | 0x002E
 WM_CHARTOITEM | 0x002F
 WM_SETFONT | 0x0030
 WM_GETFONT | 0x0031
 WM_SETHOTKEY | 0x0032
 WM_GETHOTKEY | 0x0033
 WM_QUERYDRAGICON | 0x0037
 WM_COMPAREITEM | 0x0039
 WM_GETOBJECT | 0x003D
 WM_COMPACTING | 0x0041
 WM_COMMNOTIFY | 0x0044
 WM_WINDOWPOSCHANGING | 0x0046
 WM_WINDOWPOSCHANGED | 0x0047
 WM_POWER | 0x0048
 WM_COPYDATA | 0x004A
 WM_CANCELJOURNAL | 0x004B
 WM_NOTIFY | 0x004E
 WM_INPUTLANGCHANGEREQUEST | 0x0050
 WM_INPUTLANGCHANGE | 0x0051
 WM_TCARD | 0x0052
 WM_HELP | 0x0053
 WM_USERCHANGED | 0x0054
 WM_NOTIFYFORMAT | 0x0055
 WM_CONTEXTMENU | 0x007B
 WM_STYLECHANGING | 0x007C
 WM_STYLECHANGED | 0x007D
 WM_DISPLAYCHANGE | 0x007E
 WM_GETICON | 0x007F
 WM_SETICON | 0x0080
 WM_NCCREATE | 0x0081
 WM_NCDESTROY | 0x0082
 WM_NCCALCSIZE | 0x0083
 WM_NCHITTEST | 0x0084
 WM_NCPAINT | 0x0085
 WM_NCACTIVATE | 0x0086
 WM_GETDLGCODE | 0x0087
 WM_SYNCPAINT | 0x0088
 WM_NCMOUSEMOVE | 0x00A0
 WM_NCLBUTTONDOWN | 0x00A1
 WM_NCLBUTTONUP | 0x00A2
 WM_NCLBUTTONDBLCLK | 0x00A3
 WM_NCRBUTTONDOWN | 0x00A4
 WM_NCRBUTTONUP | 0x00A5
 WM_NCRBUTTONDBLCLK | 0x00A6
 WM_NCMBUTTONDOWN | 0x00A7
 WM_NCMBUTTONUP | 0x00A8
 WM_NCMBUTTONDBLCLK | 0x00A9
 WM_NCXBUTTONDOWN | 0x00AB
 WM_NCXBUTTONUP | 0x00AC
 WM_NCXBUTTONDBLCLK | 0x00AD
 WM_INPUT_DEVICE_CHANGE | 0x00FE
 WM_INPUT | 0x00FF
 WM_KEYFIRST | 0x0100
 WM_KEYDOWN | 0x0100
 WM_KEYUP | 0x0101
 WM_CHAR | 0x0102
 WM_DEADCHAR | 0x0103
 WM_SYSKEYDOWN | 0x0104
 WM_SYSKEYUP | 0x0105
 WM_SYSCHAR | 0x0106
 WM_SYSDEADCHAR | 0x0107
 WM_UNICHAR | 0x0109
 WM_KEYLAST | 0x0109
 WM_IME_STARTCOMPOSITION | 0x010D
 WM_IME_ENDCOMPOSITION | 0x010E
 WM_IME_COMPOSITION | 0x010F
 WM_IME_KEYLAST | 0x010F
 WM_INITDIALOG | 0x0110
 WM_COMMAND | 0x0111
 WM_SYSCOMMAND | 0x0112
 WM_TIMER | 0x0113
 WM_HSCROLL | 0x0114
 WM_VSCROLL | 0x0115
 WM_INITMENU | 0x0116
 WM_INITMENUPOPUP | 0x0117
 WM_MENUSELECT | 0x011F
 WM_MENUCHAR | 0x0120
 WM_ENTERIDLE | 0x0121
 WM_MENURBUTTONUP | 0x0122
 WM_MENUDRAG | 0x0123
 WM_MENUGETOBJECT | 0x0124
 WM_UNINITMENUPOPUP | 0x0125
 WM_MENUCOMMAND | 0x0126
 WM_CHANGEUISTATE | 0x0127
 WM_UPDATEUISTATE | 0x0128
 WM_QUERYUISTATE | 0x0129
 WM_CTLCOLORMSGBOX | 0x0132
 WM_CTLCOLOREDIT | 0x0133
 WM_CTLCOLORLISTBOX | 0x0134
 WM_CTLCOLORBTN | 0x0135
 WM_CTLCOLORDLG | 0x0136
 WM_CTLCOLORSCROLLBAR | 0x0137
 WM_CTLCOLORSTATIC | 0x0138
 WM_MOUSEFIRST | 0x0200
 WM_MOUSEMOVE | 0x0200
 WM_LBUTTONDOWN | 0x0201
 WM_LBUTTONUP | 0x0202
 WM_LBUTTONDBLCLK | 0x0203
 WM_RBUTTONDOWN | 0x0204
 WM_RBUTTONUP | 0x0205
 WM_RBUTTONDBLCLK | 0x0206
 WM_MBUTTONDOWN | 0x0207
 WM_MBUTTONUP | 0x0208
 WM_MBUTTONDBLCLK | 0x0209
 WM_MOUSEWHEEL | 0x020A
 WM_XBUTTONDOWN | 0x020B
 WM_XBUTTONUP | 0x020C
 WM_XBUTTONDBLCLK | 0x020D
 WM_MOUSEHWHEEL | 0x020E
 WM_MOUSELAST | 0x020E
 WM_PARENTNOTIFY | 0x0210
 WM_ENTERMENULOOP | 0x0211
 WM_EXITMENULOOP | 0x0212
 WM_NEXTMENU | 0x0213
 WM_SIZING | 0x0214
 WM_CAPTURECHANGED | 0x0215
 WM_MOVING | 0x0216
 WM_POWERBROADCAST | 0x0218
 WM_DEVICECHANGE | 0x0219
 WM_MDICREATE | 0x0220
 WM_MDIDESTROY | 0x0221
 WM_MDIACTIVATE | 0x0222
 WM_MDIRESTORE | 0x0223
 WM_MDINEXT | 0x0224
 WM_MDIMAXIMIZE | 0x0225
 WM_MDITILE | 0x0226
 WM_MDICASCADE | 0x0227
 WM_MDIICONARRANGE | 0x0228
 WM_MDIGETACTIVE | 0x0229
 WM_MDISETMENU | 0x0230
 WM_ENTERSIZEMOVE | 0x0231
 WM_EXITSIZEMOVE | 0x0232
 WM_DROPFILES | 0x0233
 WM_MDIREFRESHMENU | 0x0234
 WM_IME_SETCONTEXT | 0x0281
 WM_IME_NOTIFY | 0x0282
 WM_IME_CONTROL | 0x0283
 WM_IME_COMPOSITIONFULL | 0x0284
 WM_IME_SELECT | 0x0285
 WM_IME_CHAR | 0x0286
 WM_IME_REQUEST | 0x0288
 WM_IME_KEYDOWN | 0x0290
 WM_IME_KEYUP | 0x0291
 WM_MOUSEHOVER | 0x02A1
 WM_MOUSELEAVE | 0x02A3
 WM_NCMOUSEHOVER | 0x02A0
 WM_NCMOUSELEAVE | 0x02A2
 WM_WTSSESSION_CHANGE | 0x02B1
 WM_TABLET_FIRST | 0x02c0
 WM_TABLET_LAST | 0x02df
 WM_CUT | 0x0300
 WM_COPY | 0x0301
 WM_PASTE | 0x0302
 WM_CLEAR | 0x0303
 WM_UNDO | 0x0304
 WM_RENDERFORMAT | 0x0305
 WM_RENDERALLFORMATS | 0x0306
 WM_DESTROYCLIPBOARD | 0x0307
 WM_DRAWCLIPBOARD | 0x0308
 WM_PAINTCLIPBOARD | 0x0309
 WM_VSCROLLCLIPBOARD | 0x030A
 WM_SIZECLIPBOARD | 0x030B
 WM_ASKCBFORMATNAME | 0x030C
 WM_CHANGECBCHAIN | 0x030D
 WM_HSCROLLCLIPBOARD | 0x030E
 WM_QUERYNEWPALETTE | 0x030F
 WM_PALETTEISCHANGING | 0x0310
 WM_PALETTECHANGED | 0x0311
 WM_HOTKEY | 0x0312
 WM_PRINT | 0x0317
 WM_PRINTCLIENT | 0x0318
 WM_APPCOMMAND | 0x0319
 WM_THEMECHANGED | 0x031A
 WM_CLIPBOARDUPDATE | 0x031D
 WM_DWMCOMPOSITIONCHANGED | 0x031E
 WM_DWMNCRENDERINGCHANGED | 0x031F
 WM_DWMCOLORIZATIONCOLORCHANGED | 0x0320
 WM_DWMWINDOWMAXIMIZEDCHANGE | 0x0321
 WM_GETTITLEBARINFOEX | 0x033F
 WM_HANDHELDFIRST | 0x0358
 WM_HANDHELDLAST | 0x035F
 WM_AFXFIRST | 0x0360
 WM_AFXLAST | 0x037F
 WM_PENWINFIRST | 0x0380
 WM_PENWINLAST | 0x038F
 WM_APP | 0x8000
 WM_USER | 0x0400
 WM_CPL_LAUNCH | WM_USER + 0x1000
 WM_CPL_LAUNCHED | WM_USER + 0x1001
 WM_SYSTIMER | 0x118
