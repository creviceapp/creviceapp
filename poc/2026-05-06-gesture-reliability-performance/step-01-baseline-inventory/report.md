# Report: Step 01 - Baseline Inventory

## Scope

This pass inspected the current repository at `C:\Users\seigl\projects\creviceapp` on branch `split-store-msix-release-zip`. It did not commit, push, benchmark, or edit `docs/`. Writes were limited to this Step 01 directory.

## Repository State

Initial status:

```text
## split-store-msix-release-zip...origin/split-store-msix-release-zip
 M CreviceAppTests/CreviceAppTests.csproj
?? CreviceAppTests/GestureCoverageHarnessTests.cs
?? docs/
?? poc/
```

The modified app-test project adds `GestureCoverageHarnessTests.cs` to compilation. That file is untracked and appears to be an earlier in-progress harness, so this step treated it as existing work and did not modify it.

## Test And Build Inventory

`Crevice.sln` contains:

- `CreviceLib`: SDK-style `Crevice.Core`, targeting `net462;netstandard2.0`.
- `CreviceLibTests`: MSTest .NET Framework 4.6.2 tests for core DSL/FSM/events/stroke behavior.
- `CreviceApp`: WinForms desktop app targeting .NET Framework 4.6.2.
- `CreviceAppPackage`: Windows application packaging project.
- `CreviceAppTests`: MSTest .NET Framework 4.6.2 tests referencing `CreviceApp` and `CreviceLib`.

CI (`.github/workflows/ci.yml`) runs on `windows-2022`, adds x86 MSBuild, installs legacy Windows SDK contracts, sets a build version, restores `packages.config` packages plus `dotnet restore` for `CreviceLib`, builds `CreviceLib`, `CreviceLibTests`, and `CreviceAppTests`, then runs only `CreviceLibTests`.

`CreviceLibTests` includes strong deterministic coverage for `Core.DSL`, `Core.FSM`, events, `Stroke`, and `StrokeSequence`. It also compiles `Core.EventsBenchmark.cs`, whose `EventsBenchmarkTest` invokes BenchmarkDotNet; reliability-only runs should filter this class out.

`CreviceAppTests` includes config/user-script tests, app `GestureMachine` behavior tests, callback tests, form tests, hook tests, and SendInput tests. The hook and SendInput tests are useful diagnostics, but they are not CI-safe reliability replay tests because they install global hooks, inject input, depend on cursor/display state, and serialize with process-wide mutexes.

## Gesture Execution Map

Hook input enters through `CreviceApp/UI.Main.MouseGestureForm.cs`. `MouseProc` maps:

- `WM_MOUSEMOVE` to `GestureMachine.Input(NullEvent, point)`.
- button down/up events to physical press/release events plus point.
- `WM_MOUSEWHEEL` negative delta to `WheelDown`, non-negative to `WheelUp`.
- `WM_MOUSEHWHEEL` negative delta to `WheelRight`, non-negative to `WheelLeft`.

The app-level `CreviceApp/GM.GestureMachine.cs` stores cursor position when a point is supplied, suppresses configured keyboard timeout keys, then delegates to the core FSM. It also overrides the stroke watcher task factory with a high-priority `LowLatencyScheduler`.

Core input is in `CreviceLib/Core.FSM.GestureMachine.cs`. Points are sent to `StrokeWatcher.Process(...)` only while the current state is `StateN`; `NullEvent` returns false after point processing. `State0` activates matching `When` elements for single-fire or double-throw press triggers. `StateN` handles nested fire/press/release triggers and, on the normal release trigger, reads `StrokeWatcher.GetStrokeSequence()` and executes matching stroke `Do` executors before release executors.

The gesture DSL is in `CreviceLib/Core.DSL.cs`, with app alias `CreviceApp/DSL.RootElement.cs`. A root contains `When` elements; `When` supports single throw, double throw, and decomposed double throw; double-throw elements can contain nested single/double/decomposed elements and stroke elements. Stroke gestures are declared as `On(params StrokeDirection[])`.

## Default Script Loading

Runtime reload is in `CreviceApp/GM.ReloadableGestureMachine.cs`. It obtains the default script resource through `GlobalConfig.GetOrSetDefaultUserScriptFile(...)`, builds a `GestureMachineCandidate`, restores cache when allowed, otherwise parses/compiles/emits/evaluates the script, creates a `GestureMachineCluster`, runs every profile, and wires optional gesture stroke overlay callbacks.

`CreviceApp/Config.GlobalConfig.cs` writes the default user script if the configured script file does not exist. By default it disables IDE-support `#load "IDESupport...` directives before runtime/test compilation.

`CreviceApp/Scripts/DefaultUserScript.csx` currently has one active browser `When` block with eight active gestures: `RButton+WheelUp`, `RButton+WheelDown`, `RButton+U`, `RButton+D`, `RButton+L`, `RButton+R`, `RButton+UD`, and `RButton+DR`. Other taskbar/global examples are commented out.

## Reusable Helpers

Best CI-safe reusable helpers:

