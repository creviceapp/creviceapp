# Report: Step 05 - Performance Attribution Analysis

## Scope

Step 05 analyzed the Step 04 gesture performance baseline and added one focused attribution-only measurement path in `CreviceAppTests/GesturePerformanceHarnessTests.cs`. No production code was changed, no performance tuning was implemented, no commit or push was made, and the untracked `docs/` directory was not touched.

The attribution measurement is output-only and threshold-free. It separates replay total time into raw `GestureMachine.Input(...)` time, stroke-watcher observation wait time, observed-stroke read time, and validation time.

## Evidence Inputs

- Step 04 report: `poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/report.md`
- Step 04 summary: `poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/output/gesture-performance-baseline-summary.md`
- Step 04 JSON: `poc/2026-05-06-gesture-reliability-performance/step-04-performance-baseline-measurement/output/gesture-performance-baseline.json`
- Step 05 attribution JSON: `output/gesture-performance-attribution.json`
- Step 05 combined analysis JSON: `output/gesture-performance-baseline-analysis.json`
- Step 05 readable output summary: `output/gesture-performance-analysis-summary.md`

## Step 04 Baseline Distribution

Step 04 measured 336 scenarios across 5 measured iterations: 1,680 samples and 13,100 measured events.

Overall Step 04 timing:

| Metric | Value |
| --- | ---: |
| Median | 0.024 ms |
| P95 | 0.059 ms |
| Max | 14.777 ms |
| Mean | 0.202 ms |

Elapsed bands from the raw samples:

| Band | Count |
| --- | ---: |
| < 0.05 ms | 1,548 |
| 0.05 to < 0.1 ms | 91 |
| 0.1 to < 1 ms | 14 |
| >= 1 ms | 27 |

The 27 samples at or above 1 ms were isolated. Every affected scenario appeared only once in the outlier set, and outliers were spread across the five measured iterations as 5, 6, 5, 5, and 6 samples. That argues against a single deterministic slow scenario shape.

The slowest Step 04 samples were mostly generated-positive cases, plus one generated-negative below-threshold case. Generated-positive also dominates the run size: 324 of 336 scenarios and 1,620 of 1,680 samples. Group-level comparisons are therefore most meaningful for medians and p95 values, not the single-run maxima.

One important caveat: each per-scenario summary only has five samples. The current percentile implementation uses nearest-rank, so per-scenario p95 equals max. Per-scenario `max_ms`, `p95_ms`, and `mean_ms` rankings are therefore all heavily shaped by one-off outliers in this baseline.

## Stopwatch Scope

In Step 04, the stopwatch starts immediately before `scenario.Replay(gestureMachine)` and stops immediately after replay returns.

For wheel scenarios, the timed replay includes:

- `GestureMachine.Input(...)` for RButton press.
- `GestureMachine.Input(...)` for wheel fire.
- `GestureMachine.Input(...)` for RButton release.

For stroke scenarios, the timed replay includes:

- `GestureMachine.Input(...)` for RButton press.
- `GestureMachine.Input(...)` for each null/move event.
- `SpinWait.SpinUntil(...)` until the asynchronous stroke watcher observes the expected stroke, or until buffered points are observed for below-threshold negatives.
- `StrokeWatcher.GetStrokeSequence().ToString()` after the wait.
- `GestureMachine.Input(...)` for RButton release.

Validation assertions and recorder count checks run after the stopwatch stops. Handler recording itself can still occur inside input processing because the executor runs during the trigger/release path.

## Step 05 Attribution Results

The attribution run used the same 336 scenarios, 5 measured iterations, and Release build. The focused MSTest passed.

| Scope | Samples | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Validation max ms |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| overall | 1,680 | 0.02 | 0.06 | 14.03 | 0.01 | 0.04 | 2.00 | 0.01 | 0.03 | 13.98 | 0.05 |
| generated-positive | 1,620 | 0.02 | 0.06 | 14.03 | 0.01 | 0.04 | 2.00 | 0.01 | 0.03 | 13.98 | 0.05 |
| generated-negative | 20 | 0.03 | 0.05 | 0.95 | 0.02 | 0.03 | 0.93 | 0.01 | 0.02 | 0.03 | 0.00 |
| multi-stroke | 10 | 0.02 | 11.94 | 11.94 | 0.01 | 0.04 | 0.04 | 0.01 | 11.92 | 11.92 | 0.00 |
| single-stroke | 20 | 0.02 | 0.05 | 0.06 | 0.01 | 0.03 | 0.04 | 0.01 | 0.02 | 0.05 | 0.00 |
| wheel | 10 | 0.03 | 0.17 | 0.17 | 0.03 | 0.17 | 0.17 | 0.00 | 0.00 | 0.00 | 0.01 |

