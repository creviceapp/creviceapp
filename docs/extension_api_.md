  
# Extension API
  
  
### Window
  
  
`Window` is a utility static class about Windows's window.
To use this class, declare as following:
```cs
using Crevice.WinAPI.Window;
```
  
#### Methods
  
  
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
using static Crevice.WinAPI.Constants.VirtualKeys;
```
  
For more details, see [Virtual-Key Codes (Windows)](https://msdn.microsoft.com/ja-jp/library/windows/desktop/dd375731(v=vs.85 ).aspx).
  
### WindowsMessages
  
  
This class provides the windows message constants. 
To use this class, declare as following:
```cs
using static Crevice.WinAPI.Constants.WindowsMessages;
```
  
For more details, see [Window Messages (Windows)](https://msdn.microsoft.com/en-us/library/windows/desktop/ff381405(v=vs.85 ).aspx).
  
### VolumeControl
  
  
`VolumeControl` is a utility class about system audio volume.
To use this class, declare as following:
```cs
using Crevice.WinAPI.CoreAudio;
var VolumeControl = new VolumeControl();
```
  
#### GetMasterVolume()
  
  
Returns window's current master mixer volume.
This function returns a `float` value, within the range between 0 and 1.
  
#### SetMasterVolume(float value)
  
  
Sets window's current master mixer volume. The value should be within the range between 0 and 1.
  