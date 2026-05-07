# Report: Step 02 - Reliability Replay Tests

## Scope

This step converted the in-progress gesture coverage harness into deterministic reliability replay tests for the default browser gestures identified in Step 01. Work stayed in the supervised scope: app test code and Step 02 PoC artifacts only. No production code was changed, no commit or push was made, and the untracked `docs/` directory was not touched.

## Changed Files

- `CreviceAppTests/GestureCoverageHarnessTests.cs`
- `CreviceAppTests/CreviceAppTests.csproj`
  - This project-file change already existed in the working tree to compile `GestureCoverageHarnessTests.cs`; this step left that include in place and normalized the trailing newline.
- `poc/2026-05-06-gesture-reliability-performance/step-02-reliability-replay-tests/log.md`
- `poc/2026-05-06-gesture-reliability-performance/step-02-reliability-replay-tests/report.md`
- `poc/2026-05-06-gesture-reliability-performance/step-02-reliability-replay-tests/output/reliability-replay-results.json`
- `poc/2026-05-06-gesture-reliability-performance/step-02-reliability-replay-tests/output/test-results/GestureCoverageHarnessTests.Step02.trx`

## Harness Changes

`GestureCoverageHarnessTests.cs` now separates definition coverage from replay execution:

- Loads and enumerates the real `DefaultUserScript.csx` root so changes to active default browser gestures are detected.
- Replays gestures through a test-only mirror root whose handlers record labels instead of executing the default script's `SendInput` actions.
- Uses only synthetic `GestureMachine.Input(...)` calls with explicit points.
- Avoids hooks, OS input injection, foreground-window state, display state, cursor movement, and overlay UI.
- Removes the previous stopwatch/p95/max performance test from the reliability class.

The only wait left in the replay path is a generous fail-fast synchronization wait for the asynchronous `StrokeWatcher` to observe a synthetic stroke before release. It is not used as a performance assertion or benchmark threshold.

## Positive Coverage

The focused reliability tests cover all active default browser gestures from `DefaultUserScript.csx`:

- `RButton + WheelUp`
- `RButton + WheelDown`
- `RButton + U`
- `RButton + D`
- `RButton + L`
- `RButton + R`
- `RButton + UD`
- `RButton + DR`

Test methods added/retained:

- `DefaultUserScriptGestureDefinitionsAreFullyEnumerated`
- `DefaultBrowserWheelGesturesReplayThroughSyntheticInput`
- `DefaultBrowserStrokeGesturesReplayThroughSyntheticInput`

## Negative Coverage

The new reliability suite also checks:

- An unregistered `RL` stroke does not execute any handler.
- A `When` condition returning false blocks execution.
- Below-threshold movement does not execute a stroke handler.
- Wheel replay does not execute stroke handlers.
- Stroke replay does not execute wheel handlers.

Test methods:

- `NegativeGestureReplayCasesDoNotExecuteHandlers`
- `WheelAndStrokeGesturesDoNotCrossTriggerEachOther`

## Commands And Results

Initial Visual Studio MSBuild attempt:

```text
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: failed before test compilation because the local process environment contained duplicate `Path`/`PATH` keys, causing MSBuild/Roslyn to throw.

Sanitized Visual Studio MSBuild:

```text
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'; Remove-Item Env:PATH -ErrorAction SilentlyContinue; & $msbuild CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed.

Focused test run:

```text
$vstest = 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe'; Remove-Item Env:PATH -ErrorAction SilentlyContinue; & $vstest CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"FullyQualifiedName~GestureCoverageHarnessTests" /Logger:"trx;LogFileName=GestureCoverageHarnessTests.Step02.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-02-reliability-replay-tests\output\test-results"
```

Result: passed.

```text
Total tests: 5
Passed: 5
TRX: poc/2026-05-06-gesture-reliability-performance/step-02-reliability-replay-tests/output/test-results/GestureCoverageHarnessTests.Step02.trx
```

## Step 03 Recommendations

1. Reuse the test-only replay root and `MotionPatternGenerator` shape for generated-pattern coverage.
2. Keep correctness and measurement phases separate. Generated-pattern tests should assert recognized stroke sequences and handler selection, not elapsed time.
3. If Step 03 expands many generated strokes, consider moving the replay helpers into a dedicated helper file under `CreviceAppTests/` to keep the test class readable.
4. Continue running app-test builds through the sanitized Visual Studio MSBuild command on this machine unless the duplicate `Path`/`PATH` environment issue is fixed.