Outliers at or above 1 ms in the Step 05 attribution run:

| Component | Count |
| --- | ---: |
| Total replay | 28 |
| Raw input total | 6 |
| Observation wait | 22 |

The slowest attribution sample was `positive-R-start--800-360-distance-72-density-2-jitter-0`: 14.03 ms total, 0.04 ms input, 13.98 ms observation wait. The next slowest total samples followed the same pattern: roughly 12-14 ms total with about 0.02-0.04 ms input and the rest in observation wait.

The slowest raw input samples were smaller. The top input outlier was 2.00 ms total replay, with 2.00 ms total input and 1.99 ms in press input. Several other input outliers were also press-heavy. This suggests occasional state transition or stroke-watcher reset/start overhead, but these spikes are not the main explanation for the 12-15 ms Step 04 maxima.

Validation remained negligible: overall validation max was 0.05 ms and p95 rounded to 0.00 ms.

## Interpretation

The Step 04 timing distribution is mostly very fast, with isolated scheduler-shaped spikes. Step 05 attribution confirms that the largest maxima are dominated by asynchronous stroke-watcher observation wait rather than raw `GestureMachine.Input(...)` processing.

The generated-positive group has the highest visible max because it has the most samples. The small wheel, single-stroke, and multi-stroke groups are still useful sanity buckets, but their sample counts are too low for stable group-level tail claims. The multi-stroke attribution max was a single `baseline-UD` observation wait spike; it should be treated as the same watcher/scheduling phenomenon unless it repeats in future runs.

The current harness is useful, but it is becoming large. Step 06 should consider extracting shared scenario generation, replay helpers, result writers, and summary calculators before adding many more measurement variants.

## Step 06 Tuning Candidates

1. Reduce or remove measurement-side watcher wait dependency before production tuning.
   - Expected benefit: high for measurement clarity.
   - Behavioral risk: low if kept in tests only.
   - Code complexity: medium.
   - Rationale: current maxima mostly measure when the async watcher has observed state, not just `Input(...)` work. A deterministic test scheduler, callback/event signal, or explicit watcher-drain helper would make future measurements more stable.

2. Investigate `StrokeWatcher` queue/background task scheduling.
   - Expected benefit: high for real tail latency if production users experience delayed stroke establishment.
   - Behavioral risk: medium.
   - Code complexity: medium to high.
   - Rationale: `StrokeWatcher` runs asynchronous queue consumption and the harness waits for it. Tuning task scheduling, wait notification, or queue processing could reduce observation latency, but behavior must remain faithful to real async operation.

3. Inspect press input state transition and watcher reset/start cost.
   - Expected benefit: medium.
   - Behavioral risk: medium.
   - Code complexity: medium.
   - Rationale: the few raw input outliers are mostly press-heavy and may involve state transition, `ResetStrokeWatcher()`, old watcher disposal, and new watcher task startup.

4. Optimize stroke sequence reads and callback copying only if profiling shows repetition.
   - Expected benefit: low to medium.
   - Behavioral risk: low to medium.
   - Code complexity: low to medium.
   - Rationale: `GetStrokeSequence()`, `GetStorkes()`, and buffered point reads allocate copies. They were not dominant in this run, but they are plausible cleanup targets after the larger scheduling question is handled.

5. Expand repeated measurement before changing production algorithms.
   - Expected benefit: high for decision quality.
   - Behavioral risk: low.
   - Code complexity: low.
   - Rationale: one local run shows isolated outliers. Step 06 should repeat attribution on an idle machine and compare medians, p95, and max across runs before choosing production changes.

## Recommendation

Do not tune production code directly from Step 04 alone. The immediate Step 06 priority should be improving measurement determinism around stroke-watcher observation, then repeating focused attribution. If repeated runs still show watcher wait as the dominant tail, investigate `StrokeWatcher` scheduling and notification. If raw input outliers repeat, inspect press-state transition and watcher reset/start overhead next.

## Commands Run

Release build:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed.

Focused attribution run:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe' CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"TestCategory=PerformanceAttribution" /Logger:"trx;LogFileName=GesturePerformanceHarnessTests.Step05Attribution.trx" /ResultsDirectory:"poc\2026-05-06-gesture-reliability-performance\step-05-performance-attribution-analysis\output\test-results"
```

Result: passed. Total tests: 1. Passed: 1. Reported test duration: 683 ms. Total VSTest time: 1.2281 s.
