
var Browser = @when((ctx) =>
{
    return ctx.Window.ModuleName == "chrome.exe" ||
           ctx.Window.ModuleName == "firefox.exe" ||
           ctx.Window.ModuleName == "opera.exe" ||
           ctx.Window.ModuleName == "iexplore.exe";
});

Browser.
@on(RightButton).
@if(WheelUp).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_SHIFT).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_SHIFT).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Previous tab
});

Browser.
@on(RightButton).
@if(WheelDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Next tab
});

Browser.
@on(RightButton).
@if(MoveUp).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_HOME).
    ExtendedKeyUp(VK_HOME).
    Send(); // Scroll to top
});

Browser.
@on(RightButton).
@if(MoveDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_END).
    ExtendedKeyUp(VK_END).
    Send(); // Scroll to bottom
});

Browser.
@on(RightButton).
@if(MoveLeft).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_MENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_MENU).
    Send(); // Go back
});

Browser.
@on(RightButton).
@if(MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_MENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_MENU).
    Send(); // Go next
});

Browser.
@on(RightButton).
@if(MoveUp, MoveDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_F5).
    ExtendedKeyUp(VK_F5).
    Send(); // Reload tab
});

Browser.
@on(RightButton).
@if(MoveDown, MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_W).
    ExtendedKeyUp(VK_W).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Close tab
});


var Explorer = @when((ctx) =>
{
    return ctx.Window.ModuleName == "explorer.exe";
});

Explorer.
@on(RightButton).
@if(MoveUp).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_HOME).
    ExtendedKeyUp(VK_HOME).
    Send(); // Scroll to top
});

Explorer.
@on(RightButton).
@if(MoveDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_END).
    ExtendedKeyUp(VK_END).
    Send(); // Scroll to bottom
});

Explorer.
@on(RightButton).
@if(MoveLeft).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_MENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_MENU).
    Send(); // Go back
});

Explorer.
@on(RightButton).
@if(MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_MENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_MENU).
    Send(); // Go next
});

Explorer.
@on(RightButton).
@if(MoveUp, MoveDown).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_F5).
    ExtendedKeyUp(VK_F5).
    Send(); // Reflesh window
});

Explorer.
@on(RightButton).
@if(MoveDown, MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_CONTROL).
    ExtendedKeyDown(VK_W).
    ExtendedKeyUp(VK_W).
    ExtendedKeyUp(VK_CONTROL).
    Send(); // Close window
});

/* Change system master volume by WheelUp and WheelDown events when
 * the cursor on the taskbar.
 * 
var Taskbar = @when((ctx) =>
{
    return ctx.Window.OnCursor.ModuleName == "explorer.exe" &&
           ctx.Window.OnCursor.ClassName == "MSTaskListWClass";
});

Taskbar.
@if(WheelUp).
@do((ctx) =>
{
    var current = WaveVolume.GetMasterVolume() + 0.02f;
    var next = (current > 1 ? 1 : current);
    WaveVolume.SetMasterVolume(next);
    Tooltip(string.Format("Volume: {0}", (int)(next * 100)));
});

Taskbar.
@if(WheelDown).
@do((ctx) =>
{
    var current = WaveVolume.GetMasterVolume() - 0.02f;
    var next = (current < 0 ? 0 : current);
    WaveVolume.SetMasterVolume(next);
    Tooltip(string.Format("Volume: {0}", (int)(next * 100)));
});
*/
