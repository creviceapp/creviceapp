# Report: Step 10 - Final Consolidation

## Executive Summary

This PoC built a deterministic reliability and performance measurement environment for Crevice gesture replay. The work intentionally avoided live global hooks, `SendInput`, real cursor movement, foreground windows, display state, and overlay UI so that the core gesture behavior can be tested reliably in CI-like environments.

The investigation produced three concrete outcomes:

1. Default gesture correctness is now covered by synthetic replay tests.
2. Generated stroke patterns now exercise all active default stroke gestures across multiple coordinate regions, distances, densities, and jitter levels.
3. A real, narrow production improvement was found and implemented: `PointProcessor` no longer starts its skipped-point polling task when `watchInterval <= 0`.

The production change should be kept. It preserves zero-interval immediate processing semantics and removes a scheduler contention artifact that made the production-like low-latency replay scheduler show roughly 31 ms watcher observation waits.

No commit or push was made. The untracked `docs/` directory was not touched.

## Current Worktree Scope

Tracked files modified:

- `CreviceLib/Core.Stroke.cs`
- `CreviceAppTests/CreviceAppTests.csproj`

New untracked test files:

- `CreviceAppTests/GestureCoverageHarnessTests.cs`
- `CreviceAppTests/GesturePerformanceHarnessTests.cs`

New untracked PoC artifacts:

- `poc/2026-05-06-gesture-reliability-performance/**`

Pre-existing untracked directory intentionally not touched:

- `docs/`

## Reliability Coverage Added

`CreviceAppTests/GestureCoverageHarnessTests.cs` adds CI-safe deterministic correctness tests. The class contains seven focused tests:

- `DefaultUserScriptGestureDefinitionsAreFullyEnumerated`
- `DefaultBrowserWheelGesturesReplayThroughSyntheticInput`
- `DefaultBrowserStrokeGesturesReplayThroughSyntheticInput`
- `GeneratedDefaultBrowserStrokePatternsReplayThroughSyntheticInput`
- `NegativeGestureReplayCasesDoNotExecuteHandlers`
- `GeneratedNegativeStrokePatternsDoNotExecuteHandlers`
- `WheelAndStrokeGesturesDoNotCrossTriggerEachOther`

The tests cover all active default browser gestures from `DefaultUserScript.csx`:

- `RButton + WheelUp`
- `RButton + WheelDown`
- `RButton + U`
- `RButton + D`
- `RButton + L`
- `RButton + R`
- `RButton + UD`
- `RButton + DR`

The generated positive matrix covers 324 stroke scenarios:

- Gestures: `U`, `D`, `L`, `R`, `UD`, `DR`
- Start coordinates: normal center, wide-monitor right side, negative-monitor coordinate
- Distances: 32, 48, 72 px
- Point densities: 2, 4, 7 subdivisions
- Jitter: 0 and 3 px

The generated negative coverage includes below-threshold movement, unknown `RL`, unknown `LR`, and noisy `URDL` non-match scenarios.

## Measurement Harness Added

`CreviceAppTests/GesturePerformanceHarnessTests.cs` adds threshold-free diagnostic measurement categories:

- `Performance`: baseline synthetic replay measurement
- `PerformanceAttribution`: separates raw input, watcher observation wait, observed-stroke read, and validation time
- `PerformanceRepeatability`: repeats attribution to inspect run-to-run variance
- `PerformanceSchedulerComparison`: compares `Task.Factory` against production-like `LowLatencyScheduler`
- `PerformanceStep08PointProcessorZeroInterval`: measures the zero-interval guard effect
- `PerformanceStep09ProductionIntervalComparison`: compares interval 0 and production default interval 10 with scheduler modes

These tests are intentionally not hard performance gates. They should be run manually or as optional artifact-producing CI jobs. Their assertions only verify that measurement data exists and semantic replay remains correct.

## Production Change

The only production code change is in `CreviceLib/Core.Stroke.cs`.

Before:

- `PointProcessor` always started a background skipped-point polling task.
- This happened even when `watchInterval <= 0`.
- In that mode, `MustBeProcessed(...)` always returns true, so every point is processed immediately and no skipped-point flushing is needed.

After:

- `PointProcessor` starts the background skipped-point polling task only when `watchInterval > 0`.
- `watchInterval <= 0` still processes every point immediately through `OnProcess(point)`.
- Disposal now directly disposes the cancellation token source when no background task was started.

This is a narrow lifecycle guard. It does not change default production `StrokeWatchInterval = 10` behavior.

## Evidence Timeline

### Step 01 - Baseline Inventory

Established the test and gesture execution map. `CreviceLibTests` already had deterministic core coverage, while `CreviceAppTests` contained both deterministic tests and machine-dependent diagnostics. The app-test project built locally through the sanitized Visual Studio MSBuild path.

### Step 02 - Reliability Replay Tests

Converted the initial harness into deterministic correctness tests for default gestures. Result: 5 focused tests passed.

### Step 03 - Generated Pattern Coverage

Added generated positive and negative stroke replay scenarios. Result: 7 focused tests passed, including 324 positive generated scenarios and 4 generated negative scenarios.

### Step 04 - Baseline Measurement

Added the first threshold-free performance baseline:

- 336 scenarios
- 1,680 measured samples
- 13,100 measured events
- Overall median: 0.024 ms
- Overall p95: 0.059 ms
- Overall max: 14.777 ms

The max values were treated as noisy until attribution could explain them.

### Step 05 - Attribution Analysis

Separated raw input time from watcher observation wait. The largest replay spikes were dominated by `StrokeWatcher` observation wait, not raw `GestureMachine.Input(...)` processing.

Key attribution:

