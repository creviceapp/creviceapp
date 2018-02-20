
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

`ctx` is `Crevice.GestureMachine.EvaluationContext`, and have following properties:

Type | Property Name
-----|-----
System.Drawing.Point|GestureStartPosition
ForegroundWindowInfo|ForegroundWindow
PointedWindowInfo|PointedWindow

These values are guaranteed that same values are provided as `ctx`'s property in `Press`, `Do`, and `Release` clauses.

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

`ctx` is `Crevice.GestureMachine.ExecutionContext`, and have following properties:

Type | Property Name
-----|-----
System.Drawing.Point | GestureStartPosition
System.Drawing.Point | GestureEndPosition
ForegroundWindowInfo | ForegroundWindow
PointedWindowInfo | PointedWindow

These values, except for `GestureEndPosition`, are guaranteed that same values are provided as `ctx`'s property in `When` clause. 

## Press

`Press` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled, except for the last `On` clause's release event.

```cs
On(Keys.RButton). // If you press mouse's right button,
Press(ctx => // without waiting for release event,
{
    // then this code will be executed.
});
```

`ctx` is `Crevice.GestureMachine.ExecutionContext`, and have following properties:

Type | Property Name
-----|-----
System.Drawing.Point | GestureStartPosition
System.Drawing.Point | GestureEndPosition
ForegroundWindowInfo | ForegroundWindow
PointedWindowInfo | PointedWindow

These values, except for `GestureEndPosition`, are guaranteed that same values are provided as `ctx`'s property in `When` clause.

## Release

`Release` clause declares an action which will be executed only when the conditions, given by the context it to be declared, are fully filled.

```cs
On(Keys.RButton). // If you press mouse's right button,
Release(ctx => // and release mouse's right button,
{
    // then this code will be executed.
});
```

`ctx` is `Crevice.GestureMachine.ExecutionContext`, and have following properties:

Type | Property Name
-----|-----
System.Drawing.Point | GestureStartPosition
System.Drawing.Point | GestureEndPosition
ForegroundWindowInfo | ForegroundWindow
PointedWindowInfo | PointedWindow

These values, except for `GestureEndPosition`, are guaranteed that same values are provided as `ctx`'s property in `When` clause. 

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

Both above gestures are `Button gesture` by the standard buttons. `On` clause with standard buttons can be used for declare `Do` clause but also `Press` and `Release` clauses.

## Button gesture with Press/Release
`Do` clause is just simple but there is cases does not fit to use it. For example, where there is need to hook to press / release events of buttons. `Press` and `Release` clauses fit to that case. These can be written just after `On` clause.

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
Actions given in `Press` and `Release` clauses are different from it of `Do` clause, the execution of these are assured.

```cs
Whenever.
On(XButton2).
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

## Single state button gesture

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

`Stroke gesture` represents special case when a standard button is pressed, so it have the same grammatical limitation to `Single state button gesture`.

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

```cs
// event Crevice.GestureMachine.CallbackManager.StrokeResetEventHandler
Config.Callback.StrokeReset += (sender, e) { };

// event Crevice.GestureMachine.CallbackManager.StrokeUpdatedEventHandler 
Config.Callback.StrokeUpdated += (sender, e) { };

// event Crevice.GestureMachine.CallbackManager.StateChangedEventHandler 
Config.Callback.StateChanged += (sender, e) { };

// event Crevice.GestureMachine.CallbackManager.GestureCancelledEventHandler 
Config.Callback.GestureCancelled += (sender, e) { };

// event Crevice.GestureMachine.CallbackManager.GestureTimeoutEventHandler 
Config.Callback.GestureTimeout += (sender, e) { };

