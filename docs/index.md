
[TOC]

# Introduction

Crevice4 is multi purpose utility which supports gestures with mouse and keyboard. You can use C# language in your customizable userscript file, so there is nothing that can not be done for you.

Note: Crevice4 requires Windows 7 or later, and .Net Framework 4.6.1 or later.

## Quickstart

// todo link to microsoft app store.

Extract zip file to any location, and click `crevice4.exe`.

After the first execution of the application, you could find `default.csx` in the directory `%APPDATA%\Crevice4`. It's the userscript file. Please open it with a text editor and take a look through it.


After several `using` declaring lines, you will see `Browser` definition following:

```cs
var Browser = When(ctx =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe" ||
           ctx.ForegroundWindow.ModuleName == "firefox.exe" ||
           ctx.ForegroundWindow.ModuleName == "opera.exe" ||
           ctx.ForegroundWindow.ModuleName == "iexplore.exe");
});
```

When the `ModuleName` of `ForegroundWindow` is as follows, `chrome.exe`, `firefox.exe`, `opera.exe`, and so on, then, `When` returns true; this is the declaration of the context which specialized to `Browser`. 

After that, the declaration of gestures follows. Let's see the first one:

```cs
Browser.
On(Keys.RButton).
On(Keys.WheelUp).
Do(ctx =>
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.ControlKey).
    ExtendedKeyDown(Keys.ShiftKey).
    ExtendedKeyDown(Keys.Tab).
    ExtendedKeyUp(Keys.Tab).
    ExtendedKeyUp(Keys.ShiftKey).
    ExtendedKeyUp(Keys.ControlKey).
    Send(); // Previous tab
});
```

This is a mouse gesture definition; when you press and hold `Keys.RButton`, and then if you `Keys.WheelUp` the mouse, codes declared in `Do` will be executed.

This file is just a C# Script file. So, you can use `#load` directive to load another csx file, and can use `#r` directive to add assembly references to the script. By default, the script has the assembly references to `microlib.dll`, `System.dll`, `System.Core.dll`, `Microsoft.CSharp.dll` and `crevice4.exe`. In other words, if there need to add an another assembly reference to the script, it should be declared by using `#r` directive at the head of the script.

For more information about C# Script, please see [Directives - Interactive Window · dotnet/roslyn Wiki](https://github.com/dotnet/roslyn/wiki/Interactive-Window#directives).

# Gesture DSL

## When

All gesture definition starts from `When` clause, representing the condition for the activation of a gesture.
```cs
var Chrome = When(ctx =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe";
});
```

