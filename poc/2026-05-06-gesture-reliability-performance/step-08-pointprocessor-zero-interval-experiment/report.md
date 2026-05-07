# Report: Step 08 - PointProcessor Zero-Interval Experiment

## Scope

Step 08 investigated the narrow hypothesis that `PointProcessor` should not start its skipped-point polling task when `watchInterval <= 0`.

The production experiment was intentionally tiny and limited to `CreviceLib/Core.Stroke.cs`:

- `PointProcessor` now starts its background skipped-point task only when `watchInterval > 0`.
- `Process(Point)` keeps the same immediate-processing semantics. Since `MustBeProcessed(...)` returns true for `watchInterval <= 0`, every input point still calls `OnProcess(point)` synchronously in that mode.
- `Dispose` directly disposes the cancellation token source when no background task was started. The `watchInterval > 0` path still cancels the task as before.

I also added a narrow Step 08 performance test entry point in `CreviceAppTests/GesturePerformanceHarnessTests.cs` so the after-guard scheduler comparison writes artifacts under this Step 08 output directory instead of overwriting Step 07 artifacts. No hard timing threshold was added.

The untracked `docs/` directory was not touched. No commit or push was made.

## Code Inspection Evidence

`PointProcessor.MustBeProcessed(...)` returns true whenever `watchInterval <= 0`. Therefore, in the replay harness configuration where `StrokeWatchInterval = 0`, `PointProcessor.Process(Point)` always updates `lastProcessedTickCount`, calls `OnProcess(point)`, and clears `skippedPoint` immediately.

Before this experiment, the `PointProcessor` constructor still called `StartBackgroundTask()` unconditionally. That task loop checked for skipped points once per millisecond, but in zero-interval mode no skipped-point flush was needed because every point was already processed immediately.

`StrokeWatcher` separately starts its queue-consuming background task. Step 07 showed that with `LowLatencyScheduler(..., 2)`, a single watcher could occupy the two scheduler threads: one useless zero-interval `PointProcessor` polling task plus one useful `StrokeWatcher` queue task. That matched the observed low-latency scheduler wait behavior.

## Correctness

Release build:

```text
MSBuild CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed. An earlier Release build reported the existing `UwpDesktopAnalyzer` warning about missing `Microsoft.CodeAnalysis.VisualBasic`; the later build after the harness metadata change passed without repeating that warning.

Focused generated gesture correctness:

```text
vstest.console.exe CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"FullyQualifiedName~GestureCoverageHarnessTests"
```

Result: passed, 7/7.

Focused core stroke correctness:

```text
MSBuild CreviceLibTests\CreviceLibTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
vstest.console.exe CreviceLibTests\bin\Release\CreviceLibTests.dll /TestCaseFilter:"FullyQualifiedName~StrokeTest|FullyQualifiedName~GestureMachineTest.PypassesGivenPointToStrokeWatcherWhenCurrentStateIsStateN"
```

Result: passed, 2/2.

TRX artifacts:

- `output/test-results/GestureCoverageHarnessTests.Step08Correctness.trx`
- `output/test-results/CreviceLibTests.Step08CoreStroke.trx`
- `output/test-results/GesturePerformanceHarnessTests.Step08SchedulerComparison.trx`

## Measurement

Before evidence from Step 07 full scheduler comparison:

| Scheduler mode | Runs | Samples | Total median ms | Total p95 ms | Wait median ms | Wait p95 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 3 | 5040 | 0.024 | 0.059 | 0.008 | 0.029 | 68 |
| production-like-low-latency | 3 | 5040 | 30.964 | 32.063 | 30.813 | 31.918 | 5005 |

After-guard Step 08 focused scheduler comparison:

| Scheduler mode | Runs | Samples | Total median ms | Total p95 ms | Wait median ms | Wait p95 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 1 | 1680 | 0.012 | 0.026 | 0.002 | 0.013 | 2 |
| production-like-low-latency | 1 | 1680 | 0.032 | 0.060 | 0.023 | 0.050 | 22 |

The Step 08 run used the same scenario inventory as Step 07: 336 scenarios, 2 warmup iterations, 5 measured iterations, and 13,100 measured events per scheduler mode. It used one repeat per scheduler as a smaller focused equivalent because the full Step 07 symptom was large and the mission allowed a smaller comparison.

The production-like low-latency wait median improved from about 30.813 ms in Step 07 to 0.023 ms after the guard. Its wait p95 improved from about 31.918 ms to 0.050 ms. Raw input stayed small, so the observed change is specifically in watcher observation wait.

Structured artifacts:

- `output/gesture-performance-scheduler-comparison-after-zero-interval-guard.json`
- `output/gesture-performance-scheduler-comparison-after-zero-interval-guard-summary.md`
- `output/pointprocessor-zero-interval-before-after-summary.json`
- `output/pointprocessor-zero-interval-before-after-summary.md`

## Interpretation

The hypothesis is confirmed for the zero-interval replay configuration. Starting the `PointProcessor` polling task when `watchInterval <= 0` was unnecessary and created severe scheduler contention under the production-like two-thread low-latency scheduler.

The guard does not broadly tune production scheduling. The default app `StrokeWatchInterval` remains 10 ms, so the normal production path still starts the skipped-point task. This Step 08 result primarily fixes zero-interval deterministic replay and any other explicit zero-or-negative interval configuration.

The remaining after-guard low-latency mode still has occasional outliers around one scheduler quantum in `observation_wait`, but the systematic roughly 31 ms median/p95 behavior from Step 07 is gone.

## Recommendation

Keep the `PointProcessor` zero-interval guard.

Do not broaden this step into production default interval tuning. If Step 09 wants to examine the default `StrokeWatchInterval = 10` behavior, that should be a separate phase with a production-interval measurement design.
