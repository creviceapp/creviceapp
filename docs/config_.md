  
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
  
  
### StrokeReset
  
  
```cs
Config.Callback.StrokeReset += (sender, e) { };
```
This event activated when the state of mouse's stroke to be reset.
`e` is `StrokeResetEventHandler`, and it does not have special properties.
  
### StrokeUpdate
  
```cs
Config.Callback.StrokeUpdate += (sender, e) { };
```
  
This event activated when the state of mouse's stroke to be changed.
  
`e` is `StrokeUpdateEventHandler `.
Type | Property Name | Description |
-----|-----|------
IReadOnlyList\<Stroke\> | Strokes | 
  
### StateChange
  
```cs
Config.Callback.StateChange += (sender, e) { };
```
  
This event activated when the state of GestureMachine to be changed. 
`e` is `StateChangeEventHandler`.
  
Type | Property Name | Description |
-----|-----|------
State | LastState | 
State | CurrentState |
  
### GestureCancel
  
```cs
Config.Callback.GestureCancel += (sender, e) { };
```
  
This event activated when the gesture to be cancelled.
`e` is `GestureCancelEventHandler`.
  
Type | Property Name | Description |
-----|-----|------
StateN | LastState | 
  
### GestureTimeout
  
  
```cs
Config.Callback.GestureTimeout += (sender, e) { };
```
  
This event activated when the gesture to be timeout.
`e` is `GestureTimeoutEventHandler`.
  
Type | Property Name | Description |
-----|-----|------
StateN | LastState | 
  
### MachineStart
  
  
```cs
Config.Callback.MachineStart += (sender, e) { };
```
This event activated when GestureMachine to be started.
`e` is `MachineStartEventHandler`, and it does not have special properties.
  
### MachineReset
  
  
```cs
Config.Callback.MachineReset += (sender, e) { };
```
  
This event activated when GestureMachine to be reset for some reasons.
`e` is `MachineResetEventHandler`.
  
Type | Property Name | Description |
-----|-----|------
State | LastState | 
  
### MachineStop
  
  
```cs
Config.Callback.MachineStop += (sender, e) { };
```
This event activated when GestureMachine to be stopped.
`e` is `MachineStopEventHandler`, and it does not have special properties.
  