
# Profile

`Profile` is the concept extends the specification of former gesture definition in parallel direction. 

## Single context problem on Crevice3

Gesture definition on Crevice3 had a limitation. If you want to use `Keys.XButton1` as `Keys.LWin`, you can do it by using `Press` and `Release` clauses.
```cs
// Simple conversion gesture.
When(ctx => { return true; }).
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

But at the same time, if your userscript file have another gesture definition which has it's own context, do you think above gesture defintion works properly always?

```cs
// Another gesture.
When(ctx => 
{ 
    return ctx.ForegroundWindow.ModuleName == "chrome.exe"; 
}).
On(Keys.RButton).
On(Keys.MoveDown).
Do(ctx =>
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.Home).
    ExtendedKeyUp(Keys.Home).
    Send(); // then send Home to Chrome.
});
```

When you press `Keys.RButton` and hoding it, the convertion `Keys.XButton1` to `Keys.LWin` will not work. For coexisting both gesture definition, you should add redundant codes to the latter.

```cs
// Another gesture (redundant but correct).
var Chrome = When(ctx => 
{ 
    return ctx.ForegroundWindow.ModuleName == "chrome.exe"; 
});

Chrome.
On(Keys.RButton).
On(Keys.XButton1).
Press(ctx =>
{
    SendInput.ExtendedKeyDown(Keys.LWin);
}).
Release(ctx =>
{
    SendInput.ExtendedKeyUp(Keys.LWin);
});

Chrome.
On(Keys.RButton).
On(Keys.MoveDown).
Do(ctx =>
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.Home).
    ExtendedKeyUp(Keys.Home).
    Send(); // then send Home to Chrome.
});
```

## Solving the problem with DeclareProfile

`DeclareProfile()` declares new profile which has completely new `Config` and empty gesture definition. Changes added to `Config` and gesture definition declared by gesture DSL starting with `When` before that point will be held as the before profile.

```cs
Config.Core.GestureTimeout = 5000; // Change 1000ms to 5000ms.
DeclareProfile("NewProfile"); // Config and gesture definition was reset.
ToolTip(Config.Core.GestureTimeout.ToString()); // 1000 will be shown.
```

Using this function, you can represent multiple gesture definition working in parallel.

```cs
DeclareProfile("Whenever");

When(ctx => { return true; }).
On(Keys.XButton1).
Press(ctx =>
{
    SendInput.ExtendedKeyDown(Keys.LWin);
}).
Release(ctx =>
{
    SendInput.ExtendedKeyUp(Keys.LWin);
});

DeclareProfile("Chrome");

When(ctx => 
{ 
    return ctx.ForegroundWindow.ModuleName == "chrome.exe"; 
}).
On(Keys.RButton).
On(Keys.MoveDown).
Do(ctx =>
{
    SendInput.Multiple().
    ExtendedKeyDown(Keys.Home).
    ExtendedKeyUp(Keys.Home).
    Send(); // then send Home to Chrome.
});
```

Even when you press and holding `Keys.RButton`, the convertion `Keys.XButton1` to `Keys.LWin` will work perfectly.

Note: The input event will be passed sequencely from the head of the profiles to the bottom, but if the event to be consumed in the `GestureMachine` of each profile, then it will not to be passed to the next profile.
