using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CreviceApp.WinAPI.CoreAudioAPI;
using CreviceApp.WinAPI.Window;


/*
 * Gesture definitions for standard browsers. 
 */
var Browser = @when((ctx) =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe" ||
           ctx.ForegroundWindow.ModuleName == "firefox.exe" ||
           ctx.ForegroundWindow.ModuleName == "opera.exe" ||
           ctx.ForegroundWindow.ModuleName == "iexplore.exe" ||
          (ctx.ForegroundWindow.ModuleName == "explorer.exe" &&              
               ctx.PointedWindow.ClassName == "DirectUIHWND");
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
    ExtendedKeyDown(VK_LMENU).
    ExtendedKeyDown(VK_LEFT).
    ExtendedKeyUp(VK_LEFT).
    ExtendedKeyUp(VK_LMENU).
    Send(); // Go back
});

Browser.
@on(RightButton).
@if(MoveRight).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LMENU).
    ExtendedKeyDown(VK_RIGHT).
    ExtendedKeyUp(VK_RIGHT).
    ExtendedKeyUp(VK_LMENU).
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


/* 
 * Change system master volume by WheelUp and WheelDown events when
 * the cursor on the taskbar.
 */
var VolumeControl = new VolumeControl();
var VolumeDelta = 0.01f;

var Taskbar = @when((ctx) =>
{
    return ctx.PointedWindow.ModuleName == "explorer.exe" &&
              (ctx.PointedWindow.ClassName == "MSTaskListWClass" ||
               ctx.PointedWindow.ClassName == "TrayShowDesktopButtonWClass" ||
               ctx.PointedWindow.ClassName == "TrayClockWClass" ||
               ctx.PointedWindow.ClassName == "TaskbarWindow32");
});

Taskbar.
@if(WheelUp).
@do((ctx) =>
{
    var current = VolumeControl.GetMasterVolume() + VolumeDelta;
    var next = (current > 1 ? 1 : current);
    VolumeControl.SetMasterVolume(next);
    Tooltip(string.Format("Volume: {0:D2}", (int)(next * 100)));
});

Taskbar.
@if(WheelDown).
@do((ctx) =>
{
    var current = VolumeControl.GetMasterVolume() - VolumeDelta;
    var next = (current < 0 ? 0 : current);
    VolumeControl.SetMasterVolume(next);
    Tooltip(string.Format("Volume: {0:D2}", (int)(next * 100)));
});
