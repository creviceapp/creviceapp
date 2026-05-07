# Gesture Performance Analysis Summary

This Step 05 summary combines the Step 04 baseline JSON with the Step 05 attribution-only measurement. No production tuning was performed and no hard timing threshold was added.

## Step 04 Distribution

- Baseline input: 336 scenarios, 5 measured iterations, 1,680 samples, 13,100 measured events.
- Overall baseline: median 0.024 ms, p95 0.059 ms, max 14.777 ms, mean 0.202 ms.
- Elapsed bands: 1,548 samples below 0.05 ms; 91 from 0.05 to 0.1 ms; 14 from 0.1 to 1 ms; 27 at or above 1 ms.
- The 27 samples at or above 1 ms were isolated: every affected scenario appeared only once in the outlier set.
- Outliers were spread across measured iterations: 5, 6, 5, 5, and 6 outliers in iterations 0 through 4.
- Outliers were mostly generated-positive samples: 26 generated-positive and 1 generated-negative.

Because each slow scenario only appeared once, the Step 04 max/p95/mean ranking is dominated by one-off max samples. With only five samples per scenario, per-scenario p95 equals max, so it should not be read as a stable scenario-level percentile yet.

## Stopwatch Scope

The Step 04 stopwatch starts immediately before `scenario.Replay(gestureMachine)` and stops immediately after it returns. For stroke scenarios, that replay includes:

- `GestureMachine.Input(...)` for press and move/null events.
- `SpinWait.SpinUntil(...)` until the asynchronous `StrokeWatcher` observes the expected stroke, or until buffered points are observed for below-threshold negatives.
- `StrokeWatcher.GetStrokeSequence().ToString()` after the wait.
- `GestureMachine.Input(...)` for release.

Validation assertions and recorder count checks run after the Step 04 stopwatch stops.

## Step 05 Attribution

The attribution run used the same 336 scenarios and 5 measured iterations, but split total replay time into input, observation wait, observation read, and validation timings.

| Scope | Group | Samples | Total median ms | Total p95 ms | Total max ms | Input median ms | Input p95 ms | Input max ms | Wait median ms | Wait p95 ms | Wait max ms | Validation max ms |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| overall | all | 1,680 | 0.02 | 0.06 | 14.03 | 0.01 | 0.04 | 2.00 | 0.01 | 0.03 | 13.98 | 0.05 |
| generated-positive | generated-positive | 1,620 | 0.02 | 0.06 | 14.03 | 0.01 | 0.04 | 2.00 | 0.01 | 0.03 | 13.98 | 0.05 |
| generated-negative | generated-negative | 20 | 0.03 | 0.05 | 0.95 | 0.02 | 0.03 | 0.93 | 0.01 | 0.02 | 0.03 | 0.00 |
| multi-stroke | multi-stroke | 10 | 0.02 | 11.94 | 11.94 | 0.01 | 0.04 | 0.04 | 0.01 | 11.92 | 11.92 | 0.00 |
| single-stroke | single-stroke | 20 | 0.02 | 0.05 | 0.06 | 0.01 | 0.03 | 0.04 | 0.01 | 0.02 | 0.05 | 0.00 |
| wheel | wheel | 10 | 0.03 | 0.17 | 0.17 | 0.03 | 0.17 | 0.17 | 0.00 | 0.00 | 0.00 | 0.01 |

Outliers at or above 1 ms in the attribution run:

- Total replay: 28 samples.
- Total input: 6 samples.
- Observation wait: 22 samples.

The slowest total replay samples were dominated by observation wait. Example: the slowest sample was 14.03 ms total, with 0.04 ms input and 13.98 ms observation wait.

## Interpretation

Most measured gesture replay work is far below 0.1 ms locally. The largest Step 04 outliers are primarily asynchronous stroke-watcher observation latency, not deterministic raw `GestureMachine.Input(...)` cost. Validation is not part of the Step 04 replay stopwatch and remained negligible in the attribution run.

There are a few raw input outliers, mostly press input. Those are smaller than the watcher wait spikes and likely come from state transition work, stroke-watcher reset/start scheduling, and normal local scheduler noise. They are worth checking in Step 06, but they are not the dominant source of the 12-15 ms maxima seen in Step 04.
