## 2. Goals and Scope

### 2.1 Product Goals

Crevice4 is a Windows desktop utility for mouse and keyboard gestures.

Crevice4 lets users define gesture behavior with C# user scripts. The script model exposes window context, input simulation, configuration, and Windows API helpers.

Crevice4 includes default browser gestures for common navigation actions. The default script covers tab navigation, page navigation, scrolling, reload, and close-tab behavior for standard browser environments.

Crevice4 runs as a desktop application and integrates with Windows input APIs. The release model includes direct zip distribution and Microsoft Store distribution.

### 2.2 Engineering Goals

The gesture core stays deterministic under synthetic input. Gesture recognition logic is testable without installing real global hooks.

Windows API interaction remains isolated behind small boundaries in `CreviceApp`. Tests cover those boundaries without relying on interactive desktop state in default CI.

Build and release automation run on GitHub-hosted Windows runners. The repository no longer relies on AppVeyor for CI or release builds.

Store upload packaging and GitHub release zip packaging are separate workflows. Section 9 defines artifact shape and signing behavior.

### 2.3 Scope Exclusions

Crevice4 is not a general-purpose macro recorder.

Crevice4 is not a screen recorder, cursor mirror, or remote-control application.

Crevice4 does not require a background Windows service.

Crevice4 does not require administrator privileges for desktop use.

Crevice4 does not make performance diagnostics part of default CI pass/fail behavior.

Crevice4 does not publish private signing certificates, certificate passwords, or other signing secrets in public artifacts.