// event Crevice.GestureMachine.CallbackManager.MachineResetEventHandler 
Config.Callback.MachineReset += (sender, e) { };
```

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

Added in Crevice 3.0.

# Crevice4 Core API

## EvaluationContext

`Crevice.GestureMachine.EvaluationContext` have following properties:

### Property

Type | Name | Description |
-----|-----|-----|
System.Drawing.Point | GestureStartPosition
ForegroundWindowInfo | ForegroundWindow | The window which was on the foreground when a gesture started. This is an instance of `WindowInfo`.
PointedWindowInfo | PointedWindow | The window which was under the cursor when a gesture started. This is an instance of `WindowInfo`.

These values are guaranteed that same values are provided as `ExecutionContext`'s property in `Press`, `Do`, and `Release` clauses.

### ExecutionContext

`Crevice.GestureMachine.ExecutionContext` have following properties:

### Property

Type | Name | Description |
-----|-----|-----|
System.Drawing.Point | GestureStartPosition
System.Drawing.Point | GestureEndPosition
ForegroundWindowInfo | ForegroundWindow | The window which was on the foreground when a gesture started. This is an instance of `WindowInfo`.
PointedWindowInfo | PointedWindow | The window which was under the cursor when a gesture started. This is an instance of `WindowInfo`.

These values, except for `GestureEndPosition`, are guaranteed that same values are provided as `EvaluationContext`'s property in `When` clause. 

### WindowInfo

`WindowInfo` is a thin wrapper of the handle of a window. This class provides properties and methods to use window handles more easily.

#### Properties
This class provides properties as following; `WindowHandle`, `ThreadId`, `ProcessId`, `WindowId`, `Text`, `ClassName`, `Parent`, `ModulePath`, `ModuleName`.

#### Methods

##### SendMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `SendMessage(WindowHandle, Msg, wParam, lParam)`. 
This function returns a `long` value directly from win32 API.

##### PostMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `PostMessage(WindowHandle, Msg, wParam, lParam)`.
This function returns a `bool` value directly from win32 API.

##### BringWindowToTop()

A shortcut to win32 API `BringWindowToTop(WindowHandle)`.
This function returns a `bool` value directly from win32 API.

##### FindWindowEx(IntPtr hwndChildAfter, string lpszClass, string lpszWindow)

A shortcut to win32 API `FindWindowEx(WindowHandle, hwndChildAfter, lpszClass, lpszWindow)`.
This function returns an instance of `WindowInfo`.

##### FindWindowEx(string lpszClass, string lpszWindow)

A shortcut to win32 API `FindWindowEx(WindowHandle, IntPtr.Zero, lpszClass, lpszWindow)`.
This function returns an instance of `WindowInfo`.

##### GetChildWindows()

A shortcut to win32 API `EnumChildWindows(WindowHandle, EnumWindowProc, IntPtr.Zero)`.
This function returns an instance of `IEmumerable<WindowInfo>`.

##### GetPointedDescendantWindows(Point point, Window.WindowFromPointFlags flags)

A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, flags)`.
This function recursively calls `ChildWindowFromPointEx` until the last descendant window and returns an instance of `IEmumerable<WindowInfo>`.

##### GetPointedDescendantWindows(Point point)

A shortcut to win32 API `ChildWindowFromPointEx(hWnd, point, Window.WindowFromPointFlags.CWP_ALL)`.
This function recursively calls `ChildWindowFromPointEx` until the last descendant window and returns an instance of `IEmumerable<WindowInfo>`.

##### Activate()
Brings window into the foreground and activates the window.

### SendInput

Send mouse and keyboard input events to the foreground window. 
This API provides single and multiple sending method. 
The events sent by single sending method is guaranteed to arrive to the window in order, but this does not necessarily mean it will not be interrupted by the other events. 
Multiple sending method guarantees that the events sent by it will not be interrupted by the other events.
Both methods support the same API for sending mouse and keyboard events except that multiple sending method is need to be called `Send()` at last.

```cs
SendInput.ExtendedKeyDown(VK_LWIN);
// When D key interrupts here,
// Win+D will be invoked unintentionally.
SendInput.ExtendedKeyUp(VK_LWIN); 
```

```cs
SendInput.Multiple().
ExtendedKeyDown(VK_LWIN).
ExtendedKeyUp(VK_LWIN).
Send(); // This won't be interrupted by any other input.
```

#### Mouse event
`Down`, `Up`, and `Click` events are supported for the push-release type buttons of mouse devices as following; `LeftButton`, `MiddleButton`, `RightButton`, `X1Button`, `X2Button`. For example, the provided API for `LeftButton` is `LeftDown()`, `LeftUp()` and `LeftClick()`. 

For single push type buttons, `WheelUp()`, `WheelDown()`, `WheelLeft()` and `WheelRight()` are provided. 

For move events, `Move(int dx, int dy)` and `MoveTo(int x, int y)` are provided.

##### Complete list of supported methods
- LeftDown()
- LeftUp()
- LeftClick()
- RightDown()
- RightUp()
- RightClick()
- Move(int dx, int dy)
- MoveTo(int x, int y)
- MiddleDown()
- MiddleUp()
- MiddleClick()
- VerticalWheel(short delta)
- WheelDown()
- WheelUp()
- HorizontalWheel(short delta)
- WheelLeft()
- WheelRight()
- X1Down()
- X1Up()
- X1Click()
- X2Down()
- X2Up()
- X2Click()

