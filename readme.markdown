 
| master | develop |
|--------|---------|
| [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) |

CreviceApp is a mouse gesture utility that consists of fully tested small and robust core of 2000 lines, thin GUI wrapper and [Microsoft Roslyn](https://github.com/dotnet/roslyn).
Mouse gestures can be defined as a csx file, so there is noting can not do.<sup>[citation needed]</sup>

This software requires Windows7 or later, and .Net Framework 4.6.

## User script

After first starting of `CreviceApp.exe`, move to `%APPDATA%\Crevice\CreviceApp`, and you could find `default.csx`. It's the user script file. Please open it with a text editor and look through it. 

`default.csx` is merely a csharp script file so that you can use `#load` directive to load another csx file and can use `#r` directive to add  assembly references to the script. By default, the script has the assembly references to `microlib.dll`, `System.dll`, `System.Core.dll`, `Microsoft.CSharp.dll` and `CreviceApp.exe`. In other words, if there is need to add an another assembly reference to the script, it should be declared by using `#r` directive at the head of the script.

For more details, see [Directives - Interactive Window · dotnet/roslyn Wiki](https://github.com/dotnet/roslyn/wiki/Interactive-Window#directives).

## Mouse gesture DSL

All mouse gesture definition start with `@when` clause represents the condition for the activation of a mouse gesture.
```cs
var Chrome = @when((ctx) =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe";
});
```

Following clauses to `@when` is `@on`, `@if` and `@do`. 

```cs
Chrome.
@on(RightButton).
@if(MoveDown, MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LCONTROL).
    KeyDown(VK_W).
    KeyUp(VK_W).
    ExtendedKeyUp(VK_LCONTROL).
    Send(); // Send Ctrl+W to Chrome
});
```


`@on` caluse tells the system that which mouse button will be used at start of the gesture. 
`@if` clause also tells the trigger of the action of the gesture. 
And finally, `@do` clause represents the action of the gesture will be acivated when all given conditions to be satisfied. 

### Stroke gestures

Mouse gestures with strokes, namely, stroke gestures, is the most used and needed of some kinds of mouse gestures. 
CreviceApp naturally supports this.
`@if` clause takes movements of the mouse, combination of `MoveUp`, `MoveDown`, `MoveLeft` and `MoveRight`, as it's argument then.

```cs
Chrome.
@on(RightButton).
@if(MoveUp, MoveDown, MoveLeft, MoveRight, ...). // There is no limit on the length.
@do((ctx) => {
    SendInput.Multiple().
    ExtendedKeyDown(VK_LCONTROL).
    ExtendedKeyDown(VK_LSHIFT).
    KeyDown(VK_T).
    KeyUp(VK_T).
    ExtendedKeyUp(VK_LSHIFT).
    ExtendedKeyUp(VK_LCONTROL).
    Send(); // Send Ctrl+Shift+T to Chrome
});
```

### Button gestures
As you may know, mouse gestures with buttons is called "rocker gestures" around mouse gesture utility communities. 
But we call it "button gestures" here. 
CreviceApp supports two kinds of button gestures. 
Both these button gestures are almost the same except that one have `@on` clause and the other one do not have it. 
At this time, `@if` clause takes a mouse button, any of following, `LeftButton`, `MiddleButton`, `RightButton`, `WheelUp`, `WheelDown`, `WheelLeft`, `WheelRight`, `X1Button` and `X2Button`, as it's argument to tell the system that which mouse button is the trigger of the action of the gesture.

#### Button gestures (with `@on` clause)

```cs
Chrome.
@on(RightButton).
@if(WheelDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Send Ctrl+Tab to Chrome
});
```

#### Button gestures (without `@on` clause)

```cs
Chrome.
@if(WheelLeft).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LMENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_LMENU).
    Send(); // Send Alt+Left to Chrome
});
```


## Config

The system default parameters can be configured by using `Config` as following:

```cs
// When moved distance of the cursor is exceeded this value, the first stroke 
// will be established.
Config.Gesture.InitialStrokeThreshold = 10;

// When moved distance of the cursor is exceeded this value and the direction 
// of the movement is different from the current stroke, new stroke 
// for new direction will be established.
Config.Gesture.StrokeDirectionChangeThreshold = 20;

// When moved distance of the cursor is exceeded this value and the direction 
// of the movement is the same as the current stroke, it will be extended.
Config.Gesture.StrokeExtensionThreshold = 10;

// Interval time for updating strokes.
Config.Gesture.WatchInterval = 10;

// When there is no established stroke yet and this period of time has passed, 
// the gesture will be canceled and the original click event will be reproduced.
Config.Gesture.Timeout = 1000;

// The period of time for showing a tooltip message.
Config.UI.TooltipTimeout = 3000;

// The period of time for showing a baloon message.
Config.UI.BaloonTimeout = 10000;

// Binding for the position of tooltip messages.
Config.UI.TooltipPositionBinding = (point) =>
{
    var newPoint = // Create new point.
    return newPoint;
}
```

## Core API

### ExecutionContext
`@when` clause and `@do` clause take a function as it's argument, and the function takes an `ExecutionContext` as it's argument. 
An `ExecutionContext` will be generated each time gestures started, and the same instance of it will be given to the functions of `@when` and `@do` to guarantee that these functions will be executed on the same context.

#### Properties

##### ForegroundWindow

The window which was on the foreground when a gesture started. 
This is an instance of `WindowInfo`.

##### PointedWindow

The window which was under the cursor when a gesture started. 
This is an instance of `WindowInfo`.

### WindowInfo

`WindowInfo` is a thin wrapper of the handle of a window. This class provides properties and methods to use window handles more easily.

#### Properties
This class provides `WindowHandle`, `ThreadId`, `ProcessId`, `WindowId`, `Text`, `ClassName`, `Parent`, `ModulePath` and `ModuleName` as it's property.

#### Methods

##### SendMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `SendMessage(WindowHandle, Msg, wParam, lParam)`. 
This function returns a `long` value returned by win32 API directly.

##### PostMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `PostMessage(WindowHandle, Msg, wParam, lParam)`.
This function returns a `bool` value returned by win32 API directly.

##### BringWindowToTop()

A shortcut to win32 API `BringWindowToTop(WindowHandle)`.
This function returns a `bool` value returned by win32 API directly.

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
Send(); // This won't interrupted by any other input.
```

#### Mouse event
`Down`, `Up` and `Click` events are supported for the push-release type buttons of mouse devices, `LeftButton`, `MiddleButton`, `RightButton`, `X1Button` and `X2Button`. For example, the provided API for `LeftButton` is `LeftDown()`, `LeftUp()`, `LeftClick()`. 

For single push type buttons, `WheelUp()`, `WheelDown()`, `WheelLeft()` and `WheelRight()` are provided. 

For move events, `Move(int dx, int dy)` and `MoveTo(int x, int y)` are also provided.

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

A keyboard event is synthesized from a key code with two logical flags, `ExtendedKey` and  `ScanCode`. For sending `Up` and `Down` events for a key, `KeyDown(ushort keyCode)` and `KeyUp(ushort keyCode)` are provided. 

```cs
SendInput.KeyDown(VK_A);
SendInput.KeyUp(VK_A); // Send `A` to the foreground application.
```

`ExetendedKeyDown(ushort keyCode)` and `ExtentedKeyUp(ushort keyCode)` are provided when `ExtendedKey` flag is needed to be set.

```cs
SendInput.ExetendedKeyDown(VK_LWIN);
SendInput.ExtentedKeyUp(VK_LWIN); // Send `Win` to the foreground application.
```

For the API combined four API above mentioned with `ScanCode` flag,
`KeyDownWithScanCode(ushort keyCode)`, `KeyUpWithScanCode(ushort keyCode)`, `ExtendedKeyDownWithScanCode(ushort keyCode)` and `ExtendedKeyUpWithScanCode(ushort keyCode)` are also provided.

```cs
SendInput.ExtendedKeyDownWithScanCode(VK_LCONTROL);
SendInput.KeyDownWithScanCode(VK_S);
SendInput.KeyUpWithScanCode(VK_S);
SendInput.ExtendedKeyUpWithScanCode(VK_LCONTROL); // Send `Ctrl+S` with scan code to the foreground application.
```

And finally, for the API to support an other special `Unicode` flag, `UnicodeKeyDown(char c)`, `UnicodeKeyUp(char c)` and `UnicodeKeyStroke(string str)` are provided.

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
Tooptip("This is tooltip.");
```

#### Tooltip(string text, Point point)

Show a tooltip message at the specified point.

#### Tooltip(string text, Point point, int duration)

Show a tooltip message at the specified point for a specified period.

#### Baloon(string text)

Show a baloon message.

```cs
Baloon("This is baloon.");
```

#### Baloon(string text, string title)

Show a baloon message with a title.

#### Baloon(string text, string title, int timeout)

Show a baloon message with a title for a specified period.

#### Baloon(string text, string title, ToolTipIcon icon)

Show a baloon message with a title and a icon.

#### Baloon(string text, string title, ToolTipIcon icon, int timeout)

Show a baloon message with a title and a icon for a specified period.

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

`VolumeControl` is a utility static class about system audio volume.
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

####

## Lisence

MIT Lisense

## Latest releases (not recommended)

| Branch | Status | Download |
|--------|---------------|--------- |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=master&job=Configuration%3A+Release) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=develop&job=Configuration%3A+Release) |
