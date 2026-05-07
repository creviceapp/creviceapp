# Report: Step 07 - StrokeWatcher Scheduler Tuning Investigation

## Scope

Step 07 stayed test-only. I added a focused `PerformanceSchedulerComparison` MSTest mode in `CreviceAppTests/GesturePerformanceHarnessTests.cs` and did not change production code.

The new mode reuses the same deterministic scenario inventory as the Step 05 attribution and Step 06 repeatability harnesses. It preserves the existing semantic validation on every replay, and compares:

- `task-factory-default`: the current test replay scheduler, `Task.Factory`.
- `production-like-low-latency`: `LowLatencyScheduler("Step07StrokeWatcherTaskScheduler", ThreadPriority.Highest, 2)` through a replay-machine `TaskFactory`.

No hard timing thresholds were added. Assertions only verify that both scheduler modes produce samples and that each mode records all requested runs.

The untracked `docs/` directory was not touched. No commit or push was made.

## Code Inspection Notes

The real app gesture machine overrides `StrokeWatcherTaskFactory` with `new TaskFactory(new LowLatencyScheduler("StrokeWatcherTaskScheduler", ThreadPriority.Highest, 2))`.

The replay harness previously always used `Task.Factory`. Step 07 adds a scheduler lease to the replay gesture machine so the same replay and validation path can be run against either scheduler mode.

One important implementation detail shaped the result: `StrokeWatcher` inherits from `PointProcessor`, and both `PointProcessor` and `StrokeWatcher` start background tasks through the same `TaskFactory`. With a two-thread low-latency scheduler, each active watcher can occupy both scheduler threads: one for the `PointProcessor` loop and one for the stroke queue consumer. During replay, state transitions reset watchers often, so newly created watcher tasks can queue behind the previous watcher tasks until they observe disposal/cancellation and exit.

## Measurement Change

Added:

- `SyntheticGestureReplaySchedulerComparison`
- `[TestCategory("PerformanceSchedulerComparison")]`
- `GestureSchedulerComparisonMeasurer`
- `ReplaySchedulerMode` and `ReplaySchedulerLease`
- `SchedulerComparisonRunResult`, scheduler mode summaries, component comparisons, and scheduler-tagged slow sample records
- Step 07 output writers for `gesture-performance-scheduler-comparison.json` and `gesture-performance-scheduler-comparison-summary.md`

The structured output records total replay timing, raw input timing, watcher observation wait timing, and samples at or above 1 ms by scheduler mode. It also includes per-run summaries and the slowest raw-input and watcher-wait samples per mode.

## Commands Run

Release build:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed. Built `CreviceAppTests\bin\Release\Crevice4Tests.dll`.

Focused Step 07 scheduler comparison:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"TestCategory=PerformanceSchedulerComparison" /Logger:"trx;LogFileName=GesturePerformanceHarnessTests.Step07SchedulerComparison.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-07-strokewatcher-scheduler-tuning\output\test-results"
```

Result: passed. Total tests: 1. Passed: 1. Reported test duration: 3 m 17 s. Total VSTest time: 3.3006 min.

Affected correctness tests:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"FullyQualifiedName~GestureCoverageHarnessTests" /Logger:"trx;LogFileName=GestureCoverageHarnessTests.Step07Correctness.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-07-strokewatcher-scheduler-tuning\output\test-results"
```

Result: passed. Total tests: 7. Passed: 7. Total VSTest time: 2.8373 s.

Note: an initial scheduler comparison attempt used a 120 s command timeout and was killed before artifacts were written. The same test completed successfully with a longer 300 s command timeout.

## Artifacts

- `output/gesture-performance-scheduler-comparison.json`
- `output/gesture-performance-scheduler-comparison-summary.md`
- `output/test-results/GesturePerformanceHarnessTests.Step07SchedulerComparison.trx`
- `output/test-results/GestureCoverageHarnessTests.Step07Correctness.trx`

The JSON was parsed successfully with PowerShell `ConvertFrom-Json`.

## Evidence

The focused Step 07 run used 3 repeat runs per scheduler. Each repeat run used 2 warmup iterations, 5 measured iterations, 336 scenarios, 1,680 samples, and 13,100 measured events. Each scheduler mode recorded 5,040 samples and 39,300 measured events.

Scheduler mode aggregate:

| Scheduler mode | Samples | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 5,040 | 0.024 | 0.059 | 14.789 | 0.014 | 0.033 | 1.680 | 0.008 | 0.029 | 14.712 | 75 | 7 | 68 |
| production-like-low-latency | 5,040 | 30.964 | 32.063 | 33.135 | 0.141 | 0.274 | 1.549 | 30.813 | 31.918 | 32.852 | 5,006 | 6 | 5,005 |

Per-run observation wait:

| Scheduler mode | Run | Wait median ms | Wait p95 ms | Wait max ms | Wait >=1 ms |
| --- | ---: | ---: | ---: | ---: | ---: |
| task-factory-default | 0 | 0.008 | 0.037 | 14.712 | 22 |
| task-factory-default | 1 | 0.008 | 0.027 | 14.059 | 24 |
| task-factory-default | 2 | 0.007 | 0.025 | 14.183 | 22 |
| production-like-low-latency | 0 | 30.824 | 31.918 | 32.385 | 1,669 |
| production-like-low-latency | 1 | 30.802 | 31.924 | 32.573 | 1,669 |
| production-like-low-latency | 2 | 30.805 | 31.911 | 32.852 | 1,667 |

Raw input did not explain the production-like mode slowdown. The production-like raw input median was 0.141 ms and p95 was 0.274 ms. Almost all of the roughly 31 ms total replay time was watcher observation wait.

Semantic validation passed in both scheduler modes. The low-latency mode is therefore not causing incorrect gesture outcomes in this harness, but it is changing the measurement shape dramatically.

## Interpretation

The Step 06 suspicion that the default test scheduler contributed to watcher observation tails was not supported by this direct comparison. The production-like replay scheduler did not shrink the Step 06 tails; it made watcher observation wait dominate nearly every stroke sample.

This points to a scheduler topology/lifecycle interaction rather than a simple "test scheduler is too noisy" artifact. In this code path, a `StrokeWatcher` starts two long-lived tasks through the same scheduler. A production-like scheduler with pool size 2 can be fully occupied by the current watcher. When replay resets watchers repeatedly, the next watcher work can wait behind old watcher tasks until they unwind.

The existing `Task.Factory` replay mode remains the better local harness default for now because it keeps the deterministic replay measurement fast enough to run and still exposes the intermittent watcher observation tails seen in Steps 05 and 06.

## Recommendation

Do not switch the Step 08 performance harness default to the production-like low-latency scheduler yet. Keep the scheduler comparison as an explicit diagnostic mode.

Do not proceed directly to CI performance integration. The production-like mode takes minutes locally and would produce noisy, scheduler-sensitive artifacts without first resolving what it is measuring.

Step 08 should investigate narrow production `StrokeWatcher` scheduling/lifecycle tuning before CI integration. The next focused question should be whether `PointProcessor` and `StrokeWatcher` should share the same two-thread low-latency scheduler, and whether watcher reset should wait for or otherwise drain old watcher tasks before relying on newly created watcher work.

No broad production tuning was made in Step 07.
