## 7. Windows Integration

### 7.1 Hook Boundaries

CreviceApp wraps Windows low-level mouse and keyboard hooks behind application-level classes. Hook activation, callback lifetime, unhooking, and disposal are explicit runtime states.

Managed hook delegates stay alive for the hook lifetime. Unhooking releases native hook state once.

Hook callbacks keep work minimal and preserve pass-through behavior for input that is not consumed by a matched gesture path.

### 7.2 Input Simulation

`SendInput` wrappers synthesize keyboard and mouse input for gesture executors. Default browser gestures use `SendInput.Multiple()` to emit ordered key down and key up sequences.

Input simulation preserves key release ordering for modifier chords.

### 7.3 Window and Device Context

Window helpers read foreground and pointed window metadata for user script predicates. The default browser predicate depends on process module names, class names, and window text.

Device and core audio helpers are script APIs outside the gesture recognition core.

### 7.4 Test Isolation

Unit tests and default CI tests do not install real global hooks. Tests use synthetic replay and boundary-level wrappers for deterministic verification.

Interactive hook behavior is validated through opt-in manual or diagnostic paths, not the default CI test path.
