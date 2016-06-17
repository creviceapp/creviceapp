### What is this?
CreviceApp is a mouse gesture utility that consists of fully tested small and robust core of 2000 lines, thin GUI wrapper and [Microsoft Roslyn](https://github.com/dotnet/roslyn).
Mouse gestures can be defined as a csx file, so there is noting can not do.<sup>[citation needed]</sup>



### Mouse gesture DSL

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


`@on` caluse tells the system that with which mouse button the gesture starts. 
`@if` clause also tells the trigger of the action of the gesture. 
And finally, `@do` clause represents the action of the gesture will be acivated when all given conditions to be satisfied. 

#### Stroke gestures

As you know, mouse gestures with strokes, namely, stroke gestures, is the most used and  needed of some kinds of mouse gestures. 
CreviceApp naturally supports this.
`@if` clause takes movements of the mouse, combination of `MoveUp`, `MoveDown`, `MoveLeft` and `MoveRight`, as it's argument then.

#### Button gestures
As you may know, mouse gestures with buttons is called "rocker gestures" around mouse gesture utility communities. 
But we call it "button gestures" here. 
CreviceApp supports two kinds of button gestures. 
Both these button gestures are almost the same except that one have `@on` clause and the other one do not have it. 
At this time, `@if` clause takes a mouse button, any of following, `LeftButton`, `MiddleButton`, `RightButton`, `WheelUp`, `WheelDown`, `WheelLeft`, `WheelRight`, `X1Button` and `X2Button`, as it's argument to tell the system that which mouse button is the trigger of the action of the gesture.

##### Button gestures (with `@on` clause)

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

##### Button gestures (without `@on` clause)

```cs
Chrome.
@if(WheelLeft).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_ALT).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_ALT).
    Send(); // Send Alt+Left to Chrome
});
```


### Download

#### Stable

#### Latest

| Branch | Configuration | Download |
|--------|---------------|--------- |
| master [![Build status](https://ci.appveyor.com/api/projects/status/uuthd05870dkkj3w/branch/master?svg=true)](https://ci.appveyor.com/project/rubyu/creviceapp/branch/master) | Release       | [crevice.zip](https://ci.appveyor.com/api/projects/rubyu/creviceapp/artifacts/crevice.zip?branch=master&job=Configuration%3A+Release) |

