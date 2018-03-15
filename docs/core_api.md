
# Core API

## EvaluationContext

`Crevice.GestureMachine.EvaluationContext` have following properties:

### Properties {ignore=true}

Type | Property Name | Description |
-----|-----|-----|
System.Drawing.Point | GestureStartPosition
ForegroundWindowInfo | ForegroundWindow | The window which was on the foreground when a gesture started. This is an instance of `WindowInfo`.
PointedWindowInfo | PointedWindow | The window which was under the cursor when a gesture started. This is an instance of `WindowInfo`.

These values are guaranteed that same values are provided as `ExecutionContext`'s property in `Press`, `Do`, and `Release` clauses.

## ExecutionContext

`Crevice.GestureMachine.ExecutionContext` have following properties:

### Properties {ignore=true}

Type | Property Name | Description |
-----|-----|-----|
System.Drawing.Point | GestureStartPosition
System.Drawing.Point | GestureEndPosition
WindowInfo | ForegroundWindow | The window which was on the foreground when a gesture started. This is an instance of `WindowInfo`.
WindowInfo | PointedWindow | The window which was under the cursor when a gesture started. This is an instance of `WindowInfo`.

These values, except for `GestureEndPosition`, are guaranteed that same values are provided as `EvaluationContext`'s property in `When` clause. 

## GestureMachineProfile

`Crevice.GestureMachine.GestureMachineProfile` have following properties:

### Properties {ignore=true}
Type | Property Name | Description
-----|-----|------
RootElement | RootElement
GestureMachine | GestureMachine
UserConfig | UserConfig
string | ProfileName

## WindowInfo

`WindowInfo` is a thin wrapper of the handle of a window. This class provides properties and methods to use window handles more easily.

### Properties {ignore=true}

Type | Property Name | Description |
-----|-----|-----|
IntPtr | WindowHandle
int | ThreadId
int | ProcessId
IntPtr | WindowId
string | Text
string | ClassName
WindowInfo | Parent
string | ModulePath
string | ModuleName

### Methods {ignore=true}

Return Value | Method Definition | Description |
-----|-----|-----|
long | SendMessage(int Msg, int wParam, int lParam) | A shortcut to win32 API `SendMessage(WindowHandle, Msg, wParam, lParam)`.
bool | PostMessage(int Msg, int wParam, int lParam) | A shortcut to win32 API `PostMessage(WindowHandle, Msg, wParam, lParam)`.
bool | BringWindowToTop() | A shortcut to win32 API `BringWindowToTop(WindowHandle)`.
WindowInfo | FindWindowEx(IntPtr hwndChildAfter, string lpszClass, string lpszWindow) | A shortcut to win32 API `FindWindowEx(WindowHandle, hwndChildAfter, lpszClass, lpszWindow)`. 
WindowInfo | FindWindowEx(string lpszClass, string lpszWindow) | A shortcut to win32 API `FindWindowEx(WindowHandle, IntPtr.Zero, lpszClass, lpszWindow)`.
IReadOnlyList\<WindowInfo\> | GetChildWindows() | A shortcut to win32 API `EnumChildWindows(WindowHandle, EnumWindowProc, IntPtr.Zero)`.
IReadOnlyList\<WindowInfo\> |  GetPointedDescendantWindows(Point point, Window.WindowFromPointFlags flags) | A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, flags)`. This function recursively calls `ChildWindowFromPointEx` until reach to the last descendant window.
IReadOnlyList\<WindowInfo\> | GetPointedDescendantWindows(Point point) | A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, Window.WindowFromPointFlags.CWP_ALL)`. This function recursively calls `ChildWindowFromPointEx` until reach to the last descendant window.
void | Activate() | Brings window into the foreground and activates the window.

## SendInput

Send mouse and keyboard input events to the foreground window. This API provides single and multiple sending method. The events sent by single sending method is guaranteed to arrive to the window in order, but this does not necessarily mean it will not be interrupted by the other events. Multiple sending method guarantees that the events sent by it will not be interrupted by the other events.Both methods support the same API for sending mouse and keyboard events except that multiple sending method is need to be called `Send()` at last.

```cs
SendInput.ExtendedKeyDown(Keys.LWin);
// When D key interrupts here,
// Win+D will be invoked unintentionally.
SendInput.ExtendedKeyUp(Keys.LWin); 
```

```cs
SendInput.Multiple().
ExtendedKeyDown(Keys.LWin).
ExtendedKeyUp(Keys.LWin).
Send(); // This won't be interrupted by any other input.
```

### Mouse event

`Down`, `Up`, and `Click` events are supported for the standard push-release type buttons of mouse devices. For example, the provided API for mouse's left button is `LeftDown()`, `LeftUp()` and `LeftClick()`. For single state buttons, `WheelUp()`, `WheelDown()`, `WheelLeft()` and `WheelRight()` are provided. In addition to these, for move event of mouse cursor, `Move(int dx, int dy)` and `MoveTo(int x, int y)` are provided.

##### Methods {ignore=true}

Button | Method Definition | Description
-----|-----|-----
Keys.LButton | LeftDown()
Keys.LButton | LeftUp()
Keys.LButton | LeftClick() | Shortcut to `LeftDown()` and `LeftUp()`.
Keys.RButton | RightDown()
Keys.RButton | RightUp()
Keys.RButton | RightClick() | Shortcut to `RightDown()` and `RightUp()`.
 - | Move(int dx, int dy) | Move the cursor relatively. `dx` and `dy` are relative values.
 - | MoveTo(int x, int y) | Move the cursor to the specified point. `x` and `y` are absolute values.
