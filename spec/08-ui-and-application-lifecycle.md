## 8. UI and Application Lifecycle

### 8.1 Forms

CreviceApp uses WinForms for its desktop UI. The form set includes launcher, main, cluster main, reloadable main, mouse gesture, product info, and tooltip notifier surfaces.

UI classes live under `CreviceApp/UI.*.cs` and resource files live beside their form definitions.

### 8.2 Main Runtime Lifetime

The main application initializes configuration, user script state, gesture machines, Windows API boundaries, and UI surfaces before input processing starts.

Shutdown releases hooks, gesture machines, UI resources, and notification surfaces through deterministic disposal paths.

### 8.3 Gesture Overlay

The gesture stroke overlay is controlled by `Config.GestureStrokeOverlay`. The default script exposes a commented configuration line that disables the visualization feature.

Overlay behavior is a UI feature and does not change gesture recognition semantics.

### 8.4 Notifications and Product Information

Tooltip notification and product information forms present runtime information to the user. These surfaces do not own gesture recognition state.

### 8.5 Error Boundaries

UI-level error reporting occurs outside low-level hook callbacks. Runtime failures cross into UI reporting through application-level coordination.