#### Keyboard event

A keyboard event is synthesized from a key code and two logical flags, `ExtendedKey` and  `ScanCode`. For sending `Up` and `Down` events, `KeyDown(ushort keyCode)` and `KeyUp(ushort keyCode)` are provided. 

```cs
SendInput.KeyDown(VK_A);
SendInput.KeyUp(VK_A); // Send `A` to the foreground application.
```

`ExetendedKeyDown(ushort keyCode)` and `ExtentedKeyUp(ushort keyCode)` are provided when `ExtendedKey` flag is needed to be set.

```cs
SendInput.ExetendedKeyDown(VK_LWIN);
SendInput.ExtentedKeyUp(VK_LWIN); // Send `Win` to the foreground application.
```

For four API above mentioned, combined it with `ScanCode` flag,
`KeyDownWithScanCode(ushort keyCode)`, `KeyUpWithScanCode(ushort keyCode)`, `ExtendedKeyDownWithScanCode(ushort keyCode)` and `ExtendedKeyUpWithScanCode(ushort keyCode)` are provided.

```cs
SendInput.ExtendedKeyDownWithScanCode(VK_LCONTROL);
SendInput.KeyDownWithScanCode(VK_S);
SendInput.KeyUpWithScanCode(VK_S);
SendInput.ExtendedKeyUpWithScanCode(VK_LCONTROL); // Send `Ctrl+S` with scan code to the foreground application.
```

And finally, for to support `Unicode` flag, following functions are provided; `UnicodeKeyDown(char c)`, `UnicodeKeyUp(char c)`,  `UnicodeKeyStroke(string str)`.

```cs
SendInput.UnicodeKeyDown('🍣');
SendInput.UnicodeKeyUp('🍣'); // Send `Sushi` to the foreground application.
```

Note: `keyCode` is a virtual key code. See [VirtualKeys](#virtualkeys).

##### Complete list of supported methods

- KeyDown(ushort keyCode)
- KeyUp(ushort keyCode)
- ExtendedKeyDown(ushort keyCode)
- ExtendedKeyUp(ushort keyCode)
- KeyDownWithScanCode(ushort keyCode)
- KeyUpWithScanCode(ushort keyCode)
- ExtendedKeyDownWithScanCode(ushort keyCode)
- ExtendedKeyUpWithScanCode(ushort keyCode)
- UnicodeKeyDown(char c)
- UnicodeKeyUp(char c)
- UnicodeKeyStroke(string str)

### Notification

#### Tooltip(string text)

Show tooltip message at the right bottom corner of the display on the cursor.

```cs
Tooltip("This is tooltip.");
```

#### Tooltip(string text, Point point)

Show a tooltip message at the specified position.

#### Tooltip(string text, Point point, int duration)

Show a tooltip message at the specified position for a specified period.

#### Balloon(string text)

Show a balloon message.

```cs
Balloon("This is balloon.");
```

#### Balloon(string text, string title)

Show a balloon message and a title.

#### Balloon(string text, string title, int timeout)

Show a balloon message and a title for a specified period.

#### Balloon(string text, string title, ToolTipIcon icon)

Show a balloon message, a title, and a icon.

#### Balloon(string text, string title, ToolTipIcon icon, int timeout)

Show a balloon message, a title, and a icon for a specified period.

## Extension API

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

### Window

`Window` is a utility static class about Windows's window.
To use this class, declare as following:
```cs
using CreviceApp.WinAPI.Window;
```

#### From(IntPtr hWnd)

This function wraps `IntPtr` and returns an instance of `WindowInfo`.

#### GetCursorPos()

Returns current position of the cursor.
This function returns an instance of `Point`.

#### WindowFromPoint(Point point)

Returns a window under the cursor.
This function returns an instance of `WindowInfo`.

#### FindWindow(string lpClassName, string lpWindowName)

Find a window matches given class name and window name.
This function returns an instance of `WindowInfo`.

#### GetTopLevelWindows()

Enumerates all windows.
This function returns an instance of `IEnumerable<WindowInfo>`.

#### GetThreadWindows(uint threadId)

Enumerates all windows belonging specified thread.
This function returns an instance of `IEnumerable<WindowInfo>`.

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
