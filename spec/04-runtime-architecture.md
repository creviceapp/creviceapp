## 4. Runtime Architecture

### 4.1 Projects

`Crevice.sln` contains the implementation, tests, and packaging project.

Primary projects:

- `CreviceLib`: reusable gesture core and DSL library.
- `CreviceLibTests`: tests for `CreviceLib`.
- `CreviceApp`: Windows desktop application, user script host, Windows API boundaries, UI, and runtime coordination.
- `CreviceAppTests`: tests for `CreviceApp` behavior and deterministic gesture replay.
- `CreviceAppPackage`: Windows Application Packaging Project for Store upload package generation.

`CreviceAppPackage` is the only Store packaging project in the current build and release contract.

### 4.2 Core Library

`CreviceLib` contains the core abstractions for keys, context, DSL evaluation, events, finite-state gesture recognition, stroke processing, and stroke sequence handling.

The core library stays independent from WinForms UI and packaging. Tests exercise it through deterministic inputs.

### 4.3 Application Layer

`CreviceApp` owns the Windows desktop application behavior. It integrates hooks, SendInput, window inspection, user script execution, configuration, UI forms, and runtime gesture machines.

The application layer translates Windows input events into the core library's gesture and event model.

### 4.4 User Script Layer

The user script layer compiles and executes C# script definitions. The script layer exposes the Crevice DSL root object, context helpers, input helpers, and additional Windows API helper namespaces.

Script reload replaces active gesture definitions through the reloadable application boundary.

### 4.5 Test Layer

`CreviceLibTests` covers core state machines and stroke processing. `CreviceAppTests` covers application-level behavior, script loading, gesture replay, Windows API wrappers, and diagnostics.

Gesture replay tests construct gesture machines directly and feed synthetic events. This keeps default CI independent of interactive desktop hooks.

### 4.6 Packaging Layer

`CreviceAppPackage` packages the desktop app for Microsoft Store upload through a Windows Application Packaging Project. The packaging workflow builds Store upload artifacts without local certificate signing.
