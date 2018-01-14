using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CreviceApp.WinAPI.Window;

using static CreviceApp.WinAPI.Constants.WindowsMessages;
using static CreviceApp.WinAPI.Constants.VirtualKeys;


/*
 * Examples for gestures for standard browsers.
 */

var Browser = @when((ctx) =>
{
    return ctx.ForegroundWindow.ModuleName == "chrome.exe" ||
           ctx.ForegroundWindow.ModuleName == "firefox.exe" ||
           ctx.ForegroundWindow.ModuleName == "opera.exe" ||
           ctx.ForegroundWindow.ModuleName == "iexplore.exe" ||
          (ctx.ForegroundWindow.ModuleName == "ApplicationFrameHost.exe" &&
               ctx.PointedWindow.Text == "Microsoft Edge") ||
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
 * Examples for CoreAudio and Tooltip API.
 * The system master volume will be changed by WheelUp and WheelDown when the cursor is on the taskbar.
 *

// This using clause should be placed the top of this script.
using CreviceApp.WinAPI.CoreAudio;

var VolumeControl = new VolumeControl();
var VolumeDelta = 0.01f;

var Taskbar = @when((ctx) =>
{
    return ctx.PointedWindow.ModuleName == "explorer.exe" &&
              (ctx.PointedWindow.ClassName == "MSTaskListWClass" ||
               ctx.PointedWindow.ClassName == "TrayShowDesktopButtonWClass" ||
               ctx.PointedWindow.ClassName == "TrayButton" ||
               ctx.PointedWindow.ClassName == "TrayClockWClass" ||
               ctx.PointedWindow.ClassName == "ClockButton" ||
               ctx.PointedWindow.ClassName == "Start" ||
               ctx.PointedWindow.ClassName == "TaskbarWindow32");
});

Taskbar.
@if(WheelUp).
@do((ctx) =>
{
    VolumeControl.SetMasterVolume(VolumeControl.GetMasterVolume() + VolumeDelta);
    Tooltip(string.Format("Volume: {0:D2}", (int)(VolumeControl.GetMasterVolume() * 100)));
});

Taskbar.
@if(WheelDown).
@do((ctx) =>
{
    VolumeControl.SetMasterVolume(VolumeControl.GetMasterVolume() - VolumeDelta);
    Tooltip(string.Format("Volume: {0:D2}", (int)(VolumeControl.GetMasterVolume() * 100)));
});
 */


/*
 * Examples for global gestures.
 * Caution: Unfortunately, It would be impossible for you to send special key strokes 
 * like Alt+Tab no matter how hard you try on Windows 8. This is the limitation of 
 * that operating system. You should have upgraded it to Windows 10.
 * 

var Whenever = @when((ctx) =>
{
    return true;
});

Whenever.
@if(X1Button).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_MENU).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_MENU).
    Send(); // Assign X1Button to Alt+Tab
});

Whenever.
@if(X2Button).
@do((ctx) =>
{
    SendInput.Multiple().
    ExtendedKeyDown(VK_LWIN).
    ExtendedKeyDown(VK_TAB).
    ExtendedKeyUp(VK_TAB).
    ExtendedKeyUp(VK_LWIN).
    Send(); // Assign X2Button to Win+Tab
});
 */