Keys.MButton | MiddleDown()
Keys.MButton | MiddleUp()
Keys.MButton | MiddleClick() | Shortcut to `MiddleDown()` and `MiddleUp()`.
- | VerticalWheel(int delta) | Send vertical wheel message. If `delta` is positive value, the direction of the wheel is up, otherwise down.
Keys.WheelDown | WheelDown() | Shortcut to `VerticalWheel(-120)`.
Keys.WheelUp | WheelUp() | Shortcut to `VerticalWheel(120)`.
- | HorizontalWheel(int delta) | Send horizontal wheel message. If `delta` is positive value, the direction of the wheel is right, otherwise left.
Keys.WheelLeft | WheelLeft() |  Shortcut to `HorizontalWheel(-120)`.
Keys.WheelRight | WheelRight() |  Shortcut to `HorizontalWheel(120)`.
Keys.XButton1 | X1Down()
Keys.XButton1 | X1Up()
Keys.XButton1 | X1Click() | Shortcut to `X1Down()` and `X1Up()`.
Keys.XButton2 | X2Down()
Keys.XButton2 | X2Up()
Keys.XButton2 | X2Click() | Shortcut to `X2Down()` and `X2Up()`.

### Keyboard event

A keyboard event is synthesized from a key code and two logical flags, `ExtendedKey` and  `ScanCode`. For sending `Up` and `Down` events, `KeyDown(int keyCode)` and `KeyUp(int keyCode)` are provided. 

```cs
SendInput.KeyDown(Keys.A);
SendInput.KeyUp(Keys.A); // Send `A` to the foreground application.
```

`ExetendedKeyDown(int keyCode)` and `ExtentedKeyUp(int keyCode)` are provided when `ExtendedKey` flag is needed to be set.

```cs
SendInput.ExetendedKeyDown(Keys.LWin);
SendInput.ExtentedKeyUp(Keys.LWin); // Send `Win` to the foreground application.
```

For four API above mentioned, combined it with `ScanCode` flag,
`KeyDownWithScanCode(int keyCode)`, `KeyUpWithScanCode(int keyCode)`, `ExtendedKeyDownWithScanCode(int keyCode)` and `ExtendedKeyUpWithScanCode(int keyCode)` are provided.

```cs
SendInput.ExtendedKeyDownWithScanCode(Keys.LControlKey);
SendInput.KeyDownWithScanCode(Keys.S);
SendInput.KeyUpWithScanCode(Keys.S);
SendInput.ExtendedKeyUpWithScanCode(Keys.LControlKey); // Send `Ctrl+S` with scan code to the foreground application.
```

And finally, for to support `Unicode` flag, following functions are provided; `UnicodeKeyDown(char c)`, `UnicodeKeyUp(char c)`,  `UnicodeKeyStroke(string str)`.

```cs
SendInput.UnicodeKeyDown('🍣');
SendInput.UnicodeKeyUp('🍣'); // Send `Sushi` to the foreground application.
```

##### Methods {ignore=true}

Flag | Method Definition | Description
-----|-----|-----
- | KeyDown(int keyCode) |
- | KeyUp(int keyCode) |
Extended | ExetendedKeyDown(int keyCode)
Extended | ExtentedKeyUp(int keyCode)
ScanCode | KeyDownWithScanCode(int keyCode)
ScanCode | KeyUpWithScanCode(int keyCode)
Extended & ScanCode | ExtendedKeyDownWithScanCode(int keyCode)
Extended & ScanCode | ExtendedKeyUpWithScanCode(int keyCode)
- | UnicodeKeyDown(char c)
- | UnicodeKeyUp(char c)
- | UnicodeKeyStroke(string str)

## Notification

### Tooltip

```cs
Tooltip("This is tooltip.");
```

#### Methods {ignore=true}

 Method Definition | Description
-----|-----
Tooltip(string text) | Show tooltip message at the right bottom corner of the display on the cursor, by default. You can configure the position by changing `Config.UI.TooltipPositionBinding`, see [Config/Bindings](#bindings).
Tooltip(string text, Point point) | Show a tooltip message at the specified position.
Tooltip(string text, Point point, int duration) | Show a tooltip message at the specified position for a specified period.

### Balloon

```cs
Balloon("This is balloon.");
```
#### Methods {ignore=true}

 Method Definition | Description
-----|-----
Balloon(string text) | Show a balloon message.
Balloon(string text, string title) | Show a balloon message and a title.
Balloon(string text, string title, int timeout) | Show a balloon message and a title for a specified period.
Balloon(string text, string title, ToolTipIcon icon) | Show a balloon message, a title, and a icon.
Balloon(string text, string title, ToolTipIcon icon, int timeout) | Show a balloon message, a title, and a icon for a specified period.

## Keys

`Keys` provides the definition of all buttons and keys of mouse and keyboard for it's property. This is almost all same as [System.Windows.Forms.Keys](https://msdn.microsoft.com/library/system.windows.forms.keys(v=vs.110).aspx) but for some extentions, wheel and stroke events.

### Differences from System.Windows.Forms.Keys {ignore=true}

#### Extended properties {ignore=true}

Property Name | Description
-----|-----
WheelUp |
WheelDown |
WheelLeft |
WheelRight |
MoveUp |
MoveDown | 
MoveLeft |
MoveRight |

These extended properties are differ than the other properties; these can not be treated as a int value.

```cs
var n = 1 + Keys.A; // n == 65
```

```cs
var n = 1 + Keys.WheelUp; // Compilation error.
```

#### Indexer {ignore=true}

`Keys` supports indexer for getting a key represents specified keyCode.

```cs
Assert.AreEquals(Keys[64], Keys.A);
```

This is useful for getting a key which is not assigned as a `Keys`'s property, but be careful to that the keyCode have the range, 0 to 255.

```cs
var key = Keys[256]; // This throws an IndexOutOfRangeException(); 
```