- Overall total median: 0.02 ms
- Overall total p95: 0.06 ms
- Overall total max: 14.03 ms
- Raw input max: 2.00 ms
- Observation wait max: 13.98 ms
- Validation max: 0.05 ms

### Step 06 - Repeatability

Repeated the attribution measurement three times. The watcher observation wait tail repeated consistently:

- Watcher wait samples >= 1 ms: 21 to 22 per run
- Raw input samples >= 1 ms: 5 to 6 per run
- Total max: 13.662 to 14.876 ms
- Watcher wait max: 13.628 to 14.786 ms

This justified investigating scheduler and watcher lifecycle rather than tuning random gesture recognition code.

### Step 07 - Scheduler Comparison

Compared current `Task.Factory` replay scheduler with a production-like `LowLatencyScheduler(..., ThreadPriority.Highest, 2)`.

Unexpected result before the zero-interval guard:

| Scheduler mode | Wait median ms | Wait p95 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: |
| `Task.Factory` | 0.008 | 0.029 | 68 |
| production-like low latency | 30.813 | 31.918 | 5005 |

This exposed a scheduler topology problem in the zero-interval replay configuration.

### Step 08 - Zero-Interval Guard

Confirmed that `PointProcessor` did unnecessary background polling when `watchInterval <= 0`. Implemented the guard in `Core.Stroke.cs`.

Before/after evidence:

| Evidence | Scheduler mode | Wait median ms | Wait p95 ms | Wait >=1 ms |
| --- | --- | ---: | ---: | ---: |
| Step 07 before | production-like low latency | 30.813 | 31.918 | 5005 |
| Step 08 after | production-like low latency | 0.023 | 0.050 | 22 |

Correctness remained green:

- `GestureCoverageHarnessTests`: 7/7 passed
- Focused core stroke tests: 2/2 passed
- Step 08 scheduler comparison: 1/1 passed

### Step 09 - Production Interval Measurement

Measured production default `StrokeWatchInterval = 10` separately, using production-paced point delivery so multi-stroke semantics remain valid.

Key result for default production-like mode:

- Interval: 10 ms
- Scheduler: production-like low-latency, pool size 2
- Wait median: 0.015 ms
- Wait p95: 0.034 ms
- Wait max: 0.064 ms
- Wait samples >= 1 ms: 0

Pool size 4 did not improve the interval 10 result. Step 09 therefore recommended final consolidation rather than further production tuning.

## Final Verification

Final correctness verification was run after all code changes.

Release build:

- `CreviceAppTests\CreviceAppTests.csproj`: passed
- `CreviceLibTests\CreviceLibTests.csproj`: passed

Focused tests:

- `GestureCoverageHarnessTests`: 7 total, 7 passed
- Core stroke focused tests: 2 total, 2 passed

TRX files saved under:

- `poc/2026-05-06-gesture-reliability-performance/step-10-final-consolidation/output/test-results/GestureCoverageHarnessTests.Step10Correctness.trx`
- `poc/2026-05-06-gesture-reliability-performance/step-10-final-consolidation/output/test-results/CreviceLibTests.Step10CoreStroke.trx`

`git diff --check` returned no whitespace errors. It only reported line-ending warnings for files that Git will convert to CRLF when touched.

## CI Recommendation

### Put In Normal CI

Run these as standard correctness coverage:

- Release build of `CreviceLibTests`
- Release build of `CreviceAppTests`
- Existing deterministic `CreviceLibTests`, excluding benchmark-style tests if needed
- `GestureCoverageHarnessTests`

`GestureCoverageHarnessTests` are suitable for normal CI because they avoid OS hooks, `SendInput`, real cursor movement, foreground windows, display state, and overlay UI.

### Keep Manual Or Diagnostic

Keep these measurement categories out of hard-gated CI for now:

- `Performance`
- `PerformanceAttribution`
- `PerformanceRepeatability`
- `PerformanceSchedulerComparison`
- `PerformanceStep08PointProcessorZeroInterval`
- `PerformanceStep09ProductionIntervalComparison`

They are valuable for artifact collection and local tuning, but their wall-clock numbers are still machine and scheduler sensitive. If they are added to GitHub Actions, they should be a manual workflow or non-gating artifact job.

### Optional Future CI Artifact Job

A future manual workflow could run `TestCategory=PerformanceStep09ProductionIntervalComparison` and upload JSON summaries, Markdown summaries, and TRX files. It should not fail on timing thresholds.

## Risks And Follow-Ups

### Large Harness File

`GesturePerformanceHarnessTests.cs` is intentionally self-contained for the PoC, but it is large. Before turning this into a polished PR, consider extracting shared scenario generation, replay helpers, result models, and writers into test helper files under `CreviceAppTests/`.

### Existing Machine-Dependent Tests

This PoC did not try to make hook or `SendInput` tests CI-safe. Those should remain diagnostic/manual unless they are redesigned around controlled OS integration environments.

### Production Default Interval

Step 09 did not justify changing default `StrokeWatchInterval = 10`. The interval 10 production-like measurement looked healthy when replayed with production-paced input.

### Performance Gates

Do not add timing gates yet. The current performance harness is useful for observation and comparison, not pass/fail enforcement.

## Final Recommendation

Keep the following changes:

1. `GestureCoverageHarnessTests.cs`
2. `GesturePerformanceHarnessTests.cs`
3. `CreviceAppTests.csproj` includes for the new test files
4. The `PointProcessor` zero-interval guard in `CreviceLib/Core.Stroke.cs`
5. The PoC artifacts under `poc/2026-05-06-gesture-reliability-performance/`

Before committing, decide whether to keep the full PoC directory in the repository. If the repository should only contain production/test changes, the PoC artifacts can remain uncommitted or be moved to a documentation branch. The user previously said not to commit `docs/`; this report leaves `docs/` untouched and uncommitted.
