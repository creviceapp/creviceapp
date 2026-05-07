## 3. Product Behavior

### 3.1 Application Startup

`crevice4.exe` starts as a Windows desktop application. It initializes configuration, loads the user script, creates the main runtime objects, and begins processing input through the configured gesture machines.

Startup failure reporting remains an open decision in Appendix B.

### 3.2 Gesture Processing

Crevice4 evaluates input events against configured gestures. A gesture executes only when its trigger sequence and context predicate both match.

Mouse button, wheel, and movement tokens are represented through the shared key and stroke abstractions. Stroke direction recognition converts pointer movement into directional tokens and feeds those tokens into gesture evaluation.

A gesture executor runs once for a completed gesture match. Section 5 defines negative paths, threshold behavior, sequence matching, and cross-trigger prevention.

### 3.3 Default Browser Gestures

`CreviceApp/Scripts/DefaultUserScript.csx` defines the active browser gesture set shipped with the application.

The default browser predicate matches browser and browser-like Windows shell contexts. The predicate covers Chrome, Firefox, Opera, Internet Explorer, Microsoft Edge, `ApplicationFrameHost.exe` windows whose pointed window text is `Microsoft Edge`, and `explorer.exe` windows whose pointed window class is `DirectUIHWND`.

The active default browser gesture set contains:

- `RButton` + `WheelUp`: previous tab.
- `RButton` + `WheelDown`: next tab.
- `RButton` + `MoveUp`: scroll to top.
- `RButton` + `MoveDown`: scroll to bottom.
- `RButton` + `MoveLeft`: go back.
- `RButton` + `MoveRight`: go forward.
- `RButton` + `MoveUp, MoveDown`: reload tab.
- `RButton` + `MoveDown, MoveRight`: close tab.

The default gesture inventory is covered by synthetic replay tests.

### 3.4 Context-Sensitive Behavior

Gesture predicates receive the current execution context. The execution context exposes foreground window and pointed window information.

A false predicate prevents the gesture path from consuming the trigger input for that predicate branch and prevents executor invocation.

### 3.5 Failure Behavior

Input processing errors stay outside the low-level Windows hook hot path. Hook callbacks preserve pass-through behavior when unexpected errors occur.

Resource cleanup runs during normal shutdown and repeated disposal paths. Section 5 defines `PointProcessor` disposal behavior.