- `CreviceLibTests/TestEnv.cs`: `TestGestureMachine`, `TestCallbackManager`, `TestEvents`.
- `CreviceLibTests/Core.FSM.GestureMachineTest.cs`: stroke watcher substitution and FSM point forwarding patterns.
- `CreviceLibTests/Core.StrokeTest.cs`: stroke input thresholds and `StrokeWatchInterval = 0` pattern.
- `CreviceAppTests/TestHelpers.cs`: temporary user directory and `SetupUserDirectory`.
- `CreviceAppTests/ScriptsTests.cs`: default script parse/compile/evaluate flow.
- `CreviceAppTests/GM.GestureMachineCandidateTests.cs`: cache/restoration checks.
- `CreviceAppTests/GestureCoverageHarnessTests.cs`: in-progress enumeration and synthetic replay helpers.

Step 02 should reuse the harness ideas, but split reliability assertions from the timing assertion currently named `PerfHarnessMeasuresGeneratedMotionReplay`. That method uses `Stopwatch`, p95/max thresholds, and should not be part of the reliability suite.

## CI-Safe Boundaries

Safe for deterministic CI replay:

- Root/DSL coverage enumeration.
- Synthetic `GestureMachine.Input(...)` sequences using physical events and explicit points.
- RButton plus wheel fire events, without OS hooks.
- RButton plus stroke movement plus release, with controlled thresholds and waits.
- Default script parse/compile/evaluate into `RootElement` in a temporary user directory.
- Core `StrokeSequence` equality/hash/string expectations.

Keep diagnostic/manual:

- Global low-level hook installation.
- SendInput injection and hook capture.
- Current cursor position and display scaling.
- Foreground/pointed window matching against real OS windows.
- Gesture stroke overlay rendering.
- BenchmarkDotNet or timing-threshold pass/fail tests.

## Commands And Results

Tooling notes:

- `msbuild`, `nuget`, and `vstest.console` were not on PATH.
- `.NET SDK 10.0.203` was installed.
- .NET Framework 4.6.2 targeting pack was present.
- Required legacy package folders existed under `packages/`.
- Visual Studio test runner was found at `C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe`.

Commands run:

```text
dotnet msbuild CreviceLib\CreviceLib.csproj /p:Configuration=Release /verbosity:minimal
Result: passed.

dotnet msbuild CreviceLibTests\CreviceLibTests.csproj /p:Configuration=Release /verbosity:minimal
Result: passed.

dotnet msbuild CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /verbosity:minimal
Result: failed locally. CreviceApp could not resolve System.Runtime.WindowsRuntime and System.Runtime.WindowsRuntime.UI.Xaml, leading to IAsyncOperation<StartupTask>.AsTask/GetAwaiter compile errors. CI's legacy Windows SDK contract installation is required for this path.

dotnet test CreviceLibTests\CreviceLibTests.csproj --no-build -c Release --filter "FullyQualifiedName!~EventsBenchmarkTest" --logger "trx;LogFileName=CreviceLibTests.Step01.trx" --results-directory "poc\2026-05-06-gesture-reliability-performance\step-01-baseline-inventory\output\test-results"
Result: exited 0 but produced no TRX, so it is not treated as a reliable test result.

vstest.console.exe CreviceLibTests\bin\Release\CreviceLibTests.dll /TestCaseFilter:"FullyQualifiedName!~EventsBenchmarkTest" /Logger:"trx;LogFileName=CreviceLibTests.Step01.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-01-baseline-inventory\output\test-results"
Result: passed. 157 total, 157 passed. TRX saved at output/test-results/CreviceLibTests.Step01.trx.
```

## Step 02 Recommendations

1. Create a reliability-focused app test class for default gestures using synthetic `GestureMachine.Input(...)`, not hooks or SendInput.
2. Promote the useful parts of `GestureCoverageHarnessTests.cs` into a deterministic helper, but remove or quarantine `PerfHarnessMeasuresGeneratedMotionReplay` from reliability runs.
3. Add separate coverage assertions for default browser wheel gestures and stroke gestures so regressions point at the broken class of gesture.
4. Add replay cases for `RButton + WheelUp`, `RButton + WheelDown`, `RButton + U/D/L/R/UD/DR`, and at least one non-matching stroke to confirm no false execution.
5. Use temporary script directories and `GlobalConfig.GetOrSetDefaultUserScriptFile(...)` for default script tests, but assert on `RootElement`/executor selection rather than real foreground windows.
6. Keep `CreviceLibTests` as the CI-safe core suite, filtered to exclude `EventsBenchmarkTest` for reliability-only runs.
7. For local app-test build verification, either run in the same sanitized environment as CI after legacy Windows SDK contracts are installed, or document the prerequisite explicitly; plain `dotnet msbuild` is not enough on this machine.
8. Leave hook, SendInput, cursor, display scaling, and overlay checks in a diagnostic/manual bucket unless Step 02 introduces explicit categories or traits to keep them out of normal CI reliability runs.

Structured inventory was also saved to `output/baseline-inventory.json`.
