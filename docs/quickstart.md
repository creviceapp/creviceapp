
## Quickstart

After the first execution of the application, you can find `default.csx` in the directory `%APPDATA%\Crevice4`. It's the userscript file. Please open it with a text editor and take a look through it.


After several `using` declaring lines, you can see `Browser` definition as following (but here, a little bit shortened):

```cs
var Browser = When(ctx =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe" ||
           ctx.ForegroundWindow.ModuleName == "firefox.exe" ||
           ctx.ForegroundWindow.ModuleName == "iexplore.exe");
});
```

When the `ModuleName` of `ForegroundWindow` is one of follows, `chrome.exe`, `firefox.exe`, or `iexplore.exe`, then, `When` returns true; this is the declaration of initialization of a context which specialized to `Browser`. 

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

This is a mouse gesture definition; when you press and hold `Keys.RButton`, and then if you `Keys.WheelUp` it, the actions declared in `Do` will be executed.

As long as Crevice4 is executing, you can edit userscript file at any time. While reading the following sections, of course. Crevice4 supports **hotloading** feature. Whenever Crevice4 detects an update of user script file, it will be compiled and evaluated immediately, then the userscript updated will be loaded if the compilation is successful. 

<!--
// todo movie

If the compilation is failed, error message will be shown. You can see the details of it by clicking it.

// todo movie

If the evaluation is falied, warning message will be shown. You can see the details of it by clicking it.

//todo movie
-->

The userscript file is just a C# Scripting file. You can do anything you want by writing your own script in it, or else by just copying codes from [Stack Overflow](https://stackoverflow.com/). See [Overview of C# Scripting](#overview-of-c-scripting) for more details.
