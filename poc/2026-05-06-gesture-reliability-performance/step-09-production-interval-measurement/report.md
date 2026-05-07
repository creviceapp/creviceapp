# Report: Step 09 - Production Interval Measurement

## Scope

Step 09 added a threshold-free production interval measurement mode in `CreviceAppTests/GesturePerformanceHarnessTests.cs`. No production code was changed.

The final measurement compares:

- interval 0 with `Task.Factory`;
- interval 0 with production-like `LowLatencyScheduler(..., 2)`;
- interval 10, the production default from `GestureMachineConfig.StrokeWatchInterval`, with `Task.Factory`;
- interval 10 with production-like `LowLatencyScheduler(..., 2)`;
- interval 10 with production-like `LowLatencyScheduler(..., 4)` as an exploratory scheduler-capacity check.

The test still validates semantic replay on every measured scenario. Timing assertions remain threshold-free and only require that each requested mode produces samples and run summaries.

## Reduction and Replay Cadence

The first interval-10 attempt used the same instantaneous point replay as the zero-interval harness. It failed semantic validation on `baseline-UD`: the production interval can coalesce rapid synthetic points before the direction change is observable.

A second attempt paced only generated segment boundaries. It still failed a generated `UD` case.

Following supervisor direction to reduce the matrix if the full interval comparison was too slow, the final run kept all four required modes and kept pool size 4 because it remained cheap enough, but reduced Step 09 to:

- warmup iterations: 0;
- measured iterations: 1;
- repeat runs per mode: 1;
- scenario inventory: all 336 existing deterministic scenarios.

For `StrokeWatchInterval > 0`, the replay now sleeps `interval + 1 ms` after each synthetic move point. That pacing is intentionally outside the measured raw input call timing but inside total replay timing. It preserves production-interval semantics while making clear that total replay time includes synthetic input cadence and should not be compared as a pure scheduler cost.

## Verification

Release build:

```text
Remove-Item Env:PATH -ErrorAction SilentlyContinue
$env:PATH = 'C:\Program Files\dotnet;C:\Windows\System32;C:\Windows;C:\Windows\System32\Wbem;C:\Windows\System32\WindowsPowerShell\v1.0'
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' CreviceAppTests\CreviceAppTests.csproj /p:Configuration=Release /p:Platform=AnyCPU /verbosity:minimal
```

Result: passed.

Focused correctness:

```text
vstest.console.exe CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"FullyQualifiedName~GestureCoverageHarnessTests"
```

Result: passed, 7/7.

Focused Step 09 measurement:

```text
vstest.console.exe CreviceAppTests\bin\Release\Crevice4Tests.dll /TestCaseFilter:"TestCategory=PerformanceStep09ProductionIntervalComparison"
```

Result: passed, 1/1, about 1.5 minutes.

TRX artifacts:

- `output/test-results/GestureCoverageHarnessTests.Step09Correctness.trx`
- `output/test-results/GesturePerformanceHarnessTests.Step09ProductionIntervalComparison.trx`

Structured artifacts:

- `output/gesture-performance-production-interval-comparison.json`
- `output/gesture-performance-production-interval-comparison-summary.md`

## Evidence

| Role | Interval ms | Scheduler mode | Samples | Total median ms | Total p95 ms | Input median ms | Input p95 ms | Wait median ms | Wait p95 ms | Wait max ms | Wait >=1 ms |
| --- | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| primary | 0 | interval-0-task-factory-default | 336 | 0.015 | 0.070 | 0.011 | 0.036 | 0.003 | 0.029 | 15.255 | 4 |
| primary | 0 | interval-0-production-like-low-latency | 336 | 0.031 | 0.163 | 0.007 | 0.055 | 0.022 | 0.069 | 16.241 | 12 |
| primary | 10 | interval-10-task-factory-default | 336 | 63.140 | 218.087 | 0.243 | 0.407 | 0.015 | 0.038 | 0.493 | 0 |
| primary | 10 | interval-10-production-like-low-latency | 336 | 63.044 | 218.484 | 0.225 | 0.396 | 0.015 | 0.034 | 0.064 | 0 |
| exploratory | 10 | interval-10-production-like-low-latency-pool4 | 336 | 63.324 | 218.054 | 0.243 | 0.431 | 0.015 | 0.042 | 0.210 | 0 |

Interpretation:

- Interval 10 passed semantic validation only with production-paced replay. Instantaneous replay is not a valid production-interval semantic measurement for multi-stroke cases.
- Interval 10 watcher observation wait was small and stable after paced input. The production-like pool-2 mode had wait median 0.015 ms, p95 0.034 ms, max 0.064 ms, and no wait samples at or above 1 ms.
- The interval-10 total replay median around 63 ms is expected from synthetic per-move pacing. It is not evidence of scheduler delay.
- Pool size 4 did not improve the interval-10 result. Pool 4 had similar total replay, slightly higher wait p95, and a higher wait max than pool 2 in this run.
- The interval-0 modes still show occasional local outliers, consistent with prior noisy one-run measurements. The systematic Step 07 low-latency 30 ms wait median did not return.

## Step 10 Recommendation

Choose final consolidation, not production tuning.

Step 09 does not justify changing the production default interval or low-latency scheduler pool size. The production default interval behaves semantically correctly when replayed with production-like cadence, and the pool-size 4 exploratory check did not improve the relevant wait metrics.

For CI, promote the deterministic correctness coverage and consider a reduced, threshold-free production-interval smoke measurement that records artifacts but does not gate on wall-clock timings. Avoid CI hard timing gates; keep any performance output informational.
