## 6. User Scripting and Configuration

### 6.1 Script Format

Crevice4 user scripts are C# script files. The default script uses `.csx` syntax and supports `#r` and `#load` directives at the top of the file.

The script environment loads IDE support mocks for editing support. Runtime execution supplies the real Crevice API environment.

### 6.2 DSL Surface

The script DSL exposes `When`, `On`, and `Do` composition. `When` defines a context predicate. `On` defines trigger tokens and sequence tokens. `Do` defines the executor.

Executors run with an execution context that exposes window context and runtime helpers.

### 6.3 Configuration

Configuration classes under `CreviceApp/Config.*.cs` define global, user interface, command-line, and gesture overlay settings.

Configuration defaults are explicit in code and covered by tests where behavior depends on defaults.

### 6.4 Reloading

Reloadable application forms and main runtime objects replace script-backed gesture configuration during reload. Reload preserves process lifetime.

A reload with compilation errors keeps the previously applied gesture configuration active.

A reload with user script evaluation errors applies the gesture configuration created from the script and reports the evaluation error as a warning.

### 6.5 API Helpers

User scripts expose helper APIs for input simulation and Windows integration. `SendInput` drives synthesized input. Window helpers expose foreground and pointed window information. Core audio helpers are available when imported by the script.

Examples inside the default script remain comments until a user enables them.