`ctx` is EvaluationContext, see [Crevice4 Core API/EvaluationContext](#evaluationcontext) for more details.

The next to `When` are `On` and `Do` clauses.

## On 

`On` clause takes a button or a sequence of stroke as it's argument. And `On` clause can be declared successively if you needed. Declaration of one or more `On` clause is a gesture.

```cs
// Button gesture.
On(Keys.RButton). // If you press mouse's right button,
```

```cs
// Button gesture with two buttons.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.LButton). // and press mouse's left button,
```

```cs
// Storke gesture.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.MoveUp, Keys.MoveDown). // and draw stroke to up and to down by the pointer,
```

## Do

`Do` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled.

```cs
On(Keys.RButton). // If you press mouse's right button,
Do(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

`ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details.

## Press

`Press` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled, except for the last `On` clause's release event.

```cs
On(Keys.RButton). // If you press mouse's right button,
Press(ctx => // without waiting for release event,
{
    // then this code will be executed.
});
```

`ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details.

## Release

`Release` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled.

```cs
On(Keys.RButton). // If you press mouse's right button,
Release(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

`ctx` is `ExecutionContext`, see [Crevice4 Core API/ExecutionContext](#executioncontext) for more details.

## Button gesture
As you may know, mouse gestures with buttons are called "rocker gesture" in mouse gesture utility communities. But we call it, including it with keyboard buttons, simply `Button gesture` here. 

```cs
// Button gesture.
Browser.
On(Keys.RButton). // If you press mouse's right button,
Do(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

```cs
// Button gesture with two buttons.
Browser.
On(Keys.RButton). // If you press mouse's right button,
On(Keys.LButton). // and press mouse's left button,
Do(ctx => // and release mouse's left or right button,
{
    // then this code will be executed.
});
```

Even if after pressing a button which means the start of a gesture, you can cancel it by holding the button pressing until it to be timeout.

```cs
Browser.
On(Keys.RButton). // If you WRONGLY pressed mouse's right button,
Do(ctx => // you hold the button until timeout and release it,
{
    // then this code will NOT be executed.
});
```

This means actions declared in `Do` clause is not assured it's execution.

Above three gestures are `Button gesture` by the standard buttons. `On` clause with standard buttons can be used for declare `Do` clause but also `Press` and `Release` clauses.

### Button gesture with Press/Release
`Do` clause is just simple but there are cases do not fit to use it. For example, where there is need to hook to press / release events of buttons. `Press` and `Release` clauses fit to this case. These can be written just after `On` clause.

```cs
var Whenever = When(ctx => {
    return true;
});

// Convert Keys.XButton1 to Keys.LWin.
Whenever.
On(Keys.XButton1).
Press(ctx =>
{
    SendInput.ExtendedKeyDown(Keys.LWin);
}).
Release(ctx =>
{
    SendInput.ExtendedKeyUp(Keys.LWin);
});
```
 
And for `Release` clause, it can be after `Do` clause.

```cs
Whenever.
On(Keys.XButton2).
Press(ctx =>
{
    // Assured.
}).
Do(ctx =>
{
    // Not assured. 
    // e.g. When the gesture to timeout,
    //      this action will not be executed.
}).
Release(ctx =>
{
    // Assured.
});
```
Actions declared in `Press` and `Release` clauses are different from it of `Do` clause, the execution of these are assured.

### Button gesture with single state button

Few of the buttons in `Keys` are different from the standard buttons; these have only one state, and only one event. So, `On` clauses with these can not be used with `Press` and `Release` clauses.

```cs
Browser.
On(Keys.WheelUp).
Press(ctx => { }); // Compilation error
```

```cs
Browser.
On(Keys.WheelUp).
Do(ctx => { }); // OK
```

```cs
Browser.
On(Keys.WheelUp).
Release(ctx => { }); // Compilation error
```

Grammatical limitations:
* `On` clause with single state button does not have `Press()` and `Release()` functions.

Single state buttons are `Keys.WheelUp`,  `Keys.WheelDown`,  `Keys.WheelLeft`, and  `Keys.WheelRight`.

## Stroke gesture

"Mouse gestures by strokes", namely `Stroke gesture` is the most important part in the functions of mouse gesture utilities.

`On` clause takes arguments that consist of combination of `Keys.MoveUp`, `Keys.MoveDown`, `Keys.MoveLeft` and `Keys.MoveRight`. These are representing directions of movements of the mouse pointer.

```cs
Browser.
On(Keys.RButton). // If you press right button,
On(Keys.MoveDown, Keys.MoveRight). // and draw stroke to down and to right by the pointer,
Do(ctx => // and release right button,
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.ControlKey).
    ExtendedKeyDown(Keys.W).
    ExtendedKeyUp(Keys.W).
    ExtendedKeyUp(Keys.ControlKey).
    Send(); // then send Ctrl+W to Chrome.
});
```

`Stroke gesture` represents special case when a standard button is pressed, so it have the same grammatical limitation to `Button gesture with single state button`.

Grammatical limitations:
* `On` clause with `Keys.Move*` does not have `Press()` and `Release()` functions.
* `On` clause with `Keys.Move*` should have `Button gesture` by standard button as the previous element.
* `On` clause with `Keys.Move*` should be the last element of the sequence of `On` clauses.

# Config

The system default parameters can be configured by using `Config` as following:

## Values
```cs
// When moved distance of the cursor is exceeded this value, the first stroke 
// will be established.
Config.Core.StrokeStartThreshold = 10;

// When moved distance of the cursor is exceeded this value, and the direction is changed,
// new stroke for new direction will be established.
Config.Core.StrokeDirectionChangeThreshold = 20;

// When moved distance of the cursor is exceeded this value, and the direction is not changed, 
// it will be extended.
Config.Core.StrokeExtensionThreshold = 10;

// Interval time for updating strokes.
Config.Core.WatchInterval = 10; // ms

// When stroke is not established and this period of time has passed, 
// the gesture will be canceled and the original click event will be reproduced.
Config.Core.GestureTimeout = 1000; // ms

// The period of time for showing a tooltip message.
Config.UI.TooltipTimeout = 3000; // ms

// The period of time for showing a balloon message.
Config.UI.BalloonTimeout = 10000; // ms
```

## Bindings

```cs
// Binding for the position of tooltip messages.
Config.UI.TooltipPositionBinding = (point) =>
{
    return point;
}
```

## Events

### StrokeReset {ignore=true}

```cs
Config.Callback.StrokeReset += (sender, e) { };
```
This event activated when the state of mouse's stroke to be reset.
`e` is `StrokeResetEventHandler`, and it does not have special properties.


### StrokeUpdated {ignore=true}
```cs
Config.Callback.StrokeUpdated += (sender, e) { };
```

This event activated when the state of mouse's stroke to be changed.

`e` is `StrokeUpdatedEventHandler `.
Type | Property Name | Description |
-----|-----|------
IReadOnlyList\<Stroke\> | Strokes | 

### StateChanged {ignore=true}
```cs
Config.Callback.StateChanged += (sender, e) { };
```

This event activated when the state of GestureMachine to be changed. 
`e` is `StateChangedEventHandler`.

Type | Property Name | Description |
-----|-----|------
State | LastState | 
State | CurrentState |

### GestureChancelled {ignore=true}
```cs
Config.Callback.GestureCancelled += (sender, e) { };
```

This event activated when the gesture to be cancelled.
`e` is `GestureCancelledEventHandler`.

Type | Property Name | Description |
-----|-----|------
StateN | LastState | 

### GestureTimeout {ignore=true}

```cs
Config.Callback.GestureTimeout += (sender, e) { };
```

This event activated when the gesture to be timeout.
`e` is `GestureTimeoutEventHandler`.

Type | Property Name | Description |
-----|-----|------
StateN | LastState | 

### MachineReset {ignore=true}

```cs
Config.Callback.MachineReset += (sender, e) { };
```

This event activated when GestureMachine to be reset for some reasons.
`e` is `MachineResetEventHandler`.

Type | Property Name | Description |
-----|-----|------
State | LastState | 

# Comamand line interface

```
Usage:
  crevice4.exe [--nogui] [--script path] [--help]

  -g, --nogui       (Default: False) Disable GUI features. Set to true if you 
                    use Crevice as a CUI application.

  -n, --nocache     (Default: False) Disable user script assembly caching. 
                    Strongly recommend this value to false because compiling 
                    task consumes CPU resources every startup of application if
                    true.

  -s, --script      (Default: default.csx) Path to user script file. Use this 
                    option if you need to change the default location of user 
                    script. If given value is relative path, Crevice will 
                    resolve it to absolute path based on the default directory 
                    (%USERPROFILE%\AppData\Roaming\Crevice\CreviceApp).

  -p, --priority    (Default: High) Process priority. Acceptable values are the
                    following: AboveNormal, BelowNormal, High, Idle, Normal, 
                    RealTime.

  -V, --verbose     (Default: False) Show details about running application.

  -v, --version     (Default: False) Display product version.

  --help            Display this help screen.
```

# Crevice4 Core API

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

## WindowInfo

`WindowInfo` is a thin wrapper of the handle of a window. This class provides properties and methods to use window handles more easily.

#### Properties {ignore=true}

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

#### Methods {ignore=true}

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

These properties are differ than the other properties; these can not be treated as a int value.

```cs
var n = 0 + Keys.A; // n == 65
```

```cs
var n = 0 + Keys.WheelUp; // Compilation error.
```

#### Indexer {ignore=true}

`Keys` supports indexer for getting a key represents specified keyCode.

```cs
Assert.AreEquals(Keys[64], Keys.A);
```

This is useful for getting a key which is not assigned as a `Keys`'s property, but be careful to that the keyCode have the range, 0 to 255.

```cs
var key = Keys[256]; // This throws IndexOutOfRangeException(); 
```

# Extension API

### Window

`Window` is a utility static class about Windows's window.
To use this class, declare as following:
```cs
using CreviceApp.WinAPI.Window;
```

#### Methods {ignore=true}

Return Value | Method Definition | Description
-----|-----|-----
WindowInfo | Window.From(IntPtr hWnd) | This function wraps `IntPtr` and returns an instance of `WindowInfo`.
System.Drawing.Point | Window.GetCursorPos() | A shortcut to win32 API `GetCursorPos()`.
System.Drawing.Point | Window.GetLogicalCursorPos() | Returns logical cursor position culculated based on win32 API `GetPhysicalCursorPos()` and physical and logical screen size.
System.Drawing.Point | Window.GetPhysicalCursorPos() | A shortcut to win32 API `GetPhysicalCursorPos()`.
WindowInfo | WindowFromPoint(Point point) | Returns a window under the cursor.
WindowInfo | FindWindow(string lpClassName, string lpWindowName) | Find a window matches given class name and window name.
IReadOnlyList<WindowInfo> | GetTopLevelWindows() | Enumerates all windows.
IReadOnlyList<WindowInfo> | GetThreadWindows(int threadId) | Enumerates all windows belonging specified thread.

### VirtualKeys

This class provides the virtual key constants. 

Note: for `VK_0` to `VK_9` and `VK_A` to `VK_Z`, this is an extension for convenience limited in this application.

To use this class, declare as following:
```cs
using static CreviceApp.WinAPI.Constants.VirtualKeys;
```

For more details, see [Virtual-Key Codes (Windows)](https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx).

### WindowsMessages

This class provides the windows message constants. 
To use this class, declare as following:
```cs
using static CreviceApp.WinAPI.Constants.WindowsMessages;
```

For more details, see [Window Messages (Windows)](https://msdn.microsoft.com/en-us/library/windows/desktop/ff381405(v=vs.85).aspx).

### VolumeControl

`VolumeControl` is a utility class about system audio volume.
To use this class, declare as following:
```cs
using CreviceApp.WinAPI.CoreAudio;
var VolumeControl = new VolumeControl();
```

#### GetMasterVolume()

Returns window's current master mixer volume.
This function returns a `float` value, within the range between 0 and 1.

#### SetMasterVolume(float value)

Sets window's current master mixer volume. The value should be within the range between 0 and 1.

## Lisence

MIT Lisense

## Author
[Rubyu](https://twitter.com/ruby_U), [Yasuyuki Nishiseki](mailto:tukigase@gmail.com)

## Latest releases (not recommended)

| Branch | Status | Download |
|--------|---------------|--------- |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=master&job=Configuration%3A+Release) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=develop&job=Configuration%3A+Release) |
