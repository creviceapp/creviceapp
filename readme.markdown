 
| master | develop |
|--------|---------|
| [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) |

## What is this?
CreviceApp is a mouse gesture utility that consists of fully tested small and robust core of 2000 lines, thin GUI wrapper and [Microsoft Roslyn](https://github.com/dotnet/roslyn).
Mouse gestures can be defined as a csx file, so there is noting can not do.<sup>[citation needed]</sup>

This software requires Windows7 or later, and .Net Framework 4.6.

## Edit config csx file

After first starting of `CreviceApp.exe`, move to `%APPDATA%\Crevice\CreviceApp`, and you could find `default.csx`. It's the config file. Please open it with a text editor and look through it. 

## Mouse gesture DSL

Firstly, all gestures start with `@when` clause represent the condition for the activation of a mouse gesture.
```cs
var Chrome = @when((ctx) =>
{
    return ctx.Window.ModuleName == "chrome.exe";
});
```

The following clauses to `@when` is `@on`, `@if` and `@do`. 

```cs
Chrome.
@on(RightButton).
@if(MoveDown, MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    KeyDown(VK_W).
    KeyUp(VK_W).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Send Ctrl+W to Chrome
});
```


`@on` caluse tells the system that which mouse button will be used when the gesture starts. 
`@if` clause also tells the trigger of the action of the gesture. 
And finally, `@do` clause represents the action of the gesture will be acivated when all given conditions to be satisfied. 

### Stroke gestures

Mouse gestures with strokes, namely, stroke gestures, is the most used and  needed of some kinds of mouse gestures. 
CreviceApp naturally supports this.
`@if` clause takes movements of the mouse, combination of `MoveUp`, `MoveDown`, `MoveLeft` and `MoveRight`, as it's argument then.

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

## API

### ExecutionContext
`@when` clause and `@do` clause take a function as it's argument, and the function takes an ExecutionContext as it's argument. 
An ExecutionContext will be generated each time gestures started, and the same instance of it will be given to the functions of `@when` and `@do` to guarantee that these functions will be executed on the same context.

#### ExecutionContext.Window

The window which was on the foreground when a gesture started. 
This is an instance of `Window`.

#### ExecutionContext.Window.OnCursor

The window which was under the cursor when a gesture started. 
This is an instance of `Window`.

#### ExecutionContext.Window.Now()

If you would like to get current `Window`, `Window.Now()` provides it. 
This is an instance of `Window`.

### Window

This class provides `Handle`, `ThreadId`, `ProcessId`, `Id`, `Text`, `ClassName`, `Parent`, `ModulePath` and `ModuleName` as it's property.

#### Window.BringToTop()

A shortcut to win32 API `BringWindowToTop(Handle)`.

#### Window.SendMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `SendMessage(Handle, Msg, wParam, lParam)`.

#### Window.PostMessage(uint Msg, uint wParam, uint lParam)

A shortcut to win32 API `SendMessage(Handle, Msg, wParam, lParam)`.

### SendInput

Send mouse and keyboard input events to the foreground window. 
This API provides single and multiple sending method. 
The events sent by single sending method is guaranteed to arrive the window in order, but this does not necessarily mean the events will not be interrupted by the other events. 
Multiple sending method guarantees the events sent by it will not be interrupted by the other events.
Both methods support the same API for sending mouse events and keyboard events, but for multiple sending method, there is need to explicitly call `Send()` at last.

#### Mouse event

#### Keyboard event


#### SendInput.XXXX()

```cs
SendInput.ExtendedKeyDown(VK_LWIN);
// When D key interrupts here,
// Win+D will be invoked unintentionally.
SendInput.ExtendedKeyUp(VK_LWIN); 

```

#### SendInput.Muptiple().XXXX

```cs
SendInput.Multiple().
ExtendedKeyDown(VK_LWIN).
ExtendedKeyUp(VK_LWIN).
Send(); // This won't interrupted by any other input.
```

### VK_XXXX

Virtual key codes. See [Virtual-Key Codes (Windows)](https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85).aspx).

Note: CreviceApp provides VK_0 to VK_9 and VK_A to VK_Z but this is an extension for convenience.

### Notification

#### Tooltip(string text)

Show tooltip message on the right bottom corner of the display on the cusor.

#### Baloon(string text)

Show baloon message.

## Lisence

MIT Lisense.

## Latest releases (not recommended)

| Branch | Status | Download |
|--------|---------------|--------- |
| master | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=master&job=Configuration%3A+Release) |
| develop | [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/develop?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/develop) | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=develop&job=Configuration%3A+Release) |
