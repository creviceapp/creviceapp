# Report: Step 06 - Measurement Determinism Tuning

## Scope

Step 06 stayed test-only. I added one focused `PerformanceRepeatability` MSTest mode in `CreviceAppTests/GesturePerformanceHarnessTests.cs` and did not change production code. The mode repeats the Step 05 attribution measurement three times, keeps the existing semantic validation inside each run, and writes Step 06 JSON/Markdown summaries that make raw `GestureMachine.Input(...)` timing, stroke-watcher observation wait, and run-to-run variance visible together.

No hard timing threshold was added. Assertions only verify that each requested run produced samples and component variance summaries.

The untracked `docs/` directory was not touched. No commit or push was made.

## Code Inspection Notes

`GestureMachine.Input(...)` enqueues cursor points into `StrokeWatcher.Process(...)` while the machine is in `StateN`, then handles the physical event and state transition under the gesture-machine lock.

`StrokeWatcher` consumes queued points on a background task through `BlockingCollection<Point>.GetConsumingEnumerable()`. The Step 05 and Step 06 harnesses wait for the asynchronous watcher state by polling `GetStrokeSequence()` or `GetBufferedPoints()` with `SpinWait.SpinUntil(...)`.

That means the raw input aggregate measures direct `Input(...)` calls, while observation wait measures when the background watcher has caught up enough for the test to observe the expected stroke or buffered points.

## Measurement Change

Added:

- `SyntheticGestureReplayPerformanceRepeatability`
- `[TestCategory("PerformanceRepeatability")]`
- `GestureRepeatabilityMeasurer`
- `RepeatabilityRunResult`, per-run summary records, component variance summaries, and slow-sample records
- Step 06 output writers for `gesture-performance-repeatability.json` and `gesture-performance-repeatability-summary.md`

The change intentionally reuses the Step 05 attribution replay and validation path instead of refactoring the 1800-line harness. This keeps the Step 06 behavior additive and avoids broad churn while the PoC is still collecting evidence.

## Commands Run

Release build:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed. Built `CreviceAppTests\bin\Release\Crevice4Tests.dll`.

Focused Step 06 measurement:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"TestCategory=PerformanceRepeatability" /Logger:"trx;LogFileName=GesturePerformanceHarnessTests.Step06Repeatability.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-06-measurement-determinism-tuning\output\test-results"
```

Result: passed. Total tests: 1. Passed: 1. Reported test duration: 1 s. Total VSTest time: 2.1260 s.

## Artifacts

- `output/gesture-performance-repeatability.json`
- `output/gesture-performance-repeatability-summary.md`
- `output/test-results/GesturePerformanceHarnessTests.Step06Repeatability.trx`

The JSON was parsed successfully with PowerShell `ConvertFrom-Json`.

## Evidence

The focused Step 06 run used 3 repeat runs. Each repeat run used 2 warmup iterations, 5 measured iterations, 336 scenarios, 1,680 samples, and 13,100 measured events. Total measured events across repeat runs: 39,300.

Per-run overall attribution:

| Run | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Total >=1 ms | Input >=1 ms | Wait >=1 ms |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 0 | 0.023 | 0.069 | 14.876 | 0.013 | 0.037 | 1.887 | 0.008 | 0.035 | 14.786 | 28 | 5 | 22 |
| 1 | 0.024 | 0.075 | 14.692 | 0.012 | 0.042 | 1.732 | 0.008 | 0.038 | 14.663 | 28 | 6 | 22 |
| 2 | 0.022 | 0.070 | 13.662 | 0.012 | 0.042 | 2.174 | 0.007 | 0.035 | 13.628 | 26 | 5 | 21 |

Run-to-run ranges:

| Component | Median range ms | P95 range ms | Max range ms | >=1 ms total |
| --- | ---: | ---: | ---: | ---: |
| total_replay | 0.001 | 0.005 | 1.214 | 82 |
| raw_input | 0.001 | 0.005 | 0.442 | 16 |
| observation_wait | 0.001 | 0.003 | 1.157 | 65 |
| validation | 0.000 | 0.000 | 0.021 | 0 |

The largest total replay samples still match the Step 05 shape: a roughly 13-15 ms total replay sample with very small raw input time and nearly all of the elapsed time in watcher observation wait. The slowest Step 06 sample was 14.876 ms total with 0.089 ms raw input and 14.786 ms observation wait.

Raw input tails repeated but remained smaller. The slowest raw input sample was 2.174 ms, and most listed raw input outliers were dominated by press input. That keeps the Step 05 interpretation intact: press transition and watcher reset/start work are worth inspecting, but they do not explain the largest replay maxima.

## Interpretation

Step 06 made the Step 05 distinction more repeatable. Medians and p95 values were very stable across runs. The maximum values remained noisy, but the same component dominated every repeat: watcher observation wait had 21-22 samples at or above 1 ms per run, while raw input had 5-6.

This points away from a single deterministic slow scenario and toward scheduler-shaped delay in the asynchronous watcher observation path. The generated-positive group dominates the slowest-sample list mostly because it dominates the scenario count; the top scenarios are not identical across runs.

## Step 07 Recommendation

Proceed to a narrow production-tuning investigation of `StrokeWatcher` scheduling and observation notification, not CI integration yet.

Recommended Step 07 focus:

1. Inspect whether `StrokeWatcher` can signal stroke/buffer updates without polling `GetStrokeSequence()` under `SpinWait`, while preserving current async behavior.
2. Investigate `TaskFactory.StartNew(...)` scheduling and watcher reset/start cost from `GestureMachine.CurrentState` transitions, especially press-heavy raw input outliers.
3. Keep any production experiment tiny, reversible, and measured with the new `PerformanceRepeatability` mode before and after.

Do not add a CI performance gate yet. The measurement remains local and scheduler-sensitive; CI would be useful later for artifact collection, but not as a pass/fail gate from this evidence.
